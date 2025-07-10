using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMove : MonoBehaviour
{
    PlayerStateMachine state_ma;
    Rigidbody2D rb;
    [SerializeField]
    [Header("速度設定")]
    private float maxspeed = 5f;          // 最高速度

    public float maxspeed_read { get; private set; } = 0f;

    private float currentspeed = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        state_ma = GetComponentInParent<PlayerStateMachine>();
        maxspeed_read = maxspeed;
    }

    public void Move()
    {
        //加速処理
        if (state_ma.direction != 0)
        {
            currentspeed += maxspeed * Time.deltaTime;
            currentspeed = Mathf.Min(currentspeed, maxspeed);
        }
        else
        {
            // 減速処理
            currentspeed -= maxspeed * Time.deltaTime;
            currentspeed = Mathf.Max(currentspeed, 0f);
        }
        //rb.AddForceX(state_ma.direction * maxspeed);
        //Vector2 movepos = rb.position + new Vector2(state_ma.direction * currentspeed * Time.fixedDeltaTime, 0);
        //rb.MovePosition(movepos);
        //rb.position += new Vector2(state_ma.direction * currentspeed * Time.deltaTime, 0);
        rb.linearVelocity = new Vector2(state_ma.direction * currentspeed,rb.linearVelocity.y);
    }

    public void Stop()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    public void AllStop()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = 0f;
    }
}

