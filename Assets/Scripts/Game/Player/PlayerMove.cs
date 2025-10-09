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

    public void InitJump(int direction, float blocksize)//�W�����v�̏�����
    {
        Stop();
        const int endDistance = 2;
        startPos = new(transform.position.x, transform.position.y); // �J�n�ʒu

        // 2�u���b�N����v�Z
        endPos = startPos + new Vector2(direction * blocksize * endDistance, 0);

        // ����_�i���Ԓn�_ + �����j
        Vector2 mid = (startPos + endPos) / 2f;
        controlPos = mid + Vector2.up * jumpHeight * blocksize;

        elapsed = 0f; // �o�ߎ��Ԃ����Z�b�g
        rb.gravityScale = 0; // �d�͂𖳌���
    }

    public bool Jump() //�W�����v����
    {
        elapsed += Time.deltaTime; // �o�ߎ��Ԃ��X�V
        float t = Mathf.Clamp01(elapsed / duration); // 0����1�͈̔͂ɐ��K��

        // �x�W�F�Ȑ�
        float x = Mathf.Pow(1 - t, 2) * startPos.x +
                  2 * (1 - t) * t * controlPos.x +
                  Mathf.Pow(t, 2) * endPos.x;

        float y = Mathf.Pow(1 - t, 2) * startPos.y +
                  2 * (1 - t) * t * controlPos.y +
                  Mathf.Pow(t, 2) * endPos.y;

        transform.position = new Vector2(x, y);

        if (t >= 1f) // �K�莞�Ԃ��o�߂��Ă���΁A�W�����v�I��
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

    public bool Goal(Vector2 goalpos)//�S�[�������A�S�[���Ɍ������Ĉړ�����
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

    public int Return(float angle) // �����ύX�����A�w�肳�ꂽ�����ɂȂ�
    {
        int direction = (int)PlayerState.Direction.STOP;
        if (angle < 0) // �p�x��␳����
        {
            // �E����
            direction = (int)PlayerState.Direction.RIGHT;
            angle = -180;
        }
        else
        {
            // ������
            direction = (int)PlayerState.Direction.LEFT;
            angle = 0;
        }
        transform.eulerAngles = new Vector3(transform.rotation.x, angle, transform.rotation.z);
        return direction;
    }
}
