using System.Collections.Generic;
using UnityEngine;

public class StringManager_Canvas : MonoBehaviour
{
    private const int RIGHT = 0;
    private const int LEFT = 1;
    private const int UP = 2;
    private const int DOWN = 3;
    private const int First = 4;

    private const bool m_NoString = false;
    private const bool m_isString = true;

    [SerializeField] private PlayerController m_PlayerController;
    [SerializeField] private StageUILoader m_StageLoader;
    [SerializeField] private RectTransform m_StringPrefub;      // UI用の糸プレハブ（RectTransformに変更）
    [SerializeField] private RectTransform m_Tamadome;          // 糸の先端に付く玉止めオブジェクト
    [SerializeField] private RectTransform m_StringCursol;      // 糸を張る際のカーソル
    [SerializeField] private RectTransform m_CanvasTransform;   // CanvasのRectTransform参照

    [SerializeField] private float m_MirrorOffsetX = 5.0f;
    private Vector2 m_StrinngScale = new Vector2(100f, 100f); // UIスケールに合わせた糸のサイズ
    private Vector2 m_HitBoxScale = new(1, 1);
    private Vector2 m_Offset_X;
    private Vector2 m_Offset_Y;

    private List<RectTransform> m_Strings = new List<RectTransform>();
    private List<RectTransform> m_MirrorStrings = new List<RectTransform>();
    private List<RectTransform> m_FrontStrings = new List<RectTransform>();
    private List<RectTransform> BackStrings = new List<RectTransform>();
    private List<StringAnimation_Canvas> m_AnimStrings = new List<StringAnimation_Canvas>();
    private List<StringAnimation_Canvas> m_MirrorAnimStrings = new List<StringAnimation_Canvas>();
    private List<int> m_StringNum;
    private List<int> m_CopyStringNum;
    private List<RectTransform> m_Tamadomes = new List<RectTransform>();
    private List<int> m_Directions = new List<int>();
    [SerializeField] private GameObject m_Cutter;               // 糸を切るためのカッターオブジェクト
    private int m_CurrentIndex = 0;

    //[SerializeField] private ShowStringNum m_ListDisplay;       // 糸の数などを表示するUI管理クラス
    [SerializeField] private VerticalNumberUI m_ListDisplay;       // 糸の数などを表示するUI管理クラス
    private InputSystem_Actions inputActions;
    private float m_PauseDirection;
    private int m_LastDirection = First;
    private bool m_StringMode = m_NoString;

    private int m_StageWidth = 0;
    private int m_StageHeight = 0;
    private int m_CutNum_front = 0; // 表側の切った回数をカウント
    private int m_CutNum_back = 0; // 裏側の切った回数をカウント

    private List<Vector2Int>  m_PreCursolPosition=new List<Vector2Int>();

    private int m_firstcount = 0;//firstpointのカウント 

