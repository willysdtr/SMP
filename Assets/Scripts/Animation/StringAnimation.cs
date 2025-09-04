using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class StringAnimation : MonoBehaviour
{
    public StringManager m_StringManager;
    public void EndAnimarion()
    {
        Debug.Log("アニメーション終了しました");

        // 子オブジェクトを親から外してアクティブにする
        foreach (Transform child in transform)
        {
            Debug.Log(child, m_StringManager);
            // 先にワールド座標を保存しておく
            Vector3 worldPos = child.position;
            Quaternion worldRot = child.rotation;
            Vector3 worldScale = child.lossyScale;//ワールド空間の絶対スケール

            // 親子関係を解除
            child.SetParent(null);

            // ワールド位置と回転を再設定（Transformのズレ防止）
            child.position = worldPos;
            child.rotation = worldRot;
            // スケールを再設定（特殊）
            SetWorldScale(child, worldScale); // 
           // this.transform.localScale.x
            // 必要ならアクティブ化
            child.gameObject.SetActive(true);
            if(m_StringManager.EndSiting==true)
            {
                m_StringManager.BallStopper();
                Debug.Log("EndSitingがtrueになりました");
                m_StringManager.EndSiting = true;
            }
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
            target.localScale = new Vector3//親のスケールを考慮してローカルスケールを計算
                (
                desiredWorldScale.x / parentScale.x,
                desiredWorldScale.y / parentScale.y,
                desiredWorldScale.z / parentScale.z
            );
            //desiredWorldScale = localScale × parentScale→localScale = desiredWorldScale ÷ parentScale
        }
    }
}
