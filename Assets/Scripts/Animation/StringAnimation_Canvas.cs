using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class StringAnimation_Canvas : MonoBehaviour
{
    public RectTransform canvasTransform;
    public  List<Transform> ImageList=new List<Transform>();

    public int index = 0;//自身が何番目か

    // 親Canvasを登録する関数（生成直後に呼び出す）
    public void SetCanvas(RectTransform canvas)
    {
        Debug.Log("SetCanvas called with: " + canvas?.name);
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

            // Canvasに移動
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
        ImageList[i].gameObject.SetActive(false);
    }
}
