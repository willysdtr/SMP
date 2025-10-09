using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{

    private PlayerController cont;  // Controller経由でStateにアクセス

    private HashSet<GameObject> ground_obj = new HashSet<GameObject>();
    private HashSet<GameObject> wall_obj = new HashSet<GameObject>();

    [SerializeField] private Vector2 checkSize = new Vector2(0.5f, 1.0f);
    [SerializeField] private Vector2 checkOffset = new Vector2(0f, 0f);
    [SerializeField] private LayerMask climbLayer;

    private Rigidbody2D rb;
    private RectTransform rect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cont = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        rect = GetComponent<RectTransform>();
        // 判定サイズをRectTransformのサイズに合わせる
        checkSize = new Vector2(checkSize.x * rect.sizeDelta.x, checkSize.y * rect.sizeDelta.y);
    }

    // Update is called once per frame
    void Update()
    {
        //OverlapBoxの作成、Climb処理に使用
        Vector2 center = (Vector2)transform.position + checkOffset;

        Collider2D hit = Physics2D.OverlapBox(center, checkSize, 0f, climbLayer);

        cont.ishit = hit;
    }

    private void OnDrawGizmos()
    {
        //OverlapBoxの描画
        Gizmos.color = Color.red;
        Vector2 center = (Vector2)transform.position + checkOffset;
        Gizmos.DrawWireCube(center, checkSize);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {

        if (((1 << collision.gameObject.layer) & cont.groundlayers) != 0)//インスペクターで設定したLayerとのみ判定を取る
        {

            if (collision.gameObject.tag == "Kaesi")//返し縫いに当たった時の処理
            {
                cont.PlayerReturn(collision.transform.rotation.y);//プレイヤーの向きを変える

            }

            if (collision.gameObject.tag == "Goal")
            {
                cont.state.currentstate = PlayerState.State.GOAL;// ゴール状態に変更
                cont.Goal(collision.transform.position);
            }
            else
            {
                foreach (ContactPoint2D contact in collision.contacts)
                {

                    // 上向きに接触した場合のみカウント
                    if (Vector2.Angle(contact.normal, Vector2.up) < 20f)
                    {
                        if (collision.gameObject.tag == "Spring")//ばねに当たった時の処理
                        {
                            transform.position = new Vector2(collision.transform.position.x, transform.position.y);
                            cont.state.IS_JUMP = true;
                            cont.state.IS_MOVE = false;
                            cont.state.IS_GROUND = false;

                        }
                        else // 通常の地面に当たった時の処理
                        {
                            cont.state.IS_GROUND = true;
                            cont.state.IS_MOVE = true;
                            ground_obj.Add(collision.gameObject);
                        }

                    }
                    // 横向きに接触した場合のみカウント
                    if (contact.normal == Vector2.left || contact.normal == Vector2.right)
                    {

                        if (collision.gameObject.tag == "String" && cont.state.IS_CLIMB_NG == false)
                        {

                            bool isVertical = collision.transform.rotation.z != 0;
                            if (isVertical)
                            {
                                // 縦の糸ならTriggerに切り替え
                                GetComponent<BoxCollider2D>().isTrigger = true;
                                //糸に当たった時の処理
                                cont.state.IS_MOVE = false;
                                cont.state.IS_CLIMB = true;
                                cont.hitobj_pos = collision.transform.position;
                                rb.linearVelocity = Vector2.zero;
                                rb.bodyType = RigidbodyType2D.Kinematic;
                            }
                        }
                        else
                        {
                            wall_obj.Add(collision.gameObject);
                            cont.state.IS_MOVE = false;

                        }
                    }
                }
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        ground_obj.Remove(collision.gameObject);//地面判定したオブジェクトを削除
        wall_obj.Remove(collision.gameObject);//壁判定したオブジェクトを削除
        if (ground_obj.Count == 0)
        {//地面判定したオブジェクトがすべてなくなれば、地面から離れた状態にする
            cont.state.IS_GROUND = false;
        }

        if (wall_obj.Count == 0)
        {//壁判定したオブジェクトがすべてなくなれば、移動可能にする
            cont.state.IS_MOVE = true;
        }

    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "String")
        {
            //糸に当たった時の処理
            cont.state.IS_MOVE = false;
            cont.state.IS_CLIMB = true;
            cont.hitobj_pos = collider.transform.position;
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (!cont.ishit)
        {//OverlapBoxが重なってないときに実行(誤作動するため)
            if (collider.gameObject.tag == "String")
            {//糸から離れた時の処理
                cont.state.IS_MOVE = true;
                cont.state.IS_CLIMB = false;
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.linearVelocity = Vector2.zero;
                GetComponent<BoxCollider2D>().isTrigger = false;//Trigger解除
            }
        }

    }
}
