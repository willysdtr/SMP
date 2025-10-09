using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{

    private PlayerController cont;  // Controller�o�R��State�ɃA�N�Z�X

    private HashSet<GameObject> ground_obj = new HashSet<GameObject>();
    private HashSet<GameObject> wall_obj = new HashSet<GameObject>();

    [SerializeField] private Vector2 checkSize = new Vector2(0.5f, 1.0f);
    [SerializeField] private Vector2 checkOffset = new Vector2(0f, 0f);
    [SerializeField] private LayerMask climbLayer;

    private Rigidbody2D rb;
    private RectTransform rect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cont = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        rect = GetComponent<RectTransform>();
        // ����T�C�Y��RectTransform�̃T�C�Y�ɍ��킹��
        checkSize = new Vector2(checkSize.x * rect.sizeDelta.x, checkSize.y * rect.sizeDelta.y);
    }

    // Update is called once per frame
    void Update()
    {
        //OverlapBox�̍쐬�AClimb�����Ɏg�p
        Vector2 center = (Vector2)transform.position + checkOffset;

        Collider2D hit = Physics2D.OverlapBox(center, checkSize, 0f, climbLayer);

        cont.ishit = hit;
    }

    private void OnDrawGizmos()
    {
        //OverlapBox�̕`��
        Gizmos.color = Color.red;
        Vector2 center = (Vector2)transform.position + checkOffset;
        Gizmos.DrawWireCube(center, checkSize);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {

        if (((1 << collision.gameObject.layer) & cont.groundlayers) != 0)//�C���X�y�N�^�[�Őݒ肵��Layer�Ƃ̂ݔ�������
        {

            if (collision.gameObject.tag == "Kaesi")//�Ԃ��D���ɓ����������̏���
            {
                cont.PlayerReturn(collision.transform.rotation.y);//�v���C���[�̌�����ς���

            }

            if (collision.gameObject.tag == "Goal")
            {
                cont.state.currentstate = PlayerState.State.GOAL;// �S�[����ԂɕύX
                cont.Goal(collision.transform.position);
            }
            else
            {
                foreach (ContactPoint2D contact in collision.contacts)
                {

                    // ������ɐڐG�����ꍇ�̂݃J�E���g
                    if (Vector2.Angle(contact.normal, Vector2.up) < 20f)
                    {
                        if (collision.gameObject.tag == "Spring")//�΂˂ɓ����������̏���
                        {
                            transform.position = new Vector2(collision.transform.position.x, transform.position.y);
                            cont.state.IS_JUMP = true;
                            cont.state.IS_MOVE = false;
                            cont.state.IS_GROUND = false;

                        }
                        else // �ʏ�̒n�ʂɓ����������̏���
                        {
                            cont.state.IS_GROUND = true;
                            cont.state.IS_MOVE = true;
                            ground_obj.Add(collision.gameObject);
                        }

                    }
                    // �������ɐڐG�����ꍇ�̂݃J�E���g
                    if (contact.normal == Vector2.left || contact.normal == Vector2.right)
                    {

                        if (collision.gameObject.tag == "String" && cont.state.IS_CLIMB_NG == false)
                        {

                            bool isVertical = collision.transform.rotation.z != 0;
                            if (isVertical)
                            {
                                // �c�̎��Ȃ�Trigger�ɐ؂�ւ�
                                GetComponent<BoxCollider2D>().isTrigger = true;
                                //���ɓ����������̏���
                                cont.state.IS_MOVE = false;
                                cont.state.IS_CLIMB = true;
                                cont.hitobj_pos = collision.transform.position;
                                rb.linearVelocity = Vector2.zero;
                                rb.bodyType = RigidbodyType2D.Kinematic;
                            }
                        }
                        else
                        {
                            wall_obj.Add(collision.gameObject);
                            cont.state.IS_MOVE = false;

                        }
                    }
                }
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        ground_obj.Remove(collision.gameObject);//�n�ʔ��肵���I�u�W�F�N�g���폜
        wall_obj.Remove(collision.gameObject);//�ǔ��肵���I�u�W�F�N�g���폜
        if (ground_obj.Count == 0)
        {//�n�ʔ��肵���I�u�W�F�N�g�����ׂĂȂ��Ȃ�΁A�n�ʂ��痣�ꂽ��Ԃɂ���
            cont.state.IS_GROUND = false;
        }

        if (wall_obj.Count == 0)
        {//�ǔ��肵���I�u�W�F�N�g�����ׂĂȂ��Ȃ�΁A�ړ��\�ɂ���
            cont.state.IS_MOVE = true;
        }

    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "String")
        {
            //���ɓ����������̏���
            cont.state.IS_MOVE = false;
            cont.state.IS_CLIMB = true;
            cont.hitobj_pos = collider.transform.position;
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (!cont.ishit)
        {//OverlapBox���d�Ȃ��ĂȂ��Ƃ��Ɏ��s(��쓮���邽��)
            if (collider.gameObject.tag == "String")
            {//�����痣�ꂽ���̏���
                cont.state.IS_MOVE = true;
                cont.state.IS_CLIMB = false;
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.linearVelocity = Vector2.zero;
                GetComponent<BoxCollider2D>().isTrigger = false;//Trigger����
            }
        }

    }
}
