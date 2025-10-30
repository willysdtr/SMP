using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class StringAnimation_Canvas : MonoBehaviour
{
    public RectTransform canvasTransform;
    public  List<Transform> ImageList=new List<Transform>();

    public int index = 0;//���g�����Ԗڂ�

    // �eCanvas��o�^����֐��i��������ɌĂяo���j
    public void SetCanvas(RectTransform canvas)
    {
        //Debug.Log("SetCanvas called with: " + canvas?.name);
        canvasTransform = canvas;
    }

    public void EndAnimarion()
    {
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
            ImageList.Add(child);
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
    public void DeleteImage(int i)
    {
        if (ImageList == null || ImageList.Count == 0)
        {
            Debug.LogWarning($"[{name}] ImageList����̂��ߍ폜�ł��܂���B");
            return;
        }

        if (i < 0 || i >= ImageList.Count)
        {
            Debug.LogWarning($"[{name}] �s���ȃC���f�b�N�X {i} ���w�肳��܂����BImageList.Count={ImageList.Count}");
            return;
        }

        if (ImageList[i] != null)
        {
            ImageList[i].gameObject.SetActive(false);
        }
    }
}
