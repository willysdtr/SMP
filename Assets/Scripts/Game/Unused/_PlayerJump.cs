using UnityEngine;

public class _PlayerJump : MonoBehaviour
{
    //プレイヤーのジャンプ処理スクリプト
    private Rigidbody2D rb;

    private Vector2 velocity;
    private float gravity = -9.81f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gravity = Physics.gravity.y;
    }

    // Update is called once per frame

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
        rb.linearVelocity = new  Vector2(velocity.x, 0);
    }

}
