using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class StringManager : MonoBehaviour
{
    //�萔�錾
    public const int RIGHT = 0;
    public const int LEFT = 1;
    public const int Up = 2;
    public const int Down = 3;

    [SerializeField] private GameObject StringPrefub;

    public Vector3 m_StrinngScale = new Vector3(1.0f, 1.0f, 0.0f);
    private Vector2 m_Offset_X=new Vector2(1.0f,0.0f);
    private Vector2 m_Offset_Y =new Vector2(0.0f,-1.0f);
    private List<GameObject> Strings = new List<GameObject>();
    private List<GameObject> FrontStrings = new List<GameObject>();
    private List<GameObject> BackStrings = new List<GameObject>();
    [SerializeField] private int StringNum;
    private InputSystem_Actions inputActions;
    private float m_PauseDirection;//���ʒ����̓��͒l
    private int m_LastDirection;//�O��̓��͒l
    void Awake()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Stirng.nami.performed += ctx =>
        {
            float value = ctx.ReadValue<float>();
            m_PauseDirection = value;
            if (m_PauseDirection == 1)//��
            {
                OnUpInput();
            }
            else if (m_PauseDirection == -1)//��
            {
                OnDownInput();
            }
            else if (m_PauseDirection == 2)//�E
            {
                OnRightInput();
            }
            else if (m_PauseDirection == 3)//��
            {
                OnLeftInput();
            }
        };
    }

    void Start()
    {
        //�ŏ��̏��_�����߂�
        m_Offset_X=new Vector2(m_StrinngScale.x, 0.0f);
        m_Offset_Y=new Vector2(0.0f,-m_StrinngScale.y);
       GameObject first = Instantiate(StringPrefub, Vector3.zero, Quaternion.identity);
        Strings.Add(first);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnRightInput()
    {
        if (m_LastDirection == LEFT) return;
        // �Ō�̃I�u�W�F�N�g�̉E�ɐ���
        Vector3 lastPos = Strings[^1].transform.position;//�Ō�̃I�u�W�F�N�g�̈ʒu���擾
        Vector3 FrontlastPos;
        Vector3 BacklastPos;
        Vector3 newPos = lastPos + (Vector3)m_Offset_X;

        if (m_LastDirection == Up)
        {
            newPos = lastPos + (Vector3)m_Offset_X / 2 - (Vector3)m_Offset_Y / 2;
        }
        else if (m_LastDirection == Down)
        {
            newPos = lastPos + (Vector3)m_Offset_X / 2 + (Vector3)m_Offset_Y / 2;
        }
        FrontlastPos = newPos + (Vector3)m_Offset_X / 2; // ������̂Ƃ��͏�����ɂ��炷
        BacklastPos = newPos - (Vector3)m_Offset_X / 2; // ������̂Ƃ��͏������ɂ��炷

        if(CheckString(newPos, FrontlastPos, BacklastPos))
        {
            GameObject obj = Instantiate(StringPrefub, newPos, Quaternion.identity);
            //�A�j���[�V���������s
            Animator animator = obj.GetComponent<Animator>();
            animator.SetTrigger("Play"); // �A�j���[�V�������Đ�
            Strings.Add(obj);
            GameObject frontobj = Instantiate(StringPrefub, FrontlastPos, Quaternion.identity);
            FrontStrings.Add(frontobj);
            GameObject backobj = Instantiate(StringPrefub, BacklastPos, Quaternion.identity);
            BackStrings.Add(backobj);

            m_LastDirection = RIGHT; // ���O�̕������X�V
        }
    }
    void OnLeftInput()
    {
        if (m_LastDirection == RIGHT) return;
        // �Ō�̃I�u�W�F�N�g�̍��ɐ���
        Vector3 lastPos = Strings[^1].transform.position; // �Ō�̃I�u�W�F�N�g�̈ʒu
        Vector3 FrontlastPos;
        Vector3 BacklastPos;
        Vector3 newPos = lastPos - (Vector3)m_Offset_X;        // �� offset���}�C�i�X�ɂ��č�����

        if (m_LastDirection == Up)
        {
            newPos = lastPos - (Vector3)m_Offset_X / 2 - (Vector3)m_Offset_Y / 2; // ������̂Ƃ��͏������ɂ��炷
        }
        else if (m_LastDirection == Down)
        {
            newPos = lastPos-(Vector3)m_Offset_X / 2 + (Vector3)m_Offset_Y / 2; // �������̂Ƃ��͏�����ɂ��炷
        }
        FrontlastPos = newPos - (Vector3)m_Offset_X / 2; // ������̂Ƃ��͏�����ɂ��炷
        BacklastPos = newPos + (Vector3)m_Offset_X / 2; // ������̂Ƃ��͏������ɂ��炷
        if (CheckString(newPos, FrontlastPos, BacklastPos))
        {
            GameObject obj = Instantiate(StringPrefub, newPos, Quaternion.identity);
            obj.transform.rotation = Quaternion.Euler(0, 180, 0); // �������ɉ�]
            //�A�j���[�V���������s
            Animator animator = obj.GetComponent<Animator>();
            animator.SetTrigger("Play"); // �A�j���[�V�������Đ�
            Strings.Add(obj);
            GameObject frontobj = Instantiate(StringPrefub, FrontlastPos, Quaternion.identity);
            FrontStrings.Add(frontobj);
            GameObject backobj = Instantiate(StringPrefub, BacklastPos, Quaternion.identity);
            BackStrings.Add(backobj);

            m_LastDirection = LEFT; // ���O�̕������X�V
        }
        //GameObject obj = Instantiate(StringPrefub, newPos, Quaternion.identity);
        //obj.transform.rotation = Quaternion.Euler(0, 180, 0); // �������ɉ�]

        //Animator animator = obj.GetComponent<Animator>();
        //animator.SetTrigger("Play");
        //Strings.Add(obj);
        //m_LastDirection = LEFT; // ���O�̕������X�V
    }
    void OnUpInput()
    {
        if (m_LastDirection == Down) return;
        // �Ō�̃I�u�W�F�N�g�̍��ɐ���
        Vector3 lastPos = Strings[^1].transform.position; // �Ō�̃I�u�W�F�N�g�̈ʒu
        Vector3 FrontlastPos;
        Vector3 BacklastPos;
        Vector3 newPos= new Vector3(0.0f,0.0f,0.0f);//������

        if (m_LastDirection==RIGHT)
        {
           newPos = lastPos + (Vector3)m_Offset_X / 2 - (Vector3)m_Offset_Y / 2;        //offset���}�C�i�X�ɂ��ĉE����
        }
        else if (m_LastDirection==LEFT)
        {
            newPos = lastPos - (Vector3)m_Offset_X/2 - (Vector3)m_Offset_Y / 2;        //offset���}�C�i�X�ɂ��č�����
        }
        else if (m_LastDirection == Up)
        {
            newPos = lastPos - (Vector3)m_Offset_Y;        //offset���}�C�i�X�ɂ��č�����
        }
        FrontlastPos = newPos - (Vector3)m_Offset_Y / 2; // ������̂Ƃ��͏�����ɂ��炷
        BacklastPos = newPos + (Vector3)m_Offset_Y / 2; // ������̂Ƃ��͏������ɂ��炷

        if (CheckString(newPos, FrontlastPos, BacklastPos))
        {
            GameObject obj = Instantiate(StringPrefub, newPos, Quaternion.identity);
            obj.transform.rotation = Quaternion.Euler(0,0, 90); // ������ɉ�]
            //�A�j���[�V���������s
            Animator animator = obj.GetComponent<Animator>();
            animator.SetTrigger("Play"); // �A�j���[�V�������Đ�
            Strings.Add(obj);
            GameObject frontobj = Instantiate(StringPrefub, FrontlastPos, Quaternion.identity);
            FrontStrings.Add(frontobj);
            GameObject backobj = Instantiate(StringPrefub, BacklastPos, Quaternion.identity);
            BackStrings.Add(backobj);

            m_LastDirection = Up; // ���O�̕������X�V
        }
        //GameObject obj = Instantiate(StringPrefub, newPos, Quaternion.identity);
        //obj.transform.rotation = Quaternion.Euler(0,0, 90); // ������ɉ�]

        //Animator animator = obj.GetComponent<Animator>();
        //animator.SetTrigger("Play");
        //Strings.Add(obj);
        //m_LastDirection = Up;
    }
    void OnDownInput()
    {
        if(m_LastDirection == Up) return;
        // �Ō�̃I�u�W�F�N�g�̍��ɐ���
        Vector3 lastPos = Strings[^1].transform.position; // �Ō�̃I�u�W�F�N�g�̈ʒu
        Vector3 newPos = new Vector3(0.0f, 0.0f, 0.0f);//������
        Vector3 FrontlastPos;
        Vector3 BacklastPos;
        if (m_LastDirection == RIGHT)
        {
            newPos = lastPos + (Vector3)m_Offset_X /2 + (Vector3)m_Offset_Y /2;        //offset���}�C�i�X�ɂ��ĉE����
        }
        else if (m_LastDirection == LEFT)
        {
            newPos = lastPos - (Vector3)m_Offset_X /2+ (Vector3)m_Offset_Y / 2;        //offset���}�C�i�X�ɂ��č�����
        }
        else if (m_LastDirection == Down)
        {
            newPos = lastPos + (Vector3)m_Offset_Y;        //offset���}�C�i�X�ɂ��č�����
        }
        FrontlastPos = newPos + (Vector3)m_Offset_Y / 2; // ������̂Ƃ��͏�����ɂ��炷
        BacklastPos = newPos - (Vector3)m_Offset_Y / 2; // ������̂Ƃ��͏������ɂ��炷

        if (CheckString(newPos, FrontlastPos, BacklastPos))
        {
            GameObject obj = Instantiate(StringPrefub, newPos, Quaternion.identity);
            //�A�j���[�V���������s
            obj.transform.rotation = Quaternion.Euler(0, 0, 270); // ������ɉ�]
            Animator animator = obj.GetComponent<Animator>();
            animator.SetTrigger("Play"); // �A�j���[�V�������Đ�
            Strings.Add(obj);
            GameObject frontobj = Instantiate(StringPrefub, FrontlastPos, Quaternion.identity);
            FrontStrings.Add(frontobj);
            GameObject backobj = Instantiate(StringPrefub, BacklastPos, Quaternion.identity);
            BackStrings.Add(backobj);

            m_LastDirection =Down; // ���O�̕������X�V
        }
        //GameObject obj = Instantiate(StringPrefub, newPos, Quaternion.identity);
        //obj.transform.rotation = Quaternion.Euler(0, 0, 270); // ������ɉ�]

        //Animator animator = obj.GetComponent<Animator>();
        //animator.SetTrigger("Play");
        //Strings.Add(obj);
        //m_LastDirection = Down;
    }

    bool CheckString(Vector3 newPos, Vector3 FrontlastPos, Vector3 BacklastPos)
    {
        // �d�Ȃ�`�F�b�N�i�����ȃY���h�~�̂��ߋ����Ŕ���j
        foreach (GameObject str in Strings)
        {
            if (Vector3.Distance(str.transform.position, FrontlastPos) < 0.001f)
            {
                return false; // ���łɓ����ʒu�ɑ��� �� �������f
            }
        }
        foreach (GameObject str in FrontStrings)
        {
            if (Vector3.Distance(str.transform.position, FrontlastPos) < 0.001f)
            {
                return false; // ���łɓ����ʒu�ɑ��� �� �������f
            }
        }
        foreach (GameObject str in BackStrings)
        {
            if (Vector3.Distance(str.transform.position, FrontlastPos) < 0.001f)
            {
                return false; // ���łɓ����ʒu�ɑ��� �� �������f
            }
        }
        return true; // �d�Ȃ肪�Ȃ��ꍇ��true��Ԃ�
    }
    void OnEnable()
    {
        inputActions.Stirng.Enable();
    }
    void OnDisable()
    {
        inputActions.Stirng.Disable();
    }
}
