using System.Collections.Generic;
using UnityEngine;
public class StringManager : MonoBehaviour
{
    //定数宣言
    private const int RIGHT = 0;
    private const int LEFT = 1;
    private const int Up = 2;
    private const int Down = 3;
    private const int Middle = 4;

    private const bool NoString=false;
    private const bool isString=true;

    [SerializeField] private GameObject StringPrefub;

    public Vector3 m_StrinngScale = new Vector3(1.0f, 1.0f, 0.0f);
    private Vector2 m_Offset_X=new Vector2(1.0f,0.0f);
    private Vector2 m_Offset_Y =new Vector2(0.0f,-1.0f);
    private List<GameObject> Strings = new List<GameObject>();
    private List<GameObject> FrontStrings = new List<GameObject>();
    private List<GameObject> BackStrings = new List<GameObject>();
    [SerializeField] List<int> StringNum;
    private int currentIndex = 0;

    [SerializeField] private ShowStringNum listDisplay; // 表示クラスをインスペクターでセット
    [SerializeField] GameObject Tamadome;
    [SerializeField] GameObject StringCursol;
    private InputSystem_Actions inputActions;
    private float m_PauseDirection;//音量調整の入力値
    private int m_LastDirection;//前回の入力値

    bool m_StringMode = NoString;//ストリングモードのフラグ

    public bool EndSiting = false; // たまを止めるかどうかのフラグ

    void Awake()
    {
        inputActions = new InputSystem_Actions();

        inputActions.Stirng.nami.performed += ctx =>
        {
     
            float value = ctx.ReadValue<float>();
            if(m_StringMode== isString)
            {
                // すべての要素が0の場合、処理を行わない
                while (currentIndex < StringNum.Count && StringNum[currentIndex] <= 0)
                {
                    currentIndex++;
                }

                // 現在処理可能な要素がなければ終了
                if (currentIndex >= StringNum.Count)
                {
                    Debug.Log("すべての処理が完了しました");
                    return;
                }

                // 対象要素を1減らす
                StringNum[currentIndex]--;

                Debug.Log($"Index {currentIndex} の要素を1減らしました。残り: {StringNum[currentIndex]}");

                listDisplay.UpdateDisplay(StringNum);// Text表示を更新

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
                // もし現在の要素が0になったら、次回は次のインデックスへ進むようになる
                if (StringNum[currentIndex] == 0)
                {
                    currentIndex++;
                    Debug.Log($"Index {currentIndex} の要素が0になりました。次の要素へ進みます。");
                    //EndSiting = true;←　これAnimationに入れてるんやけどなんかStringsが反映されてないっぽいです
                    BallStopper();//たまを止める処理を呼び出す
                }
            }
            else if (m_StringMode == NoString)
            {
                m_PauseDirection = value;
                if (m_PauseDirection == 1)//上
                {
                    StringCursol.transform.position -= (Vector3)m_Offset_Y;
                }
                else if (m_PauseDirection == -1)//下
                {
                    StringCursol.transform.position += (Vector3)m_Offset_Y;
                }
                else if (m_PauseDirection == 2)//右
                {
                    StringCursol.transform.position += (Vector3)m_Offset_X;
                }
                else if (m_PauseDirection == 3)//左
                {
                    StringCursol.transform.position -= (Vector3)m_Offset_X;
                }
            }
        };
        inputActions.Stirng.tama.performed += ctx =>
        {
            // たまを生成する処理
            if (Strings.Count > 0)
            {
                BallStopper();
            }
        };
        inputActions.Stirng.start.performed += ctx =>
        {
            //最初の初点を決める
            // GameObject first = Instantiate(StringPrefub, StringCursol.transform.position, Quaternion.identity);
            GameObject dummy = new GameObject("FirstPoint");
            dummy.transform.position = StringCursol.transform.position;
            Strings.Add(dummy);
            m_StringMode = isString; // ストリングモードを有効にする
        };
    }

