using UnityEngine;

public class BoxSizeSet : MonoBehaviour
{
    private BoxCollider2D col;

    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        UpdateColliderSize();
    }

    public void UpdateColliderSize()
    {
        if (col == null) return;

        // RectTransform ‚ÌƒTƒCƒY‚ðŽæ“¾‚µ‚Ä BoxCollider2D ‚É”½‰f
        RectTransform rect = GetComponent<RectTransform>();
        if (rect != null)
        {
            col.size = rect.rect.size;
        }
    }
}
