using System.Collections.Generic;
using UnityEngine;
public class StringManager : MonoBehaviour
{
    //�萔�錾
    private const int RIGHT = 0;
    private const int LEFT = 1;
    private const int Up = 2;
    private const int Down = 3;
    private const int Middle = 4;

    private const bool NoString=false;
    private const bool isString=true;

    [SerializeField] private GameObject StringPrefub;

    public Vector3 m_StrinngScale = new Vector3(1.0f, 1.0f, 0.0f);
    private Vector2 m_Offset_X=new Vector2(1.0f,0.0f);
    private Vector2 m_Offset_Y =new Vector2(0.0f,-1.0f);
    private List<GameObject> Strings = new List<GameObject>();
    private List<GameObject> FrontStrings = new List<GameObject>();
    private List<GameObject> BackStrings = new List<GameObject>();
    [SerializeField] List<int> StringNum;
    private int currentIndex = 0;

    [SerializeField] private ShowStringNum listDisplay; // �\���N���X���C���X�y�N�^�[�ŃZ�b�g
    [SerializeField] GameObject Tamadome;
    [SerializeField] GameObject StringCursol;
    private InputSystem_Actions inputActions;
    private float m_PauseDirection;//���ʒ����̓��͒l
    private int m_LastDirection;//�O��̓��͒l

    bool m_StringMode = NoString;//�X�g�����O���[�h�̃t���O

    public bool EndSiting = false; // ���܂��~�߂邩�ǂ����̃t���O

    void Awake()
    {
        inputActions = new InputSystem_Actions();

        inputActions.Stirng.nami.performed += ctx =>
        {
     
            float value = ctx.ReadValue<float>();
            if(m_StringMode== isString)
            {
                // ���ׂĂ̗v�f��0�̏ꍇ�A�������s��Ȃ�
                while (currentIndex < StringNum.Count && StringNum[currentIndex] <= 0)
                {
                    currentIndex++;
                }

                // ���ݏ����\�ȗv�f���Ȃ���ΏI��
                if (currentIndex >= StringNum.Count)
                {
                    Debug.Log("���ׂĂ̏������������܂���");
                    return;
                }

                // �Ώۗv�f��1���炷
                StringNum[currentIndex]--;

                Debug.Log($"Index {currentIndex} �̗v�f��1���炵�܂����B�c��: {StringNum[currentIndex]}");

                listDisplay.UpdateDisplay(StringNum);// Text�\�����X�V

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
                // �������݂̗v�f��0�ɂȂ�����A����͎��̃C���f�b�N�X�֐i�ނ悤�ɂȂ�
                if (StringNum[currentIndex] == 0)
                {
                    currentIndex++;
                    Debug.Log($"Index {currentIndex} �̗v�f��0�ɂȂ�܂����B���̗v�f�֐i�݂܂��B");
                    //EndSiting = true;���@����Animation�ɓ���Ă��₯�ǂȂ�Strings�����f����ĂȂ����ۂ��ł�
                    BallStopper();//���܂��~�߂鏈�����Ăяo��
                }
            }
            else if (m_StringMode == NoString)
            {
                m_PauseDirection = value;
                if (m_PauseDirection == 1)//��
                {
                    StringCursol.transform.position -= (Vector3)m_Offset_Y;
                }
                else if (m_PauseDirection == -1)//��
                {
                    StringCursol.transform.position += (Vector3)m_Offset_Y;
                }
                else if (m_PauseDirection == 2)//�E
                {
                    StringCursol.transform.position += (Vector3)m_Offset_X;
                }
                else if (m_PauseDirection == 3)//��
                {
                    StringCursol.transform.position -= (Vector3)m_Offset_X;
                }
            }
        };
        inputActions.Stirng.tama.performed += ctx =>
        {
            // ���܂𐶐����鏈��
            if (Strings.Count > 0)
            {
                BallStopper();
            }
        };
        inputActions.Stirng.start.performed += ctx =>
        {
            //�ŏ��̏��_�����߂�
            // GameObject first = Instantiate(StringPrefub, StringCursol.transform.position, Quaternion.identity);
            GameObject dummy = new GameObject("FirstPoint");
            dummy.transform.position = StringCursol.transform.position;
            Strings.Add(dummy);
            m_StringMode = isString; // �X�g�����O���[�h��L���ɂ���
        };
    }

    void Start()
    {
        //�ŏ��̏��_�����߂�
        m_Offset_X=new Vector2(m_StrinngScale.x, 0.0f);
        m_Offset_Y=new Vector2(0.0f,-m_StrinngScale.y);
        m_LastDirection = Middle;
        listDisplay.UpdateDisplay(StringNum);// Text�\�����X�V
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnRightInput()
    {
        if (m_LastDirection == LEFT) return;
        // �Ō�̃I�u�W�F�N�g�̉E�ɐ���
        Vector3 lastPos = Strings[^1].transform.position;//�Ō�̃I�u�W�F�N�g�̈ʒu���擾���������ŏ������n�_�̏ꏊ�Ɏw�肵����ŏ����s���Ȃ�������������
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
        else if (m_LastDirection == Middle)
        {
            newPos = lastPos + (Vector3)m_Offset_X/2; // �ŏ��̈ʒu����E�ɂ��炷
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
        else if (m_LastDirection == Middle)
        {
            newPos = lastPos - (Vector3)m_Offset_X/2; // �ŏ��̈ʒu����E�ɂ��炷
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
        else if (m_LastDirection == Middle)
        {
            newPos = lastPos - (Vector3)m_Offset_Y/2; // �ŏ��̈ʒu����E�ɂ��炷
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
        else if (m_LastDirection == Middle)
        {
            newPos = lastPos + (Vector3)m_Offset_Y / 2; // �ŏ��̈ʒu����E�ɂ��炷
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
    public void BallStopper()
    {
        Vector3 lastPos = Strings[^1].transform.position;
        Vector3 newPos = new Vector3(0.0f, 0.0f, 0.0f);//������
        switch (m_LastDirection)
        {
            case RIGHT:
                // �E�ɂ��܂��~�߂鏈��
                newPos = lastPos + (Vector3)m_Offset_X / 2;        //offset���}�C�i�X�ɂ��ĉE����
                break;
            case LEFT:
                // ���ɂ��܂��~�߂鏈��
                newPos = lastPos - (Vector3)m_Offset_X / 2;        //offset���}�C�i�X�ɂ��ĉE����
                break;
            case Up:
                // ��ɂ��܂��~�߂鏈��
                newPos = lastPos - (Vector3)m_Offset_Y / 2;
                break;
            case Down:
                // ���ɂ��܂��~�߂鏈��
                newPos = lastPos + (Vector3)m_Offset_Y / 2;
                break;
        }
        // ���܂��~�߂鏈��
        GameObject tama = Instantiate(Tamadome, newPos, Quaternion.identity);
        m_StringMode = NoString;
        m_LastDirection = Middle; // ���O�̕�����������

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
