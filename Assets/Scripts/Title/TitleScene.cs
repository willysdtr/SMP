using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour
{
    private InputSystem_Actions inputActions;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        inputActions = new InputSystem_Actions();
        inputActions.TItle.Entry.performed += ctx =>
        {
            Debug.Log("タイトル画面からセレクト画面へ移動");
            SceneManager.LoadScene("SelectScene");
        };
        inputActions.TItle.GameEnd.performed += ctx =>
        {
            Debug.Log("ゲーム終了");
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
            #else
                Application.Quit();//ゲームプレイ終了
            #endif
        };
    }
    // Update is called once per frame
    void OnEnable()
    {
        inputActions.TItle.Enable();//PlayerInputActionsを有効化
    }

    void OnDisable()
    {
        inputActions.TItle.Disable();//PlayerInputActionsを無効化
    }
}
