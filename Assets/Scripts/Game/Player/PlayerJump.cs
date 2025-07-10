using UnityEngine;

public class PlayerJump : MonoBehaviour
{

    private Rigidbody2D rb;
    [SerializeField]
    [Header("�W�����v��")]
    private float jumpForce = 5f;
    [SerializeField]
    [Header("�΂˂̃W�����v��")]
    private float gimjumpForce = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame

    public void Jump(bool gim)
    {
        float jump = gim ? gimjumpForce : jumpForce;
        //rb.linearVelocity = new Vector2(100f, rb.linearVelocity.y);
        rb.AddForce(Vector2.up * jump, ForceMode2D.Impulse);
    }

    public void Move(float speed)
    {
        rb.position += new Vector2(speed * Time.deltaTime, 0);
    }

    //���X�ɉ������ĖڕW�n�_�ɍs��(�����O��)
}
