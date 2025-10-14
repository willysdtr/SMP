using Unity.VisualScripting;
using UnityEngine;

public class PLHit : MonoBehaviour
{
    PlayerJump jump;
    PLMove move;

    bool isJump = false;
    bool direction = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        jump = GetComponent<PlayerJump>();
        move = GetComponent<PLMove>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isJump)
        {
            isJump = !jump.Jump(transform.position.x, transform.position.y, direction);
        }
        else
        {
            move.Move(transform.position.x, transform.position.y, direction);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "Spring")//ÇŒÇÀÇ…ìñÇΩÇ¡ÇΩéûÇÃèàóù
        {
            transform.position = new Vector2(collision.gameObject.transform.position.x,transform.position.y);
            isJump = true;
            Debug.Log("Jump");
        }
            
    }
}