    void Awake()
    {
        inputActions = new InputSystem_Actions();

        // 糸の縫い操作
        inputActions.Stirng.nami.performed += ctx =>
        {
            if (m_PlayerController.GetStart()) return;
            if (PauseApperance.Instance.isPause || (SoundChangeSlider.Instance != null && SoundChangeSlider.Instance.IsSoundChange)) return;//ポーズ中は操作できないようにする
            float value = ctx.ReadValue<float>();
            if (m_StringMode == m_isString)
            {
                while (m_CurrentIndex < m_StringNum.Count && m_StringNum[m_CurrentIndex] <= 0)
                {
                    m_CurrentIndex++;
                    m_CurrentIndex = Mathf.Clamp(m_CurrentIndex, 0, m_StringNum.Count - 1);
                }
                Debug.Log($"Current Index: {m_CurrentIndex}, StringNum: {m_StringNum[m_CurrentIndex]}");
                // 糸縫いモード時の方向操作
                m_PauseDirection = value;

                switch (m_PauseDirection)
                {
                    case 1:
                        OnUpInput();
                        break;
                    case -1:
                        OnDownInput();
                        break;
                    case 2:
                        OnRightInput();
                        break;
                    case 3:
                        OnLeftInput();
                        break;
                }
                if (m_StringNum[m_CurrentIndex] == 0)
                {
                    m_CurrentIndex++;
                    BallStopper();
                    Debug.Log(m_StringNum[m_CurrentIndex]);
                }
            }
            else
            {
                // 非糸縫いモード時のカーソル移動
                Vector2 offset = Vector2.zero;
                if (value == 1 && m_StageHeight > 0)
                {
                    offset = -m_Offset_Y;
                    m_StageHeight--;
                }
                else if (value == -1 && m_StageHeight < StageUILoader.stage.STAGE_HEIGHT)
                {
                    m_StageHeight++;
                    offset = m_Offset_Y;
                }
                else if (value == 2 && m_StageWidth < StageUILoader.stage.STAGE_WIDTH)
                {
                    m_StageWidth++;
                    offset = m_Offset_X;
                }
                else if (value == 3 && m_StageWidth > 0)
                {
                    m_StageWidth--;
                    offset = -m_Offset_X;
                }
                m_StringCursol.anchoredPosition += offset;
            }
            m_ListDisplay.UpdateNumbers(m_StringNum); // UI表示を更新
        };

        // 玉止め（糸の終端）設置操作
        inputActions.Stirng.tama.performed += ctx =>
        {
            if (m_PlayerController.GetStart()) return;
            if (m_CurrentIndex >= m_StringNum.Count)
            {
                return;
            }
            if (m_Strings.Count > 0) BallStopper();
        };

        // 糸を縫う処理
        inputActions.Stirng.start.performed += ctx =>
        {
            if (m_PlayerController.GetStart()) return;
            if (m_CurrentIndex >= m_StringNum.Count)
            {
                return;
            }
            if (PauseApperance.Instance.isPause || (SoundChangeSlider.Instance != null && SoundChangeSlider.Instance.IsSoundChange)) return;//ポーズ中は操作できないようにする
            if (m_StringMode == m_isString || m_CurrentIndex >= m_StringNum.Count) return;

            RectTransform dummy = new GameObject("FirstPoint", typeof(RectTransform)).GetComponent<RectTransform>();
            dummy.SetParent(m_CanvasTransform, false);
            dummy.anchoredPosition = m_StringCursol.anchoredPosition;
            Debug.Log($"CursorPos: {m_StringCursol.anchoredPosition}");
            m_Strings.Add(dummy);
            m_StringMode = m_isString;
            m_firstcount++;
        };

        inputActions.Stirng.BackString.performed += ctx =>// 糸の一針戻す操作
        {
            if (m_PlayerController.GetStart()) return;
            RemoveLastStitch();
            m_ListDisplay.UpdateNumbers(m_StringNum); // UI表示を更新
        };

        // 返し縫いを生成(没のためコメントアウト)
        //inputActions.Stirng.kaesi.performed += ctx =>
        //{
        //    if (m_StringMode == NoString || currentIndex >= StringNum.Count)
        //    {
        //        Debug.Log(CopyStringNum[currentIndex]);
        //        Debug.Log(StringNum[currentIndex]);
        //        return;
        //    }
        //    OnKaesiInput();
        //};
    }

