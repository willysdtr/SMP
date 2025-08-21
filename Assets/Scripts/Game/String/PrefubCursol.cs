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
        //インスペクター上でセットしたスケールをセット
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
            if (!isMoving) return; // 移動モードでない場合は何もしない
            float value = ctx.ReadValue<float>();
            if (value == 1)//左
            {
                transform.position -= (Vector3)m_Offset_X;
            }
            else if (value == 2)//上
            {
                transform.position -= (Vector3)m_Offset_Y;
            }
            else if (value == 3)//右
            {
                transform.position += (Vector3)m_Offset_X;
            }
            else if (value == 4)//下
            {
                transform.position += (Vector3)m_Offset_Y;
            }
        };
       inputActions.PrefubCursol.@catch.performed += ctx =>
        {
            if (!isMoving && isCatch) return; // 移動モードでない場合は何もしない
            if (prefabList.Count > 0 && currentPrefab == null)
           {
                isCatch=true; // カーソルがPrefabをキャッチしている状態にする
                currentPrefab = Instantiate(prefabList[0], cursor.position, Quaternion.identity); // Listの先頭をカーソルに追従させる
                currentPrefab.transform.SetParent(cursor); // カーソルに追従させる
           }
        };
        inputActions.PrefubCursol.release.performed += ctx =>
        {
            if (!isMoving&& isCatch) return; // 移動モードでない場合は何もしない
            if (currentPrefab != null)
            {
                // カーソルの位置にPrefabを配置
                currentPrefab.transform.SetParent(null); // カーソルから切り離す
                currentPrefab = null;

                // 先頭をリストから削除
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
