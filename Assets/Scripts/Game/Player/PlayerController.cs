using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //�v���C���[�̃X�e�[�g��������

    PlayerMove move;
    PlayerState state;
    private Animator anim = null;
    private float jumptime = 0.0f;//�W�����v����

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
        {//�v���C���[������邩�̔���
            state.currentstate = PlayerState.State.DEATH;
            return;
        }

        switch (state.currentstate)//���݂̏�Ԃ����~�Ɉڍs���邩
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

        if (state.currentstate != PlayerState.State.STOP) { return; }//��~��ԂłȂ���Ώ�ԕω��������s��Ȃ�
        if (!state.IS_CLIMB_NG)
        {//�ړ��s�t���O���I���Ȃ�A��~��A������ԈȊO�Ɉڍs���Ȃ�
            if (state.IS_CLIMB)//CLIMB��Ԃւ̈ڍs
            {
                state.currentstate = PlayerState.State.CLIMB;
                transform.position = new Vector2(state.hitobj_pos.x, transform.position.y);
            }
            else
            {
                if (state.IS_JUMP && !state.IS_CEILING_HIT)//JUMP��Ԃւ̈ڍs
                {
                    //�W�����v���������s���A�t���O���I�t�ɂ��鎖�ŁA��x�������s����l�ɂ��Ă���
                    move.Stop();
                    move.InitJump(state.initialVelocity);
                    state.currentstate = PlayerState.State.JUMP;
                    jumptime = 0.0f;
                    state.IS_JUMP = false;

                }
                else if (state.IS_MOVE)//WALK��Ԃւ̈ڍs
                {
                    state.currentstate = PlayerState.State.WALK;
                }
            }
        }

        //�����t���O���I���ŁA�������łȂ��A�W�����v���ł��Ȃ����
        if (!state.IS_GROUND)
        {
            state.currentstate = PlayerState.State.FALL;// �����J�n
        }
    }

    public void HandleState()//��Ԃ��Ƃ�Update����
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
