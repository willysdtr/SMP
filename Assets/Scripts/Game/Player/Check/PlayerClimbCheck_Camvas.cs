using UnityEngine;

public class PlayerClimbCheck_Canvas : MonoBehaviour
{
    //�v���C���[���o��̂Ɏז��ȏ�Q�������邩�̔���p�X�N���v�g
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
            if(collider.gameObject.tag == "Cutter" || collider.gameObject.tag == "SeeSaw" || collider.gameObject.tag == "Goal") { return; }//�J�b�^�[�ƃV�[�\�[�̓X���[
            ++contactcount;
            m_cont.state.IS_CLIMB_NG = true;

        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (((1 << collider.gameObject.layer) & m_cont.groundlayers) != 0)//���������I�u�W�F�N�g�̐������炵�A0�ɂȂ�Ώ�Q��������I�t�ɂ���
        {
            --contactcount;

            if (contactcount <= 0) {
                contactcount = 0;
                m_cont.state.IS_CLIMB_NG = false;
            }
        }
    }
}