    public void RemoveLastStitch(int count = 1)
    {
        while (m_CurrentIndex > 0 && m_StringNum[m_CurrentIndex - 1] <= 0 && m_StringNum[m_CurrentIndex] == m_CopyStringNum[m_CurrentIndex])//wawa
        {
            Debug.Log("インデックス増やすよ");
            m_CurrentIndex--;
            Destroy(m_Tamadomes[^1].gameObject);
            m_Tamadomes.RemoveAt(m_Tamadomes.Count - 1);
            m_StringMode = m_isString;
            //カーソル位置を戻す
            m_StageWidth = m_PreCursolPosition[^1].x;
            m_StageHeight = m_PreCursolPosition[^1].y;
            m_PreCursolPosition.RemoveAt(m_PreCursolPosition.Count - 1);

        }
        // 糸が存在しない or FirstPointしかない場合は何もしない
        if (m_Strings.Count <= 1)
        {
            Debug.LogWarning("削除できる糸がありません。");
            m_StringMode = m_NoString;

            //最初のFirstPointだけ削除
            Destroy(m_Strings[0].gameObject);
            m_Strings.RemoveAt(0);
            return;
        }

        int removeCount = Mathf.Min(count, m_MirrorStrings.Count);

        for (int i = 0; i < removeCount; i++)
        {
            int index = m_MirrorStrings.Count - 1;
            if (index < 0) break;

            Vector2 resumePos = (m_Strings.Count > 1 && m_Strings[^2] != null)
           ? BackStrings[^1].anchoredPosition
           : Vector2.zero;

            // --- アニメ関連を安全に削除 ---
            if (m_AnimStrings.Count > 0 && m_AnimStrings[^1] != null)
            {
                m_AnimStrings[^1].DeleteImage(0);
                Destroy(m_AnimStrings[^1].gameObject);
                m_AnimStrings.RemoveAt(m_AnimStrings.Count - 1);
            }

            if (m_MirrorAnimStrings.Count > 0 && m_MirrorAnimStrings[^1] != null)
            {
                m_MirrorAnimStrings[^1].DeleteImage(0);
                Destroy(m_MirrorAnimStrings[^1].gameObject);
                m_MirrorAnimStrings.RemoveAt(m_MirrorAnimStrings.Count - 1);
            }

            // --- 残りの削除処理 ---
            if (m_Strings.Count > 0 && m_Strings[^1] != null)
                Destroy(m_Strings[^1].gameObject);
            if (m_MirrorStrings.Count > 0 && m_MirrorStrings[^1] != null)
                Destroy(m_MirrorStrings[^1].gameObject);
            if (m_FrontStrings.Count > 0 && m_FrontStrings[^1] != null)
                Destroy(m_FrontStrings[^1].gameObject);
            if (BackStrings.Count > 0 && BackStrings[^1] != null)
                Destroy(BackStrings[^1].gameObject);

            // リスト削除
            if (m_Strings.Count > 0) m_Strings.RemoveAt(m_Strings.Count - 1);
            if (m_MirrorStrings.Count > 0) m_MirrorStrings.RemoveAt(m_MirrorStrings.Count - 1);
            if (m_FrontStrings.Count > 0) m_FrontStrings.RemoveAt(m_FrontStrings.Count - 1);
            if (BackStrings.Count > 0) BackStrings.RemoveAt(BackStrings.Count - 1);

            // null除去
            m_Strings.RemoveAll(s => s == null);
            m_MirrorStrings.RemoveAll(s => s == null);
            m_FrontStrings.RemoveAll(s => s == null);
            BackStrings.RemoveAll(s => s == null);

            m_StringCursol.anchoredPosition = resumePos;

            // 糸数を戻す
            if (m_CurrentIndex >= 0 && m_CurrentIndex < m_StringNum.Count)
                m_StringNum[m_CurrentIndex]++;


            // 縫い方向を初期化
            switch (m_LastDirection)
            {
                case RIGHT:
                    m_StageWidth--;
                    break;
                case LEFT:
                    m_StageWidth++;
                    break;
                case UP:
                    m_StageHeight++;
                    break;
                case DOWN:
                    m_StageHeight--;
                    break;
                case First:
                    break;
            }
            m_Directions.RemoveAt(m_Directions.Count - 1);
            if (m_Directions.Count > 0)
            {
                m_LastDirection = m_Directions[m_Directions.Count - 1];
            }
            else
            {
                m_LastDirection = First;
            }
        }
        if (m_Strings[^1].name == "FirstPoint")
        {
            m_StringMode = m_NoString;
            Destroy(m_Strings[^1].gameObject);
            m_Strings.RemoveAt(m_Strings.Count - 1);
            m_LastDirection = First;
            m_firstcount--;
        }
    }

    void Start()
    {
        m_Offset_X = new Vector2(m_StrinngScale.x, 0f);
        m_Offset_Y = new Vector2(0f, -m_StrinngScale.y);

        m_StringNum = new List<int>(StageUILoader.stage.STRING_COUNT); // ステージの糸数情報を取得
        m_CopyStringNum = new List<int>(StageUILoader.stage.STRING_COUNT);

        m_ListDisplay.ShowNumbers(m_StringNum); // UI表示を更新
    }

    void OnEnable()
    {
        inputActions.Stirng.Enable();
    }

    void OnDisable()
    {
        inputActions.Stirng.Disable();
    }

