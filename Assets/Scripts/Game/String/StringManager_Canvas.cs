using StageInfo;
using System;
using System.Collections.Generic;
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

    [SerializeField] private RectTransform StringPrefub; // UI用にRectTransformへ変更
    [SerializeField] private RectTransform Tamadome;//  どーやって玉止めすんねん
    [SerializeField] private RectTransform StringCursol;
    [SerializeField] private RectTransform canvasTransform; // CanvasのRectTransform

    [SerializeField] private float mirrorOffsetX = 5.0f;
    private Vector2 m_StrinngScale = new Vector2(100f, 100f); // UIサイズに合わせて単位変更
    private Vector2 HitBoxScale = new(1, 1);
    private Vector2 m_Offset_X;
    private Vector2 m_Offset_Y;

    private List<RectTransform> Strings = new List<RectTransform>();
    private List<RectTransform> MirrorStrings = new List<RectTransform>();
    private List<RectTransform> FrontStrings = new List<RectTransform>();
    private List<RectTransform> BackStrings = new List<RectTransform>();
    private List<StringAnimation_Canvas> AnimStrings = new List<StringAnimation_Canvas>();
    private List<StringAnimation_Canvas> MirrorAnimStrings = new List<StringAnimation_Canvas>();
    [SerializeField] List<int> StringNum;
    [SerializeField] List<int> CopyStringNum;
    [SerializeField] private GameObject Cutter; // 最大数を設定
    private int currentIndex = 0;

    [SerializeField] private ShowStringNum listDisplay; // 表示クラスをインスペクターでセット

    private InputSystem_Actions inputActions;
    private float m_PauseDirection;
    private int m_LastDirection;
    private bool m_StringMode = NoString;

    private int StageWidth = 0;
    private int StageHeight = 0;
    private StageData stage;
    public int CutNum = 0;

    //[SerializeField] public  BoxCollider2D stageCollider;

    //StringAnimation_Canvas anim;



    void Awake()
    {
        inputActions = new InputSystem_Actions();

        inputActions.Stirng.nami.performed += ctx =>
        {
            float value = ctx.ReadValue<float>();

            if (m_StringMode == isString)
            {
                m_PauseDirection = value;
                if (m_PauseDirection == 1) OnUpInput();
                else if (m_PauseDirection == -1) OnDownInput();
                else if (m_PauseDirection == 2) OnRightInput();
                else if (m_PauseDirection == 3) OnLeftInput();
            }
            else
            {
                Vector2 offset = Vector2.zero;
                if (value == 1) offset = -m_Offset_Y;
                else if (value == -1) offset = m_Offset_Y;
                else if (value == 2) offset = m_Offset_X;
                else if (value == 3) offset = -m_Offset_X;

                StringCursol.anchoredPosition += offset;
            }
        };

        inputActions.Stirng.tama.performed += ctx =>
        {
            if (Strings.Count > 0) BallStopper();
        };

        inputActions.Stirng.start.performed += ctx =>
        {
            if (m_StringMode == isString || currentIndex >= StringNum.Count) return;
            RectTransform dummy = new GameObject("FirstPoint", typeof(RectTransform)).GetComponent<RectTransform>();
            dummy.SetParent(canvasTransform, false);
            dummy.anchoredPosition = StringCursol.anchoredPosition;
            Strings.Add(dummy);
            m_StringMode = isString;
        };

        inputActions.Stirng.kaesi.performed += ctx =>
        {

            if (m_StringMode == NoString || currentIndex >= StringNum.Count/* || StringNum[currentIndex] == CopyStringNum[currentIndex]*/)
            {
                Debug.Log(CopyStringNum[currentIndex]);
                Debug.Log(StringNum[currentIndex]);
                return;
            }
            OnKaesiInput();
        };

        inputActions.Stirng.cutstring.performed += ctx =>
        {
            if (CutNum > 0)
            {
                CutString(0);
                CutNum--;
            }
        };
        //inputActions.Stirng.test.performed += ctx =>
        //{
        //    Cutter.SetActive(true);

        //};

    }

    void Start()
    {
        m_Offset_X = new Vector2(m_StrinngScale.x, 0f);
        m_Offset_Y = new Vector2(0f, -m_StrinngScale.y);
        listDisplay.UpdateDisplay(StringNum);// Text表示を更新
        CopyStringNum = new List<int>(StringNum);
        //StringCursol.anchoredPosition=stage.START_POS_front.ToVector2()*m_StrinngScale;グリッドの右端とれるならこれで
    }

    void OnEnable()
    {
        inputActions.Stirng.Enable();
    }

    void OnDisable()
    {
        inputActions.Stirng.Disable();
    }

    void CutString(int index)
    {
        // 実体を削除
        Destroy(MirrorStrings[index].gameObject);
        Destroy(Strings[index].gameObject);
        Destroy(FrontStrings[index].gameObject);
        Destroy(BackStrings[index].gameObject);

        AnimStrings[index].DeleteImage(0);
        MirrorAnimStrings[index].DeleteImage(0);
        // リストからも削除
        MirrorStrings.RemoveAt(index);
        Strings.RemoveAt(index);
        FrontStrings.RemoveAt(index);
        BackStrings.RemoveAt(index);
        AnimStrings.RemoveAt(index);
        MirrorAnimStrings.RemoveAt(index);
    }
    void OnRightInput()
    {
        if (m_LastDirection == LEFT) return;
        Vector2 lastPos = Strings[^1].anchoredPosition;
        Vector2 newPos = lastPos + m_Offset_X;

        if (m_LastDirection == UP) newPos = lastPos + m_Offset_X / 2 - m_Offset_Y / 2;
        else if (m_LastDirection == DOWN) newPos = lastPos + m_Offset_X / 2 + m_Offset_Y / 2;

        Vector2 frontPos = newPos + m_Offset_X / 2;
        Vector2 backPos = newPos - m_Offset_X / 2;

        if (CheckString(newPos, frontPos, backPos) && StageWidth < StageUICanvasLoader2.stage.STAGE_WIDTH)
        {

            AddString(newPos, frontPos, backPos, Quaternion.identity);
            m_LastDirection = RIGHT;
            StageWidth++;
        }
    }
    public void ShowCutter()
    {
        Cutter.SetActive(true);
    }
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

        if (CheckString(newPos, frontPos, backPos) && StageHeight < StageUICanvasLoader2.stage.STAGE_HEIGHT)
        {
            AddString(newPos, frontPos, backPos, Quaternion.Euler(0, 0, 270));
            m_LastDirection = DOWN;
            StageHeight++;
        }
    }

    void OnKaesiInput()
    {
        Vector3 lastPos = Strings[^1].transform.position;
        Vector3 newPos = new Vector3(0.0f, 0.0f, 0.0f);
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
                return;
            case DOWN:
                return;
        }
    }

    void AddString(Vector2 main, Vector2 front, Vector2 back, Quaternion rot)
    {
        RectTransform mainStr = Instantiate(StringPrefub, canvasTransform);
        mainStr.anchoredPosition = main;
        mainStr.sizeDelta = m_StrinngScale;//サイズ変更
        RectTransform childstr = mainStr.GetComponentsInChildren<RectTransform>()[0];
        mainStr.rotation = rot;
        mainStr.GetComponent<Animator>()?.SetTrigger("Play");
        StringAnimation_Canvas anim = mainStr.GetComponent<StringAnimation_Canvas>();
        if (anim != null)
        {
            anim.SetCanvas(canvasTransform);
        }
        AnimStrings.Add(anim);
        BoxCollider2D col = mainStr.GetComponent<BoxCollider2D>();
        if (col != null)
        {
            col.size *= HitBoxScale; // RectTransformに合わせて拡縮
        }
        Strings.Add(mainStr);

        Vector3 mirrorPos = main;
        float mirrorCenterX = 0.0f;
        mirrorPos.x = mirrorCenterX - (main.x - mirrorCenterX);
        RectTransform mirrorStr = Instantiate(StringPrefub, canvasTransform);
        mirrorStr.anchoredPosition = mirrorPos;
        mirrorStr.sizeDelta = m_StrinngScale;//サイズ変更
        mirrorStr.rotation = rot;
        if (Mathf.Abs(rot.y) > 0.5f)//縦の場合は反転させない
        {
            mirrorStr.rotation *= Quaternion.Euler(0, 180f, 0);// 元の回転 rot に対して Y軸に180度反転を追加する
        }

        //mirrorStr.tag = "Nami_Mirror";
        Animator mirrorAnimator = mirrorStr.GetComponent<Animator>();
        mirrorStr.GetComponent<Animator>()?.SetTrigger("Play");
        anim = mirrorStr.GetComponent<StringAnimation_Canvas>();
        if (anim != null)
        {
            anim.SetCanvas(canvasTransform);
        }
        MirrorAnimStrings.Add(anim);
        //col = mirrorStr.GetComponent<BoxCollider2D>();
        //if (col != null)
        //{
        //    col.size *= HitBoxScale; // RectTransformに合わせて拡縮
        //}
        MirrorStrings.Add(mirrorStr);

        RectTransform frontStr = Instantiate(StringPrefub, canvasTransform);
        frontStr.sizeDelta = m_StrinngScale;//サイズ変更
        frontStr.anchoredPosition = front;
        anim = frontStr.GetComponent<StringAnimation_Canvas>();
        if (anim != null)
        {
            anim.SetCanvas(canvasTransform);
        }
        //col = frontStr.GetComponent<BoxCollider2D>();
        //if (col != null)
        //{
        //    col.size *= HitBoxScale; // RectTransformに合わせて拡縮
        //}
        FrontStrings.Add(frontStr);

        RectTransform backStr = Instantiate(StringPrefub, canvasTransform);
        backStr.sizeDelta = m_StrinngScale;//サイズ変更
        backStr.anchoredPosition = back;
        anim = backStr.GetComponent<StringAnimation_Canvas>();
        if (anim != null)
        {
            anim.SetCanvas(canvasTransform);
        }
        //col = backStr.GetComponent<BoxCollider2D>();
        //if (col != null)
        //{
        //    col.size *= HitBoxScale; // RectTransformに合わせて拡縮
        //}
        BackStrings.Add(backStr);
        //当たり判定
        AddColliderToPrefab(mainStr);
        AddColliderToPrefab(mirrorStr);
        AddColliderToPrefab(frontStr);
        AddColliderToPrefab(backStr);
    }


    void AddColliderToPrefab(RectTransform targetRect)//BoxCollider2Dを使う都合上結局やってること谷口と変わらん
    {
        //if (stageCollider == null) return;

        // 新しいオブジェクトを生成（Colliderだけ持つ）
        GameObject colObj = new GameObject("StringCollider", typeof(BoxCollider2D));
        colObj.transform.SetParent(targetRect, false);

        // RectTransformに合わせて位置とサイズを調整
        BoxCollider2D col = colObj.GetComponent<BoxCollider2D>();

        // UIの中心を基準にコライダーを置く
        col.offset = Vector2.zero;

        // サイズはUIのサイズにスケールを掛けて変換
        Vector2 size = targetRect.sizeDelta / 2;
        size.x *= targetRect.lossyScale.x / 2;
        size.y *= targetRect.lossyScale.y / 2;
        col.size = size / 2;
    }
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
        tama.sizeDelta = m_StrinngScale;//サイズ変更
        tama.anchoredPosition = newPos;

        m_StringMode = NoString;
        m_LastDirection = RIGHT;
    }

    public void SetStringSize(Vector2 size, Vector2 BoxScale)
    {
        m_StrinngScale = size; // UIサイズに合わせて単位変更
        HitBoxScale = BoxScale;
    }
}

