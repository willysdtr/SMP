using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class ResultScreen : MonoBehaviour
{
    [SerializeField] private PlayerController king;
    [SerializeField] private PlayerController queen;

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

    private float horizontalInput;

    private bool resultTrigger = false;

    private void Awake()
    {
        inputsystem = new InputSystem_Actions();

    }
    void Start()
    {
       
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
        isWin = win;

        panel.anchoredPosition = hiddenPos;

        winScreen.gameObject.SetActive(win);
        loseScreen.gameObject.SetActive(!win);

        RectTransform activeScreen = win ? winScreen.rectTransform : loseScreen.rectTransform;
        cursor.rectTransform.SetParent(activeScreen, worldPositionStays: false);

        int textIndex = activeScreen.Find("Text").GetSiblingIndex();
        cursor.rectTransform.SetSiblingIndex(textIndex);

        cursor.gameObject.SetActive(true);
        MoveCursor(isLeftSelected ? leftButtonPos : rightButtonPos, instant: true);

        panel.DOAnchorPos(centerPos, 0.5f).SetEase(Ease.InOutQuad);
    }

    void Update()
    {
        if (!resultTrigger)
        {
            if (king.death || queen.death)
            {
                ShowResult(false);
                resultTrigger = true;
            }
               
            else if (king.goal && queen.goal)
            {
                ShowResult(true);
                resultTrigger = true;
            }
                

            
        }
        

        // Ignore input if the result screen is hidden
        if (!panel.gameObject.activeInHierarchy) return;

        inputsystem.Select.Move.performed += ctx =>
        {
            horizontalInput = ctx.ReadValue<float>();
            if (horizontalInput != 1)
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
        //SMPState.Instance.m_CurrentGameState = SMPState.GameState.SelectStage;//Gameplay状態にする
        // ゲームステージをロードする
        SceneManager.LoadScene("SelectScene");
    }

    void OnWinRight()
    {
        SMPState.CURRENT_STAGE += 1;
        //SMPState.Instance.m_CurrentGameState = SMPState.GameState.PlayGame;//Gameplay状態にする
        // ゲームステージをロードする
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnLoseLeft()
    {
        //SMPState.Instance.m_CurrentGameState = SMPState.GameState.SelectStage;//Gameplay状態にする
        // ゲームステージをロードする
        SceneManager.LoadScene("SelectScene");
    }

    void OnLoseRight()
    {
        //SMPState.Instance.m_CurrentGameState = SMPState.GameState.PlayGame;//Gameplay状態にする
        // ゲームステージをロードする
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnEnable()
    {
        inputsystem.Enable();

        inputsystem.Select.Move.performed += OnMovePerformed;
        inputsystem.Select.SelectStage.performed += OnSelectPerformed;
    }

    void OnDisable()
    {
        inputsystem.Select.Move.performed -= OnMovePerformed;
        inputsystem.Select.SelectStage.performed -= OnSelectPerformed;

        inputsystem.Disable();
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        float horizontalInput = ctx.ReadValue<float>();

        if (horizontalInput > 0)
            SetSelection(false); // right
        else if (horizontalInput < 0)
            SetSelection(true); // left
    }

    private void OnSelectPerformed(InputAction.CallbackContext ctx)
    {
        ConfirmSelection();
    }
}
