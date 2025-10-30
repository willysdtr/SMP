using UnityEngine;

public class PlayerCeilingCheck_Canvas : MonoBehaviour
{
    //�v���C���[�̓���ɏ�Q�������邩�̔���p�X�N���v�g
    PlayerController m_cont;
    private int contactcount = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_cont = GetComponentInParent<PlayerController>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (((1 << collider.gameObject.layer) & m_cont.groundlayers) != 0)//���������I�u�W�F�N�g�̐����L�^���A��Q��������I���ɂ���
        {
            if(collider.gameObject.tag == "PinCuttion" || collider.gameObject.tag == "Cutter") { return; }//�j�R�ƃJ�b�^�[�̓X���[
            ++contactcount;
            m_cont.state.IS_CEILING_HIT= true;
            m_cont.state.IS_JUMP = false;

        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (((1 << collider.gameObject.layer) & m_cont.groundlayers) != 0)//���������I�u�W�F�N�g�̐������炵�A0�ɂȂ�Ώ�Q��������I�t�ɂ���
        {
            --contactcount;

            if (contactcount <= 0) {
                contactcount = 0;
                m_cont.state.IS_CEILING_HIT = false;
            }
        }
    }
}
