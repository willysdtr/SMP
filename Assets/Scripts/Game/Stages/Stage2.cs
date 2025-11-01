using StageInfo;

public static class Stage2
{
    public static readonly StageData Stage2_1 = new StageData(
        5,                          // ステージ: 横
        4,                          // ステージ: 縦

        new Int2(0, 3),           // 表: スタート位置
        new Int2(4, 0),           // 表: ゴール位置

        new Int2(3, 0),           // 裏: スタート位置
        new Int2(1, 2),           // 裏: ゴール位置

        new SoulPos(true, 0, 0),    // 魂の位置: 左側, X=0, Y=3

        false,                      // 向き変更可能か?
        false,                       // 王様は左向きか?
        true,                      // 姫様は左向きか?

        new int[] { 3 },            // 縫える回数 と 縫える長さ

        /*========== 鉄板 の位置 ==========*/
        new Int2[]                // 表
        {
        },
        new Int2[]                // 裏
        {
        },

        /*========== 針山 の位置 ==========*/
        new Int2[]                // 裏
        {
        },
        new Int2[]                // 裏
        {
        },

        /*========== 風穴 の位置 ==========*/
        new WindPos[]               // 表
        {
        },
        new WindPos[]               // 裏
        {
        },

        /*======== シーソー の位置 ========*/
        new SeeSaw[]                // 表
        {
        },
        new SeeSaw[]                // 裏
        {
        },

        /*========== バネ の位置 ==========*/
        new Int2[]                // 裏
        {
            new Int2(3, 2)
        },
        new Int2[]                // 裏
        {
        },

        /*========== ハサミ の位置 ==========*/
        new Int2[]                // 裏
        {
        },
        new Int2[]                // 裏
        {
        }
    );


    public static readonly StageData Stage2_2 = new StageData(
        5,                          // ステージ: 横
        4,                          // ステージ: 縦

        new Int2(3, 2),           // 表: スタート位置
        new Int2(0, 0),           // 表: ゴール位置

        new Int2(0, 2),           // 裏: スタート位置
        new Int2(3, 2),           // 裏: ゴール位置

        new SoulPos(true, 0, 0),    // 魂の位置: 左側, X=0, Y=3

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
        },

        /*========== 針山 の位置 ==========*/
        new Int2[]                // 裏
        {
        },
        new Int2[]                // 裏
        {
        },

        /*========== 風穴 の位置 ==========*/
        new WindPos[]               // 表
        {
        },
        new WindPos[]               // 裏
        {
        },

        /*======== シーソー の位置 ========*/
        new SeeSaw[]                // 表
        {
        },
        new SeeSaw[]                // 裏
        {
        },

        /*========== バネ の位置 ==========*/
        new Int2[]                // 裏
        {
        },
        new Int2[]                // 裏
        {
        },

