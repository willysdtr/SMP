using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    [SerializeField] GameObject Select;
    [SerializeField] GameObject[] ButtonPos;
    private int m_SelectCount = 0;
    private float PauseInput = 0;//音量調整の入力値
    private InputSystem_Actions inputActions;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();//PlayerInputActionsのインスタンスを生成
        inputActions.Pause.PauseSelect.performed += ctx =>
        {
            PauseInput = ctx.ReadValue<float>();
            m_SelectCount=PauseInput switch
            {
                1 => Mathf.Clamp(m_SelectCount + 1, 0, ButtonPos.Length-1), // 上(ボタン数が５なので、)
                -1 => Mathf.Clamp(m_SelectCount - 1, 0, ButtonPos.Length-1), // 下
                _ => m_SelectCount // その他の入力は変更しない
            };
        };
        inputActions.Pause.Submit.performed += ctx =>
        {
            // Submit処理をここに追加
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
            case 0://続ける
                Debug.Log("Resume Game");

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
                ManualDisable();
                //Soundを出す（あっちでflgをOnにしたら）
                break;
            case 4:
                //セレクトに戻る
                break;
            case 5:
                //タイトルに戻る
               // Application.Quit(); // ゲームを終了
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
