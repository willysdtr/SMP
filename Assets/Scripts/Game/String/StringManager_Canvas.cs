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
    [SerializeField] List<int> CopyStringNum;
    [SerializeField] private GameObject Cutter;               // 糸を切るためのカッターオブジェクト
    private int currentIndex = 0;

    [SerializeField] private ShowStringNum listDisplay;       // 糸の数などを表示するUI管理クラス

    private InputSystem_Actions inputActions;
    private float m_PauseDirection;
    private int m_LastDirection;
    private bool m_StringMode = NoString;

    private int StageWidth = 0;
    private int StageHeight = 0;
    public int CutNum = 0;

    void Awake()
    {
        inputActions = new InputSystem_Actions();

        // 糸の縫い操作
        inputActions.Stirng.nami.performed += ctx =>
        {
            float value = ctx.ReadValue<float>();

            if (m_StringMode == isString)
            {
                // 糸縫いモード時の方向操作
                m_PauseDirection = value;
                if (m_PauseDirection == 1) OnUpInput();
                else if (m_PauseDirection == -1) OnDownInput();
                else if (m_PauseDirection == 2) OnRightInput();
                else if (m_PauseDirection == 3) OnLeftInput();
            }
            else
            {
                // 非糸縫いモード時のカーソル移動
                Vector2 offset = Vector2.zero;
                if (value == 1) offset = -m_Offset_Y;
                else if (value == -1) offset = m_Offset_Y;
                else if (value == 2) offset = m_Offset_X;
                else if (value == 3) offset = -m_Offset_X;

                StringCursol.anchoredPosition += offset;
            }
        };

        // 玉止め（糸の終端）設置操作
        inputActions.Stirng.tama.performed += ctx =>
        {
            if (Strings.Count > 0) BallStopper();
        };

        // 糸を縫う処理
        inputActions.Stirng.start.performed += ctx =>
        {
            if (m_StringMode == isString || currentIndex >= StringNum.Count) return;

            RectTransform dummy = new GameObject("FirstPoint", typeof(RectTransform)).GetComponent<RectTransform>();
            dummy.SetParent(canvasTransform, false);
            dummy.anchoredPosition = StringCursol.anchoredPosition;
            Strings.Add(dummy);
            m_StringMode = isString;
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

        // 糸を切る操作
        inputActions.Stirng.cutstring.performed += ctx =>
        {
            if (MirrorStrings.Count > 0)
            {
                CutString(MirrorStrings.Count - 1);
            }
            else
            {
                Debug.LogWarning("カットできるStringが存在しません。");
            }
        };
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
    public void CutString(int index)
    {
<<<<<<< HEAD
        // ���̂��폜
        Destroy(MirrorStrings[index].gameObject);
        Destroy(Strings[index + 1].gameObject);//FirstPoint�̊֌W��+1����
=======
        Destroy(MirrorStrings[index].gameObject);
        Destroy(Strings[index + 1].gameObject); // FirstPointとの対応で+1
>>>>>>> origin/Work_Taniguchi4
        Destroy(FrontStrings[index].gameObject);
        Destroy(BackStrings[index].gameObject);

        AnimStrings[index].DeleteImage(0);
        MirrorAnimStrings[index].DeleteImage(0);
<<<<<<< HEAD
        // ���X�g������폜
=======

>>>>>>> origin/Work_Taniguchi4
        MirrorStrings.RemoveAt(index);
        Strings.RemoveAt(index);
        FrontStrings.RemoveAt(index);
        BackStrings.RemoveAt(index);
        AnimStrings.RemoveAt(index);
        MirrorAnimStrings.RemoveAt(index);
    }

    // 各方向への糸設置処理
    void OnRightInput()
    {
        if (m_LastDirection == LEFT) return;

        Vector2 lastPos = Strings[^1].anchoredPosition;
        Vector2 newPos = lastPos + m_Offset_X;

        if (m_LastDirection == UP) newPos = lastPos + m_Offset_X / 2 - m_Offset_Y / 2;
        else if (m_LastDirection == DOWN) newPos = lastPos + m_Offset_X / 2 + m_Offset_Y / 2;

        Vector2 frontPos = newPos + m_Offset_X / 2;
        Vector2 backPos = newPos - m_Offset_X / 2;

        if (CheckString(newPos, frontPos, backPos) && StageWidth < StageUILoader.stage.STAGE_WIDTH)
        {
            AddString(newPos, frontPos, backPos, Quaternion.identity);
            m_LastDirection = RIGHT;
            StageWidth++;
        }
    }

    public void ShowCutter() => Cutter.SetActive(true);
    public void DeleteCutter() => Cutter.SetActive(false);

    void OnLeftInput()
    {
        if (m_LastDirection == RIGHT) return;

        Vector2 lastPos = Strings[^1].anchoredPosition;
        Vector2 newPos = lastPos - m_Offset_X;

        if (m_LastDirection == UP) newPos = lastPos - m_Offset_X / 2 - m_Offset_Y / 2;
        else if (m_LastDirection == DOWN) newPos = lastPos - m_Offset_X / 2 + m_Offset_Y / 2;

        Vector2 frontPos = newPos - m_Offset_X / 2;
        Vector2 backPos = newPos + m_Offset_X / 2;

        if (CheckString(newPos, frontPos, backPos) && StageWidth > 0)
        {
            AddString(newPos, frontPos, backPos, Quaternion.Euler(0, 180, 0));
            m_LastDirection = LEFT;
            StageWidth--;
        }
    }

    void OnUpInput()
    {
        if (m_LastDirection == DOWN) return;

        Vector2 lastPos = Strings[^1].anchoredPosition;
        Vector2 newPos = lastPos;

        if (m_LastDirection == RIGHT) newPos = lastPos + m_Offset_X / 2 - m_Offset_Y / 2;
        else if (m_LastDirection == LEFT) newPos = lastPos - m_Offset_X / 2 - m_Offset_Y / 2;
        else if (m_LastDirection == UP) newPos = lastPos - m_Offset_Y;

        Vector2 frontPos = newPos - m_Offset_Y / 2;
        Vector2 backPos = newPos + m_Offset_Y / 2;

        if (CheckString(newPos, frontPos, backPos) && StageHeight > 0)
        {
            AddString(newPos, frontPos, backPos, Quaternion.Euler(0, 0, 90));
            m_LastDirection = UP;
            StageHeight--;
        }
    }

    void OnDownInput()
    {
        if (m_LastDirection == UP) return;

        Vector2 lastPos = Strings[^1].anchoredPosition;
        Vector2 newPos = lastPos;

        if (m_LastDirection == RIGHT) newPos = lastPos + m_Offset_X / 2 + m_Offset_Y / 2;
        else if (m_LastDirection == LEFT) newPos = lastPos - m_Offset_X / 2 + m_Offset_Y / 2;
        else if (m_LastDirection == DOWN) newPos = lastPos + m_Offset_Y;

        Vector2 frontPos = newPos + m_Offset_Y / 2;
        Vector2 backPos = newPos - m_Offset_Y / 2;

        if (CheckString(newPos, frontPos, backPos) && StageHeight < StageUILoader.stage.STAGE_HEIGHT)
        {
            AddString(newPos, frontPos, backPos, Quaternion.Euler(0, 0, 270));
            m_LastDirection = DOWN;
            StageHeight++;
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

<<<<<<< HEAD
       void AddString(Vector2 main, Vector2 front, Vector2 back, Quaternion rot)
       {
       RectTransform mainStr = Instantiate(StringPrefub, canvasTransform);
       mainStr.anchoredPosition = main;
       mainStr.sizeDelta = m_StrinngScale;//�T�C�Y�ύX
       RectTransform childstr = mainStr.GetComponentsInChildren<RectTransform>()[0];
       mainStr.rotation = rot;
       mainStr.GetComponent<Animator>()?.SetTrigger("Play");
       StringAnimation_Canvas anim = mainStr.GetComponent<StringAnimation_Canvas>();
       if (anim != null)
       {
           anim.SetCanvas(canvasTransform);
           anim.index = Strings.Count - 1;//FirstPoint�̊֌W��-1����
        }
       AnimStrings.Add(anim);
       BoxCollider2D col = mainStr.GetComponent<BoxCollider2D>();
       if (col != null)
       {
           col.size *= HitBoxScale; // RectTransform�ɍ��킹�Ċg�k
       }
       Strings.Add(mainStr);

        Vector3 mirrorPos = main;
        float mirrorCenterX = 0.0f;
        mirrorPos.x = mirrorCenterX - (main.x - mirrorCenterX);
        RectTransform mirrorStr = Instantiate(StringPrefub, canvasTransform);
        mirrorStr.anchoredPosition = mirrorPos;
        mirrorStr.sizeDelta = m_StrinngScale;//�T�C�Y�ύX
        mirrorStr.rotation = rot;
        if (Mathf.Abs(rot.y) > 0.5f)//�c�̏ꍇ�͔��]�����Ȃ�
        {
            mirrorStr.rotation *= Quaternion.Euler(0, 180f, 0);// ���̉�] rot �ɑ΂��� Y����180�x���]��ǉ�����
        }

        //mirrorStr.tag = "Nami_Mirror";
        Animator mirrorAnimator = mirrorStr.GetComponent<Animator>();
        mirrorStr.GetComponent<Animator>()?.SetTrigger("Play");
        anim = mirrorStr.GetComponent<StringAnimation_Canvas>();
        if (anim != null)
        {
            anim.SetCanvas(canvasTransform);
            anim.index = MirrorStrings.Count;
        }
        MirrorAnimStrings.Add(anim);
        col = mirrorStr.GetComponent<BoxCollider2D>();
        if (col != null)
        {
            col.size *= HitBoxScale; // RectTransform�ɍ��킹�Ċg�k
        }
        MirrorStrings.Add(mirrorStr);

        //�D���Ȃ�����p�̎��A�O��
        RectTransform frontStr = Instantiate(StringPrefub, canvasTransform);
        frontStr.sizeDelta = m_StrinngScale;//�T�C�Y�ύX
        frontStr.anchoredPosition = front;
        anim = frontStr.GetComponent<StringAnimation_Canvas>();
        if (anim != null)
        {
            anim.SetCanvas(canvasTransform);
        }
        col = frontStr.GetComponent<BoxCollider2D>();
        if (col != null)
        {
            col.size = new Vector2(0, 0); // �����蔻��𖳂���
        }
        FrontStrings.Add(frontStr);

        //�D���Ȃ�����p�̎��A���
        RectTransform backStr = Instantiate(StringPrefub, canvasTransform);
        backStr.sizeDelta = m_StrinngScale;//�T�C�Y�ύX
        backStr.anchoredPosition = back;
        anim = backStr.GetComponent<StringAnimation_Canvas>();
        if (anim != null)
        {
            anim.SetCanvas(canvasTransform);
        }
        col = backStr.GetComponent<BoxCollider2D>();
        if (col != null)
        {
            col.size = new Vector2(0, 0); // �����蔻��𖳂���
        }
        BackStrings.Add(backStr);

        //�����蔻��
        //AddColliderToPrefab(mainStr);
        //AddColliderToPrefab(mirrorStr);
        //AddColliderToPrefab(frontStr);
        //AddColliderToPrefab(backStr);
    }


    void AddColliderToPrefab(RectTransform targetRect)//BoxCollider2D���g���s���㌋�ǂ���Ă邱�ƒJ���ƕς���
=======
    // 糸を生成・配置する（表裏と前後パーツ）
    void AddString(Vector2 main, Vector2 front, Vector2 back, Quaternion rot)
>>>>>>> origin/Work_Taniguchi4
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
        foreach (var str in Strings)
            if (Vector2.Distance(str.anchoredPosition, front) < 0.001f) return false;
        foreach (var str in FrontStrings)
            if (Vector2.Distance(str.anchoredPosition, front) < 0.001f) return false;
        foreach (var str in BackStrings)
            if (Vector2.Distance(str.anchoredPosition, front) < 0.001f) return false;
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

        m_StringMode = NoString;
        m_LastDirection = RIGHT;
    }

    // 糸のサイズと当たり判定スケールを設定
    public void SetStringSize(Vector2 size, Vector2 BoxScale)
    {
        m_StrinngScale = size;
        HitBoxScale = BoxScale;
    }
}
