using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StringManager : MonoBehaviour
{
    [SerializeField] private GameObject StringPrefub;
    [SerializeField] Vector2 offset;
    private List<GameObject> Strings = new List<GameObject>();
    [SerializeField] private int StringNum;
    private InputSystem_Actions inputActions;
    private float m_PauseDirection;//音量調整の入力値
    void Awake()
    {
        inputActions.Stirng.nami.performed += ctx =>
        {
            float value = ctx.ReadValue<float>();
            m_PauseDirection = value;
            if (m_PauseDirection == 1)//上
            {
                Debug.Log("上のアニメーション再生");
            }
            else if (m_PauseDirection == -1)//下
            {
                Debug.Log("下のアニメーション再生");
            }
            else if (m_PauseDirection == 2)//右
            {
                OnRightInput();
                Debug.Log("右のアニメーション再生");
            }
            else if (m_PauseDirection == 3)//左
            {
                Debug.Log("左のアニメーション再生");
            }
        };
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject first = Instantiate(StringPrefub, Vector3.zero, Quaternion.identity);
        Strings.Add(first);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnRightInput()
    {
        //場所変わってへん！
        // 最後のオブジェクトの右に生成
        Vector3 lastPos = Strings[^1].transform.position;
        Vector3 newPos = lastPos + (Vector3)offset;
        GameObject obj = Instantiate(StringPrefub, newPos, Quaternion.identity);
        Strings.Add(obj);
        Debug.Log(Strings);
    }
    void SewString()
    {

    }


    void OnEnable()
    {
        if (inputActions == null)
        {
            inputActions = new InputSystem_Actions();
        }
        inputActions.Stirng.Enable();
    }
    void OnDisable()
    {
        if (inputActions == null)
        {
            inputActions = new InputSystem_Actions();
        }
        inputActions.Stirng.Disable();
    }
}
