using UnityEngine;

public class PlayerClimb : MonoBehaviour
{
    //プレイヤーの登る処理スクリプト
    private Rigidbody2D rb;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Climb(float speed)//speedの値だけ上に移動する
    {
        rb.position += new Vector2(0, speed * Time.deltaTime);
    }
}
