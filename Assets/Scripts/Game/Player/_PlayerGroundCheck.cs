using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    [Header("地面判定用の子オブジェクト")]
    public Transform groundCheck;   // 足元判定の子オブジェクト

    public LayerMask groundLayers;

    public bool IsGrounded { get; private set; }

    private Vector2 checkSize;

    void Start()
    {
        if (groundCheck == null)
        {
            Debug.LogError("groundCheckがセットされていません！");
            return;
        }

        // 子オブジェクトのBoxCollider2Dを取得
        BoxCollider2D box = groundCheck.GetComponent<BoxCollider2D>();

        if (box != null)
        {
            // BoxCollider2Dのサイズを判定サイズに設定
            checkSize = box.size;
        }
        else
        {
            Debug.LogWarning("groundCheckにBoxCollider2Dがありません。デフォルトサイズを使います。");
            checkSize = new Vector2(0.5f, 0.1f);
        }
    }

    void FixedUpdate()
    {
        if (groundCheck == null) return;

        IsGrounded = Physics2D.OverlapBox(groundCheck.position, checkSize, 0f, groundLayers);
    }
}