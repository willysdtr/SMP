using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerCollision : MonoBehaviour
{
    //プレイヤー本体部分の当たり判定管理スクリプト

    private PlayerStateMachine state_ma;

    private HashSet<GameObject> ground_obj = new HashSet<GameObject>();
    private HashSet<GameObject> wall_obj = new HashSet<GameObject>();

    private Rigidbody2D rb;

    [SerializeField] private Vector2 checkSize = new Vector2(0.5f, 1.0f);
    [SerializeField] private Vector2 checkOffset = new Vector2(0f, 0f);
    [SerializeField] private LayerMask climbLayer;

    public Vector2 hitobj_pos { get; private set; } = new Vector2(0.0f,0.0f);
    private bool ishit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        state_ma = GetComponent<PlayerStateMachine>();
    }

    void FixedUpdate()
    {
        //OverlapBoxの作成、Climb処理に使用
        Vector2 center = (Vector2)transform.position + checkOffset;

        Collider2D hit = Physics2D.OverlapBox(center, checkSize, 0f, climbLayer);
        
        ishit = hit;
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
        
        if (((1 << collision.gameObject.layer) & state_ma.groundlayers) != 0)//インスペクターで設定したLayerとのみ判定を取る
        {
            if(collision.gameObject.tag == "String")
            {
                bool isVertical = Mathf.Abs(collision.transform.up.y) > 0.9f;
                if (isVertical)
                {
                    // 縦のStringならTriggerに切り替え
                    GetComponent<BoxCollider2D>().isTrigger = true;
                }
            }


            if (collision.gameObject.tag == "Spring")//ばねに当たった時の処理
            {
                //予測線スクリプトがあれば、処理を実行
                JumpLine pad = collision.gameObject.GetComponent<JumpLine>();
                if (pad != null)
                {
                    state_ma.SetInitVelocity(pad.GetInitialVelocity());
                    transform.position = new Vector3(collision.transform.position.x, transform.position.y, 0);
                    state_ma.SetJumpFg(true);
                    state_ma.SetGimJumpFg(true);
                    state_ma.SetMoveFg(false);
                }
            }
            else
            {
                foreach (ContactPoint2D contact in collision.contacts)
                {

                    // 上向きに接触した場合のみカウント
                    if (contact.normal == Vector2.up)
                    {
                        state_ma.SetIsGround(true);
                        ground_obj.Add(collision.gameObject);

                    }
                    // 横向きに接触した場合のみカウント
                    if (contact.normal == Vector2.left || contact.normal == Vector2.right)
                    {
                        wall_obj.Add(collision.gameObject);
                        state_ma.SetMoveFg(false);
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
           state_ma.SetIsGround(false);
        }

        if (wall_obj.Count == 0)
        {//壁判定したオブジェクトがすべてなくなれば、移動可能にする
            state_ma.SetMoveFg(true);
        }

    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "String")
        {
            //糸に当たった時の処理
            state_ma.SetMoveFg(false);
            state_ma.SetClimbFg(true);
            state_ma.SetHitObjPos(collider.transform.position);
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (!ishit)
        {//OverlapBoxが重なってないときに実行(誤作動するため)
            if (collider.gameObject.tag == "String")
            {//糸から離れた時の処理
                state_ma.SetClimbFg(false);
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.linearVelocity = Vector2.zero;
                GetComponent<BoxCollider2D>().isTrigger = false;
            }
        }
        
    }

}
