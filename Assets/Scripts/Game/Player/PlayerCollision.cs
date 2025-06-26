using UnityEngine;
using UnityEngine.Rendering;

public class PlayerCollision : MonoBehaviour
{

    private PlayerStateMachine state_ma;

    private bool moveFg;
    private bool jumpFg;
    private bool isground;
    private bool celling_hit;
    private bool ngFg;
    private bool climbFg;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        state_ma = GetComponent<PlayerStateMachine>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        //PlayerHitProcess();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wrinkles"))
        {
            //state_ma.SetDownFg(true);
        }
        if (((1 << collision.gameObject.layer) & state_ma.groundlayers) != 0)
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                Vector2 normal = contact.normal;

                if (normal == Vector2.left || normal == Vector2.right)
                {
                    state_ma.SetMoveFg(false);
                }

                if (normal == Vector2.up)
                {
                    state_ma.SetIsGround(true);
                    if (collision.gameObject.CompareTag("Spring"))
                    {
                        state_ma.SetGimJumpFg(true);
                    }
                }

            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & state_ma.groundlayers) != 0)
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                Vector2 normal = contact.normal;

                if (normal == Vector2.left || normal == Vector2.right)
                {
                    state_ma.SetMoveFg(true);
                }

                if (normal == Vector2.up)
                {
                    state_ma.SetIsGround(false);
                }
            }
        }
    }

    public void SetClimbFg(bool fg)
    {
        climbFg = fg;  
    }

    void SetJumpFg(bool fg)
    {
        jumpFg = fg;
    }

    void SetCelingHit(bool fg)
    {
        celling_hit = fg;
    }
}
