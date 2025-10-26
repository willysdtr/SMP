using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script
{
    public class Pause : MonoBehaviour
    {
        public static Pause Instance { get; private set; }

        [SerializeField] GameObject Select;
        [SerializeField] GameObject[] ButtonPos;
        [SerializeField] GameObject AudioChange;
        private int m_SelectCount = 0;
        private float PauseInput = 0;
        private InputSystem_Actions inputActions;

        private void Awake()
        {
            // --- Additive�Ή��̃V���O���g���Ǘ� ---
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("����Pause�̃C���X�^���X�����݂��邽�߁A�Â����̂�u�������܂��B");
                Instance = this;
            }
            else
            {
                Instance = this;
            }

            inputActions = new InputSystem_Actions();

            inputActions.Pause.PauseSelect.performed += ctx =>
            {
                PauseInput = ctx.ReadValue<float>();
                m_SelectCount = PauseInput switch
                {
                    1 => Mathf.Clamp(m_SelectCount + 1, 0, ButtonPos.Length - 1),
                    -1 => Mathf.Clamp(m_SelectCount - 1, 0, ButtonPos.Length - 1),
                    _ => m_SelectCount
                };
            };

            inputActions.Pause.Submit.performed += ctx =>
            {
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

        private void OnEnable()
        {
            Debug.Log("Pause �����Ă�");
            inputActions.Pause.Enable();
        }

        private void OnDisable()
        {
            Debug.Log("Pause ����");
            inputActions.Pause.Disable();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Debug.Log("Pause �C���X�^���X�폜");
                Instance = null;
            }
        }

        private void ManualDisable()
        {
            Debug.Log("Pause �Ӑ}�I�ɎE����");
            inputActions.Pause.Disable();
        }

        public void ManualEnable()
        {
            Debug.Log("Pause �Ӑ}�I�ɐ�������");
            inputActions.Pause.Enable();
        }

        private void Update()
        {
            if (PauseApperance.Instance.isPause == true)
            {
                Select.transform.position = ButtonPos[m_SelectCount].transform.position;
            }
        }

        void ChangeScenePause()//��������
        {
            switch (m_SelectCount)
            {
                case 0://������
                    Debug.Log("Resume Game");
                    inputActions.Pause.Disable();
                    inputActions.Select.Enable();
                    SceneManager.UnloadSceneAsync("PauseScene");
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
                    AudioChange.SetActive(true);
                    PauseApperance.Instance.isPause = false;
                    ManualDisable();
                    //Sound���o���i��������flg��On�ɂ�����j
                    break;
                case 4:
                    SceneManager.LoadScene("SelectScene");
                    //�Z���N�g�ɖ߂�
                    break;
                case 5:
                    SceneManager.LoadScene("TitleScene");   
                    //�^�C�g���ɖ߂�
                    // Application.Quit(); // �Q�[�����I��
                    break;
            }
        }
    }
}