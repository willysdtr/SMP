using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMove : MonoBehaviour
{
    PlayerStateMachine state_ma;
    Rigidbody2D rb;
    [Header("���x�ݒ�")]
    private float currentspeed = 0f;     // ���݂̑��x
    private float maxspeed = 5f;          // �ō����x

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        state_ma = GetComponentInParent<PlayerStateMachine>();
    }

    public void Move()
    {
        // ��������
        //if (state_ma.direction != 0)
        //{
        //    currentspeed += maxspeed * Time.deltaTime;
        //    currentspeed = Mathf.Min(currentspeed, maxspeed);
        //}
        //else
        //{
        //    // ��������
        //    currentspeed -= maxspeed * Time.deltaTime;
        //    currentspeed = Mathf.Max(currentspeed, 0f);
        //}
        Vector2 movepos = rb.position + new Vector2(state_ma.direction * currentspeed * Time.deltaTime, 0);
        rb.MovePosition(movepos);
        //Vector2 move = new Vector2(state_ma.direction * currentspeed, 0f);
        //transform.Translate(move * Time.deltaTime);
    }

}

