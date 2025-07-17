using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class StringManager : MonoBehaviour
{
    //定数宣言
    public const int RIGHT = 0;
    public const int LEFT = 1;
    public const int Up = 2;
    public const int Down = 3;

    [SerializeField] private GameObject StringPrefub;

    public Vector3 m_StrinngScale = new Vector3(1.0f, 1.0f, 0.0f);
    private Vector2 m_Offset_X=new Vector2(1.0f,0.0f);
    private Vector2 m_Offset_Y =new Vector2(0.0f,-1.0f);
    private List<GameObject> Strings = new List<GameObject>();
    [SerializeField] private int StringNum;
    private InputSystem_Actions inputActions;
    private float m_PauseDirection;//音量調整の入力値
    private int m_LastDirection;//前回の入力値
    void Awake()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Stirng.nami.performed += ctx =>
        {
            float value = ctx.ReadValue<float>();
            m_PauseDirection = value;
            if (m_PauseDirection == 1)//上
            {
                OnUpInput();
            }
            else if (m_PauseDirection == -1)//下
            {
                OnDownInput();
            }
            else if (m_PauseDirection == 2)//右
            {
                OnRightInput();
            }
            else if (m_PauseDirection == 3)//左
            {
                OnLeftInput();
            }
        };
    }

    void Start()
    {
        m_Offset_X=new Vector2(m_StrinngScale.x, 0.0f);
        m_Offset_Y=new Vector2(0.0f,-m_StrinngScale.y);
       GameObject first = Instantiate(StringPrefub, Vector3.zero, Quaternion.identity);
        Strings.Add(first);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnRightInput()
    {
        if (m_LastDirection == LEFT) return;
        // 最後のオブジェクトの右に生成
        Vector3 lastPos = Strings[^1].transform.position;//最後のオブジェクトの位置を取得
        Vector3 newPos = lastPos + (Vector3)m_Offset_X;
        if (m_LastDirection == Up)
        {
            newPos = lastPos + (Vector3)m_Offset_X / 2 - (Vector3)m_Offset_Y / 2;
        }
        else if (m_LastDirection == Down)
        {
            newPos = lastPos + (Vector3)m_Offset_X / 2 + (Vector3)m_Offset_Y / 2;
        }
        GameObject obj = Instantiate(StringPrefub, newPos, Quaternion.identity);

        //アニメーションを実行
        Animator animator=obj.GetComponent<Animator>();
        animator.SetTrigger("Play"); // アニメーションを再生
        Strings.Add(obj);
        m_LastDirection=RIGHT; // 直前の方向を更新
    }
    void OnLeftInput()
    {
        if (m_LastDirection == RIGHT) return;
        // 最後のオブジェクトの左に生成
        Vector3 lastPos = Strings[^1].transform.position; // 最後のオブジェクトの位置
        Vector3 newPos = lastPos - (Vector3)m_Offset_X;        // ← offsetをマイナスにして左側に
        if (m_LastDirection == Up)
        {
            newPos = lastPos - (Vector3)m_Offset_X / 2 - (Vector3)m_Offset_Y / 2; // 上向きのときは少し下にずらす
        }
        else if (m_LastDirection == Down)
        {
            newPos = lastPos-(Vector3)m_Offset_X / 2 + (Vector3)m_Offset_Y / 2; // 下向きのときは少し上にずらす
        }
        GameObject obj = Instantiate(StringPrefub, newPos, Quaternion.identity);
        obj.transform.rotation = Quaternion.Euler(0, 180, 0); // 左向きに回転

        Animator animator = obj.GetComponent<Animator>();
        animator.SetTrigger("Play");
        Strings.Add(obj);
        m_LastDirection = LEFT; // 直前の方向を更新
    }
    void OnUpInput()
    {
        if (m_LastDirection == Down) return;
        // 最後のオブジェクトの左に生成
        Vector3 lastPos = Strings[^1].transform.position; // 最後のオブジェクトの位置
        Vector3 newPos= new Vector3(0.0f,0.0f,0.0f);//初期化
        if (m_LastDirection==RIGHT)
        {
           newPos = lastPos + (Vector3)m_Offset_X / 2 - (Vector3)m_Offset_Y / 2;        //offsetをマイナスにして右側に
        }
        else if (m_LastDirection==LEFT)
        {
            newPos = lastPos - (Vector3)m_Offset_X - (Vector3)m_Offset_Y / 2;        //offsetをマイナスにして左側に
        }
        else if (m_LastDirection == Up)
        {
            newPos = lastPos - (Vector3)m_Offset_Y;        //offsetをマイナスにして左側に
        }

        GameObject obj = Instantiate(StringPrefub, newPos, Quaternion.identity);
        obj.transform.rotation = Quaternion.Euler(0,0, 90); // 上向きに回転

        Animator animator = obj.GetComponent<Animator>();
        animator.SetTrigger("Play");
        Strings.Add(obj);
        m_LastDirection = Up;
    }
    void OnDownInput()
    {
        if(m_LastDirection == Up) return;
        // 最後のオブジェクトの左に生成
        Vector3 lastPos = Strings[^1].transform.position; // 最後のオブジェクトの位置
        Vector3 newPos = new Vector3(0.0f, 0.0f, 0.0f);//初期化
        if (m_LastDirection == RIGHT)
        {
            newPos = lastPos + (Vector3)m_Offset_X /2 + (Vector3)m_Offset_Y /2;        //offsetをマイナスにして右側に
        }
        else if (m_LastDirection == LEFT)
        {
            newPos = lastPos - (Vector3)m_Offset_X /2+ (Vector3)m_Offset_Y / 2;        //offsetをマイナスにして左側に
        }
        else if (m_LastDirection == Down)
        {
            newPos = lastPos + (Vector3)m_Offset_Y;        //offsetをマイナスにして左側に
        }

        GameObject obj = Instantiate(StringPrefub, newPos, Quaternion.identity);
        obj.transform.rotation = Quaternion.Euler(0, 0, 270); // 上向きに回転

        Animator animator = obj.GetComponent<Animator>();
        animator.SetTrigger("Play");
        Strings.Add(obj);
        m_LastDirection = Down;
    }
    void OnEnable()
    {
        inputActions.Stirng.Enable();
    }
    void OnDisable()
    {
        inputActions.Stirng.Disable();
    }
}
