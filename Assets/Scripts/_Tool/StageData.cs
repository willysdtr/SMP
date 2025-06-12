// ステージ関連全てのステータス
public class StageData
{
    // プレイヤーステータス
    public static int MAX_HP = 100;
    public static int HP = 100;
    public static float MOVE_SPEED = 5f;
    public static float RUN_SPEED = 5f;
    public static float GRAVITY_SPEED = 5f;
    public static float MOUSE_SENSITIVITY = 1f;
    public static float JUMP_FORCE = 10f;

    // プレイヤーの現在の言弾解放状況
    public static int AMMO_WORD = 0;

    // プレイヤーリロードした言葉(配列数上限まで)
    public static int RELOAD_NUM = 0;

    public static int[] BULLET_DAMAGE = { 18, 20, 23, 27, -25, -20, -15, -20, 10 };
    public static int MAX_BULLET_NUM = 1;
    public static int BULLET_NUM = 0;

    //ゲームオーバーフラグ
    public static bool GameOver = false;

    // クリアフラグ: せりり
     public static bool SERIRI_GET = false;


    //キャラクタークリアフラグ
    public enum FlagType
    {
        SERIRI_GET = 0,
        KOTORI_GET,
        USAGI_GET,
        AOI_GET,
        HATO_GET,
        KORI_GET
    }

    public static bool[] flags = new bool[6];
}


