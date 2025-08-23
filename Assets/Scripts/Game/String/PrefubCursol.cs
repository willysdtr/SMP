using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PrefubCursol : MonoBehaviour
{
    public Vector3 m_StrinngScale = new Vector3(1.0f, 1.0f, 0.0f);
    private Vector2 m_Offset_X = new Vector2(1.0f, 0.0f);
    private Vector2 m_Offset_Y = new Vector2(0.0f, -1.0f);
    [SerializeField] private List<GameObject> prefabList;
    [SerializeField] private Transform cursor;           
    private GameObject currentPrefab;                    
    private InputSystem_Actions inputActions;
    private bool isMoving = false;
    public bool IsMoving => isMoving;
    private bool isCatch = false;
    void Start()
    {
        //�C���X�y�N�^�[��ŃZ�b�g�����X�P�[�����Z�b�g
        m_Offset_X = new Vector2(m_StrinngScale.x, 0.0f);
        m_Offset_Y = new Vector2(0.0f, -m_StrinngScale.y);
    }
    void Awake()
    {
        inputActions = new InputSystem_Actions();
        inputActions.PrefubCursol.changemode.performed += ctx =>
        {
            isMoving = !isMoving;
        };
        inputActions.PrefubCursol.move.performed += ctx =>
        {
            if (!isMoving) return; // �ړ����[�h�łȂ��ꍇ�͉������Ȃ�
            float value = ctx.ReadValue<float>();
            if (value == 1)//��
            {
                transform.position -= (Vector3)m_Offset_X;
            }
            else if (value == 2)//��
            {
                transform.position -= (Vector3)m_Offset_Y;
            }
            else if (value == 3)//�E
            {
                transform.position += (Vector3)m_Offset_X;
            }
            else if (value == 4)//��
            {
                transform.position += (Vector3)m_Offset_Y;
            }
        };
       inputActions.PrefubCursol.@catch.performed += ctx =>
        {
            if (!isMoving && isCatch) return; // �ړ����[�h�łȂ��ꍇ�͉������Ȃ�
            if (prefabList.Count > 0 && currentPrefab == null)
           {
                isCatch=true; // �J�[�\����Prefab���L���b�`���Ă����Ԃɂ���
                currentPrefab = Instantiate(prefabList[0], cursor.position, Quaternion.identity); // List�̐擪���J�[�\���ɒǏ]������
                currentPrefab.transform.SetParent(cursor); // �J�[�\���ɒǏ]������
           }
        };
        inputActions.PrefubCursol.release.performed += ctx =>
        {
            if (!isMoving&& isCatch) return; // �ړ����[�h�łȂ��ꍇ�͉������Ȃ�
            if (currentPrefab != null)
            {
                // �J�[�\���̈ʒu��Prefab��z�u
                currentPrefab.transform.SetParent(null); // �J�[�\������؂藣��
                currentPrefab = null;

                // �擪�����X�g����폜
                prefabList.RemoveAt(0);
            }
        };

    }
    void OnEnable()
    {
        inputActions.PrefubCursol.Enable();
    }
    void OnDisable()
    {
        inputActions.PrefubCursol.Disable();
    }
}
