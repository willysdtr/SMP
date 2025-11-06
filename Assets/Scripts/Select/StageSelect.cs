using DG.Tweening;	//DOTweenを使うときはこのusingを入れる
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelect : MonoBehaviour
{
    [SerializeField] private GameObject m_StageArrow; //ステージ選択の矢印
    [SerializeField] private GameObject m_BackGround;//背景
    [SerializeField] private GameObject[] m_InActiveStage;//Stageの非表示オブジェクト
    [SerializeField] private GameObject[] m_ActiveStage;//Stageの表示オブジェクト
    [SerializeField] private GameObject[] m_WorldIcon;
    [SerializeField] private float m_UpPosX;
    [SerializeField] private float m_UpPosY;
    [SerializeField] private float m_UpPosX_Even;
    [SerializeField] private float m_UpPosY_Even;
    [SerializeField] private float m_UpPosY_Even_Left;
    [SerializeField] private float m_IconPosUp;
    [SerializeField] private float m_ArrowSpeed;
   private InputSystem_Actions inputActions;

    private float horizontalInput = 0f;// 水平方向の入力値
    private int m_StageNum;

    private int m_CurrentArrow = 0;
    // 現在位置（開始地点）
    Vector3 startPos ;

    // 各段階の目標地点を先に計算
    Vector3 downPos ;
    Vector3 rightPos;
    Vector3 finalPos;

    Vector3 BackGroundstartPos;
    bool MoveRight = false;
    bool MoveLeft = false;
    int World = 0;

    void Awake()
    {
        m_StageArrow.transform.position = m_ActiveStage[SMPState.CURRENT_STAGE].transform.position;
        m_CurrentArrow = SMPState.CURRENT_STAGE;
        m_StageNum = m_InActiveStage.Length;//大きさを取得
        inputActions = new InputSystem_Actions();//PlayerInputActionsのインスタンスを生成

          // ワールド番号を計算（0始まり）
        int worldIndex = SMPState.CURRENT_STAGE / 5;

        // 背景の移動距離を1ワールドあたり1837とする
        float moveX = 1837f * worldIndex;

        // 背景の位置を補正
        m_BackGround.transform.position = new Vector3(
             m_BackGround.transform.position.x - moveX,
            m_BackGround.transform.position.y,
            m_BackGround.transform.position.z
        );

        // 現在のワールド番号をセット
        World = worldIndex;
        switch (worldIndex)
        {
            case 0:
                break;
            case 1:
                m_CurrentArrow = 0;
                m_CurrentArrow+= SMPState.CURRENT_STAGE%5;
                break;
                case 2:
                m_CurrentArrow = 0;
                m_CurrentArrow+= SMPState.CURRENT_STAGE%5;
                break;
        }


        inputActions.Select.Move.performed += ctx =>//上が１、下が０,右が２、左が３
        {
            horizontalInput = ctx.ReadValue<float>();
            if (horizontalInput==1)//右
            {
                if (SMPState.CURRENT_STAGE == m_StageNum-1|| MoveRight == true|| MoveLeft == true) return;//最後のステージの時は動かない
                BackGroundstartPos = m_BackGround.transform.position;//現在のBackGroundの位置を保存
                SMPState.CURRENT_STAGE += 1;
                m_CurrentArrow += 1;
                if (m_CurrentArrow % 2 == 1)//奇数ステージを選択中
                {
                    // 現在位置（開始地点）
                    startPos = m_StageArrow.transform.position;
                    // 各段階の目標地点を先に計算
                    downPos = new Vector3(startPos.x, startPos.y - m_UpPosY, startPos.z);
                    rightPos = new Vector3(startPos.x + m_UpPosX, downPos.y, startPos.z);
                    finalPos = new Vector3(rightPos.x, m_ActiveStage[SMPState.CURRENT_STAGE].transform.position.y + m_IconPosUp, startPos.z);
                }
                else if (m_CurrentArrow % 2 == 0)//偶数ステージを選択中
                {
                    // 現在位置（開始地点）
                    startPos = m_StageArrow.transform.position;
                    // 各段階の目標地点を先に計算
                    downPos = new Vector3(startPos.x, startPos.y + m_UpPosY_Even, startPos.z);
                    rightPos = new Vector3(startPos.x + m_UpPosX_Even, downPos.y, startPos.z);
                    finalPos = new Vector3(rightPos.x, m_ActiveStage[SMPState.CURRENT_STAGE].transform.position.y + m_IconPosUp, startPos.z);
                }
                MoveRight = true;
                MoveRightArrow();
            }
            else//左
            {
                if (SMPState.CURRENT_STAGE == 0 || MoveRight == true || MoveLeft == true) return;// 最初のステージの時は動かない
                BackGroundstartPos = m_BackGround.transform.position;//現在のBackGroundの位置を保存
                SMPState.CURRENT_STAGE -= 1;
                m_CurrentArrow -= 1;
                if (m_CurrentArrow % 2 == 1)
                {
                    // 現在位置（開始地点）
                    startPos = m_StageArrow.transform.position;
                    // 各段階の目標地点を先に計算
                    downPos = new Vector3(startPos.x, startPos.y + m_UpPosY_Even_Left, startPos.z);
                    rightPos = new Vector3(startPos.x - m_UpPosX, downPos.y, startPos.z);
                    finalPos = new Vector3(rightPos.x, m_ActiveStage[SMPState.CURRENT_STAGE].transform.position.y + m_IconPosUp, startPos.z);
                }
                else if (m_CurrentArrow % 2 == 0)
                {
                    // 現在位置（開始地点）
                    startPos = m_StageArrow.transform.position;
                    // 各段階の目標地点を先に計算
                    downPos = new Vector3(startPos.x, startPos.y - m_UpPosY_Even, startPos.z);
                    rightPos = new Vector3(startPos.x - m_UpPosX_Even, downPos.y, startPos.z);
                    finalPos = new Vector3(rightPos.x, m_ActiveStage[SMPState.CURRENT_STAGE].transform.position.y + m_IconPosUp, startPos.z);
                }
                MoveLeft = true;
                MoveLeftArrow();
            }
        }
        ;
        inputActions.Select.Move.canceled += ctx =>
        {
            horizontalInput = 0f;
        };

        inputActions.Select.Cancel.performed += ctx =>
        {
            //タイトルに戻る処理
            SMPState.Instance.m_CurrentGameState = SMPState.GameState.Title;//Title状態にする
            SceneManager.LoadScene("TitleScene");
        };

        //選択しているステージをロードする
        inputActions.Select.SelectStage.performed += ctx =>
        {
            LoadSelectedStage();
        };

        //inputActions.PauseApperance.Apperance.performed += ctx =>//ここの処理をSMP_SceneManagerに移動させよう！
        //{
        //    Debug.Log("PauseSceneLoad");
        //    if (MoveRight == true || MoveLeft == true) return;//矢印移動中は出さない
        //    SMPState.Instance.m_CurrentGameState = SMPState.GameState.Pause;//Pause状態にする
        //    inputActions.Select.Disable();//PlayerInputActionsを無効化
        //    SceneManager.LoadScene("PauseScene", LoadSceneMode.Additive);
        //};



    }
        // Update is called once per frame
    void FixedUpdate()//ここから
    {
        if (MoveRight==false&&MoveLeft==false)//選択中のアイコンを表示(矢印移動中は出さない)
        {
            m_ActiveStage[SMPState.CURRENT_STAGE].SetActive(true);
        }
        else
        {
            for(int i=0;i< m_InActiveStage.Length;i++)
            {
                m_ActiveStage[i].SetActive(false);
            }
        }
        for (int i = 0; i < m_WorldIcon.Length; i++)//Worldのアイコンを表示
        {
            m_WorldIcon[i].SetActive(i == World);
        }

    }

    void MoveRightArrow()//矢印右移動
    {
        if (m_CurrentArrow % 5 == 0)//World移動
        {
            m_CurrentArrow = 0;
            m_StageArrow.SetActive(false);//画面遷移中は矢印を非表示
            m_BackGround.transform.DOMoveX(BackGroundstartPos.x - 1837, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                m_StageArrow.transform.position = m_ActiveStage[SMPState.CURRENT_STAGE].transform.position;
                m_StageArrow.SetActive(true);
                MoveRight = false;
                World += 1;
            });
        }
        else

        {
            // DOTween シーケンスで順番に実行
            Sequence seq = DOTween.Sequence();
            seq.Append(m_StageArrow.transform.DOMove(downPos, m_ArrowSpeed).SetEase(Ease.Linear));
            seq.Append(m_StageArrow.transform.DOMove(rightPos, m_ArrowSpeed).SetEase(Ease.Linear));
            seq.Append(m_StageArrow.transform.DOMove(finalPos, m_ArrowSpeed).SetEase(Ease.Linear));
            seq.OnComplete(() =>
            {
                MoveRight = false;
            });

        }
    }
    void MoveLeftArrow()//矢印左移動
    {
        if (m_CurrentArrow < 0)//World移動
        {
            m_CurrentArrow = 4;
            m_StageArrow.SetActive(false);//画面遷移中は矢印を非表示
            m_BackGround.transform.DOMoveX(BackGroundstartPos.x + 1837, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                m_StageArrow.SetActive(true);   
                m_StageArrow.transform.position = m_ActiveStage[SMPState.CURRENT_STAGE].transform.position;
                MoveLeft = false;

                World -= 1;
            });
        }
        else
        {
            // DOTween シーケンスで順番に実行
            Sequence seq = DOTween.Sequence();
            seq.Append(m_StageArrow.transform.DOMove(downPos, m_ArrowSpeed).SetEase(Ease.Linear));
            seq.Append(m_StageArrow.transform.DOMove(rightPos, m_ArrowSpeed).SetEase(Ease.Linear));
            seq.Append(m_StageArrow.transform.DOMove(finalPos, m_ArrowSpeed).SetEase(Ease.Linear));
            seq.OnComplete(() =>
            {
                MoveLeft = false;
            });

        }
    }
    void LoadSelectedStage()
    {
        SMPState.Instance.m_CurrentGameState = SMPState.GameState.PlayGame;//Gameplay状態にする
        // ゲームステージをロードする
        SceneManager.LoadScene("GameScene");
        Debug.Log("現在のシーン: " + SceneManager.GetActiveScene().name);  // ← 確認用
    }


    void OnEnable()
    {
        inputActions.Enable();//PlayerInputActionsを有効化
    }

    void OnDisable()
    {
        inputActions.Disable();//PlayerInputActionsを無効化
    }
}
