using UnityEngine;

public class ClimbCheck : MonoBehaviour
{
    private PlayerCollision player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponentInParent<PlayerCollision>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("String"))
        {
            player.SetClimbFg(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("String"))
        {
            player.SetClimbFg(false);
        }
    }
}
