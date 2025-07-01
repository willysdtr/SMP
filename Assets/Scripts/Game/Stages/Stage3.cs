using StageInfo;

public static class Stage3
{
    public static readonly StageData Stage3_1 = new StageData(
        25,                          // ステージ: 横
        25,                          // ステージ: 縦

        new Int2(11, 0),           // 表: スタート位置
        new Int2(3, 4),           // 表: ゴール位置

        new Int2(5, 5),           // 裏: スタート位置
        new Int2(2, 14),           // 裏: ゴール位置

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
            new Int2(14, 4),
            new Int2(4, 4),
            new Int2(4, 14),
        },
        new Int2[]                // 裏
        {
            new Int2(3, 2),
            new Int2(12, 4),
        },

        /*========== 風穴 の位置 ==========*/
        new WindPos[]               // 表
        {
            new WindPos(Facing.Right, 3, 3),
            new WindPos(Facing.Down,  13, 0),
        },
        new WindPos[]               // 裏
        {
            new WindPos(Facing.Left, 4, 10),
            new WindPos(Facing.Up,   3, 3),
        }
    );


    public static readonly StageData Stage3_2 = new StageData(
    50,                          // ステージ: 横
    50,                          // ステージ: 縦

    new Int2(11, 0),           // 表: スタート位置
    new Int2(3, 34),           // 表: ゴール位置

    new Int2(25, 5),           // 裏: スタート位置
    new Int2(49, 49),           // 裏: ゴール位置
     
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
            new Int2(34, 4),
    },

    /*========== しわ の位置 ==========*/
    new Int2[]                // 裏
    {
            new Int2(5, 22),
            new Int2(24, 4),
            new Int2(4, 24),
            new Int2(34, 4),
    },
    new Int2[]                // 裏
    {
            new Int2(13, 2),
            new Int2(2, 24),
    },

    /*========== 風穴 の位置 ==========*/
    new WindPos[]               // 表
    {
            new WindPos(Facing.Right, 13, 3),
            new WindPos(Facing.Down,  3, 10),
    },
    new WindPos[]               // 裏
    {
            new WindPos(Facing.Left, 4, 0),
            new WindPos(Facing.Up,   13, 3),
    }
);
}
