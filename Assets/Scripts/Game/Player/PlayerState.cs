using UnityEngine;

public class PlayerState
{
    public enum State { STOP, WALK, JUMP, FALL, CLIMB, GOAL, DEATH };
    public enum Direction { LEFT = -1, STOP = 0, RIGHT = 1 };

    public int m_direction = 0;

    public State currentstate = State.STOP;

    public const float MAX_SPEED = 5f;

    public bool IS_MOVE = true;
    public bool IS_JUMP;
    public bool IS_GROUND;
    public bool IS_DOWN;
    public bool IS_CEILING_HIT = false;
    public bool IS_CLIMB_NG;
    public bool IS_CLIMB;
    public bool IS_GIMJUMP;

    public Vector2 hitobj_pos;
    public Vector2 initialVelocity;//ジャンプで渡す用

    [SerializeField]
    [Header("ジャンプを行う最低時間")]
    public const float jumptime_max = 0.3f;//ジャンプを行う最低時間
    

    //public LayerMask groundlayers => GroundLayers;

    [SerializeField]
    [Header("当たり判定を取るレイヤー")]
    public LayerMask GroundLayers;

    public PlayerState(LayerMask groundLayers)
    {
        GroundLayers = groundLayers;
    }

    //このスクリプトは情報を持つだけ
}
