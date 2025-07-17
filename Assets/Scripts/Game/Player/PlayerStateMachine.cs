using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public enum PlayerState { STOP, WALK, JUMP, FALL, CLIMB, GOAL, DEATH }
public class PlayerStateMachine : MonoBehaviour
{
    //�v���C���[�̏�ԊǗ��X�N���v�g
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
    [Header("�W�����v���s���Œ᎞��")]
    private const float jumptime_max = 0.1f;//�W�����v���s���Œ᎞��
    private float jumptime = 0.0f;//�W�����v����

    private Vector2 hitobj_pos;
    Vector2 initialVelocity;//�W�����v�œn���p

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
        {//�v���C���[������邩�̔���
            currentstate = PlayerState.DEATH;
            return;
        }

        switch (currentstate)//���݂̏�Ԃ����~�Ɉڍs���邩
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

        if (currentstate != PlayerState.STOP) { return; }//��~��ԂłȂ���Ώ�ԕω��������s��Ȃ�
        if (!ngFg)
        {//�ړ��s�t���O���I���Ȃ�A��~��A������ԈȊO�Ɉڍs���Ȃ�
            if (climbFg)//CLIMB��Ԃւ̈ڍs
            {
                currentstate = PlayerState.CLIMB;
                transform.position = new Vector2(hitobj_pos.x, transform.position.y);
            }
            else
            {
                if (jumpFg && !ceiling_hit)//JUMP��Ԃւ̈ڍs
                {
                    //�W�����v���������s���A�t���O���I�t�ɂ��鎖�ŁA��x�������s����l�ɂ��Ă���
                    move.Stop();
                    jump.InitJump(initialVelocity);
                    currentstate = PlayerState.JUMP;
                    jumptime = 0.0f;
                    jumpFg = false;

                }
                else if (moveFg)//WALK��Ԃւ̈ڍs
                {
                    currentstate = PlayerState.WALK;
                }
            }
        }

        //�����t���O���I���ŁA�������łȂ��A�W�����v���ł��Ȃ����
        if (!isground) {
            currentstate = PlayerState.FALL;// �����J�n
        }
    }

    public void HandleState()//��Ԃ��Ƃ�Update����
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
