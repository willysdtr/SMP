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

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (((1 << collider.gameObject.layer) & state_ma.groundlayers) != 0)
        {
            ++contactcount;
            state_ma.SetNgFg(true);

        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (((1 << collider.gameObject.layer) & state_ma.groundlayers) != 0)
        {
            --contactcount;

            if (contactcount <= 0) {
                contactcount = 0;
                state_ma.SetNgFg(false);
            }
        }
    }
}
