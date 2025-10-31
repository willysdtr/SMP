using StageInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class StringManager_Canvas : MonoBehaviour
{
    private const int RIGHT = 0;
    private const int LEFT = 1;
    private const int UP = 2;
    private const int DOWN = 3;
    private const int First = 4;

    private const bool NoString = false;
    private const bool isString = true;

    [SerializeField] private StageUILoader StageLoader;
    [SerializeField] private RectTransform StringPrefub;      // UI用の糸プレハブ（RectTransformに変更）
    [SerializeField] private RectTransform Tamadome;          // 糸の先端に付く玉止めオブジェクト
    [SerializeField] private RectTransform StringCursol;      // 糸を張る際のカーソル
    [SerializeField] private RectTransform canvasTransform;   // CanvasのRectTransform参照

    [SerializeField] private float mirrorOffsetX = 5.0f;
    private Vector2 m_StrinngScale = new Vector2(100f, 100f); // UIスケールに合わせた糸のサイズ
    private Vector2 HitBoxScale = new(1, 1);
    private Vector2 m_Offset_X;
    private Vector2 m_Offset_Y;

    private List<RectTransform> Strings = new List<RectTransform>();
    private List<RectTransform> MirrorStrings = new List<RectTransform>();
    private List<RectTransform> FrontStrings = new List<RectTransform>();
    private List<RectTransform> BackStrings = new List<RectTransform>();
    private List<StringAnimation_Canvas> AnimStrings = new List<StringAnimation_Canvas>();
    private List<StringAnimation_Canvas> MirrorAnimStrings = new List<StringAnimation_Canvas>();
    private List<int> StringNum;
    private List<RectTransform> Tamadomes=new List<RectTransform>();
    private List<int>Directions=new List<int>();
    [SerializeField] List<int> CopyStringNum;
    [SerializeField] private GameObject Cutter;               // 糸を切るためのカッターオブジェクト
    private int currentIndex = 0;

    [SerializeField] private ShowStringNum listDisplay;       // 糸の数などを表示するUI管理クラス

    private InputSystem_Actions inputActions;
    private float m_PauseDirection;
    private int m_LastDirection=First;
    private bool m_StringMode = NoString;

    private int StageWidth = 0;
    private int StageHeight = 0;
    public int CutNum = 0;
    private int m_PreDirection= First;

    void Awake()
    {
        inputActions = new InputSystem_Actions();

        // 糸の縫い操作
        inputActions.Stirng.nami.performed += ctx =>
        {
            if (PauseApperance.Instance.isPause || (SoundChangeSlider.Instance != null && SoundChangeSlider.Instance.IsSoundChange)) return;//ポーズ中は操作できないようにする

             float value = ctx.ReadValue<float>();
            if (m_StringMode == isString)
            {
                while (currentIndex < StringNum.Count && StringNum[currentIndex] <= 0)
                {
                    currentIndex++;
                }
                Debug.Log($"Current Index: {currentIndex}, StringNum: {StringNum[currentIndex]}");
                // 糸縫いモード時の方向操作
                m_PauseDirection = value;

                //カーソルを動かす操作はOn○○Inputに移動
                //Vector2 offset = Vector2.zero;
                if (m_PauseDirection == 1)
                {
                    Debug.Log("上入力");
                    OnUpInput();
                    //offset = -m_Offset_Y;
                }
                else if (m_PauseDirection == -1)
                {
                    Debug.Log("下入力");
                    OnDownInput();
                    //offset = m_Offset_Y;
                }
                else if (m_PauseDirection == 2)
                {
                    // 右入力
                    Debug.Log("右入力");
                    OnRightInput();
                    //offset = m_Offset_X;
                }
                else if (m_PauseDirection == 3)
                {
                    // 左入力
                    Debug.Log("左入力");
                    OnLeftInput();
                    //offset = -m_Offset_X;
                }
                //StringCursol.anchoredPosition += offset;
                if (StringNum[currentIndex] == 0)
                {
                    currentIndex++;
                    BallStopper();
                    Debug.Log(StringNum[currentIndex]);
                }
            }
            else
            {
                // 非糸縫いモード時のカーソル移動
                Vector2 offset = Vector2.zero;
                if (value == 1 && StageHeight > 0)
                {
                    offset = -m_Offset_Y;
                    StageHeight--;
                }
                else if (value == -1 && StageHeight < StageUILoader.stage.STAGE_HEIGHT)
                {
                    StageHeight++;
                    offset = m_Offset_Y;
                }
                else if (value == 2 && StageWidth < StageUILoader.stage.STAGE_WIDTH)
                {
                    StageWidth++;
                    offset = m_Offset_X;
                }
                else if (value == 3 && StageWidth > 0)
                {
                    StageWidth--;
                    offset = -m_Offset_X;
                }
                StringCursol.anchoredPosition += offset;
            }
            listDisplay.UpdateDisplay(StringNum); // UI表示を更新
        };

        // 玉止め（糸の終端）設置操作
        inputActions.Stirng.tama.performed += ctx =>
        {
            if (currentIndex >= StringNum.Count)
            {
                return;
            }
            if (Strings.Count > 0) BallStopper();
        };

        // 糸を縫う処理
        inputActions.Stirng.start.performed += ctx =>
        {
            if (currentIndex >= StringNum.Count)
            {
                return;
            }
            if (PauseApperance.Instance.isPause|| (SoundChangeSlider.Instance != null && SoundChangeSlider.Instance.IsSoundChange)) return;//ポーズ中は操作できないようにする
            if (m_StringMode == isString || currentIndex >= StringNum.Count) return;

            RectTransform dummy = new GameObject("FirstPoint", typeof(RectTransform)).GetComponent<RectTransform>();
            dummy.SetParent(canvasTransform, false);
            dummy.anchoredPosition = StringCursol.anchoredPosition;
            Debug.Log($"CursorPos: {StringCursol.anchoredPosition}");
            Strings.Add(dummy);
            m_StringMode = isString;
        };

        inputActions.Stirng.BackString.performed += ctx =>// 糸の一針戻す操作
        {
            RemoveLastStitch();
            listDisplay.UpdateDisplay(StringNum); // UI表示を更新
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
        while (currentIndex > 0 && StringNum[currentIndex - 1] <= 0)
        {
            Debug.Log("インデックス増やすよ");
            currentIndex--;
            Destroy(Tamadomes[^1].gameObject);
            Tamadomes.RemoveAt(Tamadomes.Count - 1);
            m_StringMode = isString;

        }
        // 糸が存在しない or FirstPointしかない場合は何もしない
        if (Strings.Count <= 1)
        {
            Debug.LogWarning("削除できる糸がありません。");
            m_StringMode = NoString;
            return;
        }

        int removeCount = Mathf.Min(count, MirrorStrings.Count);

        for (int i = 0; i < removeCount; i++)
        {
            int index = MirrorStrings.Count - 1;
            if (index < 0) break;

            Vector2 resumePos = (Strings.Count > 1 && Strings[^2] != null)
           ? BackStrings[^1].anchoredPosition
           : Vector2.zero;

            // --- アニメ関連を安全に削除 ---
            if (AnimStrings.Count > 0 && AnimStrings[^1] != null)
            {
                AnimStrings[^1].DeleteImage(0);
                Destroy(AnimStrings[^1].gameObject);
                AnimStrings.RemoveAt(AnimStrings.Count - 1);
            }

            if (MirrorAnimStrings.Count > 0 && MirrorAnimStrings[^1] != null)
            {
                MirrorAnimStrings[^1].DeleteImage(0);
                Destroy(MirrorAnimStrings[^1].gameObject);
                MirrorAnimStrings.RemoveAt(MirrorAnimStrings.Count - 1);
            }

            // --- 残りの削除処理 ---
            if (Strings.Count > 0 && Strings[^1] != null)
                Destroy(Strings[^1].gameObject);
            if (MirrorStrings.Count > 0 && MirrorStrings[^1] != null)
                Destroy(MirrorStrings[^1].gameObject);
            if (FrontStrings.Count > 0 && FrontStrings[^1] != null)
                Destroy(FrontStrings[^1].gameObject);
            if (BackStrings.Count > 0 && BackStrings[^1] != null)
                Destroy(BackStrings[^1].gameObject);

            // リスト削除
            if (Strings.Count > 0) Strings.RemoveAt(Strings.Count - 1);
            if (MirrorStrings.Count > 0) MirrorStrings.RemoveAt(MirrorStrings.Count - 1);
            if (FrontStrings.Count > 0) FrontStrings.RemoveAt(FrontStrings.Count - 1);
            if (BackStrings.Count > 0) BackStrings.RemoveAt(BackStrings.Count - 1);

            // null除去
            Strings.RemoveAll(s => s == null);
            MirrorStrings.RemoveAll(s => s == null);
            FrontStrings.RemoveAll(s => s == null);
            BackStrings.RemoveAll(s => s == null);

            StringCursol.anchoredPosition = resumePos;

            // 糸数を戻す
            if (currentIndex >= 0 && currentIndex < StringNum.Count)
                StringNum[currentIndex]++;


            // 縫い方向を初期化
            switch (m_LastDirection)
            {
                case RIGHT:
                  StageWidth--;
                  break;
                case LEFT:
                  StageWidth++;
                  break;
                case UP:
                  StageHeight++;
                  break;
                case DOWN:
                  StageHeight--;
                  break;
            }
            Directions.RemoveAt(Directions.Count - 1);
            if (Directions.Count > 0)
            {
                m_LastDirection = Directions[Directions.Count - 1];
            }
            else
            {
                m_LastDirection = First;
            }
        }

    }

    void Start()
    {
        m_Offset_X = new Vector2(m_StrinngScale.x, 0f);
        m_Offset_Y = new Vector2(0f, -m_StrinngScale.y);

        StringNum = new List<int>(StageUILoader.stage.STRING_COUNT); // ステージの糸数情報を取得

        listDisplay.UpdateDisplay(StringNum); // UI表示を更新
        CopyStringNum = new List<int>(StringNum);
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
    public void CutString(int index, bool front)
    {
        if (front)
        {
            Destroy(Strings[index + 1].gameObject); // FirstPointとの対応で+1
            AnimStrings[index].DeleteImage(0);
            Strings.RemoveAt(index);
            AnimStrings.RemoveAt(index);
        }
        else
        {
            Destroy(MirrorStrings[index].gameObject);
            MirrorAnimStrings[index].DeleteImage(0);
            MirrorStrings.RemoveAt(index);
            MirrorAnimStrings.RemoveAt(index);
        }
        Destroy(FrontStrings[index].gameObject);
        Destroy(BackStrings[index].gameObject);

        FrontStrings.RemoveAt(index);
        BackStrings.RemoveAt(index);
    }

    // 各方向への糸設置処理
    void OnRightInput()
    {
        if (m_LastDirection == LEFT) return;
        m_PreDirection=m_LastDirection;//直前の方向を保存
        Vector2 lastPos = Strings[^1].anchoredPosition;
        Vector2 newPos = lastPos + m_Offset_X;

        if (m_LastDirection == UP) newPos = lastPos + m_Offset_X / 2 - m_Offset_Y / 2;
        else if (m_LastDirection == DOWN) newPos = lastPos + m_Offset_X / 2 + m_Offset_Y / 2;
        else if (m_LastDirection == First) newPos = lastPos + m_Offset_X/2;

            Vector2 frontPos = newPos + m_Offset_X / 2;
        Vector2 backPos = newPos - m_Offset_X / 2;

        if (CheckString(newPos, frontPos, backPos) && StageWidth < StageUILoader.stage.STAGE_WIDTH)
        {
            AddString(newPos, frontPos, backPos, Quaternion.identity);
            m_LastDirection = RIGHT;
            Directions.Add(m_LastDirection);
            StageWidth++;
            StringNum[currentIndex]--;
            StringCursol.anchoredPosition += m_Offset_X;
        }
    }

    public void ShowCutter() => Cutter.SetActive(true);
    public void DeleteCutter() => Cutter.SetActive(false);

    void OnLeftInput()
    {
        if (m_LastDirection == RIGHT) return;

        m_PreDirection = m_LastDirection;//直前の方向を保存

        Vector2 lastPos = Strings[^1].anchoredPosition;
        Vector2 newPos = lastPos - m_Offset_X;

        if (m_LastDirection == UP) newPos = lastPos - m_Offset_X / 2 - m_Offset_Y / 2;
        else if (m_LastDirection == DOWN) newPos = lastPos - m_Offset_X / 2 + m_Offset_Y / 2;
        else if (m_LastDirection == First) newPos= lastPos - m_Offset_X / 2;

        Vector2 frontPos = newPos - m_Offset_X / 2;
        Vector2 backPos = newPos + m_Offset_X / 2;

        if (CheckString(newPos, frontPos, backPos) && StageWidth > 0)
        {
            AddString(newPos, frontPos, backPos, Quaternion.Euler(0, 180, 0));
            m_LastDirection = LEFT;
            Directions.Add(m_LastDirection);
            StageWidth--;
            StringNum[currentIndex]--;
            StringCursol.anchoredPosition -= m_Offset_X;
        }
    }

    void OnUpInput()
    {
        if (m_LastDirection == DOWN) return;

        m_PreDirection = m_LastDirection;//直前の方向を保存

        Vector2 lastPos = Strings[^1].anchoredPosition;
        Vector2 newPos = lastPos;

        if (m_LastDirection == RIGHT) newPos = lastPos + m_Offset_X / 2 - m_Offset_Y / 2;
        else if (m_LastDirection == LEFT) newPos = lastPos - m_Offset_X / 2 - m_Offset_Y / 2;
        else if (m_LastDirection == UP) newPos = lastPos - m_Offset_Y;
        else if (m_LastDirection == First) newPos= lastPos - m_Offset_Y / 2;
        Vector2 frontPos = newPos - m_Offset_Y / 2;
        Vector2 backPos = newPos + m_Offset_Y / 2;

        if (CheckString(newPos, frontPos, backPos) && StageHeight > 0)
        {
            AddString(newPos, frontPos, backPos, Quaternion.Euler(0, 0, 90));
            m_LastDirection = UP;
            Directions.Add(m_LastDirection);
            StageHeight--;
            StringNum[currentIndex]--;
            StringCursol.anchoredPosition -= m_Offset_Y;
        }
    }

    void OnDownInput()
    {
        if (m_LastDirection == UP) return;

        m_PreDirection = m_LastDirection;//直前の方向を保存

        Vector2 lastPos = Strings[^1].anchoredPosition;
        Vector2 newPos = lastPos;

        if (m_LastDirection == RIGHT) newPos = lastPos + m_Offset_X / 2 + m_Offset_Y / 2;
        else if (m_LastDirection == LEFT) newPos = lastPos - m_Offset_X / 2 + m_Offset_Y / 2;
        else if (m_LastDirection == DOWN) newPos = lastPos + m_Offset_Y;
        else if (m_LastDirection == First) newPos= lastPos + m_Offset_Y / 2;

        Vector2 frontPos = newPos + m_Offset_Y / 2;
        Vector2 backPos = newPos - m_Offset_Y / 2;

        if (CheckString(newPos, frontPos, backPos) && StageHeight < StageUILoader.stage.STAGE_HEIGHT)
        {
            AddString(newPos, frontPos, backPos, Quaternion.Euler(0, 0, 270));
            m_LastDirection = DOWN;
            Directions.Add(m_LastDirection);
            StageHeight++;//いったん消します
            StringNum[currentIndex]--;
            StringCursol.anchoredPosition += m_Offset_Y;
        }
    }

    // 返し縫いを作成する(没)
    void OnKaesiInput()
    {
        Vector3 lastPos = Strings[^1].transform.position;
        Vector3 newPos = Vector3.zero;
        RectTransform obj = new RectTransform();
        Animator animator = new Animator();

        switch (m_LastDirection)
        {
            case RIGHT:
                newPos = lastPos - (Vector3)m_Offset_Y / 10;
                obj = Instantiate(StringPrefub, newPos, Quaternion.identity);
                obj.transform.rotation = Quaternion.Euler(0, 180, 0);
                obj.tag = "Kaesi";
                animator = obj.GetComponent<Animator>();
                animator.SetTrigger("Play");
                newPos = lastPos;

                obj = Instantiate(StringPrefub, newPos, Quaternion.identity);
                Strings.Add(obj);
                m_LastDirection = LEFT;
                break;

            case LEFT:
                newPos = lastPos - (Vector3)m_Offset_Y / 10;
                obj = Instantiate(StringPrefub, newPos, Quaternion.identity);
                obj.tag = "Kaesi";
                animator = obj.GetComponent<Animator>();
                animator.SetTrigger("Play");
                newPos = lastPos;

                obj = Instantiate(StringPrefub, newPos, Quaternion.identity);
                Strings.Add(obj);
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
        RectTransform mainStr = Instantiate(StringPrefub, canvasTransform);
        mainStr.anchoredPosition = main;
        mainStr.sizeDelta = m_StrinngScale; // サイズ設定
        mainStr.rotation = rot;
        mainStr.GetComponent<Animator>()?.SetTrigger("Play");

        StringAnimation_Canvas anim = mainStr.GetComponent<StringAnimation_Canvas>();
        if (anim != null)
        {
            anim.SetCanvas(canvasTransform);
            anim.index = Strings.Count - 1; // FirstPoint対応で-1
            anim.front = true;
        }
        AnimStrings.Add(anim);

        BoxCollider2D col = mainStr.GetComponent<BoxCollider2D>();
        if (col != null) col.size *= HitBoxScale;
        Strings.Add(mainStr);

        // 反対側の糸も生成
        Vector3 mirrorPos = main;
        float mirrorCenterX = 0.0f;
        mirrorPos.x = mirrorCenterX - (main.x - mirrorCenterX);
        RectTransform mirrorStr = Instantiate(StringPrefub, canvasTransform);
        mirrorStr.anchoredPosition = mirrorPos;
        mirrorStr.sizeDelta = m_StrinngScale;
        mirrorStr.rotation = rot;
        if (Mathf.Abs(rot.y) > 0.5f)
            mirrorStr.rotation *= Quaternion.Euler(0, 180f, 0);

        mirrorStr.GetComponent<Animator>()?.SetTrigger("Play");
        anim = mirrorStr.GetComponent<StringAnimation_Canvas>();
        if (anim != null)
        {
            anim.SetCanvas(canvasTransform);
            anim.index = MirrorStrings.Count;
            anim.front = false;
        }
        MirrorAnimStrings.Add(anim);
        col = mirrorStr.GetComponent<BoxCollider2D>();
        if (col != null) col.size *= HitBoxScale;
        MirrorStrings.Add(mirrorStr);

        // 前後に設置する補助パーツ（接触判定無効）縫えない判定のため使用
        RectTransform frontStr = Instantiate(StringPrefub, canvasTransform);
        frontStr.sizeDelta = m_StrinngScale;
        frontStr.anchoredPosition = front;
        anim = frontStr.GetComponent<StringAnimation_Canvas>();
        if (anim != null) anim.SetCanvas(canvasTransform);
        col = frontStr.GetComponent<BoxCollider2D>();
        if (col != null)
        {
            col.size = Vector2.zero;
            col.isTrigger = true;
        }
        FrontStrings.Add(frontStr);

        RectTransform backStr = Instantiate(StringPrefub, canvasTransform);
        backStr.sizeDelta = m_StrinngScale;
        backStr.anchoredPosition = back;
        anim = backStr.GetComponent<StringAnimation_Canvas>();
        if (anim != null) anim.SetCanvas(canvasTransform);
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
        foreach (var str in Strings)
        {
            if (str == null || !str) continue;
            if (Vector2.Distance(str.anchoredPosition, front) < 0.001f) return false;
        }

        foreach (var str in FrontStrings)
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
        Vector2 lastPos = Strings[^1].anchoredPosition;
        Vector2 newPos = lastPos;

        switch (m_LastDirection)
        {
            case RIGHT: newPos += m_Offset_X / 2; break;
            case LEFT: newPos -= m_Offset_X / 2; break;
            case UP: newPos -= m_Offset_Y / 2; break;
            case DOWN: newPos += m_Offset_Y / 2; break;
        }

        RectTransform tama = Instantiate(Tamadome, canvasTransform);
        tama.sizeDelta = m_StrinngScale;
        tama.anchoredPosition = newPos;
        Tamadomes.Add(tama);


        m_StringMode = NoString;
        m_LastDirection = First;
    }

    // 糸のサイズと当たり判定スケールを設定
    public void SetStringSize(Vector2 size, Vector2 BoxScale)
    {
        m_StrinngScale = size;
        HitBoxScale = BoxScale;
    }

    public void SetCursor(Vector2 pos)
    {
        StringCursol.anchoredPosition = pos;
    }
}

