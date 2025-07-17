using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //プレイヤーの動き全般のスクリプト
    PlayerStateMachine state_ma;
    Rigidbody2D rb;
    [SerializeField]
    [Header("速度設定")]
    private float maxspeed = 5f;          // 最高速度

    public float maxspeed_read { get; private set; } = 0f;//外部からは読み取り専用、書き込みはスクリプト内部でのみ可能

    private float currentspeed = 0f;

    private Vector2 velocity;
    private float gravity = -9.81f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        state_ma = GetComponentInParent<PlayerStateMachine>();
        maxspeed_read = maxspeed;//読み取り専用変数への代入
        var a = PlayerState.State.CLIMB;
    }

    public void Move()
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
        rb.linearVelocity = new Vector2(state_ma.direction * currentspeed, rb.linearVelocity.y);//速度を代入
    }

    public void InitJump(Vector2 initialVelocity)//ジャンプ初期化
    {
        rb.gravityScale = 0;
        velocity = initialVelocity;
    }

    public void Jump()//ジャンプ処理
    {
        //放物線の起動で移動する

        // 重力を速度に加える
        velocity += Vector2.down * Mathf.Abs(gravity) * Time.deltaTime;

        // 移動する
        Vector2 displacement = velocity * Time.deltaTime;
        transform.position += (Vector3)displacement;

    }

    public void EndJump()//ジャンプ終了処理
    {
        rb.gravityScale = 1;
        rb.linearVelocity = new Vector2(velocity.x, 0);
    }

    public void Climb(float speed)//speedの値だけ上に移動する
    {
        rb.position += new Vector2(0, speed * Time.deltaTime);
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
