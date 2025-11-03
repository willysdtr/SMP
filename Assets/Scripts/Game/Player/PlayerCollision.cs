using System.Collections.Generic;
//using UnityEditor;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    //定数
    private const float FloorAngle = 20f; // 上向き法線 床kakudo

    
    private PlayerController m_Cont;            // PlayerControllerを参照してステートを制御

    [SerializeField] private StringManager_Canvas m_StringManager; // 糸管理: Canvas上

    /* 判定 */
    [SerializeField] private Vector2 m_CheckSize = new Vector2(0.5f, 1.0f);   // OverlapBox のサイズ
    [SerializeField] private Vector2 m_CheckOffset = new Vector2(0f, 0f);     // OverlapBox の中心位置オフセット

    /* コンポーネント */
    private Rigidbody2D m_Rb;
    private RectTransform m_Rect;
    private BoxCollider2D m_Collider;

    /*===== 状態（衝突中オブジェクトなど） =====*/
    private readonly HashSet<GameObject> m_RroundObj = new HashSet<GameObject>();
    private readonly HashSet<GameObject> m_WallBbj = new HashSet<GameObject>();

    private bool Wallhit = false; // 壁に当たっているか（ジャンプ中の跳ね返り判定用）
    private bool Jumphit = false; // ばねに当たっているか
    private float Setdiff = 0.0f; // 段差補正で加えたY軸差分の保存

    //=====================================================================
    // Start()
    //=====================================================================
    private void Start()
    {
        m_Cont = GetComponent<PlayerController>();
        m_Rb = GetComponent<Rigidbody2D>();
        m_Rect = GetComponent<RectTransform>();
        m_Collider = GetComponent<BoxCollider2D>();

        // RectTransform 基準
        // OverlapBox 実寸調整
        m_CheckSize = new Vector2(m_CheckSize.x * m_Rect.sizeDelta.x, m_CheckSize.y * m_Rect.sizeDelta.y);
    }

    //=====================================================================
    // Update()
    //=====================================================================
    private void Update()
    {
        if (PauseApperance.Instance.isPause || (SoundChangeSlider.Instance != null && SoundChangeSlider.Instance.IsSoundChange))
        {
            if (m_Rb.bodyType != RigidbodyType2D.Kinematic)
            {
                m_Rb.simulated = false;
               
            }

            return;
        }
        else
        {
            if (m_Rb.bodyType != RigidbodyType2D.Kinematic)
            {
                m_Rb.simulated = true;
            }
        }//ポーズ中は動かないようにする

        // Climb: OverlapBox -> m_Cont.ishit 直接代入
        Vector2 center = (Vector2)transform.position + m_CheckOffset;
        Collider2D hit = Physics2D.OverlapBox(center, m_CheckSize, 0f, m_Cont.climblayers);
        m_Cont.ishit = hit;

        // 毎フレームの初期化
        Wallhit = false;
        Setdiff = 0.0f;
        Jumphit = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 center = (Vector2)transform.position + m_CheckOffset;
        Gizmos.DrawWireCube(center, m_CheckSize);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var go = collision.gameObject;
        int layerID = go.layer;
        string layerName = LayerMask.LayerToName(layerID);

        // String と Gimmick 以外は無視
        if (!(layerName == "String" || layerName == "Gimmick"))
            return;
        
        // タグ即時処理
        if (HandleImmediateTagCollisions(collision))
            return;
        

        // ゴール処理
        if (go.CompareTag("Goal"))
        {
            if (m_Cont.state.currentstate != PlayerState.State.GOAL)
                m_Cont.Goal(collision.transform.position);

            //段差補正
            Bounds myBounds = m_Collider.bounds;
            var targetCollider = collision.gameObject.GetComponent<BoxCollider2D>();
            Bounds targetBounds = targetCollider.bounds;

            float playerFootY = myBounds.min.y;
            float topY = targetBounds.max.y;

            float diff = topY - playerFootY;
            transform.position += new Vector3(m_Cont.state.m_direction * PlayerState.MAX_SPEED * 0.1f, diff, 0f);

            m_Rb.bodyType = RigidbodyType2D.Kinematic;

            return;
        }

        // 通常床, 壁, 段差, 登り
        HandleContacts(collision, layerName);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        m_RroundObj.Remove(collision.gameObject);
        m_WallBbj.Remove(collision.gameObject);

        // 全床から離れたら IS_GROUND -> OFF
        if (m_RroundObj.Count == 0)
            m_Cont.state.IS_GROUND = false;
        

        // 全壁から離れたら移動できる？
        if (m_WallBbj.Count == 0)
            m_Cont.state.IS_MOVE = true;

        ExitTagCollisions(collision);
        
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        // 自Collider 以外ならreturn & 糸カット可能なら無視
        if (collider != m_Collider || m_Cont.cutCt > 0) return;

        int layerID = collider.gameObject.layer;
        string layerName = LayerMask.LayerToName(layerID);

        if (layerName == "String")
            BeginClimb(collider.transform.position);
        
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        string layerName = LayerMask.LayerToName(collider.gameObject.layer);

        // OverlapBox に触れ消えたときだけ離脱処理
        if (!m_Cont.ishit)
            EndClimb();
    }

    //======================================================================
    // タグ即時処理
    //======================================================================
    private bool HandleImmediateTagCollisions(Collision2D collision)
    {
        var go = collision.gameObject;

        // SeeSaw: 地面
        if (go.CompareTag("SeeSaw"))
        {
            m_Cont.state.IS_MOVE = true;
            m_Cont.state.IS_GROUND = true;
            m_Cont.state.IS_JUMP = false;
            m_Rb.linearVelocity = new Vector2(m_Cont.state.m_direction * PlayerState.MAX_SPEED, m_Rb.linearVelocity.y);
            return false;
        }

        // 返し縫い
        //if (go.CompareTag("Kaesi"))
        //{
        //    m_Cont.PlayerReturn(collision.transform.rotation.y);
        //}

        // カッター 回数+1, 非表示, 以降ない
        if (go.CompareTag("Cutter"))
        {
            // m_StringManager.CutNum += 1;
            go.SetActive(false);
            m_Cont.cutCt++;
            return true;
        }

        // 針山
        if (go.CompareTag("PinCuttion"))
        {
            m_Cont.state.IS_DOWN = true;
            return true;
        }

        // ばね
        if (go.CompareTag("Spring"))
        {
            HandleSpringCollision(collision);
            return true;
        }

        return false;
    }

    //======================================================================
    // 衝突(床, 壁, 登り, 段差
    //======================================================================
    private void HandleContacts(Collision2D collision, string layerName)
    {
        foreach (var contact in collision.contacts)
        {
            // 床判定 
            if (IsFloorContact(contact))
            {
                LandOnFloor(collision.gameObject);
                return;
            }

            // 壁判定
            if (IsWallNormal(contact.normal))
            {
                // 糸レイヤーの壁触: ジャンプ中除外
                if (layerName == "String" && !m_Cont.state.IS_JUMP)
                {
                    // 糸を切れる場合、切る: 優先
                    if (TryCutString(collision.gameObject))
                    {
                        m_Cont.state.IS_GROUND = true;
                        m_Cont.PlaySE(3,true);
                        return;
                    }

                    // 垂直糸なら [登り] Start: 禁止, 天井, ジャンプ中、落下中でないなら
                    if (IsVerticalString(collision) &&
                        !(m_Cont.state.IS_CLIMB_NG || m_Cont.state.IS_CEILING_HIT || m_Cont.state.IS_JUMP || !m_Cont.state.IS_GROUND))
                    {
                        BeginClimb(collision.transform.position);
                        m_RroundObj.Clear();
                        return;
                    }
                }

                // === 段差補正 ===
                if (TryStepCorrection(collision)) return;

                // 壁衝突扱い
                Wallhit = true;

                // 直前に段差持ち上げなら、戻す
                if (Setdiff != 0f && m_Cont.state.currentstate == PlayerState.State.JUMP)
                {
                    transform.position -= new Vector3(0f, Setdiff, 0f);
                    m_Cont.state.IS_GROUND = false;
                    m_Cont.state.IS_MOVE = false;
                    m_Cont.state.IS_JUMP = true;
                }

                // ジャンプ中の壁衝突 -> 跳ね返り
                if (m_Cont.state.IS_JUMP)
                {
                    m_Cont.PlayerJumpReturn();
                    return;
                }

                // 登り禁止 or 天井ヒット中なら、方向反転: ジャンプ中除外
                if ((m_Cont.state.IS_CLIMB_NG || m_Cont.state.IS_CEILING_HIT) && !m_Cont.state.IS_JUMP)
                {
                    if (contact.normal == Vector2.left)
                    {
                        m_Cont.PlayerReturn(0);
                        return;
                    }
                    else if (contact.normal == Vector2.right)
                    {
                        m_Cont.PlayerReturn(-180);
                        return;
                    }
                }
            }
        }
    }

    //======================================================================
    // 個別処理ヘルパー
    //======================================================================*/
    private void HandleSpringCollision(Collision2D collision)
    {
        // プレイヤーとスプリングの BoxCollider2D を取得
        BoxCollider2D playerCol = GetComponent<BoxCollider2D>();
        BoxCollider2D springCol = collision.collider.GetComponent<BoxCollider2D>();

        if (playerCol == null || springCol == null)
        {
            Debug.LogWarning("BoxCollider2D が足りません。");
            return;
        }

        // スプリングの上端のワールド座標を取得
        Vector2 springTop = GetColliderTop(springCol);

        // プレイヤーの下端のワールド座標を取得
        Vector2 playerBottom = GetColliderBottom(playerCol);

        // 現在の差分（プレイヤー足元がスプリング上面より下にある分）
        float offsetY = springTop.y - playerBottom.y;

        // プレイヤーを上方向にスナップ（落下速度に依存しない）
        transform.position = new Vector2(transform.position.x, transform.position.y + offsetY);

        // 横位置をばねのXにスナップ
        transform.position = new Vector2(collision.transform.position.x, transform.position.y);


        // 横位置をばねにスナップ
        transform.position = new Vector2(collision.transform.position.x, transform.position.y);

        // ジャンプ中なら STOP を挟む -> その後に再ジャンプ
        //if (m_Cont.state.IS_JUMP)
            m_Cont.state.currentstate = PlayerState.State.STOP;

        m_Cont.state.IS_JUMP = true;
        m_Cont.state.IS_MOVE = false;
        m_Cont.state.IS_GROUND = false;

        Jumphit = true;
        m_RroundObj.Clear();
    }

    private void LandOnFloor(GameObject floor)
    {
        m_Cont.state.IS_GROUND = true;
        m_Cont.state.IS_MOVE = true;
        m_Cont.state.IS_JUMP = false;
        m_RroundObj.Add(floor);
    }

    private bool TryCutString(GameObject stringObject)
    {
        if (m_Cont.cutCt <= 0) return false;
        
        var anim = stringObject.GetComponent<StringAnimation_Canvas>();

        m_StringManager.CutString(anim.index, anim.front,anim.firstct);
        m_Cont.cutCt--;
        return true;
    }

    private static bool IsVerticalString(Collision2D collision)
    {
        // rotation.z != 0: 垂直
        return collision.transform.rotation.z != 0f;
    }

    private bool TryStepCorrection(Collision2D collision)
    {
        Bounds myBounds = m_Collider.bounds;
        var targetCollider = collision.gameObject.GetComponent<BoxCollider2D>();
        Bounds targetBounds = targetCollider.bounds;

        float playerFootY = myBounds.min.y;
        float playerHeight = myBounds.size.y;
        float thresholdY = playerFootY + playerHeight * 0.8f; // プレイヤー高さの0.8上
        float topY = targetBounds.max.y;

        // 段の上端がプレイヤー足元より少し低い -> 段差補正
        if (topY < thresholdY && !Wallhit)
        {
            float diff = topY - playerFootY;
            transform.position += new Vector3(m_Cont.state.m_direction * PlayerState.MAX_SPEED * 0.1f, diff, 0f);
            Setdiff = diff;

            m_Cont.state.IS_GROUND = true;
            m_Cont.state.IS_MOVE = true;
            m_Cont.state.IS_JUMP = false;

            m_RroundObj.Add(collision.gameObject);
            return true;
        }

        return false;
    }

    private void BeginClimb(Vector3 anchorPosition)
    {
        // Trigger化, Kinematic化
        m_Collider.isTrigger = true;
        m_Cont.state.IS_MOVE = false;
        m_Cont.state.IS_CLIMB = true;
        m_Cont.hitobj_pos = anchorPosition;
        m_Rb.linearVelocity = Vector2.zero;
        m_Rb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void EndClimb()
    {
        m_Cont.state.IS_MOVE = true;
        m_Cont.state.IS_CLIMB = false;
        m_Rb.bodyType = RigidbodyType2D.Dynamic;
        m_Rb.linearVelocity = Vector2.zero;
        m_Collider.isTrigger = false;
    }

    private static bool IsFloorContact(in ContactPoint2D contact)
    {
        return Vector2.Angle(contact.normal, Vector2.up) < FloorAngle;
    }

    private static bool IsWallNormal(in Vector2 n)
    {
        // 左右比較
        return n == Vector2.left || n == Vector2.right;
    }

    // BoxCollider2D の上端ワールド座標
    private Vector2 GetColliderTop(BoxCollider2D box)
    {
        Vector2 size = box.size;
        Vector2 offset = box.offset;
        Vector2 localTop = offset + new Vector2(0, size.y / 2);
        return box.transform.TransformPoint(localTop);
    }

    // BoxCollider2D の下端ワールド座標
    private Vector2 GetColliderBottom(BoxCollider2D box)
    {
        Vector2 size = box.size;
        Vector2 offset = box.offset;
        Vector2 localBottom = offset + new Vector2(0, -size.y / 2);
        return box.transform.TransformPoint(localBottom);
    }

    //======================================================================
    // Exitした時のタグ即時処理
    //======================================================================
    private void ExitTagCollisions(Collision2D collision)
    {
        var go = collision.gameObject;
       
        if (go.CompareTag("SeeSaw"))
        {
            m_Rb.linearVelocity = new Vector2(0, m_Rb.linearVelocity.y);
            return;
        }
    }

}
