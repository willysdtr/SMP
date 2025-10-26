using UnityEngine;

public class SeeSawReturn : MonoBehaviour
{

    private bool returnFgRight = false;
    private bool returnFgLeft = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float angleZ = transform.eulerAngles.z;
        if (angleZ > 180f) angleZ -= 360f;  // -180�`180�ɕϊ�
        if (returnFgRight) { 
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z - 0.1f); 
            if (angleZ <= -27) { 
                returnFgRight = false; 
            } 
            Debug.Log("aaa"); 
        }
        if(returnFgLeft) { 
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + 0.1f); 
            if (angleZ >= 27) { 
                returnFgLeft = false; 
            } 
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.tag != "Player") return;
        // ��ɂ���I�u�W�F�N�g������Ώۂɂ������ꍇ
        if (collision.transform.position.y > transform.position.y)
        {
            float angleZ = transform.eulerAngles.z;
            if (angleZ > 180f) angleZ -= 360f;  // -180�`180�ɕϊ�
            float diffX = collision.transform.position.x - transform.position.x;

            if (diffX > 0 && angleZ >= -27)
            {
                Debug.Log($"{collision.gameObject.name} �͉E���ɂ��܂�");
                returnFgRight = true;
                //transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z - 1f);
            }
            else if (diffX < 0 && angleZ <= 27)
            {
                Debug.Log($"{collision.gameObject.name} �͍����ɂ��܂�");
                returnFgLeft = true;
                //transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + 0.01f);
            }
            else
            {
                Debug.Log($"{collision.gameObject.name} �͒����ɂ��܂�");
            }
        }
    }
}
