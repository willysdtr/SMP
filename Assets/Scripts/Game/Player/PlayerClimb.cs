using UnityEngine;

public class PlayerClimb : MonoBehaviour
{
    //�v���C���[�̓o�鏈���X�N���v�g
    private Rigidbody2D rb;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Climb(float speed)//speed�̒l������Ɉړ�����
    {
        rb.position += new Vector2(0, speed * Time.deltaTime);
    }
}
