using UnityEngine;

public class PlayerJumpCheck : MonoBehaviour
{
    PlayerStateMachine state_ma;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        state_ma = GetComponentInParent<PlayerStateMachine>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (((1 << collider.gameObject.layer) & state_ma.groundlayers) != 0)
        {
            state_ma.SetJumpFg(true);
            state_ma.SetMoveFg(false);
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (((1 << collider.gameObject.layer) & state_ma.groundlayers) != 0)
        {
            state_ma.SetJumpFg(false);
        }
    }
}
