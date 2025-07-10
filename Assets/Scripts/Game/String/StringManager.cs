using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StringManager : MonoBehaviour
{
    [SerializeField] private GameObject StringPrefub;
    [SerializeField] Vector2 offset;
    private List<GameObject> Strings = new List<GameObject>();
    [SerializeField] private int StringNum;
    private InputSystem_Actions inputActions;
    private float m_PauseDirection;//���ʒ����̓��͒l
    void Awake()
    {
        inputActions.Stirng.nami.performed += ctx =>
        {
            float value = ctx.ReadValue<float>();
            m_PauseDirection = value;
            if (m_PauseDirection == 1)//��
            {
                Debug.Log("��̃A�j���[�V�����Đ�");
            }
            else if (m_PauseDirection == -1)//��
            {
                Debug.Log("���̃A�j���[�V�����Đ�");
            }
            else if (m_PauseDirection == 2)//�E
            {
                OnRightInput();
                Debug.Log("�E�̃A�j���[�V�����Đ�");
            }
            else if (m_PauseDirection == 3)//��
            {
                Debug.Log("���̃A�j���[�V�����Đ�");
            }
        };
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject first = Instantiate(StringPrefub, Vector3.zero, Quaternion.identity);
        Strings.Add(first);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnRightInput()
    {
        //�ꏊ�ς���Ăւ�I
        // �Ō�̃I�u�W�F�N�g�̉E�ɐ���
        Vector3 lastPos = Strings[^1].transform.position;
        Vector3 newPos = lastPos + (Vector3)offset;
        GameObject obj = Instantiate(StringPrefub, newPos, Quaternion.identity);
        Strings.Add(obj);
        Debug.Log(Strings);
    }
    void SewString()
    {

    }


    void OnEnable()
    {
        if (inputActions == null)
        {
            inputActions = new InputSystem_Actions();
        }
        inputActions.Stirng.Enable();
    }
    void OnDisable()
    {
        if (inputActions == null)
        {
            inputActions = new InputSystem_Actions();
        }
        inputActions.Stirng.Disable();
    }
}
