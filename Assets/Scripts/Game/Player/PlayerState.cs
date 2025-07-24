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
    public Vector2 initialVelocity;//�W�����v�œn���p

    [SerializeField]
    [Header("�W�����v���s���Œ᎞��")]
    public const float jumptime_max = 0.3f;//�W�����v���s���Œ᎞��
    

    //public LayerMask groundlayers => GroundLayers;

    [SerializeField]
    [Header("�����蔻�����郌�C���[")]
    public LayerMask GroundLayers;

    public PlayerState(LayerMask groundLayers)
    {
        GroundLayers = groundLayers;
    }

    //���̃X�N���v�g�͏���������
}
