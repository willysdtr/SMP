using StageInfo;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //�v���C���[�̓����S�ʂ̃X�N���v�g
    Rigidbody2D rb;
    [SerializeField]

    private float currentspeed = 0f;

    private Vector2 velocity;
    private float gravity = -9.81f;

    public int jumpHeight = 2;  // �W�����v�̍���(�u���b�N�P��)
    public float duration = 2f;   // �W�����v�ɂ����鎞��

    private float elapsed;//�W�����v�ɂ�����������
    private Vector2 startPos;
    private Vector2 endPos;
    private Vector2 controlPos;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Move(int direction)
    {
        //��������
        if (direction != 0)//������0�łȂ����
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
        rb.linearVelocity = new Vector2(direction * currentspeed, rb.linearVelocity.y);//���x����
    }

    public void InitJump(Vector2 initialVelocity)//�W�����v������
    {
        rb.gravityScale = 0;
        velocity = initialVelocity;
    }

    //public void Jump()//�W�����v����
    //{
    //    //�������̋N���ňړ�����

    //    // �d�͂𑬓x�ɉ�����
    //    velocity += Vector2.down * Mathf.Abs(gravity) * Time.deltaTime;

    //    // �ړ�����
    //    Vector2 displacement = velocity * Time.deltaTime;
    //    transform.position += (Vector3)displacement;

    //}

    public void InitJump(int direction, float blocksize)
    {
        const int endDistance = 2;
        startPos = new(transform.position.x, transform.position.y);

        // 2�u���b�N����v�Z
        endPos = startPos + new Vector2(direction * blocksize * endDistance, 0);

        // ����_�i���Ԓn�_ + �����j
        Vector2 mid = (startPos + endPos) / 2f;
        controlPos = mid + Vector2.up * jumpHeight * blocksize;

        elapsed = 0f;
        rb.gravityScale = 0;
    }

    public bool Jump()
    {
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / duration);

        // �x�W�F�Ȑ�
        float x = Mathf.Pow(1 - t, 2) * startPos.x +
                  2 * (1 - t) * t * controlPos.x +
                  Mathf.Pow(t, 2) * endPos.x;

        float y = Mathf.Pow(1 - t, 2) * startPos.y +
                  2 * (1 - t) * t * controlPos.y +
                  Mathf.Pow(t, 2) * endPos.y;

        transform.position = new Vector2(x, y);

        if (t >= 1f)
        {
            EndJump();
            return true;
        }

        return false;
    }

    public void EndJump()//�W�����v�I������
    {
        rb.gravityScale = 30;
    }

    public void Climb(float speed)//speed�̒l������Ɉړ�����
    {
        rb.position += new Vector2(0, speed * Time.deltaTime);
    }

    public bool Goal(Vector2 goalpos)
    {
        int direction = 0;
        AllStop();
        if (Mathf.Abs(goalpos.x) - Mathf.Abs(transform.position.x) < PlayerState.MAX_SPEED / 10)
        {
            return true;//�S�[�����o���I�������true��Ԃ�
        }
        else
        {
            if (goalpos.x > transform.position.x)
            {
                direction = (int)PlayerState.Direction.RIGHT;//�E
            }
            else if (goalpos.x < transform.position.x)
            {
                direction = (int)PlayerState.Direction.LEFT;//��
            }
        }

        currentspeed = (PlayerState.MAX_SPEED * Time.deltaTime) / 10;
        rb.linearVelocity = new Vector2(direction * currentspeed, 0);
        Debug.Log("GOAL");
        return false;
    }

    public void Stop()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);//�������̑��x�݂̂�0�ɂ���
    }

    public void AllStop()
    {
        //���������S�Ɏ~�߂�
        currentspeed = 0f;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = 0f;
    }
}
