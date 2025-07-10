using UnityEngine;

public class PlayerClimbCheck : MonoBehaviour
{
    PlayerStateMachine state_ma;
    private int contactcount = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        state_ma = GetComponentInParent<PlayerStateMachine>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & state_ma.groundlayers) != 0)
        {
            ++contactcount;
            foreach (ContactPoint2D contact in collision.contacts)
            {
                Vector2 normal = contact.normal;

                if (normal == Vector2.down)
                {
                    state_ma.SetNgFg(true);
                }

            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & state_ma.groundlayers) != 0)
        {
            --contactcount;

            if (contactcount <= 0) {
                contactcount = 0;
                state_ma.SetNgFg(false);
            }
        }
    }
}
