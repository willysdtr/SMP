using UnityEngine;

public class PlayerJumpCheck : MonoBehaviour
{
    PlayerStateMachine state_ma;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        state_ma = GetComponentInParent<PlayerStateMachine>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & state_ma.groundlayers) != 0)
        {
            state_ma.SetJumpFg(true);
        }
    }
}
