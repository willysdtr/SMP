using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private PlayerController cont;  // PlayerControllerを参照してステートを制御

    private HashSet<GameObject> ground_obj = new HashSet<GameObject>(); // 接地中のオブジェクト
    private HashSet<GameObject> wall_obj = new HashSet<GameObject>();   // 壁接触中のオブジェクト

    [SerializeField] private Vector2 checkSize = new Vector2(0.5f, 1.0f);   // OverlapBox のサイズ
    [SerializeField] private Vector2 checkOffset = new Vector2(0f, 0f);     // OverlapBox の中心位置オフセット

    [SerializeField] private StringManager_Canvas stringManager; // 糸の管理スクリプト（Canvas上のUIと連携）

    private Rigidbody2D rb;
    private RectTransform rect;
    private BoxCollider2D m_collider;

    private bool wallhit = false; // 壁に当たっているか（ジャンプ中の跳ね返り判定用）
    private bool jumphit = false; //ばねに当たっているか
    private float setdiff = 0.0f; // 段差補正で加えたY軸差分の保存

    void Start()
    {
        cont = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        rect = GetComponent<RectTransform>();
        m_collider = GetComponent<BoxCollider2D>();

        // RectTransformのサイズに合わせてOverlapBoxサイズを調整
        checkSize = new Vector2(checkSize.x * rect.sizeDelta.x, checkSize.y * rect.sizeDelta.y);
    }

    void Update()
    {
        // OverlapBoxの作成（Climb判定用）
        Vector2 center = (Vector2)transform.position + checkOffset;
        Collider2D hit = Physics2D.OverlapBox(center, checkSize, 0f, cont.climblayers);
        cont.ishit = hit;

        //壁ヒット判定と段差補正値をリセット(ジャンプ中の処理に使用)
        wallhit = false;
        setdiff = 0.0f;
        //ばねヒット判定をリセット
        jumphit = false;
    }

    private void OnDrawGizmos()
    {
        // OverlapBox の可視化（デバッグ表示）
        Gizmos.color = Color.red;
        Vector2 center = (Vector2)transform.position + checkOffset;
        Gizmos.DrawWireCube(center, checkSize);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        int layerID = collision.gameObject.layer;
        string layerName = LayerMask.LayerToName(layerID);

        // String または Gimmick レイヤーのみに反応
        if (layerName == "String" || layerName == "Gimmick")
        {
            if (collision.gameObject.tag == "SeeSaw") { 
                cont.state.IS_MOVE = true; 
                cont.state.IS_GROUND = true;

            }
            // 返し縫いに当たった場合(没)
            if (collision.gameObject.tag == "Kaesi")
            {
                cont.PlayerReturn(collision.transform.rotation.y);
            }

            // カッターに当たった場合
            if (collision.gameObject.tag == "Cutter")
            {
                //stringManager.CutNum += 1;               // カット数を増やす
                collision.gameObject.SetActive(false);   // カッターを非表示
                cont.cutCt++;                            // 糸を切れる回数を増加
                return;                                  // これ以降の処理を行わない(壁判定に引っかかるため)
            }

            // 針山に当たった場合、死亡
            if (collision.gameObject.tag == "PinCuttion")
            {
                cont.state.IS_DOWN = true;
                return;
            }

            if (collision.gameObject.tag == "Spring") //ばねに接触した場合
            {
                // バネに乗ったときの処理
                transform.position = new Vector2(collision.transform.position.x, transform.position.y);
                //ジャンプ中なら、STOP状態に変更する、これにより再びジャンプする
                if (cont.state.IS_JUMP)
                {
                    cont.state.currentstate = PlayerState.State.STOP;
                }
                cont.state.IS_JUMP = true;
                cont.state.IS_MOVE = false;
                cont.state.IS_GROUND = false;
                jumphit = true;
                ground_obj.Clear(); // 接地オブジェクトリセット
                return;
            }

            // ゴールに接触した場合
            if (collision.gameObject.tag == "Goal")
            {
                if (cont.state.currentstate == PlayerState.State.GOAL) return; // 既にゴール状態なら無視
                cont.Goal(collision.transform.position);
            }
            else
            {
                // 通常の地形または壁との衝突処理
                foreach (ContactPoint2D contact in collision.contacts)
                {
                    // 上方向（床）との接触
                    if (Vector2.Angle(contact.normal, Vector2.up) < 20f)
                    {
                       // 通常の床に接地したとき
                       cont.state.IS_GROUND = true;
                       cont.state.IS_MOVE = true;
                       cont.state.IS_JUMP = false;
                       ground_obj.Add(collision.gameObject);
                       return;
                    }

                    // 横方向（壁）との接触
                    if (contact.normal == Vector2.left || contact.normal == Vector2.right)
                    {
                        // 糸レイヤーの壁に接触した場合（ジャンプ中は除外）
                        if (layerName == "String" && !cont.state.IS_JUMP)
                        {
                            if (cont.cutCt > 0) // 糸を切れる回数がある場合
                            {

                                bool front = collision.gameObject.GetComponent<StringAnimation_Canvas>().front;
                                int index = collision.gameObject.GetComponent<StringAnimation_Canvas>().index;
                                stringManager.CutString(index,front);
                                cont.cutCt--;
                                cont.state.IS_GROUND = true;
                                return;
                            }

                            bool isVertical = collision.transform.rotation.z != 0;

                            // 垂直な糸に接触 → 登り処理
                            if (isVertical && !(cont.state.IS_CLIMB_NG || cont.state.IS_CEILING_HIT || cont.state.IS_JUMP))
                            {
                                GetComponent<BoxCollider2D>().isTrigger = true;
                                cont.state.IS_MOVE = false;
                                cont.state.IS_CLIMB = true;
                                cont.hitobj_pos = collision.transform.position;
                                rb.linearVelocity = Vector2.zero;
                                rb.bodyType = RigidbodyType2D.Kinematic;
                                ground_obj.Clear(); // 接地オブジェクトリセット
                                return;
                            }
                        }

                        // 段差補正処理
                        Bounds myBounds = m_collider.bounds;
                        Bounds targetBounds = collision.gameObject.GetComponent<BoxCollider2D>().bounds;
                        float playerFootY = myBounds.min.y;
                        float playerHeight = myBounds.size.y;
                        float thresholdY = playerFootY + playerHeight / 2;
                        float topY = targetBounds.max.y;

                        // プレイヤーの足元が段の上端よりやや下 → 段差補正
                        if (topY < thresholdY && !wallhit)
                        {
                            float diff = topY - playerFootY;
                            transform.position += new Vector3(0f, diff, 0f);
                            setdiff = diff;
                            cont.state.IS_GROUND = true;
                            cont.state.IS_MOVE = true;
                            cont.state.IS_JUMP = false;
                            ground_obj.Add(collision.gameObject);
                            return;
                        }
                        else
                        {
                            // 段差補正失敗 → 壁衝突扱い
                            wallhit = true;
                            if (setdiff != 0 && cont.state.currentstate == PlayerState.State.JUMP)
                            {
                                transform.position -= new Vector3(0f, setdiff, 0f);
                                cont.state.IS_GROUND = false;
                                cont.state.IS_MOVE = false;
                                cont.state.IS_JUMP = true;
                            }

                            // ジャンプ中に壁へ衝突した場合 → 跳ね返り処理
                            if (cont.state.IS_JUMP)
                            {
                                cont.PlayerJumpReturn();
                                return;
                            }
                        }

                        // 登り禁止または天井ヒット中の壁衝突 → 方向反転
                        if ((cont.state.IS_CLIMB_NG || cont.state.IS_CEILING_HIT) && !cont.state.IS_JUMP)
                        {
                            if (contact.normal == Vector2.left)
                            {
                                cont.PlayerReturn(0);
                                return;
                            }
                            else if (contact.normal == Vector2.right)
                            {
                                cont.PlayerReturn(-180);
                                return;
                            }
                        }

                        // 壁に接触している状態(現状使われる場面無し)
                        //wall_obj.Add(collision.gameObject);
                        //cont.state.IS_MOVE = false;
                    }
                }
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // 接触オブジェクトを削除
        ground_obj.Remove(collision.gameObject);
        wall_obj.Remove(collision.gameObject);

        //シーソーから離れた時の処理
        //if (collision.gameObject.tag == "SeeSaw")
        //{
        //    //シーソーのy角度とプレイヤーの向きが正しければ、シーソーを反対側にする処理
        //    if ((collision.gameObject.transform.eulerAngles.y == 180) && cont.state.m_direction == (int)PlayerState.Direction.RIGHT)
        //    {
        //        collision.gameObject.transform.eulerAngles = new Vector3(transform.rotation.x, 0, transform.rotation.z);
        //    }
        //    else if ((collision.gameObject.transform.eulerAngles.y == 0) && cont.state.m_direction == (int)PlayerState.Direction.LEFT)
        //    {
        //        collision.gameObject.transform.eulerAngles = new Vector3(transform.rotation.x, 180, transform.rotation.z);
        //    }
        //}

        // 全ての床から離れた場合
        if (ground_obj.Count == 0)
        {
            cont.state.IS_GROUND = false;
        }

        // 全ての壁から離れた場合
        if (wall_obj.Count == 0)
        {
            cont.state.IS_MOVE = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        // 自身のCollider以外なら処理しない
        if (collider != m_collider || cont.cutCt > 0) return;

        int layerID = collider.gameObject.layer;
        string layerName = LayerMask.LayerToName(layerID);

        if (layerName == "String")
        {
            // 糸に接触 → 登り開始
            cont.state.IS_MOVE = false;
            cont.state.IS_CLIMB = true;
            cont.hitobj_pos = collider.transform.position;
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        int layerID = collider.gameObject.layer;
        string layerName = LayerMask.LayerToName(layerID);

        // OverlapBoxの判定が消えた場合のみ処理（誤判定防止）
        if (!cont.ishit)
        {
            if (layerName == "String")
            {
                // 糸から離れたときの処理
                cont.state.IS_MOVE = true;
                cont.state.IS_CLIMB = false;
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.linearVelocity = Vector2.zero;
                GetComponent<BoxCollider2D>().isTrigger = false;
            }
        }
    }
}
