using UnityEngine;

public class SeeSawReturn : MonoBehaviour
{

    private bool returnFgRight = false;
    private bool returnFgLeft = false;
    private BoxCollider2D box;

    void Awake()
    {
        box = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float angleZ = transform.eulerAngles.z;
        if (angleZ > 180f) angleZ -= 360f;  // -180�`180�ɕϊ�
        if (returnFgRight) {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z - 1f);
            
            if (angleZ <= -45) { 
                returnFgRight = false; 
            } 
        }
        if(returnFgLeft) { 
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + 1f);

            if (angleZ >= 45) { 
                returnFgLeft = false; 
            } 
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "Player") return;
        // ��ɂ���I�u�W�F�N�g������Ώۂɂ������ꍇ
        if (collision.transform.position.y > transform.position.y)
        {
            //float angleZ = transform.eulerAngles.z;
            //if (angleZ > 180f) angleZ -= 360f;  // -180�`180�ɕϊ�
            //float diffX = collision.transform.position.x - transform.position.x;

            //if (diffX > 0 && angleZ >= -27)
            //{
            //    Debug.Log($"{collision.gameObject.name} �͉E���ɂ��܂�");
            //    returnFgRight = true;
            //    //transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z - 1f);
            //}
            //else if (diffX < 0 && angleZ <= 27)
            //{
            //    Debug.Log($"{collision.gameObject.name} �͍����ɂ��܂�");
            //    returnFgLeft = true;
            //    //transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + 0.01f);
            //}
            //else
            //{
            //    Debug.Log($"{collision.gameObject.name} �͒����ɂ��܂�");
            //}
            returnFgRight = false;
            returnFgLeft = false;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "Player") return;
        // ��ɂ���I�u�W�F�N�g������Ώۂɂ������ꍇ
        if (collision.transform.position.y < transform.position.y) return;
        

        Transform player = collision.transform;
        Vector2 playerPos = player.position;

        // BoxCollider2D�̍��E�[�����[���h���W�Ŏ擾
        Vector2 leftEdge, rightEdge;
        GetColliderEdges(box,out leftEdge, out rightEdge);

        float angleZ = transform.eulerAngles.z;
        if (angleZ > 180f) angleZ -= 360f;  // -180�`180�ɕϊ�

        if (playerPos.x > rightEdge.x && angleZ >= -45)
        {
            Debug.Log($"{collision.gameObject.name} �͉E���ɂ��܂�");
            returnFgRight = true;
            //transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z - 1f);
        }
        else if (playerPos.x < leftEdge.x && angleZ <= 45)
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

    // BoxCollider2D �̍��E�[�̃��[���h���W���擾
    private void GetColliderEdges(BoxCollider2D box, out Vector2 leftEdge, out Vector2 rightEdge)
    {
        Vector2 size = box.size;
        Vector2 offset = box.offset;

        // ���[�J�����W�ł�4��
        Vector2[] localCorners =
        {
            offset + new Vector2(-size.x / 2, -size.y / 2),
            offset + new Vector2(-size.x / 2,  size.y / 2),
            offset + new Vector2( size.x / 2,  size.y / 2),
            offset + new Vector2( size.x / 2, -size.y / 2)
        };

        // ���[���h���W�ɕϊ�
        Vector2[] worldCorners = new Vector2[4];
        for (int i = 0; i < 4; i++)
            worldCorners[i] = box.transform.TransformPoint(localCorners[i]);

        // �ł����E�E��T��
        leftEdge = worldCorners[0];
        rightEdge = worldCorners[0];
        foreach (var p in worldCorners)
        {
            if (p.x < leftEdge.x) leftEdge = p;
            if (p.x > rightEdge.x) rightEdge = p;
        }
    }
}
