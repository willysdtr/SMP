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

        // RectTransform �̃T�C�Y���擾���� BoxCollider2D �ɔ��f
        RectTransform rect = GetComponent<RectTransform>();
        if (rect != null)
        {
            col.size = rect.rect.size;
        }
    }
}
