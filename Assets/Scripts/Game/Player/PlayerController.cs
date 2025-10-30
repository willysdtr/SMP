using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerController : MonoBehaviour
{
    //プレイヤーのステートをいじる
    private InputSystem_Actions inputActions;
    PlayerMove move;
    public PlayerState state { get; private set; }  // 外部から読み取り可
    private Animator anim = null;

    public Vector2 hitobj_pos = Vector2.zero;
    private Vector2 start_pos = Vector2.zero;
    private Vector2 goal_pos = Vector2.zero;
    public bool ishit;

    private float blocksize = 50;
    private float fallstart_y;//落下開始位置

    public LayerMask groundlayers; //地面、壁判定を行うレイヤー
    public LayerMask climblayers; //登り判定を行うレイヤー

    private RectTransform rect;
    public bool start = false;
    public bool goal = false;
    public bool death = false;

    //L、Rボタンを押しているか
    private bool lPressed = false;
    private bool rPressed = false;

    [SerializeField]
    private AudioClip[] sound; // { 歩行SE、よじ登りSE、落下SE}
    private AudioSource audiosource;

    public int cutCt = 0;//糸を切れる回数

    void Awake()
    { // 各種コンポーネントの取得
        move = GetComponent<PlayerMove>();
        anim = GetComponent<Animator>();
        rect = GetComponent<RectTransform>();
        audiosource = GetComponent<AudioSource>();
        //　Stateクラスの初期化
        state = new PlayerState(groundlayers);
        anim.speed = 0;
        inputActions = new InputSystem_Actions();
        state.m_direction = (int)PlayerState.Direction.RIGHT;

    }

    // Update is called once per frame
    void Update()
    {
        // 自身を最前面に表示
        transform.SetAsLastSibling(); //毎フレームは効率悪そうなので、そのうち変える

        if (!start || goal) { return; }//スタート中かゴール中なら、Update処理を行わない
        ChangeState();//状態変化処理
        HandleState();//状態ごとのUpdate処理
        if (transform.position.y < -600)
        {
            state.currentstate = PlayerState.State.DEATH; //画面外に落ちたら死亡
            move.AllStop();
        }
    }

    public void ChangeState()
    {
        ChangeStateStop();//停止状態に変更するか

        if (state.currentstate != PlayerState.State.STOP) { return; }//停止状態でなければ状態変化処理を行わない
        //停止状態からの状態変化処理
        if (state.IS_CLIMB)//CLIMB状態への移行
        {
            // 登れるなら、CLIMB状態に移行する
            state.currentstate = PlayerState.State.CLIMB;
            transform.position = new Vector2(hitobj_pos.x, transform.position.y);
            
        }
        else
        {
            if (state.IS_JUMP && !state.IS_CEILING_HIT)//JUMP状態への移行
            {
                //ジャンプの初期化を行う
                
                move.InitJump(state.m_direction,blocksize);
                state.currentstate = PlayerState.State.JUMP;

            }
            else if (state.IS_MOVE)//WALK状態への移行
            {
                state.currentstate = PlayerState.State.WALK;
            }
        }
        

        //地面に接触しておらず、ジャンプ中でもなければ
        if (!state.IS_GROUND && !state.IS_JUMP)
        {
            state.currentstate = PlayerState.State.FALL;// 落下開始
            fallstart_y = transform.position.y;
            PlaySE(2, true);
        }
    }

    public void HandleState()//状態ごとのUpdate処理
    {
        switch (state.currentstate)
        {
            case PlayerState.State.STOP: anim.speed = 0; move.Stop(); break;
            case PlayerState.State.WALK: anim.speed = 1; move.Move(state.m_direction); PlaySE(0, false); break;
            case PlayerState.State.JUMP: anim.speed = 0; state.IS_JUMP = !move.Jump(); break;
            case PlayerState.State.FALL: anim.speed = 0; break;
            case PlayerState.State.CLIMB: anim.speed = 1; move.Climb(PlayerState.MAX_SPEED / 2); PlaySE(1, false); break;
            case PlayerState.State.GOAL: anim.speed = 1; if (move.Goal(goal_pos)) { goal = true; anim.speed = 0; } break;
            case PlayerState.State.DEATH: anim.speed = 1; death = true; break;
        }
        anim.SetInteger("State", (int)state.currentstate);
    }

    void ChangeStateStop() //停止状態に変更
    {
        if (state.IS_DOWN)
        {//プレイヤーがやられるかの判定
            state.currentstate = PlayerState.State.DEATH;
            return;
        }

        switch (state.currentstate)//現在の状態から停止に移行するか
        {
            case PlayerState.State.WALK://WALK
                if (!state.IS_GROUND || !state.IS_CLIMB || !state.IS_MOVE) { state.currentstate = PlayerState.State.STOP; move.Stop(); }
                break;
            case PlayerState.State.CLIMB://CLIMB
                if (!state.IS_CLIMB) { state.currentstate = PlayerState.State.STOP; }
                break;
            case PlayerState.State.JUMP://JUMP
                if (state.IS_CEILING_HIT || state.IS_GROUND || !state.IS_JUMP) { state.currentstate = PlayerState.State.STOP; move.EndJump(); }
                break;
            case PlayerState.State.FALL://FALL
                if (state.IS_GROUND || state.IS_JUMP) { state.currentstate = PlayerState.State.STOP; if (fallstart_y - transform.position.y >= blocksize * 2.9 && !state.IS_JUMP) { state.currentstate = PlayerState.State.DEATH; } }
                break;
        }
    }

    public void PlaceAtPosition(RectTransform parent, Vector2 anchoredPos, Vector2 size,float _blocksize = 10 , bool isleft = false)  // プレイヤーをCanvasに、アンカー位置とサイズで配置する関数
    {
        // 親を設定（第2引数falseでローカル座標維持なし、完全に親基準で位置設定）
        rect.SetParent(parent, false);

        // 親パネル基準のローカル座標（アンカー位置）をセット
        rect.anchoredPosition = anchoredPos;

        Vector2 setScale = new ( size.x / rect.sizeDelta.x, size.y / rect.sizeDelta.y ); // サイズを合わせるためのスケール

        blocksize = _blocksize; 

        // サイズを合わせる
        rect.sizeDelta = size;

        BoxCollider2D collider = this.GetComponent<BoxCollider2D>();
        collider.size = new Vector2(collider.size.x * setScale.x, collider.size.y * setScale.y); // 本体の当たり判定サイズを合わせる

        Transform check = transform.Find("ClimbCheck");
        collider = check.GetComponent<BoxCollider2D>();
        collider.size = new Vector2(collider.size.x * setScale.x, collider.size.y * setScale.y);//相対的なサイズ変更
        collider.offset = new(collider.offset.x * setScale.x, collider.offset.y * setScale.y);//offset変更
        check.localPosition = new(check.localPosition.x * setScale.x, check.localPosition.y * setScale.y);//checkの相対位置を変更

        check = transform.Find("CeilingCheck");
        collider = check.GetComponent<BoxCollider2D>();
        collider.size = new Vector2(collider.size.x * setScale.x, collider.size.y * setScale.y);//相対的なサイズ変更
        collider.offset = new(collider.offset.x * setScale.x, collider.offset.y * setScale.y);//offset変更
        check.localPosition = new(check.localPosition.x * setScale.x, check.localPosition.y * setScale.y);//checkの相対位置を変更


        if (isleft) { state.m_direction = (int)PlayerState.Direction.LEFT; transform.rotation = Quaternion.Euler(0, 0, 0); } // 左向き
        else { state.m_direction = (int)PlayerState.Direction.RIGHT; transform.rotation = Quaternion.Euler(0, 180, 0); } //右向き
    }

    void OnEnable()
    {
        // InputSystem 有効化
        inputActions.Enable();

        // L押下
        inputActions.Stirng.PlayerStartL.performed += ctx =>
        {
            lPressed = true;
            OnStartPerformed();
        };
        inputActions.Stirng.PlayerStartL.canceled += ctx => lPressed = false;

        // R押下
        inputActions.Stirng.PlayerStartR.performed += ctx =>
        {
            rPressed = true;
            OnStartPerformed();
        };
        inputActions.Stirng.PlayerStartR.canceled += ctx => rPressed = false;

    }

    void OnDisable()
    {
        // InputSystem無効化 & イベント解除
        inputActions.Stirng.PlayerStartL.performed -= ctx => { lPressed = true; OnStartPerformed(); };
        inputActions.Stirng.PlayerStartL.canceled -= ctx => lPressed = false;

        inputActions.Stirng.PlayerStartR.performed -= ctx => { rPressed = true; OnStartPerformed(); };
        inputActions.Stirng.PlayerStartR.canceled -= ctx => rPressed = false;

        inputActions.Disable();
    }

    private void OnStartPerformed()//スタート状態と非スタートの切り替え
    {
        if (PauseApperance.Instance.isPause || (SoundChangeSlider.Instance != null && SoundChangeSlider.Instance.IsSoundChange)) return;//ポーズ中は操作できないようにする
        if(!(lPressed && rPressed)) { return; }//LRが同時押しされてなければ処理をしない
        if (!start) // スタートに切り替え
        {

            start = true;
            start_pos = transform.position;
        }
        else // 非スタートに切り替え
        {
            start = false;
            goal = false;
            transform.position = start_pos;
            move.AllStop();
            state.currentstate = PlayerState.State.STOP;
            anim.speed = 0;
            ResetFlag();
        }
    }

    private void ResetFlag()
    {
        state.IS_MOVE = true;
        state.IS_CEILING_HIT = false;
        state.IS_CLIMB = false;
        state.IS_DOWN = false;
        state.IS_CLIMB_NG = false;
        state.IS_GROUND = false;
        state.IS_JUMP = false;
        state.IS_GIMJUMP = false;
        death = false;
        start = false;
        goal = false;
    }

    public void Goal(Vector2 pos) //ゴール処理
    {
        if (fallstart_y - transform.position.y >= blocksize * 2.9 && !state.IS_JUMP) { state.currentstate = PlayerState.State.DEATH; return; } //落下死するかどうか
        state.currentstate = PlayerState.State.GOAL;// ゴール状態に変更
        goal_pos = pos;//ゴール位置をセット
        transform.position = new Vector2(transform.position.x, pos.y);//高さを補正
    }

    public void PlayerReturn(float angle)
    {
        if(angle == transform.rotation.y) { return; }//同じ向きなら何もしない
        move.AllStop();
        state.m_direction = move.Return(angle);
    }

    public void PlayerReturn() //現在の向きと逆方向に向きを変える
    {
        move.AllStop();
        state.m_direction = move.Return(-1 * transform.rotation.y);
    }

    public void PlayerJumpReturn()//ジャンプ中に反転する
    {
        PlayerReturn();
        move.JumpReturn();
    }
    private void PlaySE(int no,bool forceplay)//SE再生
    {
        if(forceplay)// 強制再生
        {
            audiosource.PlayOneShot(sound[no]);
            return;
        }

        // 再生が終わった瞬間にすぐ再再生
        if (!audiosource.isPlaying)
        {
            
            audiosource.clip = sound[no];
            audiosource.Play();

        }
    }
}
