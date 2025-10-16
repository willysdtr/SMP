using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;

public class _PlayerMove : MonoBehaviour
{
    //プレイヤーの移動スクリプト
    PlayerStateMachine state_ma;
    Rigidbody2D rb;
    [SerializeField]
    [Header("速度設定")]
    private float maxspeed = 5f;          // 最高速度

    public float maxspeed_read { get; private set; } = 0f;//外部からは読み取り専用、書き込みはスクリプト内部でのみ可能

    private float currentspeed = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        state_ma = GetComponentInParent<PlayerStateMachine>();
        maxspeed_read = maxspeed;//読み取り専用変数への代入
    }

    public void Move(int direction)
    {
        //加速処理
        if (state_ma.direction != 0)//向きが0でなければ
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
        rb.linearVelocity = new Vector2(state_ma.direction * currentspeed,rb.linearVelocity.y);//速度を代入
    }

    public void Stop()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);//横方向の速度のみを0にする
    }

    public void AllStop()
    {
        //動きを完全に止める
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = 0f;
    }
}

