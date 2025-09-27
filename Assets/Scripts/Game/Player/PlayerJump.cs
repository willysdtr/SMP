using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    public float blockSize = 50.0f;    // 1�u���b�N�̑傫��
    public float jumpHeight = 100.0f;   // �W�����v�̍���
    public float duration = 2f;   // �W�����v�ɂ����鎞��

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
            Jump(transform.position.x, transform.position.y, true); // �E�����ɃW�����v
        }
        else if(Input.GetKeyDown(KeyCode.A))
        {
            direction = false;
            Jump(transform.position.x, transform.position.y, false); // �������ɃW�����v
        }

        if(isJumping)
        {
            Jump(transform.position.x, transform.position.y, direction); // �W�����v�̍X�V
        }
    }

    void Jump(float posX, float posY, bool isDirection)
    {
        if (!isJumping) // �W�����v�J�n
        {

            const int endDistance = 2;
            startPos = new(posX, posY);

            int direction = isDirection ? 1 : -1;

            // 2�u���b�N����v�Z
            endPos = startPos + new Vector2(direction * blockSize * endDistance, 0);

            // ����_�i���Ԓn�_ + �����j
            Vector2 mid = (startPos + endPos) / 2f;
            controlPos = mid + Vector2.up * jumpHeight;

            elapsed = 0f;
            isJumping = true;
        }

        if (isJumping) //�W�����v��
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // �x�W�F�Ȑ�
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
