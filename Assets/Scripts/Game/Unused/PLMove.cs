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
            Move(transform.position.x, transform.position.y, true); // �E�����Ɉړ�
        }
        else if(Input.GetKeyDown(KeyCode.A))
        {
            Move(transform.position.x, transform.position.y, false); // �������Ɉړ�
        }
    }

    public void Move(float posX, float posY, bool isDirection)
    {
        const float blockSize = 1.0f;      // 1�u���b�N�̑傫��
        float maxspeed = blockSize;         // �ő呬�x
        float acceleration = maxspeed / 60; // �����x

        //��������
        currentspeed += acceleration;
        currentspeed = Mathf.Min(currentspeed, maxspeed);

        if (isDirection)//true�Ȃ�E�A�Ⴄ�Ȃ獶
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
