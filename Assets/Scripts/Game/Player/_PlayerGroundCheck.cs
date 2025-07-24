using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    [Header("�n�ʔ���p�̎q�I�u�W�F�N�g")]
    public Transform groundCheck;   // ��������̎q�I�u�W�F�N�g

    public LayerMask groundLayers;

    public bool IsGrounded { get; private set; }

    private Vector2 checkSize;

    void Start()
    {
        if (groundCheck == null)
        {
            Debug.LogError("groundCheck���Z�b�g����Ă��܂���I");
            return;
        }

        // �q�I�u�W�F�N�g��BoxCollider2D���擾
        BoxCollider2D box = groundCheck.GetComponent<BoxCollider2D>();

        if (box != null)
        {
            // BoxCollider2D�̃T�C�Y�𔻒�T�C�Y�ɐݒ�
            checkSize = box.size;
        }
        else
        {
            Debug.LogWarning("groundCheck��BoxCollider2D������܂���B�f�t�H���g�T�C�Y���g���܂��B");
            checkSize = new Vector2(0.5f, 0.1f);
        }
    }

    void FixedUpdate()
    {
        if (groundCheck == null) return;

        IsGrounded = Physics2D.OverlapBox(groundCheck.position, checkSize, 0f, groundLayers);
    }
}