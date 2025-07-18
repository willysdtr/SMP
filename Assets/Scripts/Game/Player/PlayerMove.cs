using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //�v���C���[�̓����S�ʂ̃X�N���v�g
    PlayerState state;
    Rigidbody2D rb;
    [SerializeField]

    private float currentspeed = 0f;

    private Vector2 velocity;
    private float gravity = -9.81f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        state = GetComponent<PlayerState>();
    }

    public void Move()
    {
        //��������
        if (state.m_direction != 0)//������0�łȂ����
        {
            currentspeed += PlayerState.MAX_SPEED * Time.deltaTime;
            currentspeed = Mathf.Min(currentspeed, PlayerState.MAX_SPEED);
        }
        else
        {
            // ��������
            currentspeed -= PlayerState.MAX_SPEED * Time.deltaTime;
            currentspeed = Mathf.Max(currentspeed, 0f);
        }
        rb.linearVelocity = new Vector2(state.m_direction * currentspeed, rb.linearVelocity.y);//���x����
    }

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
        rb.linearVelocity = new Vector2(velocity.x, 0);
    }

    public void Climb(float speed)//speed�̒l������Ɉړ�����
    {
        rb.position += new Vector2(0, speed * Time.deltaTime);
    }

    public void Stop()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);//�������̑��x�݂̂�0�ɂ���
    }

    public void AllStop()
    {
        //���������S�Ɏ~�߂�
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = 0f;
    }
}
