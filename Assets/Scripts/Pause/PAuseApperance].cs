using Script;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class PauseApperance : MonoBehaviour
{
    public static PauseApperance Instance { get; private set; }
    private InputSystem_Actions inputActions;
    public bool isPause = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
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

        inputActions = new InputSystem_Actions();//PlayerInputActionsのインスタンスを生成
        inputActions.PauseApperance.Apperance.performed += ctx =>//ここの処理をSMP_SceneManagerに移動させよう！
        {
            Debug.Log("PauseSceneLoad");
            SMPState.Instance.m_CurrentGameState = SMPState.GameState.Pause;//Pause状態にする
            inputActions.Select.Disable();//PlayerInputActionsを無効化
            SceneManager.LoadScene("PauseScene", LoadSceneMode.Additive);
            isPause = true;
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnEnable()
    {
        inputActions.Enable();//PlayerInputActionsを有効化
    }

    void OnDisable()
    {
        inputActions.Disable();//PlayerInputActionsを無効化
    }
}