        /*========== ハサミ の位置 ==========*/
        new Int2[]                // 裏
        {
        },
        new Int2[]                // 裏
        {
            new Int2 (2, 1)
        }
    );


    public static readonly StageData Stage2_3 = new StageData(
        5,                          // ステージ: 横
        4,                          // ステージ: 縦

        new Int2(0, 0),           // 表: スタート位置
        new Int2(3, 1),           // 表: ゴール位置

        new Int2(2, 1),           // 裏: スタート位置
        new Int2(2, 3),           // 裏: ゴール位置

        new SoulPos(true, 0, 0),    // 魂の位置: 左側, X=0, Y=3

        false,                      // 向き変更可能か?
        false,                      // 王様は左向きか?
        false,                      // 姫様は左向きか?

        new int[] { 3, 3 },            // 縫える回数 と 縫える長さ

        /*========== 鉄板 の位置 ==========*/
        new Int2[]                // 表
        {
        },
        new Int2[]                // 裏
        {
        },

        /*========== 針山 の位置 ==========*/
        new Int2[]                // 裏
        {
        },
        new Int2[]                // 裏
        {
        },

        /*========== 風穴 の位置 ==========*/
        new WindPos[]               // 表
        {
        },
        new WindPos[]               // 裏
        {
        },

        /*======== シーソー の位置 ========*/
        new SeeSaw[]                // 表
        {
        },
        new SeeSaw[]                // 裏
        {
        },

        /*========== バネ の位置 ==========*/
        new Int2[]                // 裏
        {
            new Int2(2, 3)
        },
        new Int2[]                // 裏
        {
        },

        /*========== ハサミ の位置 ==========*/
        new Int2[]                // 裏
        {
        },
        new Int2[]                // 裏
        {
        }
    );

    public static readonly StageData Stage2_4 = new StageData(
        5,                          // ステージ: 横
        4,                          // ステージ: 縦

        new Int2(2, 3),           // 表: スタート位置
        new Int2(2, 1),           // 表: ゴール位置

        new Int2(2, 3),           // 裏: スタート位置
        new Int2(4, 3),           // 裏: ゴール位置

        new SoulPos(true, 0, 0),    // 魂の位置: 左側, X=0, Y=3

        false,                      // 向き変更可能か?
        false,                      // 王様は左向きか?
        true,                      // 姫様は左向きか?

        new int[] { 1, 3, 1 },            // 縫える回数 と 縫える長さ

        /*========== 鉄板 の位置 ==========*/
        new Int2[]                // 表
        {
        },
        new Int2[]                // 裏
        {
        },

        /*========== 針山 の位置 ==========*/
        new Int2[]                // 裏
        {
        },
        new Int2[]                // 裏
        {
        },

        /*========== 風穴 の位置 ==========*/
        new WindPos[]               // 表
        {
        },
        new WindPos[]               // 裏
        {
        },

        /*======== シーソー の位置 ========*/
        new SeeSaw[]                // 表
        {
            new SeeSaw(false, 3, 2)
        },
        new SeeSaw[]                // 裏
        {
        },

        /*========== バネ の位置 ==========*/
        new Int2[]                // 裏
        {

        },
        new Int2[]                // 裏
        {
            new Int2(1, 3)
        },

        /*========== ハサミ の位置 ==========*/
        new Int2[]                // 裏
        {
        },
        new Int2[]                // 裏
        {
        }
    );


    public static readonly StageData Stage2_5 = new StageData(
        5,                          // ステージ: 横
        5,                          // ステージ: 縦

        new Int2(0, 0),           // 表: スタート位置
        new Int2(4, 4),           // 表: ゴール位置

        new Int2(3, 2),           // 裏: スタート位置
        new Int2(3, 1),           // 裏: ゴール位置

        new SoulPos(true, 0, 0),    // 魂の位置: 左側, X=0, Y=3

        false,                      // 向き変更可能か?
        false,                      // 王様は左向きか?
        true,                      // 姫様は左向きか?

        new int[] { 1, 3 },            // 縫える回数 と 縫える長さ

        /*========== 鉄板 の位置 ==========*/
        new Int2[]                // 表
        {
        },
        new Int2[]                // 裏
        {
        },

        /*========== 針山 の位置 ==========*/
        new Int2[]                // 裏
        {
        },
        new Int2[]                // 裏
        {
        },

        /*========== 風穴 の位置 ==========*/
        new WindPos[]               // 表
        {
        },
        new WindPos[]               // 裏
        {
        },

        /*======== シーソー の位置 ========*/
        new SeeSaw[]                // 表
        {
        },
        new SeeSaw[]                // 裏
        {
            new SeeSaw(true, 2, 2)
        },

        /*========== バネ の位置 ==========*/
        new Int2[]                // 裏
        {
            new Int2(2, 4)
        },
        new Int2[]                // 裏
        {
        },

        /*========== ハサミ の位置 ==========*/
        new Int2[]                // 裏
        {
            new Int2 (2, 1)
        },
        new Int2[]                // 裏
        {
        }
    );
}
