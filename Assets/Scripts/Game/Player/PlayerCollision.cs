using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class PlayerCollision : MonoBehaviour
{

    private PlayerController cont;  // Controller�o�R��State�ɃA�N�Z�X

    private HashSet<GameObject> ground_obj = new HashSet<GameObject>();
    private HashSet<GameObject> wall_obj = new HashSet<GameObject>();

    [SerializeField] private Vector2 checkSize = new Vector2(0.5f, 1.0f);
    [SerializeField] private Vector2 checkOffset = new Vector2(0f, 0f);

    [SerializeField] private StringManager_Canvas stringManager; // StringManager_Canvas�̎Q�ƁA�������������Ŏg�p


    private Rigidbody2D rb;
    private RectTransform rect;

    private BoxCollider2D m_collider;

    private bool wallhit = false;//壁に当たっているかの判定(ジャンプ中の跳ね返り処理に使用)

    private float setdiff = 0.0f;//補正した値を保存

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cont = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        rect = GetComponent<RectTransform>();
        m_collider = GetComponent<BoxCollider2D>(); // �e�ɂ���Collider�̂ݎ擾
        // ����T�C�Y��RectTransform�̃T�C�Y�ɍ��킹��
        checkSize = new Vector2(checkSize.x * rect.sizeDelta.x, checkSize.y * rect.sizeDelta.y);
    }

    // Update is called once per frame
    void Update()
    {
        //OverlapBox�̍쐬�AClimb�����Ɏg�p
        Vector2 center = (Vector2)transform.position + checkOffset;

        Collider2D hit = Physics2D.OverlapBox(center, checkSize, 0f, cont.climblayers);

        cont.ishit = hit;

        wallhit = false;
        setdiff = 0.0f;
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

        int layerID = collision.gameObject.layer; //���C���[ID���擾
        string layerName = LayerMask.LayerToName(layerID); // ���O�ɕϊ�

        if (layerName == "String" || layerName == "Gimmick")//�C���X�y�N�^�[�Őݒ肵��Layer�Ƃ̂ݔ������� 
                                                            //(((1 << collision.gameObject.layer) & cont.groundlayers) != 0) //�ȑO��Layer����A������ɂ����̂ŃR�����g�A�E�g
        {

            if (collision.gameObject.tag == "Kaesi")//�Ԃ��D���ɓ����������̏���
            {
                cont.PlayerReturn(collision.transform.rotation.y);//�v���C���[�̌�����ς���

            }

            if (collision.gameObject.tag == "Cutter")
            {
                stringManager.CutNum += 1;//�J�b�g���𑝂₷
                collision.gameObject.SetActive(false);//�J�b�^�[������
                cont.cutCt++;//糸を切れる回数を増やす
                return; //以降の処理を行わない(壁判定に引っかかるため)
            }
            if (collision.gameObject.tag == "PinCuttion")
            {
                cont.state.IS_DOWN = true;//死亡フラグON
                return; //以降の処理を行わない
            }


            if (collision.gameObject.tag == "Goal")
            { 
                if(cont.state.currentstate == PlayerState.State.GOAL) { return; } // ���łɃS�[�����Ă����牽�����Ȃ�
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
                            if (cont.state.IS_JUMP)
                            {
                                cont.state.currentstate = PlayerState.State.STOP;
                            }
                            cont.state.IS_JUMP = true;
                            cont.state.IS_MOVE = false;
                            cont.state.IS_GROUND = false;

                        }
                        else // �ʏ�̒n�ʂɓ����������̏���
                        {
                            cont.state.IS_GROUND = true;
                            cont.state.IS_MOVE = true;
                            cont.state.IS_JUMP = false;
                            ground_obj.Add(collision.gameObject);
                        }

                    }
                    // �������ɐڐG�����ꍇ�̂݃J�E���g
                    if (contact.normal == Vector2.left || contact.normal == Vector2.right)
                    {
                        if (layerName == "String" && !cont.state.IS_JUMP)// ����Layer�Ȃ�
                                                      //(((1 << collision.gameObject.layer) & cont.climblayers) != 0) //�ȑO��Layer����A������ɂ����̂ŃR�����g�A�E�g
                        {


                                if (cont.cutCt > 0) //糸を切れる回数があるなら
                            { //糸を切る処理
                                int index = collision.gameObject.GetComponent<StringAnimation_Canvas>().index;
                                stringManager.CutString(index);
                                cont.cutCt--;
                                return; // 糸を切るだけで他の処理はしない
                            }

                            bool isVertical = collision.transform.rotation.z != 0;

                            if (isVertical && !(cont.state.IS_CLIMB_NG || cont.state.IS_CEILING_HIT))
                            {
                                // �c�̎��Ȃ�Trigger�ɐ؂�ւ�
                                GetComponent<BoxCollider2D>().isTrigger = true;
                                //���ɓ����������̏���
                                cont.state.IS_MOVE = false;
                                cont.state.IS_CLIMB = true;
                                cont.hitobj_pos = collision.transform.position;
                                rb.linearVelocity = Vector2.zero;
                                rb.bodyType = RigidbodyType2D.Kinematic;
                                ground_obj.Clear();//�n�ʔ��肵���I�u�W�F�N�g��S�폜
                                return;
                            }
                        }


                        // �i���␳���邩�̔���
                        Bounds myBounds = m_collider.bounds;
                        Bounds targetBounds = collision.gameObject.GetComponent<BoxCollider2D>().bounds;
                        float playerFootY = myBounds.min.y;
                        float playerHeight = myBounds.size.y;
                        float thresholdY = playerFootY + playerHeight / 4;
                        float topY = targetBounds.max.y;

                        if (topY < thresholdY && !wallhit) // �v���C���[�̑�������̍���1/4�ȓ��̒i���Ȃ�␳
                        {
                            float diff = topY - playerFootY;

                            transform.position += new Vector3(0f, diff, 0f);
                            setdiff = diff;
                            cont.state.IS_GROUND = true;
                            cont.state.IS_MOVE = true;
                            cont.state.IS_JUMP = false;
                            ground_obj.Add(collision.gameObject);

                            return; // �i���␳���s������ǂƂ��ăJ�E���g���Ȃ�
                        }
                        else
                        {
                            wallhit = true;
                            if(setdiff != 0 && cont.state.currentstate == PlayerState.State.JUMP)
                            {
                                transform.position -= new Vector3(0f, setdiff, 0f);
                                cont.state.IS_GROUND = false;
                                cont.state.IS_MOVE = false;
                                cont.state.IS_JUMP = true;
                            }

                            if (cont.state.IS_JUMP)
                            {
                                cont.PlayerJumpReturn();
                                return;

                            }
                        }

                        if ((cont.state.IS_CLIMB_NG || cont.state.IS_CEILING_HIT) && !cont.state.IS_JUMP) //���]���� ���ȊO�̕ǂł����]����
                        {
                            
                            //�v���C���[�̌�����ς���
                            if (contact.normal == Vector2.left)
                            {
                                cont.PlayerReturn(0);
                                return;

                            }
                            else if (contact.normal == Vector2.right)
                            {
                                cont.PlayerReturn(-180);
                                return;
                            }
                        }

                        // �ǂɓ����������̏���

                        wall_obj.Add(collision.gameObject);
                        cont.state.IS_MOVE = false;


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
        // ������Collider�ȊO�͖���
        if (collider != m_collider) return;

        int layerID = collider.gameObject.layer; //���C���[ID���擾
        string layerName = LayerMask.LayerToName(layerID); // ���O�ɕϊ�

        if (layerName == "String")
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

        int layerID = collider.gameObject.layer; //���C���[ID���擾
        string layerName = LayerMask.LayerToName(layerID); // ���O�ɕϊ�

        if (!cont.ishit)
        {//OverlapBox���d�Ȃ��ĂȂ��Ƃ��Ɏ��s(��쓮���邽��)
            if (layerName == "String")
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
