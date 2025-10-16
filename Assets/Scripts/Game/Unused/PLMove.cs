using UnityEngine;

public class PLMove : MonoBehaviour
{

    private float currentspeed = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            Move(transform.position.x, transform.position.y, true); // 右方向に移動
        }
        else if(Input.GetKeyDown(KeyCode.A))
        {
            Move(transform.position.x, transform.position.y, false); // 左方向に移動
        }
    }

    public void Move(float posX, float posY, bool isDirection)
    {
        const float blockSize = 1.0f;      // 1ブロックの大きさ
        float maxspeed = blockSize;         // 最大速度
        float acceleration = maxspeed / 60; // 加速度

        //加速処理
        currentspeed += acceleration;
        currentspeed = Mathf.Min(currentspeed, maxspeed);

        if (isDirection)//trueなら右、違うなら左
        {
            posX += currentspeed;
        }
        else
        {
            posX -= currentspeed;
        }

        transform.position = new Vector2(posX, posY);
    }
}
