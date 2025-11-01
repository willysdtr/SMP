using UnityEngine;

public class PlayerCeilingCheck_Canvas : MonoBehaviour
{
    //プレイヤーの頭上に障害物があるかの判定用スクリプト
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
            if(collider.gameObject.tag == "PinCuttion" || collider.gameObject.tag == "Cutter" || collider.gameObject.tag == "Start" || collider.gameObject.tag == "SeeSaw" || collider.gameObject.tag == "Goal") { return; }//針山とカッターとスタートとシーソーはスルー
            if(collider.gameObject.tag == "String" && collider.transform.rotation.z != 0f) { return; }//縦糸には頭をぶつけない
            ++contactcount;
            m_cont.state.IS_CEILING_HIT= true;
            m_cont.state.IS_JUMP = false;

        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (((1 << collider.gameObject.layer) & m_cont.groundlayers) != 0)//当たったオブジェクトの数を減らし、0になれば障害物判定をオフにする
        {
            --contactcount;

            if (contactcount <= 0) {
                contactcount = 0;
                m_cont.state.IS_CEILING_HIT = false;
            }
        }
    }
}
