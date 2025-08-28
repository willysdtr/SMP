using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ResultScreen : MonoBehaviour
{
    [SerializeField] private RectTransform panel;
    [SerializeField] private RawImage winScreen;
    [SerializeField] private RawImage loseScreen;

    //------デバッグ用------
    [SerializeField] private bool useDebug;
    [SerializeField] private DebugResult debugResult = DebugResult.Win;
    private enum DebugResult { Win, Lose }

    private Vector2 hiddenPos;
    private Vector2 centerPos;


    void Start()
    {
        centerPos = panel.anchoredPosition;
        hiddenPos = new Vector2(centerPos.x, Screen.height);
        panel.anchoredPosition = hiddenPos;

        winScreen.gameObject.SetActive(false);
        loseScreen.gameObject.SetActive(false);

        if (useDebug)
        {
            ShowResult(debugResult == DebugResult.Win);
        }
    }

    public void ShowResult(bool isWin)
    {
        panel.anchoredPosition = hiddenPos;

        winScreen.gameObject.SetActive(isWin);
        loseScreen.gameObject.SetActive(!isWin);

        panel.DOAnchorPos(centerPos, 0.5f).SetEase(Ease.InOutQuad);
    }
}
