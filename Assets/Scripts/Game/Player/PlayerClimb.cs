using UnityEngine;

public class PlayerClimb : MonoBehaviour
{
    private Rigidbody2D rb;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Climb(float speed)
    {
        rb.position += new Vector2(0, speed * Time.deltaTime);
    }
}
