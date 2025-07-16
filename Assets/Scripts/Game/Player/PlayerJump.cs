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


    private Vector2 startPosition;
    private Vector2 velocity;
    private float elapsed = 0f;
    private float gravity = -9.81f;
    private float maxTime = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gravity = Physics.gravity.y;
    }

    // Update is called once per frame

    public void InitJump(Vector2 initialVelocity)
    {
        rb.gravityScale = 0;
        startPosition = transform.position;
        velocity = initialVelocity;
        elapsed = 0f;

    }

    public void Jump(bool gim)
    {
        //float jump = gim ? gimjumpForce : jumpForce;
        ////rb.linearVelocity = new Vector2(100f, rb.linearVelocity.y);
        //rb.AddForce(Vector2.up * jump, ForceMode2D.Impulse);
        elapsed += Time.deltaTime;

        // �������̌v�Z
        float x = velocity.x * elapsed;
        float y = velocity.y * elapsed + 0.5f * gravity * Mathf.Pow(elapsed, 2);

        transform.position = startPosition + new Vector2(x, y);

    }

    public void Move(float speed)
    {
        //rb.position += new Vector2(speed * Time.deltaTime, 0);
    }

    //��������ɂ΂˃W�����v�B
    //�΂˂͗\������t����B
    //�W�����v�����̓v���C���[�̈ʒu����2�}�X��B
}
