using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class PlayerCollision : MonoBehaviour
{

    private PlayerController cont;  // Controller経由でStateにアクセス

    private HashSet<GameObject> ground_obj = new HashSet<GameObject>();
    private HashSet<GameObject> wall_obj = new HashSet<GameObject>();

    [SerializeField] private Vector2 checkSize = new Vector2(0.5f, 1.0f);
    [SerializeField] private Vector2 checkOffset = new Vector2(0f, 0f);

    [SerializeField] private StringManager_Canvas stringManager; // StringManager_Canvasの参照、糸を消す処理で使用


    private Rigidbody2D rb;
    private RectTransform rect;

    private BoxCollider2D m_collider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cont = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        rect = GetComponent<RectTransform>();
        m_collider = GetComponent<BoxCollider2D>(); // 親にあるColliderのみ取得
        // 判定サイズをRectTransformのサイズに合わせる
        checkSize = new Vector2(checkSize.x * rect.sizeDelta.x, checkSize.y * rect.sizeDelta.y);
    }

    // Update is called once per frame
    void Update()
    {
        //OverlapBoxの作成、Climb処理に使用
        Vector2 center = (Vector2)transform.position + checkOffset;

        Collider2D hit = Physics2D.OverlapBox(center, checkSize, 0f, cont.climblayers);

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

        int layerID = collision.gameObject.layer; //レイヤーIDを取得
        string layerName = LayerMask.LayerToName(layerID); // 名前に変換

        if (layerName == "String" || layerName == "Gimmick")//インスペクターで設定したLayerとのみ判定を取る 
                                                            //(((1 << collision.gameObject.layer) & cont.groundlayers) != 0) //以前のLayer判定、分かりにくいのでコメントアウト
        {

            if (collision.gameObject.tag == "Kaesi")//返し縫いに当たった時の処理
            {
                cont.PlayerReturn(collision.transform.rotation.y);//プレイヤーの向きを変える

            }

            if (collision.gameObject.tag == "Goal")
            { 
                if(cont.state.currentstate == PlayerState.State.GOAL) { return; } // すでにゴールしていたら何もしない
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

                            if (layerName == "String")// 糸のLayerなら
                                                      //(((1 << collision.gameObject.layer) & cont.climblayers) != 0) //以前のLayer判定、分かりにくいのでコメントアウト
                        {
                            if (cont.cutFg) //糸を切る状態なら、当たった糸を消す
                            {
                                int index = collision.gameObject.GetComponent<StringAnimation_Canvas>().index;
                                stringManager.CutString(index);
                                cont.cutFg = false;
                                return; // 糸を消すだけで終わる
                            }

                            if (cont.state.IS_CLIMB_NG || cont.state.IS_CEILING_HIT) 
                            {
                                wall_obj.Add(collision.gameObject);
                                cont.state.IS_MOVE = false;
                                return; // 登れないなら壁としてカウントするだけ
                            }
                            

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
                                ground_obj.Clear();//地面判定したオブジェクトを全削除
                                return;
                            }
                        }

                        // 段差補正するかの判定
                        Bounds myBounds = m_collider.bounds;
                        Bounds targetBounds = collision.gameObject.GetComponent<BoxCollider2D>().bounds;
                        float playerFootY = myBounds.min.y;
                        float playerHeight = myBounds.size.y;
                        float thresholdY = playerFootY + playerHeight / 4;
                        float topY = targetBounds.max.y;

                        if (topY < thresholdY) // プレイヤーの足元から体高の1/4以内の段差なら補正
                        {
                            float diff = topY - playerFootY;

                            transform.position += new Vector3(0f, diff, 0f);
                            cont.state.IS_GROUND = true;
                            cont.state.IS_MOVE = true;
                            ground_obj.Add(collision.gameObject);

                            return; // 段差補正を行ったら壁としてカウントしない
                        }

                        // 壁に当たった時の処理
                        wall_obj.Add(collision.gameObject);
                        cont.state.IS_MOVE = false;

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
        // 自分のCollider以外は無視
        if (collider != m_collider) return;

        int layerID = collider.gameObject.layer; //レイヤーIDを取得
        string layerName = LayerMask.LayerToName(layerID); // 名前に変換

        if (layerName == "String")
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

        int layerID = collider.gameObject.layer; //レイヤーIDを取得
        string layerName = LayerMask.LayerToName(layerID); // 名前に変換

        if (!cont.ishit)
        {//OverlapBoxが重なってないときに実行(誤作動するため)
            if (layerName == "String")
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
