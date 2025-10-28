using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class ResultScreen : MonoBehaviour
{
    [SerializeField] private RectTransform panel;
    [SerializeField] private RawImage winScreen;
    [SerializeField] private RawImage loseScreen;
    [SerializeField] private RawImage cursor; 
    [SerializeField] private RectTransform leftButtonPos;  
    [SerializeField] private RectTransform rightButtonPos; 

    //------デバッグ用------
    [SerializeField] private bool useDebug;
    [SerializeField] private DebugResult debugResult = DebugResult.Win;
    private enum DebugResult { Win, Lose }

    private Vector2 hiddenPos;
    private Vector2 centerPos;

    private bool isWin;
    private bool isLeftSelected = true;

    private InputSystem_Actions inputsystem;
    private InputAction navigateAction;
    private InputAction submitAction;
    private bool inputEnabled;
    private float horizontalInput;


    void Start()
    {
        inputsystem = new InputSystem_Actions();


        centerPos = panel.anchoredPosition;
        hiddenPos = new Vector2(centerPos.x, Screen.height);
        panel.anchoredPosition = hiddenPos;

        winScreen.gameObject.SetActive(false);
        loseScreen.gameObject.SetActive(false);
        cursor.gameObject.SetActive(false);

        if (useDebug)
        {
            ShowResult(debugResult == DebugResult.Win);
        }
    }

    public void ShowResult(bool win)
    {
        panel.anchoredPosition = hiddenPos;

        winScreen.gameObject.SetActive(win);
        loseScreen.gameObject.SetActive(!win);

        cursor.gameObject.SetActive(true);
        cursor.rectTransform.anchoredPosition = rightButtonPos.anchoredPosition;
        MoveCursor(isLeftSelected ? leftButtonPos : rightButtonPos, instant: true);

        panel.DOAnchorPos(centerPos, 0.5f).SetEase(Ease.InOutQuad);
    }

    void Update()
    {
        // Ignore input if the result screen is hidden
        if (!panel.gameObject.activeInHierarchy) return;

        inputsystem.Select.Move.performed += ctx =>
        {
            horizontalInput = ctx.ReadValue<float>();
            if (horizontalInput == 1)
            {
                SetSelection(true);
            }
            else
            {
                SetSelection(false);
            }
        };

        inputsystem.Select.SelectStage.performed += ctx =>
        {
            ConfirmSelection();
        };
            
    }

    void SetSelection(bool left)
    {
        if (isLeftSelected == left) return;
        isLeftSelected = left;
        MoveCursor(isLeftSelected ? leftButtonPos : rightButtonPos);
    }

    void MoveCursor(RectTransform target, bool instant = false)
    {
        if (instant)
            cursor.rectTransform.anchoredPosition = target.anchoredPosition;
        else
            cursor.rectTransform.DOAnchorPos(target.anchoredPosition, 0.2f).SetEase(Ease.OutQuad);
    }

    void ConfirmSelection()
    {
        if (isWin)
        {
            if (isLeftSelected)
                OnWinLeft();
            else
                OnWinRight();
        }
        else
        {
            if (isLeftSelected)
                OnLoseLeft();
            else
                OnLoseRight();
        }
    }

    void OnWinLeft()
    {
        SMPState.Instance.m_CurrentGameState = SMPState.GameState.SelectStage;//Gameplay状態にする
        // ゲームステージをロードする
        SceneManager.LoadScene("SelectScene");
    }

    void OnWinRight()
    {
        SMPState.CURRENT_STAGE += 1;
        SMPState.Instance.m_CurrentGameState = SMPState.GameState.PlayGame;//Gameplay状態にする
        // ゲームステージをロードする
        SceneManager.LoadScene("GameScene");
    }

    void OnLoseLeft()
    {
        SMPState.Instance.m_CurrentGameState = SMPState.GameState.SelectStage;//Gameplay状態にする
        // ゲームステージをロードする
        SceneManager.LoadScene("SelectScene");
    }

    void OnLoseRight()
    {
        SMPState.Instance.m_CurrentGameState = SMPState.GameState.PlayGame;//Gameplay状態にする
        // ゲームステージをロードする
        SceneManager.LoadScene("GameScene");
    }


}
