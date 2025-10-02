using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI.Table;

public class PlayerController : MonoBehaviour
{
    //�v���C���[�̃X�e�[�g��������
    private InputSystem_Actions inputActions;
    PlayerMove move;
    PlayerState state;
    private Animator anim = null;
    private float jumptime = 0.0f;//�W�����v����

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
        //OverlapBox�̍쐬�AClimb�����Ɏg�p
        Vector2 center = (Vector2)transform.position + checkOffset;

        Collider2D hit = Physics2D.OverlapBox(center, checkSize, 0f, climbLayer);

        ishit = hit;
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

        if (state.currentstate != PlayerState.State.STOP) { return; }//��~��ԂłȂ���Ώ�ԕω��������s��Ȃ�
        if (!state.IS_CLIMB_NG)
        {//�ړ��s�t���O���I���Ȃ�A��~��A������ԈȊO�Ɉڍs���Ȃ�
            if (state.IS_CLIMB)//CLIMB��Ԃւ̈ڍs
            {
                state.currentstate = PlayerState.State.CLIMB;
                transform.position = new Vector2(hitobj_pos.x, transform.position.y);
            }
            else
            {
                if (state.IS_JUMP && !state.IS_CEILING_HIT)//JUMP��Ԃւ̈ڍs
                {
                    //�W�����v���������s���A�t���O���I�t�ɂ��鎖�ŁA��x�������s����l�ɂ��Ă���
                    move.Stop();
                    move.InitJump(initialVelocity);
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
        //OverlapBox�̕`��
        Gizmos.color = Color.red;
        Vector2 center = (Vector2)transform.position + checkOffset;
        Gizmos.DrawWireCube(center, checkSize);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {

        if (((1 << collision.gameObject.layer) & groundlayers) != 0)//�C���X�y�N�^�[�Őݒ肵��Layer�Ƃ̂ݔ�������
        {

            if (collision.gameObject.tag == "Spring")//�΂˂ɓ����������̏���
            {
                //�\�����X�N���v�g������΁A���������s
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
                state.currentstate = PlayerState.State.GOAL;// �S�[����ԂɕύX
                goal_pos = collision.transform.position;
            }
            else
            {
                foreach (ContactPoint2D contact in collision.contacts)
                {

                    // ������ɐڐG�����ꍇ�̂݃J�E���g
                    if (contact.normal == Vector2.up)
                    {
                        state.IS_GROUND = true;
                        state.IS_MOVE = true;
                        ground_obj.Add(collision.gameObject);
                    }
                    // �������ɐڐG�����ꍇ�̂݃J�E���g
                    if (contact.normal == Vector2.left || contact.normal == Vector2.right)
                    {
                        if (collision.gameObject.tag == "String" && state.IS_CLIMB_NG == false)
                        {
                            bool isVertical = collision.transform.rotation.z != 0;
                            if (isVertical)
                            {
                                // �c�̎��Ȃ�Trigger�ɐ؂�ւ�
                                GetComponent<BoxCollider2D>().isTrigger = true;
                                //���ɓ����������̏���
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
        ground_obj.Remove(collision.gameObject);//�n�ʔ��肵���I�u�W�F�N�g���폜
        wall_obj.Remove(collision.gameObject);//�ǔ��肵���I�u�W�F�N�g���폜
        if (ground_obj.Count == 0)
        {//�n�ʔ��肵���I�u�W�F�N�g�����ׂĂȂ��Ȃ�΁A�n�ʂ��痣�ꂽ��Ԃɂ���
            state.IS_GROUND = false;
        }

        if (wall_obj.Count == 0)
        {//�ǔ��肵���I�u�W�F�N�g�����ׂĂȂ��Ȃ�΁A�ړ��\�ɂ���
            state.IS_MOVE = true;
        }

    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "String")
        {
            //���ɓ����������̏���
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
        {//OverlapBox���d�Ȃ��ĂȂ��Ƃ��Ɏ��s(��쓮���邽��)
            if (collider.gameObject.tag == "String")
            {//�����痣�ꂽ���̏���
                state.IS_MOVE = true;
                state.IS_CLIMB = false;
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.linearVelocity = Vector2.zero;
                GetComponent<BoxCollider2D>().isTrigger = false;//Trigger����
            }
        }

    }

    // �v���C���[��Canvas�ɁA�A���J�[�ʒu�ƃT�C�Y�Ŕz�u����֐�
    public void PlaceAtPosition(RectTransform parent, Vector2 anchoredPos, Vector2 size)
    {
        // �e��ݒ�i��2����false�Ń��[�J�����W�ێ��Ȃ��A���S�ɐe��ňʒu�ݒ�j
        rect.SetParent(parent, false);

        // �e�p�l����̃��[�J�����W�i�A���J�[�ʒu�j���Z�b�g
        rect.anchoredPosition = anchoredPos;

        Vector2 setScale = new ( size.x / rect.sizeDelta.x, size.y / rect.sizeDelta.y );

        // �T�C�Y�����킹��
        rect.sizeDelta = size;

        BoxCollider2D collider = this.GetComponent<BoxCollider2D>();
        collider.size = new Vector2(collider.size.x * setScale.x, collider.size.y * setScale.y);
    }

    void OnEnable()
    {
        // InputSystem �L����
        inputActions.Enable();

        inputActions.Player.Attack.performed += OnStartPerformed;
    }

    void OnDisable()
    {
        // ������ & �C�x���g����
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