    // 糸を切る処理（指定indexの糸を削除）
    public void CutString(int index, bool front ,int firstct)
    {
        if (front)
        {
            Destroy(m_Strings[index + firstct - m_CutNum_front].gameObject); // FirstPointを加算
            m_AnimStrings[index].DeleteImage(0);
            m_Strings.RemoveAt(index + firstct - m_CutNum_front);
            m_AnimStrings.RemoveAt(index - m_CutNum_front);
            Destroy(m_FrontStrings[index - m_CutNum_front].gameObject);
            Destroy(BackStrings[index - m_CutNum_front].gameObject);

            m_FrontStrings.RemoveAt(index - m_CutNum_front);
            BackStrings.RemoveAt(index - m_CutNum_front);

            m_CutNum_front++;
        }
        else
        {
            Destroy(m_MirrorStrings[index - m_CutNum_back].gameObject);
            m_MirrorAnimStrings[index - m_CutNum_back].DeleteImage(0);
            m_MirrorStrings.RemoveAt(index - m_CutNum_back);
            m_MirrorAnimStrings.RemoveAt(index - m_CutNum_back);

            Destroy(m_FrontStrings[index - m_CutNum_back].gameObject);
            Destroy(BackStrings[index - m_CutNum_back].gameObject);

            m_FrontStrings.RemoveAt(index - m_CutNum_back);
            BackStrings.RemoveAt(index - m_CutNum_back);

            m_CutNum_back++;
        }
    }

    // 各方向への糸設置処理
    void OnRightInput()
    {
        if (m_LastDirection == LEFT) return;
        Vector2 lastPos = m_Strings[^1].anchoredPosition;
        Vector2 newPos = lastPos + m_Offset_X;

        if (m_LastDirection == UP) newPos = lastPos + m_Offset_X / 2 - m_Offset_Y / 2;
        else if (m_LastDirection == DOWN) newPos = lastPos + m_Offset_X / 2 + m_Offset_Y / 2;
        else if (m_LastDirection == First) newPos = lastPos + m_Offset_X / 2;

        Vector2 frontPos = newPos + m_Offset_X / 2;
        Vector2 backPos = newPos - m_Offset_X / 2;

        if (CheckString(newPos, frontPos, backPos) && m_StageWidth < StageUILoader.stage.STAGE_WIDTH)
        {
            AddString(newPos, frontPos, backPos, Quaternion.identity);
            m_LastDirection = RIGHT;
            m_Directions.Add(m_LastDirection);
            m_StageWidth++;
            m_StringNum[m_CurrentIndex]--;
            m_StringCursol.anchoredPosition += m_Offset_X;
        }
    }

    public void ShowCutter() => m_Cutter.SetActive(true);
    public void DeleteCutter() => m_Cutter.SetActive(false);

    void OnLeftInput()
    {
        if (m_LastDirection == RIGHT) return;


        Vector2 lastPos = m_Strings[^1].anchoredPosition;
        Vector2 newPos = lastPos - m_Offset_X;

        if (m_LastDirection == UP) newPos = lastPos - m_Offset_X / 2 - m_Offset_Y / 2;
        else if (m_LastDirection == DOWN) newPos = lastPos - m_Offset_X / 2 + m_Offset_Y / 2;
        else if (m_LastDirection == First) newPos = lastPos - m_Offset_X / 2;

        Vector2 frontPos = newPos - m_Offset_X / 2;
        Vector2 backPos = newPos + m_Offset_X / 2;

        if (CheckString(newPos, frontPos, backPos) && m_StageWidth > 0)
        {
            AddString(newPos, frontPos, backPos, Quaternion.Euler(0, 180, 0));
            m_LastDirection = LEFT;
            m_Directions.Add(m_LastDirection);
            m_StageWidth--;
            m_StringNum[m_CurrentIndex]--;
            m_StringCursol.anchoredPosition -= m_Offset_X;
        }
    }

    void OnUpInput()
    {
        if (m_LastDirection == DOWN) return;

        Vector2 lastPos = m_Strings[^1].anchoredPosition;
        Vector2 newPos = lastPos;

        if (m_LastDirection == RIGHT) newPos = lastPos + m_Offset_X / 2 - m_Offset_Y / 2;
        else if (m_LastDirection == LEFT) newPos = lastPos - m_Offset_X / 2 - m_Offset_Y / 2;
        else if (m_LastDirection == UP) newPos = lastPos - m_Offset_Y;
        else if (m_LastDirection == First) newPos = lastPos - m_Offset_Y / 2;
        Vector2 frontPos = newPos - m_Offset_Y / 2;
        Vector2 backPos = newPos + m_Offset_Y / 2;

        if (CheckString(newPos, frontPos, backPos) && m_StageHeight > 0)
        {
            AddString(newPos, frontPos, backPos, Quaternion.Euler(0, 0, 90));
            m_LastDirection = UP;
            m_Directions.Add(m_LastDirection);
            m_StageHeight--;
            m_StringNum[m_CurrentIndex]--;
            m_StringCursol.anchoredPosition -= m_Offset_Y;
        }
    }

