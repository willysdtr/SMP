using UnityEngine;

public class _PlayerJump : MonoBehaviour
{
    //�v���C���[�̃W�����v�����X�N���v�g
    private Rigidbody2D rb;

    private Vector2 velocity;
    private float gravity = -9.81f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gravity = Physics.gravity.y;
    }

    // Update is called once per frame

    public void InitJump(Vector2 initialVelocity)//�W�����v������
    {
        rb.gravityScale = 0;
        velocity = initialVelocity;
    }

    public void Jump()//�W�����v����
    {
        //�������̋N���ňړ�����

        // �d�͂𑬓x�ɉ�����
        velocity += Vector2.down * Mathf.Abs(gravity) * Time.deltaTime;

        // �ړ�����
        Vector2 displacement = velocity * Time.deltaTime;
        transform.position += (Vector3)displacement;

    }

    public void EndJump()//�W�����v�I������
    {
        rb.gravityScale = 1;
        rb.linearVelocity = new  Vector2(velocity.x, 0);
    }

}
