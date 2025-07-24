using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class StringAnimation : MonoBehaviour
{
    public void EndAnimarion()
    {
        Debug.Log("�A�j���[�V�����I�����܂���");

        // �q�I�u�W�F�N�g��e����O���ăA�N�e�B�u�ɂ���
        foreach (Transform child in transform)
        {

            // ��Ƀ��[���h���W��ۑ����Ă���
            Vector3 worldPos = child.position;
            Quaternion worldRot = child.rotation;
            Vector3 worldScale = child.lossyScale;//���[���h��Ԃ̐�΃X�P�[��

            // �e�q�֌W������
            child.SetParent(null);

            // ���[���h�ʒu�Ɖ�]���Đݒ�iTransform�̃Y���h�~�j
            child.position = worldPos;
            child.rotation = worldRot;
            // �X�P�[�����Đݒ�i����j
            SetWorldScale(child, worldScale); // 
           // this.transform.localScale.x
            // �K�v�Ȃ�A�N�e�B�u��
            child.gameObject.SetActive(true);
        }
    }
    void SetWorldScale(Transform target, Vector3 desiredWorldScale)
    {
        Transform parent = target.parent;
        if (parent == null)
        {
            target.localScale = desiredWorldScale;
        }
        else
        {
            Vector3 parentScale = parent.lossyScale;
            target.localScale = new Vector3//�e�̃X�P�[�����l�����ă��[�J���X�P�[�����v�Z
                (
                desiredWorldScale.x / parentScale.x,
                desiredWorldScale.y / parentScale.y,
                desiredWorldScale.z / parentScale.z
            );
            //desiredWorldScale = localScale �~ parentScale��localScale = desiredWorldScale �� parentScale
        }
    }
}
