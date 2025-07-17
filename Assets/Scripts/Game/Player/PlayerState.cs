using UnityEngine;

public class PlayerState
{
    public enum State { STOP, WALK, JUMP, FALL, CLIMB, GOAL, DEATH };
    public enum Direction { LEFT = -1, STOP = 0, RIGHT = 1 };

    public static float MAX_SPEED = 5f;
    public static bool IS_MOVE = true;
    public static bool IS_JUMP;
    public static bool IS_GROUND;
    public static bool IS_DOWN;
    public static bool IS_CEILING_HIT;
    public static bool IS_CLIMB_NG;
    public static bool IS_CLIMB;
    public static bool IS_GIMJUMP;

    //このスクリプトは情報を持つだけ
}
