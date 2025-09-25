using System;
using System.Collections.Generic;
using UnityEngine;

namespace StageInfo
{
    public enum Facing
    {
        Right = 0,
        Down,
        Left,
        Up
    }

    public struct Int2
    {
        public readonly int X;
        public readonly int Y;

        public Int2(int x, int y)
        {
            X = x;
            Y = y;
        }

        internal Vector2 ToVector2()
        {
            throw new NotImplementedException();
        }
    }

    public struct SoulPos
    {
        public readonly bool IsLeft;
        public readonly int X;
        public readonly int Y;

        public SoulPos(bool isLeft, int x, int y)
        {
            IsLeft = isLeft;
            X = x;
            Y = y;
        }
    }

    public struct WindPos
    {
        public readonly Facing Dir;
        public readonly int X;
        public readonly int Y;

        public WindPos(Facing dir, int x, int y)
        {
            Dir = dir;
            X = x;
            Y = y;
        }
    }

    public struct SeeSaw
    {
        public readonly bool isLeftRight;
        public readonly int X;
        public readonly int Y;

        public SeeSaw(bool dir, int x, int y)
        {
            isLeftRight = dir;
            X = x; 
            Y = y;
        }
    }

    public class StageData
    {
        // フィールド宣言
        public readonly int STAGE_WIDTH;
        public readonly int STAGE_HEIGHT;

        public readonly Int2 START_POS_front;
        public readonly Int2 GOAL_POS_front;
        public readonly Int2 START_POS_back;
        public readonly Int2 GOAL_POS_back;

        public readonly SoulPos SOUL_POS;

        public readonly bool isDIRECTION;
        public readonly bool isKING_LEFT;
        public readonly bool isQUEEN_LEFT;

        public IReadOnlyList<int> STRING_COUNT { get; }
        public IReadOnlyList<Int2> STEEL_front { get; }
        public IReadOnlyList<Int2> STEEL_back { get; }
        public IReadOnlyList<Int2> WRINKLE_front { get; }
        public IReadOnlyList<Int2> WRINKLE_back { get; }
        public IReadOnlyList<WindPos> WIND_front { get; }
        public IReadOnlyList<WindPos> WIND_back { get; }
        public IReadOnlyList<SeeSaw> seeSaw_front { get; }
        public IReadOnlyList<SeeSaw> seeSaw_back { get; }

        // シグネチャコンストラクタ
        public StageData(int stage_width, int stage_height,
            Int2 start_pos_front, Int2 goal_pos_front,
            Int2 start_pos_back, Int2 goal_pos_back,
            SoulPos soul_pos,
            bool is_direction,
            bool is_king_left,
            bool is_queen_left,
            int[] string_count,
            Int2[] steel_front, Int2[] steel_back,
            Int2[] wrinkle_front, Int2[] wrinkle_back,
            WindPos[] wind_front, WindPos[] wind_back,
            SeeSaw[] seesaw_front, SeeSaw[] seesaw_back
        )
        {
            STAGE_WIDTH = stage_width;
            STAGE_HEIGHT = stage_height;
            START_POS_front = start_pos_front;
            GOAL_POS_front = goal_pos_front;
            START_POS_back = start_pos_back;
            GOAL_POS_back = goal_pos_back;
            SOUL_POS = soul_pos;
            isDIRECTION = is_direction;
            isKING_LEFT = is_king_left;
            isQUEEN_LEFT = is_queen_left;

            // nullチェック: Array.AsReadOnly で IReadOnlyList<T> 取得
            STRING_COUNT = Array.AsReadOnly(string_count ?? Array.Empty<int>());
            STEEL_front = Array.AsReadOnly(steel_front ?? Array.Empty<Int2>());
            STEEL_back = Array.AsReadOnly(steel_back ?? Array.Empty<Int2>());
            WRINKLE_front = Array.AsReadOnly(wrinkle_front ?? Array.Empty<Int2>());
            WRINKLE_back = Array.AsReadOnly(wrinkle_back ?? Array.Empty<Int2>());
            WIND_front = Array.AsReadOnly(wind_front ?? Array.Empty<WindPos>());
            WIND_back = Array.AsReadOnly(wind_back ?? Array.Empty<WindPos>());
            seeSaw_front = Array.AsReadOnly(seesaw_front ?? Array.Empty<SeeSaw>());
            seeSaw_back = Array.AsReadOnly(seesaw_back ?? Array.Empty<SeeSaw>());
        }
    }
}
