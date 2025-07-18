using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //プレイヤーのステートをいじる

    PlayerMove move;
    PlayerState state;
    private Animator anim = null;
    private float jumptime = 0.0f;//ジャンプ時間

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        move = GetComponent<PlayerMove>();
        state = GetComponent<PlayerState>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        ChangeState();
        HandleState();
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
                if (!state.IS_GROUND || !state.IS_CLIMB || !!state.IS_MOVE) { state.currentstate = PlayerState.State.STOP; move.Stop(); }
                break;
            case PlayerState.State.CLIMB://CLIMB
                if (!state.IS_CLIMB) { state.currentstate = PlayerState.State.STOP; }
                break;
            case PlayerState.State.JUMP://JUMP
                if (!state.IS_CEILING_HIT || (state.IS_GROUND && jumptime > PlayerState.jumptime_max)) { state.currentstate = PlayerState.State.STOP; move.EndJump(); }
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
                transform.position = new Vector2(state.hitobj_pos.x, transform.position.y);
            }
            else
            {
                if (state.IS_JUMP && !state.IS_CEILING_HIT)//JUMP状態への移行
                {
                    //ジャンプ初期化を行い、フラグをオフにする事で、一度だけ実行する様にしている
                    move.Stop();
                    move.InitJump(state.initialVelocity);
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
            case PlayerState.State.WALK: anim.speed = 1; move.Move(); break;
            case PlayerState.State.JUMP: anim.speed = 0; move.Jump(); jumptime += Time.deltaTime; break;
            case PlayerState.State.FALL: anim.speed = 0; break;
            case PlayerState.State.CLIMB: anim.speed = 1; move.Climb(PlayerState.MAX_SPEED / 2); break;
            case PlayerState.State.GOAL: anim.speed = 1; break;
            case PlayerState.State.DEATH: anim.speed = 1; break;
        }
        anim.SetInteger("State", (int)state.currentstate);
    }
}
