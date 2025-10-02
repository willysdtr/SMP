using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI.Table;

public class PlayerController : MonoBehaviour
{
    //プレイヤーのステートをいじる
    private InputSystem_Actions inputActions;
    PlayerMove move;
    PlayerState state;
    private Animator anim = null;
    private float jumptime = 0.0f;//ジャンプ時間

    private HashSet<GameObject> ground_obj = new HashSet<GameObject>();
    private HashSet<GameObject> wall_obj = new HashSet<GameObject>();

    [SerializeField] private Vector2 checkSize = new Vector2(0.5f, 1.0f);
    [SerializeField] private Vector2 checkOffset = new Vector2(0f, 0f);
    [SerializeField] private LayerMask climbLayer;
    public Vector2 hitobj_pos { get; private set; } = new Vector2(0.0f, 0.0f);
    private Vector2 start_pos = Vector2.zero;
    private Vector2 goal_pos = Vector2.zero;
    private bool ishit;

    private int direction = (int)PlayerState.Direction.RIGHT;

    [SerializeField] public LayerMask groundlayers;

    Vector2 initialVelocity;

    private Rigidbody2D rb;
    private RectTransform rect;
    private bool start = false;
    public bool goal = false;

    void Awake()
    {
        move = GetComponent<PlayerMove>();
        state = new PlayerState(groundlayers);
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        rect = GetComponent<RectTransform>();
        anim.speed = 0;
        inputActions = new InputSystem_Actions();
        checkSize = new Vector2(checkSize.x * rect.sizeDelta.x, checkSize.y * rect.sizeDelta.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (!start || goal) { return; }
        ChangeState();
        HandleState();
        //OverlapBoxの作成、Climb処理に使用
        Vector2 center = (Vector2)transform.position + checkOffset;

        Collider2D hit = Physics2D.OverlapBox(center, checkSize, 0f, climbLayer);

        ishit = hit;
    }

    public void ChangeState()
    {
        if (state.IS_DOWN)
        {//プレイヤーがやられるかの判定
            state.currentstate = PlayerState.State.DEATH;
            return;
        }

        switch (state.currentstate)//現在の状態から停止に移行するか
        {
            case PlayerState.State.WALK://WALK
                if (!state.IS_GROUND || !state.IS_CLIMB || !state.IS_MOVE) { state.currentstate = PlayerState.State.STOP; move.Stop(); }
                break;
            case PlayerState.State.CLIMB://CLIMB
                if (!state.IS_CLIMB) { state.currentstate = PlayerState.State.STOP; }
                break;
            case PlayerState.State.JUMP://JUMP
                if (state.IS_CEILING_HIT || (state.IS_GROUND && jumptime > PlayerState.jumptime_max)) { state.currentstate = PlayerState.State.STOP; move.EndJump();}
                break;
            case PlayerState.State.FALL://FALL
                if (state.IS_GROUND) { state.currentstate = PlayerState.State.STOP; }
                break;
        }

        if (state.currentstate != PlayerState.State.STOP) { return; }//停止状態でなければ状態変化処理を行わない
        if (!state.IS_CLIMB_NG)
        {//移動不可フラグがオンなら、停止後、落下状態以外に移行しない
            if (state.IS_CLIMB)//CLIMB状態への移行
            {
                state.currentstate = PlayerState.State.CLIMB;
                transform.position = new Vector2(hitobj_pos.x, transform.position.y);
            }
            else
            {
                if (state.IS_JUMP && !state.IS_CEILING_HIT)//JUMP状態への移行
                {
                    //ジャンプ初期化を行い、フラグをオフにする事で、一度だけ実行する様にしている
                    move.Stop();
                    move.InitJump(initialVelocity);
                    state.currentstate = PlayerState.State.JUMP;
                    jumptime = 0.0f;
                    state.IS_JUMP = false;

                }
                else if (state.IS_MOVE)//WALK状態への移行
                {
                    state.currentstate = PlayerState.State.WALK;
                }
            }
        }

        //落下フラグがオンで、落下中でなく、ジャンプ中でもなければ
        if (!state.IS_GROUND)
        {
            state.currentstate = PlayerState.State.FALL;// 落下開始
        }
    }

    public void HandleState()//状態ごとのUpdate処理
    {
        switch (state.currentstate)
        {
            case PlayerState.State.STOP: anim.speed = 0; move.Stop(); break;
            case PlayerState.State.WALK: anim.speed = 1; move.Move(direction); break;
            case PlayerState.State.JUMP: anim.speed = 0; move.Jump(); jumptime += Time.deltaTime; break;
            case PlayerState.State.FALL: anim.speed = 0; break;
            case PlayerState.State.CLIMB: anim.speed = 1; move.Climb(PlayerState.MAX_SPEED / 2); break;
            case PlayerState.State.GOAL: anim.speed = 1; if (move.Goal(goal_pos)) { goal = true; anim.speed = 0; } break;
            case PlayerState.State.DEATH: anim.speed = 1; break;
        }
        anim.SetInteger("State", (int)state.currentstate);
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

        if (((1 << collision.gameObject.layer) & groundlayers) != 0)//インスペクターで設定したLayerとのみ判定を取る
        {

            if (collision.gameObject.tag == "Spring")//ばねに当たった時の処理
            {
                //予測線スクリプトがあれば、処理を実行
                JumpLine pad = collision.gameObject.GetComponent<JumpLine>();
                if (pad != null)
                {
                    initialVelocity = pad.GetInitialVelocity();
                    transform.position = new Vector3(collision.transform.position.x, transform.position.y, 0);
                    state.IS_JUMP = true;
                    state.IS_GIMJUMP = true;
                    state.IS_MOVE = false;
                }
            }
            else if(collision.gameObject.tag == "Goal")
            {
                state.currentstate = PlayerState.State.GOAL;// ゴール状態に変更
                goal_pos = collision.transform.position;
            }
            else
            {
                foreach (ContactPoint2D contact in collision.contacts)
                {

                    // 上向きに接触した場合のみカウント
                    if (contact.normal == Vector2.up)
                    {
                        state.IS_GROUND = true;
                        state.IS_MOVE = true;
                        ground_obj.Add(collision.gameObject);
                    }
                    // 横向きに接触した場合のみカウント
                    if (contact.normal == Vector2.left || contact.normal == Vector2.right)
                    {
                        if (collision.gameObject.tag == "String" && state.IS_CLIMB_NG == false)
                        {
                            bool isVertical = collision.transform.rotation.z != 0;
                            if (isVertical)
                            {
                                // 縦の糸ならTriggerに切り替え
                                GetComponent<BoxCollider2D>().isTrigger = true;
                                //糸に当たった時の処理
                                state.IS_MOVE = false;
                                state.IS_CLIMB = true;
                                hitobj_pos = collision.transform.position;
                                rb.linearVelocity = Vector2.zero;
                                rb.bodyType = RigidbodyType2D.Kinematic;
                            }
                        }
                        else
                        {
                            wall_obj.Add(collision.gameObject);
                            state.IS_MOVE = false;
                            
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
            state.IS_GROUND = false;
        }

        if (wall_obj.Count == 0)
        {//壁判定したオブジェクトがすべてなくなれば、移動可能にする
            state.IS_MOVE = true;
        }

    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "String")
        {
            //糸に当たった時の処理
            state.IS_MOVE= false;
            state.IS_CLIMB= true;
            hitobj_pos = collider.transform.position;
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
                state.IS_MOVE = true;
                state.IS_CLIMB = false;
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.linearVelocity = Vector2.zero;
                GetComponent<BoxCollider2D>().isTrigger = false;//Trigger解除
            }
        }

    }

    // プレイヤーをCanvasに、アンカー位置とサイズで配置する関数
    public void PlaceAtPosition(RectTransform parent, Vector2 anchoredPos, Vector2 size)
    {
        // 親を設定（第2引数falseでローカル座標維持なし、完全に親基準で位置設定）
        rect.SetParent(parent, false);

        // 親パネル基準のローカル座標（アンカー位置）をセット
        rect.anchoredPosition = anchoredPos;

        Vector2 setScale = new ( size.x / rect.sizeDelta.x, size.y / rect.sizeDelta.y );

        // サイズを合わせる
        rect.sizeDelta = size;

        BoxCollider2D collider = this.GetComponent<BoxCollider2D>();
        collider.size = new Vector2(collider.size.x * setScale.x, collider.size.y * setScale.y);
    }

    void OnEnable()
    {
        // InputSystem 有効化
        inputActions.Enable();

        inputActions.Player.Attack.performed += OnStartPerformed;
    }

    void OnDisable()
    {
        // 無効化 & イベント解除
        inputActions.Player.Attack.performed -= OnStartPerformed;
        inputActions.Disable();
    }

    private void OnStartPerformed(InputAction.CallbackContext ctx)
    {
        if (!start)
        {
            start = true;
            start_pos = transform.position;
        }
        else
        {
            start = false;
            goal = false;
            transform.position = start_pos;
            move.Stop();
            state.currentstate = PlayerState.State.STOP;
            anim.speed = 0;
            ResetFlag();
        }
    }

    private void ResetFlag()
    {
        state.IS_MOVE = false;
        state.IS_CEILING_HIT = false;
        state.IS_CLIMB = false;
        state.IS_DOWN = false;
        state.IS_CLIMB_NG = false;
        state.IS_GROUND = false;
        state.IS_JUMP = false;
    }

    public void SetClimbNG(bool fg)
    {
        state.IS_CLIMB_NG = fg;
    }
}
