using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerCollision : MonoBehaviour
{
    //�v���C���[�{�̕����̓����蔻��Ǘ��X�N���v�g

    private PlayerStateMachine state_ma;

    private HashSet<GameObject> ground_obj = new HashSet<GameObject>();
    private HashSet<GameObject> wall_obj = new HashSet<GameObject>();

    private Rigidbody2D rb;

    [SerializeField] private Vector2 checkSize = new Vector2(0.5f, 1.0f);
    [SerializeField] private Vector2 checkOffset = new Vector2(0f, 0f);
    [SerializeField] private LayerMask climbLayer;

    public Vector2 hitobj_pos { get; private set; } = new Vector2(0.0f,0.0f);
    private bool ishit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        state_ma = GetComponent<PlayerStateMachine>();
    }

    void FixedUpdate()
    {
        //OverlapBox�̍쐬�AClimb�����Ɏg�p
        Vector2 center = (Vector2)transform.position + checkOffset;

        Collider2D hit = Physics2D.OverlapBox(center, checkSize, 0f, climbLayer);
        
        ishit = hit;
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
        
        if (((1 << collision.gameObject.layer) & state_ma.groundlayers) != 0)//�C���X�y�N�^�[�Őݒ肵��Layer�Ƃ̂ݔ�������
        {
            if(collision.gameObject.tag == "String")
            {
                bool isVertical = Mathf.Abs(collision.transform.up.y) > 0.9f;
                if (isVertical)
                {
                    // �c��String�Ȃ�Trigger�ɐ؂�ւ�
                    GetComponent<BoxCollider2D>().isTrigger = true;
                }
            }


            if (collision.gameObject.tag == "Spring")//�΂˂ɓ����������̏���
            {
                //�\�����X�N���v�g������΁A���������s
                JumpLine pad = collision.gameObject.GetComponent<JumpLine>();
                if (pad != null)
                {
                    state_ma.SetInitVelocity(pad.GetInitialVelocity());
                    transform.position = new Vector3(collision.transform.position.x, transform.position.y, 0);
                    state_ma.SetJumpFg(true);
                    state_ma.SetGimJumpFg(true);
                    state_ma.SetMoveFg(false);
                }
            }
            else
            {
                foreach (ContactPoint2D contact in collision.contacts)
                {

                    // ������ɐڐG�����ꍇ�̂݃J�E���g
                    if (contact.normal == Vector2.up)
                    {
                        state_ma.SetIsGround(true);
                        ground_obj.Add(collision.gameObject);

                    }
                    // �������ɐڐG�����ꍇ�̂݃J�E���g
                    if (contact.normal == Vector2.left || contact.normal == Vector2.right)
                    {
                        wall_obj.Add(collision.gameObject);
                        state_ma.SetMoveFg(false);
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
           state_ma.SetIsGround(false);
        }

        if (wall_obj.Count == 0)
        {//�ǔ��肵���I�u�W�F�N�g�����ׂĂȂ��Ȃ�΁A�ړ��\�ɂ���
            state_ma.SetMoveFg(true);
        }

    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "String")
        {
            //���ɓ����������̏���
            state_ma.SetMoveFg(false);
            state_ma.SetClimbFg(true);
            state_ma.SetHitObjPos(collider.transform.position);
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (!ishit)
        {//OverlapBox���d�Ȃ��ĂȂ��Ƃ��Ɏ��s(��쓮���邽��)
            if (collider.gameObject.tag == "String")
            {//�����痣�ꂽ���̏���
                state_ma.SetClimbFg(false);
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.linearVelocity = Vector2.zero;
                GetComponent<BoxCollider2D>().isTrigger = false;
            }
        }
        
    }

}
