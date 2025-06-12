using StageInfo;

public static class Stage2
{
    public static readonly StageData Stage2_1 = new StageData(
        6,                          // ステージ: 横
        8,                          // ステージ: 縦

        new Int2(1, 0),           // 表: スタート位置
        new Int2(3, 4),           // 表: ゴール位置

        new Int2(5, 5),           // 裏: スタート位置
        new Int2(2, 4),           // 裏: ゴール位置

        new SoulPos(true, 0, 3),    // 魂の位置: 左側, X=0, Y=3

        false,                      // 向き変更可能か?
        true,                       // 王様は左向きか?
        false,                      // 姫様は左向きか?

        new int[] { 5 },            // 縫える回数 と 縫える長さ

        /*========== 鉄板 の位置 ==========*/
        new Int2[]                // 表
        {
        },
        new Int2[]                // 裏
        {
            new Int2(4, 4),
        },

        /*========== しわ の位置 ==========*/
        new Int2[]                // 裏
        {
            new Int2(5, 2),
            new Int2(4, 4),
            new Int2(4, 4),
            new Int2(4, 4),
        },
        new Int2[]                // 裏
        {
            new Int2(3, 2),
            new Int2(2, 4),
        },

        /*========== 風穴 の位置 ==========*/
        new WindPos[]               // 表
        {
            new WindPos(Facing.Right, 3, 3),
            new WindPos(Facing.Down,  3, 0),
        },
        new WindPos[]               // 裏
        {
            new WindPos(Facing.Left, 4, 0),
            new WindPos(Facing.Up,   3, 3),
        }
    );


    public static readonly StageData Stage2_2 = new StageData(
        6,                          // ステージ: 横
        8,                          // ステージ: 縦

        new Int2(1, 0),           // 表: スタート位置
        new Int2(3, 4),           // 表: ゴール位置

        new Int2(5, 5),           // 裏: スタート位置
        new Int2(2, 4),           // 裏: ゴール位置

        new SoulPos(true, 0, 3),    // 魂の位置: 左側, X=0, Y=3

        false,                      // 向き変更可能か?
        true,                       // 王様は左向きか?
        false,                      // 姫様は左向きか?

        new int[] { 5 },            // 縫える回数 と 縫える長さ

        /*========== 鉄板 の位置 ==========*/
        new Int2[]                // 表
        {
        },
        new Int2[]                // 裏
        {
            new Int2(4, 4),
        },

        /*========== しわ の位置 ==========*/
        new Int2[]                // 裏
        {
            new Int2(5, 2),
            new Int2(4, 4),
            new Int2(4, 4),
            new Int2(4, 4),
        },
        new Int2[]                // 裏
        {
            new Int2(3, 2),
            new Int2(2, 4),
        },

        /*========== 風穴 の位置 ==========*/
        new WindPos[]               // 表
        {
            new WindPos(Facing.Right, 3, 3),
            new WindPos(Facing.Down,  3, 0),
        },
        new WindPos[]               // 裏
        {
            new WindPos(Facing.Left, 4, 0),
            new WindPos(Facing.Up,   3, 3),
        }
    );
}