    void Start()
    {
        //最初の初点を決める
        m_Offset_X=new Vector2(m_StrinngScale.x, 0.0f);
        m_Offset_Y=new Vector2(0.0f,-m_StrinngScale.y);
        m_LastDirection = Middle;
        listDisplay.UpdateDisplay(StringNum);// Text表示を更新
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnRightInput()
    {
        if (m_LastDirection == LEFT) return;
        // 最後のオブジェクトの右に生成
        Vector3 lastPos = Strings[^1].transform.position;//最後のオブジェクトの位置を取得→ここを最初だけ始点の場所に指定したら最初左行かない問題解決しそう
        Vector3 FrontlastPos;
        Vector3 BacklastPos;
        Vector3 newPos = lastPos + (Vector3)m_Offset_X;

        if (m_LastDirection == Up)
        {
            newPos = lastPos + (Vector3)m_Offset_X / 2 - (Vector3)m_Offset_Y / 2;
        }
        else if (m_LastDirection == Down)
        {
            newPos = lastPos + (Vector3)m_Offset_X / 2 + (Vector3)m_Offset_Y / 2;
        }
        else if (m_LastDirection == Middle)
        {
            newPos = lastPos + (Vector3)m_Offset_X/2; // 最初の位置から右にずらす
        }
        FrontlastPos = newPos + (Vector3)m_Offset_X / 2; // 上向きのときは少し上にずらす
        BacklastPos = newPos - (Vector3)m_Offset_X / 2; // 上向きのときは少し下にずらす

        if(CheckString(newPos, FrontlastPos, BacklastPos))
        {
            GameObject obj = Instantiate(StringPrefub, newPos, Quaternion.identity);
            //アニメーションを実行
            Animator animator = obj.GetComponent<Animator>();
            animator.SetTrigger("Play"); // アニメーションを再生
            Strings.Add(obj);
            GameObject frontobj = Instantiate(StringPrefub, FrontlastPos, Quaternion.identity);
            FrontStrings.Add(frontobj);
            GameObject backobj = Instantiate(StringPrefub, BacklastPos, Quaternion.identity);
            BackStrings.Add(backobj);

            m_LastDirection = RIGHT; // 直前の方向を更新
        }
    }
    void OnLeftInput()
    {
        if (m_LastDirection == RIGHT) return;
        // 最後のオブジェクトの左に生成
        Vector3 lastPos = Strings[^1].transform.position; // 最後のオブジェクトの位置
        Vector3 FrontlastPos;
        Vector3 BacklastPos;
        Vector3 newPos = lastPos - (Vector3)m_Offset_X;        // ← offsetをマイナスにして左側に

        if (m_LastDirection == Up)
        {
            newPos = lastPos - (Vector3)m_Offset_X / 2 - (Vector3)m_Offset_Y / 2; // 上向きのときは少し下にずらす
        }
        else if (m_LastDirection == Down)
        {
            newPos = lastPos-(Vector3)m_Offset_X / 2 + (Vector3)m_Offset_Y / 2; // 下向きのときは少し上にずらす
        }
        else if (m_LastDirection == Middle)
        {
            newPos = lastPos - (Vector3)m_Offset_X/2; // 最初の位置から右にずらす
        }
        FrontlastPos = newPos - (Vector3)m_Offset_X / 2; // 上向きのときは少し上にずらす
        BacklastPos = newPos + (Vector3)m_Offset_X / 2; // 上向きのときは少し下にずらす
        if (CheckString(newPos, FrontlastPos, BacklastPos))
        {
            GameObject obj = Instantiate(StringPrefub, newPos, Quaternion.identity);
            obj.transform.rotation = Quaternion.Euler(0, 180, 0); // 左向きに回転
            //アニメーションを実行
            Animator animator = obj.GetComponent<Animator>();
            animator.SetTrigger("Play"); // アニメーションを再生
            Strings.Add(obj);
            GameObject frontobj = Instantiate(StringPrefub, FrontlastPos, Quaternion.identity);
            FrontStrings.Add(frontobj);
            GameObject backobj = Instantiate(StringPrefub, BacklastPos, Quaternion.identity);
            BackStrings.Add(backobj);

            m_LastDirection = LEFT; // 直前の方向を更新
        }
    }
    void OnUpInput()
    {
        if (m_LastDirection == Down) return;
        // 最後のオブジェクトの左に生成
        Vector3 lastPos = Strings[^1].transform.position; // 最後のオブジェクトの位置
        Vector3 FrontlastPos;
        Vector3 BacklastPos;
        Vector3 newPos= new Vector3(0.0f,0.0f,0.0f);//初期化

        if (m_LastDirection==RIGHT)
        {
           newPos = lastPos + (Vector3)m_Offset_X / 2 - (Vector3)m_Offset_Y / 2;        //offsetをマイナスにして右側に
        }
        else if (m_LastDirection==LEFT)
        {
            newPos = lastPos - (Vector3)m_Offset_X/2 - (Vector3)m_Offset_Y / 2;        //offsetをマイナスにして左側に
        }
        else if (m_LastDirection == Up)
        {
            newPos = lastPos - (Vector3)m_Offset_Y;        //offsetをマイナスにして左側に
        }
        else if (m_LastDirection == Middle)
        {
            newPos = lastPos - (Vector3)m_Offset_Y/2; // 最初の位置から右にずらす
        }
        FrontlastPos = newPos - (Vector3)m_Offset_Y / 2; // 上向きのときは少し上にずらす
        BacklastPos = newPos + (Vector3)m_Offset_Y / 2; // 上向きのときは少し下にずらす

        if (CheckString(newPos, FrontlastPos, BacklastPos))
        {
            GameObject obj = Instantiate(StringPrefub, newPos, Quaternion.identity);
            obj.transform.rotation = Quaternion.Euler(0,0, 90); // 上向きに回転
            //アニメーションを実行
            Animator animator = obj.GetComponent<Animator>();
            animator.SetTrigger("Play"); // アニメーションを再生
            Strings.Add(obj);
            GameObject frontobj = Instantiate(StringPrefub, FrontlastPos, Quaternion.identity);
            FrontStrings.Add(frontobj);
            GameObject backobj = Instantiate(StringPrefub, BacklastPos, Quaternion.identity);
            BackStrings.Add(backobj);

            m_LastDirection = Up; // 直前の方向を更新
        }

    }
    void OnDownInput()
    {
        if(m_LastDirection == Up) return;
        // 最後のオブジェクトの左に生成
        Vector3 lastPos = Strings[^1].transform.position; // 最後のオブジェクトの位置
        Vector3 newPos = new Vector3(0.0f, 0.0f, 0.0f);//初期化
        Vector3 FrontlastPos;
        Vector3 BacklastPos;
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
        else if (m_LastDirection == Middle)
        {
            newPos = lastPos + (Vector3)m_Offset_Y / 2; // 最初の位置から右にずらす
        }
        FrontlastPos = newPos + (Vector3)m_Offset_Y / 2; // 上向きのときは少し上にずらす
        BacklastPos = newPos - (Vector3)m_Offset_Y / 2; // 上向きのときは少し下にずらす

        if (CheckString(newPos, FrontlastPos, BacklastPos))
        {
            GameObject obj = Instantiate(StringPrefub, newPos, Quaternion.identity);
            //アニメーションを実行
            obj.transform.rotation = Quaternion.Euler(0, 0, 270); // 上向きに回転
            Animator animator = obj.GetComponent<Animator>();
            animator.SetTrigger("Play"); // アニメーションを再生
            Strings.Add(obj);
            GameObject frontobj = Instantiate(StringPrefub, FrontlastPos, Quaternion.identity);
            FrontStrings.Add(frontobj);
            GameObject backobj = Instantiate(StringPrefub, BacklastPos, Quaternion.identity);
            BackStrings.Add(backobj);

            m_LastDirection =Down; // 直前の方向を更新
        }
    }

    bool CheckString(Vector3 newPos, Vector3 FrontlastPos, Vector3 BacklastPos)
    {
        // 重なりチェック（微小なズレ防止のため距離で判定）
        foreach (GameObject str in Strings)
        {
            if (Vector3.Distance(str.transform.position, FrontlastPos) < 0.001f)
            {
                return false; // すでに同じ位置に存在 → 処理中断
            }
        }
        foreach (GameObject str in FrontStrings)
        {
            if (Vector3.Distance(str.transform.position, FrontlastPos) < 0.001f)
            {
                return false; // すでに同じ位置に存在 → 処理中断
            }
        }
        foreach (GameObject str in BackStrings)
        {
            if (Vector3.Distance(str.transform.position, FrontlastPos) < 0.001f)
            {
                return false; // すでに同じ位置に存在 → 処理中断
            }
        }
        return true; // 重なりがない場合はtrueを返す
    }
    public void BallStopper()
    {
        Vector3 lastPos = Strings[^1].transform.position;
        Vector3 newPos = new Vector3(0.0f, 0.0f, 0.0f);//初期化
        switch (m_LastDirection)
        {
            case RIGHT:
                // 右にたまを止める処理
                newPos = lastPos + (Vector3)m_Offset_X / 2;        //offsetをマイナスにして右側に
                break;
            case LEFT:
                // 左にたまを止める処理
                newPos = lastPos - (Vector3)m_Offset_X / 2;        //offsetをマイナスにして右側に
                break;
            case Up:
                // 上にたまを止める処理
                newPos = lastPos - (Vector3)m_Offset_Y / 2;
                break;
            case Down:
                // 下にたまを止める処理
                newPos = lastPos + (Vector3)m_Offset_Y / 2;
                break;
        }
        // たまを止める処理
        GameObject tama = Instantiate(Tamadome, newPos, Quaternion.identity);
        m_StringMode = NoString;
        m_LastDirection = Middle; // 直前の方向を初期化

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
