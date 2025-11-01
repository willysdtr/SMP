using UnityEngine;

public class PlayerClimbCheck_Canvas : MonoBehaviour
{
    //プレイヤーが登るのに邪魔な障害物があるかの判定用スクリプト
    PlayerController m_cont;
    private int contactcount = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_cont = GetComponentInParent<PlayerController>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (((1 << collider.gameObject.layer) & m_cont.groundlayers) != 0)//当たったオブジェクトの数を記録し、障害物判定をオンにする
        {
            if(collider.gameObject.tag == "Cutter" || collider.gameObject.tag == "SeeSaw" || collider.gameObject.tag == "Goal") { return; }//カッターとシーソーはスルー
            ++contactcount;
            m_cont.state.IS_CLIMB_NG = true;

        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (((1 << collider.gameObject.layer) & m_cont.groundlayers) != 0)//当たったオブジェクトの数を減らし、0になれば障害物判定をオフにする
        {
            --contactcount;

            if (contactcount <= 0) {
                contactcount = 0;
                m_cont.state.IS_CLIMB_NG = false;
            }
        }
    }
}
