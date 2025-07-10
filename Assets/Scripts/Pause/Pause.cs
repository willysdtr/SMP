using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    [SerializeField] GameObject Select;
    [SerializeField] GameObject[] ButtonPos;
    private int m_SelectCount = 0;
    private float PauseInput = 0;//���ʒ����̓��͒l
    private InputSystem_Actions inputActions;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();//PlayerInputActions�̃C���X�^���X�𐶐�
        inputActions.Pause.PauseSelect.performed += ctx =>
        {
            PauseInput = ctx.ReadValue<float>();
            m_SelectCount=PauseInput switch
            {
                1 => Mathf.Clamp(m_SelectCount + 1, 0, ButtonPos.Length-1), // ��(�{�^�������T�Ȃ̂ŁA)
                -1 => Mathf.Clamp(m_SelectCount - 1, 0, ButtonPos.Length-1), // ��
                _ => m_SelectCount // ���̑��̓��͕͂ύX���Ȃ�
            };
        };
        inputActions.Pause.Submit.performed += ctx =>
        {
            // Submit�����������ɒǉ�
            ChangeScenePause();
            Debug.Log("Submit pressed");
        };
        inputActions.Pause.Delete.performed += ctx =>
        {
            inputActions.Pause.Disable();
            inputActions.Select.Enable();
            SceneManager.UnloadSceneAsync("PauseScene");
        };
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Select.transform.position = ButtonPos[m_SelectCount].transform.position ;
    }


    void ChangeScenePause()
    {
        switch(m_SelectCount)
        {
            case 0://������
                Debug.Log("Resume Game");

                break;
            case 1:
                // ���g���C
                Debug.Log("Open Options");
                // �I�v�V������ʂ��J�������������ɒǉ�
                break;
            case 2:
                // �`���[�g���A��
                Debug.Log("Quit Game");

                break;
            case 3:
                //�T�E���h
                ManualDisable();
                //Sound���o���i��������flg��On�ɂ�����j
                break;
            case 4:
                //�Z���N�g�ɖ߂�
                break;
            case 5:
                //�^�C�g���ɖ߂�
               // Application.Quit(); // �Q�[�����I��
                break;
            }
        }





    private void OnEnable()
    {
        inputActions.Pause.Enable();
    }
    private void OnDisable()
    {
        inputActions.Pause.Disable();
    }
    private void ManualDisable()
    {
        inputActions.Pause.Disable();
    }
}
