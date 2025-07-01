using UnityEngine;

public class PlayerJump : MonoBehaviour
{

    private Rigidbody2D rb;
    private float jumpForce = 2f;
    [SerializeField]
    [Header("‚Î‚Ë‚ÌƒWƒƒƒ“ƒv—Í")]
    private float gimjumpForce = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame

    public void Jump(bool gim)
    {
        
        float jump = gim ? jumpForce : gimjumpForce;
        rb.AddForce(Vector2.up * jump, ForceMode2D.Impulse);
    }
}
