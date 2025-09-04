using System;
using System.Collections.Generic;
using TMPro;
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

    [SerializeField] private RectTransform StringPrefub; // UI�p��RectTransform�֕ύX
    [SerializeField] private RectTransform Tamadome;
    [SerializeField] private RectTransform StringCursol;
    [SerializeField] private RectTransform canvasTransform; // Canvas��RectTransform

    [SerializeField] private float mirrorOffsetX = 5.0f;
    private Vector2 m_StrinngScale = new Vector2(100f, 100f); // UI�T�C�Y�ɍ��킹�ĒP�ʕύX
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

    [SerializeField] private ShowStringNum listDisplay; // �\���N���X���C���X�y�N�^�[�ŃZ�b�g

    private InputSystem_Actions inputActions;
    private float m_PauseDirection;
    private int m_LastDirection;
    private bool m_StringMode = NoString;

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
            RectTransform dummy = new GameObject("FirstPoint", typeof(RectTransform)).GetComponent<RectTransform>();
            dummy.SetParent(canvasTransform, false);
            dummy.anchoredPosition = StringCursol.anchoredPosition;
            Strings.Add(dummy);
            m_StringMode = isString;
        };
        inputActions.Stirng.cutstring.performed += ctx =>
        {
            Debug.Log("sfodkok");
            CutString(0);
        };
    }

    void Start()
    {
        m_Offset_X = new Vector2(m_StrinngScale.x, 0f);
        m_Offset_Y = new Vector2(0f, -m_StrinngScale.y);
        listDisplay.UpdateDisplay(StringNum);// Text�\�����X�V
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

    void CutString(int index)
    {
             // ���̂��폜
        Destroy(MirrorStrings[index].gameObject);
        Destroy(Strings[index].gameObject);
        Destroy(FrontStrings[index].gameObject);
        Destroy(BackStrings[index].gameObject);

        AnimStrings[index].DeleteImage(0);
        MirrorAnimStrings[index].DeleteImage(0);
        // ���X�g������폜
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

        if (CheckString(newPos, frontPos, backPos))
        {
            AddString(newPos, frontPos, backPos, Quaternion.identity);
            m_LastDirection = RIGHT;
        }
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

        if (CheckString(newPos, frontPos, backPos))
        {
            AddString(newPos, frontPos, backPos, Quaternion.Euler(0, 180, 0));
            m_LastDirection = LEFT;
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

        if (CheckString(newPos, frontPos, backPos))
        {
            AddString(newPos, frontPos, backPos, Quaternion.Euler(0, 0, 90));
            m_LastDirection = UP;
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

        if (CheckString(newPos, frontPos, backPos))
        {
            AddString(newPos, frontPos, backPos, Quaternion.Euler(0, 0, 270));
            m_LastDirection = DOWN;
        }
    }

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

        mirrorStr.tag = "Nami_Mirror";
        Animator mirrorAnimator = mirrorStr.GetComponent<Animator>();
        mirrorStr.GetComponent<Animator>()?.SetTrigger("Play");
        anim = mirrorStr.GetComponent<StringAnimation_Canvas>();
        if (anim != null)
        {
            anim.SetCanvas(canvasTransform);
        }
        MirrorAnimStrings.Add(anim);
        col = mirrorStr.GetComponent<BoxCollider2D>();
        if (col != null)
        {
            col.size *= HitBoxScale; // RectTransform�ɍ��킹�Ċg�k
        }
        MirrorStrings.Add(mirrorStr);

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
            col.size *= HitBoxScale; // RectTransform�ɍ��킹�Ċg�k
        }
        FrontStrings.Add(frontStr);

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
            col.size *= HitBoxScale; // RectTransform�ɍ��킹�Ċg�k
        }
        BackStrings.Add(backStr);

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
        tama.anchoredPosition = newPos;

        m_StringMode = NoString;
        m_LastDirection = RIGHT;
    }

    public void SetStringSize(Vector2 size, Vector2 BoxScale)
    {
        m_StrinngScale = size; // UI�T�C�Y�ɍ��킹�ĒP�ʕύX
        HitBoxScale = BoxScale;
    }
}