    void OnDownInput()
    {
        if (m_LastDirection == UP) return;

        Vector2 lastPos = m_Strings[^1].anchoredPosition;
        Vector2 newPos = lastPos;

        if (m_LastDirection == RIGHT) newPos = lastPos + m_Offset_X / 2 + m_Offset_Y / 2;
        else if (m_LastDirection == LEFT) newPos = lastPos - m_Offset_X / 2 + m_Offset_Y / 2;
        else if (m_LastDirection == DOWN) newPos = lastPos + m_Offset_Y;
        else if (m_LastDirection == First) newPos = lastPos + m_Offset_Y / 2;

        Vector2 frontPos = newPos + m_Offset_Y / 2;
        Vector2 backPos = newPos - m_Offset_Y / 2;

        if (CheckString(newPos, frontPos, backPos) && m_StageHeight < StageUILoader.stage.STAGE_HEIGHT)
        {
            AddString(newPos, frontPos, backPos, Quaternion.Euler(0, 0, 270));
            m_LastDirection = DOWN;
            m_Directions.Add(m_LastDirection);
            m_StageHeight++;//いったん消します
            m_StringNum[m_CurrentIndex]--;
            m_StringCursol.anchoredPosition += m_Offset_Y;
        }
    }

    // 返し縫いを作成する(没)
    void OnKaesiInput()
    {
        Vector3 lastPos = m_Strings[^1].transform.position;
        Vector3 newPos = Vector3.zero;
        RectTransform obj = new RectTransform();
        Animator animator = new Animator();

        switch (m_LastDirection)
        {
            case RIGHT:
                newPos = lastPos - (Vector3)m_Offset_Y / 10;
                obj = Instantiate(m_StringPrefub, newPos, Quaternion.identity);
                obj.transform.rotation = Quaternion.Euler(0, 180, 0);
                obj.tag = "Kaesi";
                animator = obj.GetComponent<Animator>();
                animator.SetTrigger("Play");
                newPos = lastPos;

                obj = Instantiate(m_StringPrefub, newPos, Quaternion.identity);
                m_Strings.Add(obj);
                m_LastDirection = LEFT;
                break;

            case LEFT:
                newPos = lastPos - (Vector3)m_Offset_Y / 10;
                obj = Instantiate(m_StringPrefub, newPos, Quaternion.identity);
                obj.tag = "Kaesi";
                animator = obj.GetComponent<Animator>();
                animator.SetTrigger("Play");
                newPos = lastPos;

                obj = Instantiate(m_StringPrefub, newPos, Quaternion.identity);
                m_Strings.Add(obj);
                m_LastDirection = RIGHT;
                break;

            case UP:
            case DOWN:
                return;
        }
    }

