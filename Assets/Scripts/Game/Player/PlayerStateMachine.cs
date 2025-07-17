using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public enum PlayerState { STOP, WALK, JUMP, FALL, CLIMB, GOAL, DEATH }
public class PlayerStateMachine : MonoBehaviour
{
    //プレイヤーの状態管理スクリプト
    public PlayerState currentstate { get; private set; } = PlayerState.STOP;
    public enum Direction { LEFT = -1, STOP = 0, RIGHT = 1 };
    Transform m_transform;
    PlayerMove move;
    PlayerJump jump;
    PlayerClimb climb;

    private bool moveFg = true;
    private bool jumpFg;
    private bool isground;
    private bool downFg;
    private bool ceiling_hit;
    private bool ngFg;
    private bool climbFg;
    private bool gimjumpFg;

    [SerializeField]
    [Header("ジャンプを行う最低時間")]
    private const float jumptime_max = 0.1f;//ジャンプを行う最低時間
    private float jumptime = 0.0f;//ジャンプ時間

    private Vector2 hitobj_pos;
    Vector2 initialVelocity;//ジャンプで渡す用

    [SerializeField]
    [Header("当たり判定を取るレイヤー")]
    private LayerMask GroundLayers;

    private Animator anim = null;
    public LayerMask groundlayers => GroundLayers;
    public int direction { get; private set; } = 1;
    private void Start()
    {
        move = GetComponent<PlayerMove>();
        jump = GetComponent<PlayerJump>();
        climb = GetComponent<PlayerClimb>();
        anim = GetComponent<Animator>();
        m_transform = this.transform;
    }

    private void Update()
    {
        ChangeState();
        HandleState();
    }

    public void ChangeState()
    {
        if (downFg)
        {//プレイヤーがやられるかの判定
            currentstate = PlayerState.DEATH;
            return;
        }

        switch (currentstate)//現在の状態から停止に移行するか
        {
            case PlayerState.WALK://WALK
                if (!isground || climbFg || !moveFg) { currentstate = PlayerState.STOP; move.Stop(); }
                break;
            case PlayerState.CLIMB://CLIMB
                if(!climbFg){ currentstate = PlayerState.STOP; }
                break;
            case PlayerState.JUMP://JUMP
                if(ceiling_hit || (isground && jumptime > jumptime_max)){ currentstate = PlayerState.STOP; jump.EndJump(); }
                break;
            case PlayerState.FALL://FALL
                if(isground){ currentstate = PlayerState.STOP; }
                break;
        }

        if (currentstate != PlayerState.STOP) { return; }//停止状態でなければ状態変化処理を行わない
        if (!ngFg)
        {//移動不可フラグがオンなら、停止後、落下状態以外に移行しない
            if (climbFg)//CLIMB状態への移行
            {
                currentstate = PlayerState.CLIMB;
                transform.position = new Vector2(hitobj_pos.x, transform.position.y);
            }
            else
            {
                if (jumpFg && !ceiling_hit)//JUMP状態への移行
                {
                    //ジャンプ初期化を行い、フラグをオフにする事で、一度だけ実行する様にしている
                    move.Stop();
                    jump.InitJump(initialVelocity);
                    currentstate = PlayerState.JUMP;
                    jumptime = 0.0f;
                    jumpFg = false;

                }
                else if (moveFg)//WALK状態への移行
                {
                    currentstate = PlayerState.WALK;
                }
            }
        }

        //落下フラグがオンで、落下中でなく、ジャンプ中でもなければ
        if (!isground) {
            currentstate = PlayerState.FALL;// 落下開始
        }
    }

    public void HandleState()//状態ごとのUpdate処理
    {
        switch (currentstate)
        {
            case PlayerState.STOP: anim.speed = 0; move.Stop(); break;
            case PlayerState.WALK:  anim.speed = 1; move.Move(); break;
            case PlayerState.JUMP: anim.speed = 0; jump.Jump(); jumptime += Time.deltaTime; break;
            case PlayerState.FALL: anim.speed = 0;  break;
            case PlayerState.CLIMB: anim.speed = 1;climb.Climb(move.maxspeed_read / 2); break;
            case PlayerState.GOAL: anim.speed = 1; break;
            case PlayerState.DEATH: anim.speed = 1; break;
        }
        anim.SetInteger("State", (int)currentstate);
    }

    public void SetClimbFg(bool fg)
    {
        climbFg = fg;
    }

    public void SetJumpFg(bool fg)
    {
        jumpFg = fg;
    }

    public void SetGimJumpFg(bool fg)
    {
        gimjumpFg = fg;
    }

    public void SetCelingHit(bool fg)
    {
        ceiling_hit = fg;
    }

    public void SetMoveFg(bool fg)
    {
        moveFg = fg;
    }

    public void SetNgFg(bool fg)
    {
        ngFg = fg;
    }

    public void SetIsGround(bool fg)
    {
        isground = fg;
    }

    public void SetDirection(int _di)
    {
        direction = _di;
        if (direction == 1)
        {
            m_transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (direction == -1)
        {
            m_transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }

    public void DirectionReturn()
    {
        direction *= -1;
        m_transform.Rotate(0, 180, 0);
    }
    public void SetHitObjPos(Vector2 pos)
    {
        hitobj_pos = pos;
    }

    public void SetInitVelocity(Vector2 vec)
    {
        initialVelocity = vec;
    }
}
