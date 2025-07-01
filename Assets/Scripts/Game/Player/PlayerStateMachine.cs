using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public enum PlayerState { STOP, WALK, JUMP, FALL, CLIMB, GOAL, DEATH }
public class PlayerStateMachine : MonoBehaviour
{
    public PlayerState currentstate { get; private set; } = PlayerState.STOP;
    public enum Direction { LEFT = -1, STOP = 0, RIGHT = 1 };
    Transform m_transform;
    PlayerMove move;
    PlayerJump jump;

    private bool moveFg = true;
    private bool jumpFg;
    private bool isground;
    private bool downFg;
    private bool ceiling_hit;
    private bool ngFg;
    private bool climbFg;
    private bool gimjumpFg;

    [SerializeField]
    [Header("�����蔻�����郌�C���[")]
    private LayerMask GroundLayers;

    private Animator anim = null;
    public LayerMask groundlayers => GroundLayers;
    public int direction { get; private set; } = 1;
    private void Start()
    {
        move = GetComponent<PlayerMove>();
        jump = GetComponent<PlayerJump>();
        m_transform = this.transform;
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        ChangeState();
        HandleState();
    }

    public void ChangeState()
    {
        if (downFg)
        {//�v���C���[������邩�̔���
            currentstate = PlayerState.DEATH;
            return;
        }

        switch (currentstate)//���݂̏�Ԃ����~�Ɉڍs���邩
        {
            case PlayerState.WALK://WALK
                if (!isground || climbFg || !moveFg) { currentstate = PlayerState.STOP; }
                break;
            case PlayerState.CLIMB://CLIMB
                if(!climbFg){ currentstate = PlayerState.STOP; }
                break;
            case PlayerState.JUMP://JUMP
                if(ceiling_hit || isground){ currentstate = PlayerState.STOP; }
                break;
            case PlayerState.FALL://FALL
                if(isground){ currentstate = PlayerState.STOP; }
                break;
        }

        if (currentstate != PlayerState.STOP) { return; }//��~��ԂłȂ���Ώ�ԕω��������s��Ȃ�
        if (!ngFg)
        {//�ړ��s�t���O���I���Ȃ�A��~��A������ԈȊO�Ɉڍs���Ȃ�
            if (climbFg)
            {
                currentstate = PlayerState.CLIMB;
            }
            else
            {
                if (jumpFg)
                {
                    currentstate = PlayerState.JUMP;
                    jump.Jump(gimjumpFg);
                }
                else if (moveFg)
                {
                    currentstate = PlayerState.WALK;
                }
            }
        }

        //�����t���O���I���ŁA�������łȂ��A�W�����v���ł��Ȃ����
        //if (currentstate == PlayerState.JUMP) { return; }
        if (!isground) {
            currentstate = PlayerState.FALL;// �����J�n
        }
    }

    public void HandleState()
    {
        switch (currentstate)
        {
            case PlayerState.STOP: anim.speed = 0; break;
            case PlayerState.WALK:  anim.speed = 1; move.Move(); break;
            case PlayerState.JUMP: anim.speed = 0; jump.Move(move.maxspeed_read * direction); break;
            case PlayerState.FALL: anim.speed = 0; break;
            case PlayerState.CLIMB: anim.speed = 1; break;
            case PlayerState.GOAL: anim.speed = 1; break;
            case PlayerState.DEATH: anim.speed = 1; break;
        }
        anim.SetInteger("State", (int)currentstate);
        anim.SetBool("IsJump", jumpFg);
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
}
