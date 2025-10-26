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
            // --- Additive対応のシングルトン管理 ---
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("既にPauseのインスタンスが存在するため、古いものを置き換えます。");
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
            Debug.Log("Pause 生きてる");
            inputActions.Pause.Enable();
        }

        private void OnDisable()
        {
            Debug.Log("Pause 死んだ");
            inputActions.Pause.Disable();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Debug.Log("Pause インスタンス削除");
                Instance = null;
            }
        }

        private void ManualDisable()
        {
            Debug.Log("Pause 意図的に殺した");
            inputActions.Pause.Disable();
        }

        public void ManualEnable()
        {
            Debug.Log("Pause 意図的に生かした");
            inputActions.Pause.Enable();
        }

        private void Update()
        {
            if (PauseApperance.Instance.isPause == true)
            {
                Select.transform.position = ButtonPos[m_SelectCount].transform.position;
            }
        }

        void ChangeScenePause()//ここから
        {
            switch (m_SelectCount)
            {
                case 0://続ける
                    Debug.Log("Resume Game");
                    inputActions.Pause.Disable();
                    inputActions.Select.Enable();
                    SceneManager.UnloadSceneAsync("PauseScene");
                    break;
                case 1:
                    // リトライ
                    Debug.Log("Open Options");
                    // オプション画面を開く処理をここに追加
                    break;
                case 2:
                    // チュートリアル
                    Debug.Log("Quit Game");

                    break;
                case 3:
                    //サウンド
                    AudioChange.SetActive(true);
                    PauseApperance.Instance.isPause = false;
                    ManualDisable();
                    //Soundを出す（あっちでflgをOnにしたら）
                    break;
                case 4:
                    SceneManager.LoadScene("SelectScene");
                    //セレクトに戻る
                    break;
                case 5:
                    SceneManager.LoadScene("TitleScene");   
                    //タイトルに戻る
                    // Application.Quit(); // ゲームを終了
                    break;
            }
        }
    }
}