    // 糸を生成・配置する（表裏と前後パーツ）
    void AddString(Vector2 main, Vector2 front, Vector2 back, Quaternion rot)
    {
        //表側糸生成
        RectTransform mainStr = Instantiate(m_StringPrefub, m_CanvasTransform);
        mainStr.anchoredPosition = main;
        mainStr.sizeDelta = m_StrinngScale; // サイズ設定
        mainStr.rotation = rot;
        mainStr.GetComponent<Animator>()?.SetTrigger("Play");

        StringAnimation_Canvas anim = mainStr.GetComponent<StringAnimation_Canvas>();
        if (anim != null)
        {
            anim.SetCanvas(m_CanvasTransform);
            anim.index = m_Strings.Count - m_firstcount; // FirstPoint分消す
            anim.front = true;
            anim.firstct = m_firstcount;
        }
        m_AnimStrings.Add(anim);

        BoxCollider2D col = mainStr.GetComponent<BoxCollider2D>();
        if (col != null) col.size *= m_HitBoxScale;
        m_Strings.Add(mainStr);

        // 反対側の糸も生成
        Vector3 mirrorPos = main;
        float mirrorCenterX = 0.0f;
        mirrorPos.x = mirrorCenterX - (main.x - mirrorCenterX);
        RectTransform mirrorStr = Instantiate(m_StringPrefub, m_CanvasTransform);
        mirrorStr.anchoredPosition = mirrorPos;
        mirrorStr.sizeDelta = m_StrinngScale;
        mirrorStr.rotation = rot;
        if (Mathf.Abs(rot.y) > 0.5f)
            mirrorStr.rotation *= Quaternion.Euler(0, 180f, 0);

        mirrorStr.GetComponent<Animator>()?.SetTrigger("Play");
        anim = mirrorStr.GetComponent<StringAnimation_Canvas>();
        if (anim != null)
        {
            anim.SetCanvas(m_CanvasTransform);
            anim.index = m_MirrorStrings.Count;
            anim.front = false;
        }
        m_MirrorAnimStrings.Add(anim);
        col = mirrorStr.GetComponent<BoxCollider2D>();
        if (col != null) col.size *= m_HitBoxScale;
        m_MirrorStrings.Add(mirrorStr);

        // 前後に設置する補助パーツ（接触判定無効）縫えない判定のため使用
        RectTransform frontStr = Instantiate(m_StringPrefub, m_CanvasTransform);
        frontStr.sizeDelta = m_StrinngScale;
        frontStr.anchoredPosition = front;
        anim = frontStr.GetComponent<StringAnimation_Canvas>();
        if (anim != null) anim.SetCanvas(m_CanvasTransform);
        col = frontStr.GetComponent<BoxCollider2D>();
        if (col != null)
        {
            col.size = Vector2.zero;
            col.isTrigger = true;
        }
        m_FrontStrings.Add(frontStr);

        RectTransform backStr = Instantiate(m_StringPrefub, m_CanvasTransform);
        backStr.sizeDelta = m_StrinngScale;
        backStr.anchoredPosition = back;
        anim = backStr.GetComponent<StringAnimation_Canvas>();
        if (anim != null) anim.SetCanvas(m_CanvasTransform);
        col = backStr.GetComponent<BoxCollider2D>();
        if (col != null)
        {
            col.size = Vector2.zero;
            col.isTrigger = true;
        }
        BackStrings.Add(backStr);
    }

    // 糸の配置位置に重なりがないかをチェック
    bool CheckString(Vector2 pos, Vector2 front, Vector2 back)
    {
        // Destroy済み（MissingReference）をスキップする安全版ループ
        foreach (var str in m_Strings)
        {
            if (str == null || !str) continue;
            if (Vector2.Distance(str.anchoredPosition, front) < 0.001f) return false;
        }

        foreach (var str in m_FrontStrings)
        {
            if (str == null || !str) continue; // ←★ これを追加
            if (Vector2.Distance(str.anchoredPosition, front) < 0.001f) return false;
        }

        foreach (var str in BackStrings)
        {
            if (str == null || !str) continue; // ←★ これも追加
            if (Vector2.Distance(str.anchoredPosition, front) < 0.001f) return false;
        }

        return true;
    }

    // 糸の先端に玉止めを設置
    void BallStopper()
    {
        Vector2 lastPos = m_Strings[^1].anchoredPosition;
        Vector2 newPos = lastPos;

        int stageWidth = m_StageWidth;
        int stageHeight = m_StageHeight;
        switch (m_LastDirection)
        {
            case RIGHT:
                {
                    newPos += m_Offset_X / 2;
                    stageWidth--;
                    break;
                }
            case LEFT:
                {
                    newPos -= m_Offset_X / 2;
                    stageWidth++;
                    break;
                }
            case UP:
                {
                    newPos -= m_Offset_Y / 2;
                    stageHeight++;
                    break;
                }
            case DOWN:
                {
                    newPos += m_Offset_Y / 2;
                    stageHeight--;
                    break;
                }
        }

        RectTransform tama = Instantiate(m_Tamadome, m_CanvasTransform);
        tama.sizeDelta = m_StrinngScale * 0.5f;
        tama.anchoredPosition = newPos;
        m_Tamadomes.Add(tama);
        m_PreCursolPosition.Add(new Vector2Int(stageWidth, stageHeight));//最後の玉止めの場所の地点でのカーソル位置を保存して、玉止めの地点から戻す際にその値をセットしたらいける！！玉止め後のずれがなくなる

        m_StringMode = m_NoString;
        m_LastDirection = First;
        m_ListDisplay.UpdateNumbers(m_StringNum); // UI表示を更新
    }

    // 糸のサイズと当たり判定スケールを設定
    public void SetStringSize(Vector2 size, Vector2 BoxScale)
    {
        m_StrinngScale = size;
        m_HitBoxScale = BoxScale;
    }

    public void SetCursor(Vector2 pos)
    {
        m_StringCursol.anchoredPosition = pos;
    }

    public void CursorLastSibling()
    {
        m_StringCursol.SetAsLastSibling();
    }
}
