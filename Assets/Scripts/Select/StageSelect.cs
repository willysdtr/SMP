using DG.Tweening;	//DOTween���g���Ƃ��͂���using������
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelect : MonoBehaviour
{
    [SerializeField] private GameObject m_StageArrow; //�X�e�[�W�I���̖��
    [SerializeField] private GameObject m_BackGround;//�w�i
    [SerializeField] private GameObject[] m_InActiveStage;//Stage�̔�\���I�u�W�F�N�g
    [SerializeField] private GameObject[] m_ActiveStage;//Stage�̕\���I�u�W�F�N�g
    [SerializeField] private GameObject[] m_WorldIcon;
    [SerializeField] private float m_UpPosX;
    [SerializeField] private float m_UpPosY;
    [SerializeField] private float m_UpPosX_Even;
    [SerializeField] private float m_UpPosY_Even;
    [SerializeField] private float m_UpPosY_Even_Left;
    [SerializeField] private float m_IconPosUp;
    [SerializeField] private float m_ArrowSpeed;
   private InputSystem_Actions inputActions;

    private float horizontalInput = 0f;// ���������̓��͒l
    private int m_StageNum;

    private int m_CurrentArrow = 0;
    // ���݈ʒu�i�J�n�n�_�j
    Vector3 startPos ;

    // �e�i�K�̖ڕW�n�_���Ɍv�Z
    Vector3 downPos ;
    Vector3 rightPos;
    Vector3 finalPos;

    Vector3 BackGroundstartPos;
    bool MoveRight = false;
    bool MoveLeft = false;
    int World = 0;

    void Awake()
    {
        m_StageNum = m_InActiveStage.Length;//�傫�����擾
        inputActions = new InputSystem_Actions();//PlayerInputActions�̃C���X�^���X�𐶐�
        inputActions.Select.Move.performed += ctx =>//�オ�P�A�����O,�E���Q�A�����R
        {
            horizontalInput = ctx.ReadValue<float>();
            if (horizontalInput==1)//�E
            {
                if (SMPState.CURRENT_STAGE == m_StageNum-1|| MoveRight == true|| MoveLeft == true) return;//�Ō�̃X�e�[�W�̎��͓����Ȃ�
                BackGroundstartPos = m_BackGround.transform.position;//���݂�BackGround�̈ʒu��ۑ�
                SMPState.CURRENT_STAGE += 1;
                m_CurrentArrow += 1;
                if (m_CurrentArrow % 2 == 1)//��X�e�[�W��I��
                {
                    // ���݈ʒu�i�J�n�n�_�j
                    startPos = m_StageArrow.transform.position;
                    // �e�i�K�̖ڕW�n�_���Ɍv�Z
                    downPos = new Vector3(startPos.x, startPos.y - m_UpPosY, startPos.z);
                    rightPos = new Vector3(startPos.x + m_UpPosX, downPos.y, startPos.z);
                    finalPos = new Vector3(rightPos.x, m_ActiveStage[SMPState.CURRENT_STAGE].transform.position.y + m_IconPosUp, startPos.z);
                }
                else if (m_CurrentArrow % 2 == 0)//�����X�e�[�W��I��
                {
                    // ���݈ʒu�i�J�n�n�_�j
                    startPos = m_StageArrow.transform.position;
                    // �e�i�K�̖ڕW�n�_���Ɍv�Z
                    downPos = new Vector3(startPos.x, startPos.y + m_UpPosY_Even, startPos.z);
                    rightPos = new Vector3(startPos.x + m_UpPosX_Even, downPos.y, startPos.z);
                    finalPos = new Vector3(rightPos.x, m_ActiveStage[SMPState.CURRENT_STAGE].transform.position.y + m_IconPosUp, startPos.z);
                }
                MoveRight = true;
                MoveRightArrow();
            }
            else//��
            {
                if (SMPState.CURRENT_STAGE == 0 || MoveRight == true || MoveLeft == true) return;// �ŏ��̃X�e�[�W�̎��͓����Ȃ�
                BackGroundstartPos = m_BackGround.transform.position;//���݂�BackGround�̈ʒu��ۑ�
                SMPState.CURRENT_STAGE -= 1;
                m_CurrentArrow -= 1;
                if (m_CurrentArrow % 2 == 1)
                {
                    // ���݈ʒu�i�J�n�n�_�j
                    startPos = m_StageArrow.transform.position;
                    // �e�i�K�̖ڕW�n�_���Ɍv�Z
                    downPos = new Vector3(startPos.x, startPos.y + m_UpPosY_Even_Left, startPos.z);
                    rightPos = new Vector3(startPos.x - m_UpPosX, downPos.y, startPos.z);
                    finalPos = new Vector3(rightPos.x, m_ActiveStage[SMPState.CURRENT_STAGE].transform.position.y + m_IconPosUp, startPos.z);
                }
                else if (m_CurrentArrow % 2 == 0)
                {
                    // ���݈ʒu�i�J�n�n�_�j
                    startPos = m_StageArrow.transform.position;
                    // �e�i�K�̖ڕW�n�_���Ɍv�Z
                    downPos = new Vector3(startPos.x, startPos.y - m_UpPosY_Even, startPos.z);
                    rightPos = new Vector3(startPos.x - m_UpPosX_Even, downPos.y, startPos.z);
                    finalPos = new Vector3(rightPos.x, m_ActiveStage[SMPState.CURRENT_STAGE].transform.position.y + m_IconPosUp, startPos.z);
                }
                MoveLeft = true;
                MoveLeftArrow();
            }
        }
        ;
        inputActions.Select.Move.canceled += ctx =>
        {
            horizontalInput = 0f;
        };

        //�I�����Ă���X�e�[�W�����[�h����
        inputActions.Select.SelectStage.performed += ctx =>
        {
            LoadSelectedStage();
        };

        inputActions.PauseApperance.Apperance.performed += ctx =>//�����̏�����SMP_SceneManager�Ɉړ������悤�I
        {
            if (MoveRight == true || MoveLeft == true) return;//���ړ����͏o���Ȃ�
            SMPState.Instance.m_CurrentGameState = SMPState.GameState.Pause;//Pause��Ԃɂ���
            inputActions.Select.Disable();//PlayerInputActions�𖳌���
            SceneManager.LoadScene("PauseScene", LoadSceneMode.Additive);
        };

        

    }
        // Update is called once per frame
    void FixedUpdate()
    {
        if (MoveRight==false&&MoveLeft==false)//�I�𒆂̃A�C�R����\��(���ړ����͏o���Ȃ�)
        {
            m_ActiveStage[SMPState.CURRENT_STAGE].SetActive(true);
        }
        else
        {
            for(int i=0;i< m_InActiveStage.Length;i++)
            {
                m_ActiveStage[i].SetActive(false);
            }
        }
        for (int i = 0; i < m_WorldIcon.Length; i++)//World�̃A�C�R����\��
        {
            m_WorldIcon[i].SetActive(i == World);
        }

    }

    void MoveRightArrow()//���E�ړ�
    {
        if (m_CurrentArrow % 5 == 0)//World�ړ�
        {
            m_CurrentArrow = 0;
            m_StageArrow.SetActive(false);//��ʑJ�ڒ��͖����\��
            m_BackGround.transform.DOMoveX(BackGroundstartPos.x - 1837, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                m_StageArrow.transform.position = m_ActiveStage[SMPState.CURRENT_STAGE].transform.position;
                m_StageArrow.SetActive(true);
                MoveRight = false;
                World += 1;
            });
        }
        else

        {
            // DOTween �V�[�P���X�ŏ��ԂɎ��s
            Sequence seq = DOTween.Sequence();
            seq.Append(m_StageArrow.transform.DOMove(downPos, m_ArrowSpeed).SetEase(Ease.Linear));
            seq.Append(m_StageArrow.transform.DOMove(rightPos, m_ArrowSpeed).SetEase(Ease.Linear));
            seq.Append(m_StageArrow.transform.DOMove(finalPos, m_ArrowSpeed).SetEase(Ease.Linear));
            seq.OnComplete(() =>
            {
                MoveRight = false;
            });

        }
    }
    void MoveLeftArrow()//��󍶈ړ�
    {
        if (m_CurrentArrow < 0)//World�ړ�
        {
            m_CurrentArrow = 4;
            m_StageArrow.SetActive(false);//��ʑJ�ڒ��͖����\��
            m_BackGround.transform.DOMoveX(BackGroundstartPos.x + 1837, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                m_StageArrow.SetActive(true);   
                m_StageArrow.transform.position = m_ActiveStage[SMPState.CURRENT_STAGE].transform.position;
                MoveLeft = false;

                World -= 1;
            });
        }
        else
        {
            // DOTween �V�[�P���X�ŏ��ԂɎ��s
            Sequence seq = DOTween.Sequence();
            seq.Append(m_StageArrow.transform.DOMove(downPos, m_ArrowSpeed).SetEase(Ease.Linear));
            seq.Append(m_StageArrow.transform.DOMove(rightPos, m_ArrowSpeed).SetEase(Ease.Linear));
            seq.Append(m_StageArrow.transform.DOMove(finalPos, m_ArrowSpeed).SetEase(Ease.Linear));
            seq.OnComplete(() =>
            {
                MoveLeft = false;
            });

        }
    }
    void LoadSelectedStage()
    {
        // �Q�[���X�e�[�W�����[�h����
        SceneManager.LoadScene("testScene");
        Debug.Log("���݂̃V�[��: " + SceneManager.GetActiveScene().name);  // �� �m�F�p
    }


    void OnEnable()
    {
        inputActions.Enable();//PlayerInputActions��L����
    }

    void OnDisable()
    {
        inputActions.Disable();//PlayerInputActions�𖳌���
    }
}
