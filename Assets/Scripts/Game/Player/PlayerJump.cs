using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    public float blockSize = 50.0f;    // 1ブロックの大きさ
    public float jumpHeight = 100.0f;   // ジャンプの高さ
    public float duration = 2f;   // ジャンプにかかる時間

    private Vector2 startPos;
    private Vector2 endPos;
    private Vector2 controlPos;
    private float elapsed;
    private bool isJumping;
    bool direction = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            direction = true;
            Jump(transform.position.x, transform.position.y, true); // 右方向にジャンプ
        }
        else if(Input.GetKeyDown(KeyCode.A))
        {
            direction = false;
            Jump(transform.position.x, transform.position.y, false); // 左方向にジャンプ
        }

        if(isJumping)
        {
            Jump(transform.position.x, transform.position.y, direction); // ジャンプの更新
        }
    }

    void Jump(float posX, float posY, bool isDirection)
    {
        if (!isJumping) // ジャンプ開始
        {

            const int endDistance = 2;
            startPos = new(posX, posY);

            int direction = isDirection ? 1 : -1;

            // 2ブロック先を計算
            endPos = startPos + new Vector2(direction * blockSize * endDistance, 0);

            // 制御点（中間地点 + 高さ）
            Vector2 mid = (startPos + endPos) / 2f;
            controlPos = mid + Vector2.up * jumpHeight;

            elapsed = 0f;
            isJumping = true;
        }

        if (isJumping) //ジャンプ中
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

            if (t >= 1f) isJumping = false;
        }
    }
}
