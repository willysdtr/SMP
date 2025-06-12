using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class ToggleOblect : MonoBehaviour
{
    [Header("表示切替キー")]
    [SerializeField] private KeyCode toggleKey = KeyCode.Tab;

    private CanvasGroup cg;
    private bool isVisible = false;

    private void Reset()
    {
        cg = GetComponent<CanvasGroup>();
    }

    private void Awake()
    {
        if (cg == null) cg = GetComponent<CanvasGroup>();

        cg.alpha = 0f;
        cg.blocksRaycasts = false;
    }

    private void Update()
    {
        // トグル処理
        if (Input.GetKeyDown(toggleKey))
        {
            isVisible = !isVisible;
            cg.alpha = isVisible ? 1f : 0f;
            cg.blocksRaycasts = isVisible;
        }
    }
}
