// �X�e�[�W�֘A�S�ẴX�e�[�^�X
public class StageData
{
    // �v���C���[�X�e�[�^�X
    public static int MAX_HP = 100;
    public static int HP = 100;
    public static float MOVE_SPEED = 5f;
    public static float RUN_SPEED = 5f;
    public static float GRAVITY_SPEED = 5f;
    public static float MOUSE_SENSITIVITY = 1f;
    public static float JUMP_FORCE = 10f;

    // �v���C���[�̌��݂̌��e�����
    public static int AMMO_WORD = 0;

    // �v���C���[�����[�h�������t(�z�񐔏���܂�)
    public static int RELOAD_NUM = 0;

    public static int[] BULLET_DAMAGE = { 18, 20, 23, 27, -25, -20, -15, -20, 10 };
    public static int MAX_BULLET_NUM = 1;
    public static int BULLET_NUM = 0;

    //�Q�[���I�[�o�[�t���O
    public static bool GameOver = false;

    // �N���A�t���O: �����
     public static bool SERIRI_GET = false;


    //�L�����N�^�[�N���A�t���O
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


