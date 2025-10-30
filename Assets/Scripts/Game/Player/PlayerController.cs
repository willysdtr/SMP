using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerController : MonoBehaviour
{
    //�v���C���[�̃X�e�[�g��������
    private InputSystem_Actions inputActions;
    PlayerMove move;
    public PlayerState state { get; private set; }  // �O������ǂݎ���
    private Animator anim = null;

    public Vector2 hitobj_pos = Vector2.zero;
    private Vector2 start_pos = Vector2.zero;
    private Vector2 goal_pos = Vector2.zero;
    public bool ishit;

    private float blocksize = 50;
    private float fallstart_y;//�����J�n�ʒu

    public LayerMask groundlayers; //�n�ʁA�ǔ�����s�����C���[
    public LayerMask climblayers; //�o�蔻����s�����C���[

    private RectTransform rect;
    public bool start = false;
    public bool goal = false;
    public bool death = false;

    //L�AR�{�^���������Ă��邩
    private bool lPressed = false;
    private bool rPressed = false;

    [SerializeField]
    private AudioClip[] sound; // { ���sSE�A�悶�o��SE�A����SE}
    private AudioSource audiosource;

    public int cutCt = 0;//����؂���

    void Awake()
    { // �e��R���|�[�l���g�̎擾
        move = GetComponent<PlayerMove>();
        anim = GetComponent<Animator>();
        rect = GetComponent<RectTransform>();
        audiosource = GetComponent<AudioSource>();
        //�@State�N���X�̏�����
        state = new PlayerState(groundlayers);
        anim.speed = 0;
        inputActions = new InputSystem_Actions();
        state.m_direction = (int)PlayerState.Direction.RIGHT;

    }

    // Update is called once per frame
    void Update()
    {
        // ���g���őO�ʂɕ\��
        transform.SetAsLastSibling(); //���t���[���͌����������Ȃ̂ŁA���̂����ς���

        if (!start || goal) { return; }//�X�^�[�g�����S�[�����Ȃ�AUpdate�������s��Ȃ�
        ChangeState();//��ԕω�����
        HandleState();//��Ԃ��Ƃ�Update����
        if (transform.position.y < -600)
        {
            state.currentstate = PlayerState.State.DEATH; //��ʊO�ɗ������玀�S
            move.AllStop();
        }
    }

    public void ChangeState()
    {
        ChangeStateStop();//��~��ԂɕύX���邩

        if (state.currentstate != PlayerState.State.STOP) { return; }//��~��ԂłȂ���Ώ�ԕω��������s��Ȃ�
        //��~��Ԃ���̏�ԕω�����
        if (state.IS_CLIMB)//CLIMB��Ԃւ̈ڍs
        {
            // �o���Ȃ�ACLIMB��ԂɈڍs����
            state.currentstate = PlayerState.State.CLIMB;
            transform.position = new Vector2(hitobj_pos.x, transform.position.y);
            
        }
        else
        {
            if (state.IS_JUMP && !state.IS_CEILING_HIT)//JUMP��Ԃւ̈ڍs
            {
                //�W�����v�̏��������s��
                
                move.InitJump(state.m_direction,blocksize);
                state.currentstate = PlayerState.State.JUMP;

            }
            else if (state.IS_MOVE)//WALK��Ԃւ̈ڍs
            {
                state.currentstate = PlayerState.State.WALK;
            }
        }
        

        //�n�ʂɐڐG���Ă��炸�A�W�����v���ł��Ȃ����
        if (!state.IS_GROUND && !state.IS_JUMP)
        {
            state.currentstate = PlayerState.State.FALL;// �����J�n
            fallstart_y = transform.position.y;
            PlaySE(2, true);
        }
    }

    public void HandleState()//��Ԃ��Ƃ�Update����
    {
        switch (state.currentstate)
        {
            case PlayerState.State.STOP: anim.speed = 0; move.Stop(); break;
            case PlayerState.State.WALK: anim.speed = 1; move.Move(state.m_direction); PlaySE(0, false); break;
            case PlayerState.State.JUMP: anim.speed = 0; state.IS_JUMP = !move.Jump(); break;
            case PlayerState.State.FALL: anim.speed = 0; break;
            case PlayerState.State.CLIMB: anim.speed = 1; move.Climb(PlayerState.MAX_SPEED / 2); PlaySE(1, false); break;
            case PlayerState.State.GOAL: anim.speed = 1; if (move.Goal(goal_pos)) { goal = true; anim.speed = 0; } break;
            case PlayerState.State.DEATH: anim.speed = 1; death = true; break;
        }
        anim.SetInteger("State", (int)state.currentstate);
    }

    void ChangeStateStop() //��~��ԂɕύX
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
                if (state.IS_CEILING_HIT || state.IS_GROUND || !state.IS_JUMP) { state.currentstate = PlayerState.State.STOP; move.EndJump(); }
                break;
            case PlayerState.State.FALL://FALL
                if (state.IS_GROUND || state.IS_JUMP) { state.currentstate = PlayerState.State.STOP; if (fallstart_y - transform.position.y >= blocksize * 2.9 && !state.IS_JUMP) { state.currentstate = PlayerState.State.DEATH; } }
                break;
        }
    }

    public void PlaceAtPosition(RectTransform parent, Vector2 anchoredPos, Vector2 size,float _blocksize = 10 , bool isleft = false)  // �v���C���[��Canvas�ɁA�A���J�[�ʒu�ƃT�C�Y�Ŕz�u����֐�
    {
        // �e��ݒ�i��2����false�Ń��[�J�����W�ێ��Ȃ��A���S�ɐe��ňʒu�ݒ�j
        rect.SetParent(parent, false);

        // �e�p�l����̃��[�J�����W�i�A���J�[�ʒu�j���Z�b�g
        rect.anchoredPosition = anchoredPos;

        Vector2 setScale = new ( size.x / rect.sizeDelta.x, size.y / rect.sizeDelta.y ); // �T�C�Y�����킹�邽�߂̃X�P�[��

        blocksize = _blocksize; 

        // �T�C�Y�����킹��
        rect.sizeDelta = size;

        BoxCollider2D collider = this.GetComponent<BoxCollider2D>();
        collider.size = new Vector2(collider.size.x * setScale.x, collider.size.y * setScale.y); // �{�̂̓����蔻��T�C�Y�����킹��

        Transform check = transform.Find("ClimbCheck");
        collider = check.GetComponent<BoxCollider2D>();
        collider.size = new Vector2(collider.size.x * setScale.x, collider.size.y * setScale.y);//���ΓI�ȃT�C�Y�ύX
        collider.offset = new(collider.offset.x * setScale.x, collider.offset.y * setScale.y);//offset�ύX
        check.localPosition = new(check.localPosition.x * setScale.x, check.localPosition.y * setScale.y);//check�̑��Έʒu��ύX

        check = transform.Find("CeilingCheck");
        collider = check.GetComponent<BoxCollider2D>();
        collider.size = new Vector2(collider.size.x * setScale.x, collider.size.y * setScale.y);//���ΓI�ȃT�C�Y�ύX
        collider.offset = new(collider.offset.x * setScale.x, collider.offset.y * setScale.y);//offset�ύX
        check.localPosition = new(check.localPosition.x * setScale.x, check.localPosition.y * setScale.y);//check�̑��Έʒu��ύX


        if (isleft) { state.m_direction = (int)PlayerState.Direction.LEFT; transform.rotation = Quaternion.Euler(0, 0, 0); } // ������
        else { state.m_direction = (int)PlayerState.Direction.RIGHT; transform.rotation = Quaternion.Euler(0, 180, 0); } //�E����
    }

    void OnEnable()
    {
        // InputSystem �L����
        inputActions.Enable();

        // L����
        inputActions.Stirng.PlayerStartL.performed += ctx =>
        {
            lPressed = true;
            OnStartPerformed();
        };
        inputActions.Stirng.PlayerStartL.canceled += ctx => lPressed = false;

        // R����
        inputActions.Stirng.PlayerStartR.performed += ctx =>
        {
            rPressed = true;
            OnStartPerformed();
        };
        inputActions.Stirng.PlayerStartR.canceled += ctx => rPressed = false;

    }

    void OnDisable()
    {
        // InputSystem������ & �C�x���g����
        inputActions.Stirng.PlayerStartL.performed -= ctx => { lPressed = true; OnStartPerformed(); };
        inputActions.Stirng.PlayerStartL.canceled -= ctx => lPressed = false;

        inputActions.Stirng.PlayerStartR.performed -= ctx => { rPressed = true; OnStartPerformed(); };
        inputActions.Stirng.PlayerStartR.canceled -= ctx => rPressed = false;

        inputActions.Disable();
    }

    private void OnStartPerformed()//�X�^�[�g��ԂƔ�X�^�[�g�̐؂�ւ�
    {
        if (PauseApperance.Instance.isPause || (SoundChangeSlider.Instance != null && SoundChangeSlider.Instance.IsSoundChange)) return;//�|�[�Y���͑���ł��Ȃ��悤�ɂ���
        if(!(lPressed && rPressed)) { return; }//LR��������������ĂȂ���Ώ��������Ȃ�
        if (!start) // �X�^�[�g�ɐ؂�ւ�
        {

            start = true;
            start_pos = transform.position;
        }
        else // ��X�^�[�g�ɐ؂�ւ�
        {
            start = false;
            goal = false;
            transform.position = start_pos;
            move.AllStop();
            state.currentstate = PlayerState.State.STOP;
            anim.speed = 0;
            ResetFlag();
        }
    }

    private void ResetFlag()
    {
        state.IS_MOVE = true;
        state.IS_CEILING_HIT = false;
        state.IS_CLIMB = false;
        state.IS_DOWN = false;
        state.IS_CLIMB_NG = false;
        state.IS_GROUND = false;
        state.IS_JUMP = false;
        state.IS_GIMJUMP = false;
        death = false;
        start = false;
        goal = false;
    }

    public void Goal(Vector2 pos) //�S�[������
    {
        if (fallstart_y - transform.position.y >= blocksize * 2.9 && !state.IS_JUMP) { state.currentstate = PlayerState.State.DEATH; return; } //���������邩�ǂ���
        state.currentstate = PlayerState.State.GOAL;// �S�[����ԂɕύX
        goal_pos = pos;//�S�[���ʒu���Z�b�g
        transform.position = new Vector2(transform.position.x, pos.y);//������␳
    }

    public void PlayerReturn(float angle)
    {
        if(angle == transform.rotation.y) { return; }//���������Ȃ牽�����Ȃ�
        move.AllStop();
        state.m_direction = move.Return(angle);
    }

    public void PlayerReturn() //���݂̌����Ƌt�����Ɍ�����ς���
    {
        move.AllStop();
        state.m_direction = move.Return(-1 * transform.rotation.y);
    }

    public void PlayerJumpReturn()//�W�����v���ɔ��]����
    {
        PlayerReturn();
        move.JumpReturn();
    }
    private void PlaySE(int no,bool forceplay)//SE�Đ�
    {
        if(forceplay)// �����Đ�
        {
            audiosource.PlayOneShot(sound[no]);
            return;
        }

        // �Đ����I������u�Ԃɂ����čĐ�
        if (!audiosource.isPlaying)
        {
            
            audiosource.clip = sound[no];
            audiosource.Play();

        }
    }
}
