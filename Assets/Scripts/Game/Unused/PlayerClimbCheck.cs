using UnityEngine;

public class PlayerClimbCheck : MonoBehaviour
{
    //プレイヤーが登るのに邪魔な障害物があるかの判定用スクリプト
    PlayerStateMachine state_ma;
    private int contactcount = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        state_ma = GetComponentInParent<PlayerStateMachine>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (((1 << collider.gameObject.layer) & state_ma.groundlayers) != 0)//当たったオブジェクトの数を記録し、障害物判定をオンにする
        {
            ++contactcount;
            state_ma.SetNgFg(true);
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (((1 << collider.gameObject.layer) & state_ma.groundlayers) != 0)//当たったオブジェクトの数を減らし、0になれば障害物判定をオフにする
        {
            --contactcount;

            if (contactcount <= 0) {
                contactcount = 0;
                state_ma.SetNgFg(false);
            }
        }
    }
}
