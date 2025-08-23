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
    private List<GameObject> MirrorStrings = new List<GameObject>();
    private List<GameObject> FrontStrings = new List<GameObject>();
    private List<GameObject> BackStrings = new List<GameObject>();
    [SerializeField] private float mirrorOffsetX = 5.0f; // ���Α��ɂ��炷����
    [SerializeField] List<int> StringNum;
    [SerializeField] List<int> CopyStringNum;
    private int currentIndex = 0;

<<<<<<< HEAD
    [SerializeField] private ShowStringNum listDisplay; // �\���N���X���C���X�y�N�^�[�ŃZ�b�g
=======
    [SerializeField] private float mirrorOffsetX = 5.0f;
    [SerializeField] private ShowStringNum listDisplay; // �\���N���X���C���X�y�N�^�[�ŃZ�b�g
>>>>>>> origin/Work_Taniguchi
    [SerializeField] GameObject Tamadome;
    [SerializeField] GameObject StringCursol;
    private InputSystem_Actions inputActions;
    private float m_PauseDirection;//���ʒ����̓��͒l
    private int m_LastDirection;//�O��̓��͒l

    bool m_StringMode = NoString;//�X�g�����O���[�h�̃t���O

    public bool EndSiting = false; // ���܂��~�߂邩�ǂ����̃t���O

    [SerializeField] private PrefubCursol prefubCursol;
    void Awake()
    {
        inputActions = new InputSystem_Actions();

        inputActions.Stirng.nami.performed += ctx =>
        {
            if (prefubCursol.IsMoving) return;
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


                //StringNum�����炤������OnRightInput, OnLeftInput, OnUpInput, OnDownInput�̒��ōs��

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
            if (currentIndex >= StringNum.Count)
            {
                return;
            }
            // ���܂𐶐����鏈��
            if (Strings.Count > 0)
            {
                BallStopper();
            }
        };
        inputActions.Stirng.start.performed += ctx =>
        {
            // ���ݏ����\�ȗv�f���Ȃ���ΏI��
            if (currentIndex >= StringNum.Count)
            {
                return;
            }
            //�ŏ��̏��_�����߂�
            GameObject dummy = new GameObject("FirstPoint");
            dummy.transform.position = StringCursol.transform.position;
            Strings.Add(dummy);
            m_StringMode = isString; // �X�g�����O���[�h��L���ɂ���
        };
        inputActions.Stirng.kaesi.performed += ctx =>
        {
            
            if (m_StringMode == NoString||currentIndex >= StringNum.Count|| StringNum[currentIndex] == CopyStringNum[currentIndex])//Start�n�_�ŕԂ��D�ł��Ȃ��悤��
            {
                Debug.Log(CopyStringNum[currentIndex]);
                Debug.Log(StringNum[currentIndex] );
                return;
            }
            OnKaesiInput();
        };
    }

    void Start()
    {
        m_Offset_X =new Vector2(m_StrinngScale.x, 0.0f);
        m_Offset_Y=new Vector2(0.0f,-m_StrinngScale.y);
        m_LastDirection = Middle;
        listDisplay.UpdateDisplay(StringNum);// Text�\�����X�V
        CopyStringNum = new List<int>(StringNum);
       // CopyStringNum =StringNum; // �R�s�[���쐬(�~)
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
            obj.tag = "Nami";
            //�A�j���[�V���������s
            Animator animator = obj.GetComponent<Animator>();
            animator.SetTrigger("Play"); // �A�j���[�V�������Đ�
            Strings.Add(obj);

<<<<<<< HEAD
            // --- ���Α��̃I�u�W�F�N�g�i�~���[�Ώ́j ---
            Vector3 mirrorPos = newPos;
            float mirrorCenterX = 0.0f; // �~���[�̒��S�ʒu���v�Z
            // newPos �� X �����E���]
            mirrorPos.x = mirrorCenterX - (newPos.x - mirrorCenterX);

            // Y/Z �͂��̂܂�
=======
            Vector3 mirrorPos = newPos;
            float mirrorCenterX = 0.0f;
            mirrorPos.x = mirrorCenterX - (newPos.x - mirrorCenterX);
>>>>>>> origin/Work_Taniguchi
            GameObject mirrorObj = Instantiate(StringPrefub, mirrorPos, Quaternion.Euler(0, 180, 0));
            mirrorObj.tag = "Nami_Mirror";
            Animator mirrorAnimator = mirrorObj.GetComponent<Animator>();
            mirrorAnimator.SetTrigger("Play");
            MirrorStrings.Add(mirrorObj);

            GameObject frontobj = Instantiate(StringPrefub, FrontlastPos, Quaternion.identity);
            FrontStrings.Add(frontobj);
            GameObject backobj = Instantiate(StringPrefub, BacklastPos, Quaternion.identity);
            BackStrings.Add(backobj);

            m_LastDirection = RIGHT; // ���O�̕������X�V
                                     // �Ώۗv�f��1���炷
            StringNum[currentIndex]--;

            Debug.Log($"Index {currentIndex} �̗v�f��1���炵�܂����B�c��: {StringNum[currentIndex]}");

            listDisplay.UpdateDisplay(StringNum);// Text�\�����X�V
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
            obj.tag = "Nami"; 
            //�A�j���[�V���������s
            Animator animator = obj.GetComponent<Animator>();
            animator.SetTrigger("Play"); // �A�j���[�V�������Đ�
            Strings.Add(obj);

<<<<<<< HEAD
            // --- ���Α��̃I�u�W�F�N�g�i�~���[�Ώ́j ---
            Vector3 mirrorPos = newPos;
            float mirrorCenterX = 0.0f; // �~���[�̒��S�ʒu���v�Z
            // newPos �� X �����E���]
            mirrorPos.x = mirrorCenterX - (newPos.x - mirrorCenterX);

            // Y/Z �͂��̂܂�
            GameObject mirrorObj = Instantiate(StringPrefub, mirrorPos, Quaternion.identity);
=======
            Vector3 mirrorPos = newPos;
            float mirrorCenterX = 0.0f;
            mirrorPos.x = mirrorCenterX - (newPos.x - mirrorCenterX);
            GameObject mirrorObj = Instantiate(StringPrefub, mirrorPos, Quaternion.Euler(0, 180, 0));
>>>>>>> origin/Work_Taniguchi
            mirrorObj.tag = "Nami_Mirror";
            Animator mirrorAnimator = mirrorObj.GetComponent<Animator>();
            mirrorAnimator.SetTrigger("Play");
            MirrorStrings.Add(mirrorObj);

            GameObject frontobj = Instantiate(StringPrefub, FrontlastPos, Quaternion.identity);
            FrontStrings.Add(frontobj);
            GameObject backobj = Instantiate(StringPrefub, BacklastPos, Quaternion.identity);
            BackStrings.Add(backobj);

            m_LastDirection = LEFT; // ���O�̕������X�V
                                    // �Ώۗv�f��1���炷
            StringNum[currentIndex]--;

            Debug.Log($"Index {currentIndex} �̗v�f��1���炵�܂����B�c��: {StringNum[currentIndex]}");

            listDisplay.UpdateDisplay(StringNum);// Text�\�����X�V
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
            obj.tag = "Nami";
            //�A�j���[�V���������s
            Animator animator = obj.GetComponent<Animator>();
            animator.SetTrigger("Play"); // �A�j���[�V�������Đ�
            Strings.Add(obj);

<<<<<<< HEAD
            // --- ���Α��̃I�u�W�F�N�g�i�~���[�Ώ́j ---
            Vector3 mirrorPos = newPos;
            float mirrorCenterX = 0.0f; // �~���[�̒��S�ʒu���v�Z
            // newPos �� X �����E���]
            mirrorPos.x = mirrorCenterX - (newPos.x - mirrorCenterX);

            // Y/Z �͂��̂܂�
            GameObject mirrorObj = Instantiate(StringPrefub, mirrorPos, Quaternion.Euler(0, 0, 90));
=======
            Vector3 mirrorPos = newPos;
            float mirrorCenterX = 0.0f;
            mirrorPos.x = mirrorCenterX - (newPos.x - mirrorCenterX);
            GameObject mirrorObj = Instantiate(StringPrefub, mirrorPos, Quaternion.Euler(0, 180, 0));
>>>>>>> origin/Work_Taniguchi
            mirrorObj.tag = "Nami_Mirror";
            Animator mirrorAnimator = mirrorObj.GetComponent<Animator>();
            mirrorAnimator.SetTrigger("Play");
            MirrorStrings.Add(mirrorObj);

<<<<<<< HEAD
            //��[�A�O��̓����蔻����擾
=======
>>>>>>> origin/Work_Taniguchi
            GameObject frontobj = Instantiate(StringPrefub, FrontlastPos, Quaternion.identity);
            FrontStrings.Add(frontobj);
            GameObject backobj = Instantiate(StringPrefub, BacklastPos, Quaternion.identity);
            BackStrings.Add(backobj);

            m_LastDirection = Up; // ���O�̕������X�V
                                  // �Ώۗv�f��1���炷
            StringNum[currentIndex]--;

            Debug.Log($"Index {currentIndex} �̗v�f��1���炵�܂����B�c��: {StringNum[currentIndex]}");

            listDisplay.UpdateDisplay(StringNum);// Text�\�����X�V
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
            obj.tag = "Nami";
            animator.SetTrigger("Play"); // �A�j���[�V�������Đ�
            Strings.Add(obj);

<<<<<<< HEAD

            // --- ���Α��̃I�u�W�F�N�g�i�~���[�Ώ́j ---
            Vector3 mirrorPos = newPos;
            float mirrorCenterX = 0.0f; // �~���[�̒��S�ʒu���v�Z
            // newPos �� X �����E���]
            mirrorPos.x = mirrorCenterX - (newPos.x - mirrorCenterX);

            // Y/Z �͂��̂܂�
            GameObject mirrorObj = Instantiate(StringPrefub, mirrorPos, Quaternion.Euler(0, 0, 270));
=======
            Vector3 mirrorPos = newPos;
            float mirrorCenterX = 0.0f;
            mirrorPos.x = mirrorCenterX - (newPos.x - mirrorCenterX);
            GameObject mirrorObj = Instantiate(StringPrefub, mirrorPos, Quaternion.Euler(0, 180, 0));
>>>>>>> origin/Work_Taniguchi
            mirrorObj.tag = "Nami_Mirror";
            Animator mirrorAnimator = mirrorObj.GetComponent<Animator>();
            mirrorAnimator.SetTrigger("Play");
            MirrorStrings.Add(mirrorObj);

<<<<<<< HEAD

=======
>>>>>>> origin/Work_Taniguchi
            GameObject frontobj = Instantiate(StringPrefub, FrontlastPos, Quaternion.identity);
            FrontStrings.Add(frontobj);
            GameObject backobj = Instantiate(StringPrefub, BacklastPos, Quaternion.identity);
            BackStrings.Add(backobj);

            m_LastDirection =Down; // ���O�̕������X�V
                                   // �Ώۗv�f��1���炷
            StringNum[currentIndex]--;

            Debug.Log($"Index {currentIndex} �̗v�f��1���炵�܂����B�c��: {StringNum[currentIndex]}");

            listDisplay.UpdateDisplay(StringNum);// Text�\�����X�V
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
    void OnKaesiInput()
    {
        Vector3 lastPos = Strings[^1].transform.position;
        Vector3 newPos = new Vector3(0.0f, 0.0f, 0.0f);//������
        GameObject obj=new GameObject();
        Animator animator=new Animator();
        switch (m_LastDirection)
        {
            case RIGHT:
                // �E�ɂ��܂��~�߂鏈��
                newPos = lastPos - (Vector3)m_Offset_Y / 10;        //offset���}�C�i�X�ɂ��ĉE����
                obj = Instantiate(StringPrefub, newPos, Quaternion.identity);
                obj.transform.rotation = Quaternion.Euler(0, 180, 0); // �������ɉ�]
                obj.tag = "Kaesi";
                animator = obj.GetComponent<Animator>();
                animator.SetTrigger("Play"); // �A�j���[�V�������Đ�
                newPos = lastPos;//�z��ɓ����ꏊ�͏�Ɉړ����������Ȃ��̂Ō��ɖ߂�
                //obj.transform.position = newPos; //����͏ꏊ�͂����񂯂�obj�������Ȃ�
                obj = Instantiate(StringPrefub, newPos, Quaternion.identity);//����̏ꍇ�͗v��Ȃ�Obj�������邯�Ǐꏊ�͂�������
                Strings.Add(obj);
                m_LastDirection = LEFT; //�Ԃ��D�Ȃ̂Ŏ����t
                break;
            case LEFT:
                // ���ɂ��܂��~�߂鏈��                  
                newPos = lastPos - (Vector3)m_Offset_Y / 10;         //offset���}�C�i�X�ɂ��ĉE����
                obj = Instantiate(StringPrefub, newPos, Quaternion.identity);
                obj.tag = "Kaesi";
                animator = obj.GetComponent<Animator>();
                animator.SetTrigger("Play"); // �A�j���[�V�������Đ�
                newPos = lastPos;//�z��ɓ����ꏊ�͏�Ɉړ����������Ȃ��̂Ō��ɖ߂�
                //obj.transform.position = newPos; // �ʒu�����ɖ߂�
                 obj = Instantiate(StringPrefub, newPos, Quaternion.identity);//����̏ꍇ�͗v��Ȃ�Obj�������邯�Ǐꏊ�͂�������
                Strings.Add(obj);
                m_LastDirection = RIGHT; //�Ԃ��D�Ȃ̂Ŏ����t
                break;
            case Up:
                return;
           case Down: 
                return;
        }
                                                           
                                // �Ώۗv�f��1���炷
        StringNum[currentIndex]--;

        Debug.Log($"Index {currentIndex} �̗v�f��1���炵�܂����B�c��: {StringNum[currentIndex]}");

        listDisplay.UpdateDisplay(StringNum);// Text�\�����X�V
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
