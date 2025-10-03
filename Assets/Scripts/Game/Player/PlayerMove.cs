using StageInfo;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //プレイヤーの動き全般のスクリプト
    Rigidbody2D rb;
    [SerializeField]

    private float currentspeed = 0f;

    private Vector2 velocity;
    private float gravity = -9.81f;

    public int jumpHeight = 2;  // ジャンプの高さ(ブロック単位)
    public float duration = 2f;   // ジャンプにかかる時間

    private float elapsed;//ジャンプにかかった時間
    private Vector2 startPos;
    private Vector2 endPos;
    private Vector2 controlPos;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Move(int direction)
    {
        //加速処理
        if (direction != 0)//向きが0でなければ
        {
            currentspeed += PlayerState.MAX_SPEED * Time.deltaTime;
            currentspeed = Mathf.Min(currentspeed, PlayerState.MAX_SPEED);
        }
        else
        {
            // 減速処理
            currentspeed -= PlayerState.MAX_SPEED * Time.deltaTime;
            currentspeed = Mathf.Max(currentspeed, 0f);
        }
        rb.linearVelocity = new Vector2(direction * currentspeed, rb.linearVelocity.y);//速度を代入
    }

    public void InitJump(Vector2 initialVelocity)//ジャンプ初期化
    {
        rb.gravityScale = 0;
        velocity = initialVelocity;
    }

    //public void Jump()//ジャンプ処理
    //{
    //    //放物線の起動で移動する

    //    // 重力を速度に加える
    //    velocity += Vector2.down * Mathf.Abs(gravity) * Time.deltaTime;

    //    // 移動する
    //    Vector2 displacement = velocity * Time.deltaTime;
    //    transform.position += (Vector3)displacement;

    //}

    public void InitJump(int direction, float blocksize)
    {
        const int endDistance = 2;
        startPos = new(transform.position.x, transform.position.y);

        // 2ブロック先を計算
        endPos = startPos + new Vector2(direction * blocksize * endDistance, 0);

        // 制御点（中間地点 + 高さ）
        Vector2 mid = (startPos + endPos) / 2f;
        controlPos = mid + Vector2.up * jumpHeight * blocksize;

        elapsed = 0f;
        rb.gravityScale = 0;
    }

    public bool Jump()
    {
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / duration);

        // ベジェ曲線
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

    public void EndJump()//ジャンプ終了処理
    {
        rb.gravityScale = 30;
    }

    public void Climb(float speed)//speedの値だけ上に移動する
    {
        rb.position += new Vector2(0, speed * Time.deltaTime);
    }

    public bool Goal(Vector2 goalpos)
    {
        int direction = 0;
        AllStop();
        if (Mathf.Abs(goalpos.x) - Mathf.Abs(transform.position.x) < PlayerState.MAX_SPEED / 10)
        {
            return true;//ゴール演出が終わったらtrueを返す
        }
        else
        {
            if (goalpos.x > transform.position.x)
            {
                direction = (int)PlayerState.Direction.RIGHT;//右
            }
            else if (goalpos.x < transform.position.x)
            {
                direction = (int)PlayerState.Direction.LEFT;//左
            }
        }

        currentspeed = (PlayerState.MAX_SPEED * Time.deltaTime) / 10;
        rb.linearVelocity = new Vector2(direction * currentspeed, 0);
        Debug.Log("GOAL");
        return false;
    }

    public void Stop()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);//横方向の速度のみを0にする
    }

    public void AllStop()
    {
        //動きを完全に止める
        currentspeed = 0f;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = 0f;
    }
}
