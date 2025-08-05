using UnityEngine;

public class StringAnimation_Canvas : MonoBehaviour
{
    private RectTransform canvasTransform;

    // �eCanvas��o�^����֐��i��������ɌĂяo���j
    public void SetCanvas(RectTransform canvas)
    {
        Debug.Log("SetCanvas called with: " + canvas?.name);
        canvasTransform = canvas;
    }

    public void EndAnimarion()
    {
        Debug.Log("�A�j���[�V�����I�����܂���");
        foreach (Transform child in transform)
        {
            var rect = child as RectTransform;
            if (rect == null) continue;

            Vector3 worldPos = rect.position;
            Quaternion worldRot = rect.rotation;
            Vector3 worldScale = rect.lossyScale;

            // Canvas�Ɉړ�
            rect.SetParent(canvasTransform, true);

            rect.rotation = worldRot;
            SetWorldScale(rect, worldScale);
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
            target.localScale = new Vector3(
                desiredWorldScale.x / parentScale.x,
                desiredWorldScale.y / parentScale.y,
                desiredWorldScale.z / parentScale.z
            );
        }
    }
}
