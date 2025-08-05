using UnityEngine;

public class StringAnimation_Canvas : MonoBehaviour
{
    private RectTransform canvasTransform;

    // 親Canvasを登録する関数（生成直後に呼び出す）
    public void SetCanvas(RectTransform canvas)
    {
        Debug.Log("SetCanvas called with: " + canvas?.name);
        canvasTransform = canvas;
    }

    public void EndAnimarion()
    {
        Debug.Log("アニメーション終了しました");
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
