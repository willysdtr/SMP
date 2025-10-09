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

    public void InitJump(int direction, float blocksize)//ジャンプの初期化
    {
        Stop();
        const int endDistance = 2;
        startPos = new(transform.position.x, transform.position.y); // 開始位置

        // 2ブロック先を計算
        endPos = startPos + new Vector2(direction * blocksize * endDistance, 0);

        // 制御点（中間地点 + 高さ）
        Vector2 mid = (startPos + endPos) / 2f;
        controlPos = mid + Vector2.up * jumpHeight * blocksize;

        elapsed = 0f; // 経過時間をリセット
        rb.gravityScale = 0; // 重力を無効化
    }

    public bool Jump() //ジャンプ処理
    {
        elapsed += Time.deltaTime; // 経過時間を更新
        float t = Mathf.Clamp01(elapsed / duration); // 0から1の範囲に正規化

        // ベジェ曲線
        float x = Mathf.Pow(1 - t, 2) * startPos.x +
                  2 * (1 - t) * t * controlPos.x +
                  Mathf.Pow(t, 2) * endPos.x;

        float y = Mathf.Pow(1 - t, 2) * startPos.y +
                  2 * (1 - t) * t * controlPos.y +
                  Mathf.Pow(t, 2) * endPos.y;

        transform.position = new Vector2(x, y);

        if (t >= 1f) // 規定時間が経過していれば、ジャンプ終了
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

    public bool Goal(Vector2 goalpos)//ゴール処理、ゴールに向かって移動する
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

    public int Return(float angle) // 向き変更処理、指定された向きになる
    {
        int direction = (int)PlayerState.Direction.STOP;
        if (angle < 0) // 角度を補正する
        {
            // 右向き
            direction = (int)PlayerState.Direction.RIGHT;
            angle = -180;
        }
        else
        {
            // 左向き
            direction = (int)PlayerState.Direction.LEFT;
            angle = 0;
        }
        transform.eulerAngles = new Vector3(transform.rotation.x, angle, transform.rotation.z);
        return direction;
    }
}
