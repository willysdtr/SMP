using StageInfo;

public static class Stage1_Test
{
    public static readonly StageData Stage1_1_Test = new StageData(
        6,                          // �X�e�[�W: ��
        8,                          // �X�e�[�W: �c

        new Int2(1, 3),           // �\: �X�^�[�g�ʒu
        new Int2(5, 4),           // �\: �S�[���ʒu

        new Int2(1, 1),           // ��: �X�^�[�g�ʒu
        new Int2(4, 2),           // ��: �S�[���ʒu

        new SoulPos(false, 3, 4),    // ���̈ʒu: ����, X=2, Y=3

        false,                      // �����ύX�\��?
        false,                       // ���l�͍�������?
        true,                      // �P�l�͍�������?

        new int[] { 5 },            // �D����� �� �D���钷��

        /*========== �S�� �̈ʒu ==========*/
        new Int2[]                // �\
        {
        },
        new Int2[]                // ��
        {
            //new Int2(4, 4),
        },

        /*========== �j�R �̈ʒu ==========*/
        new Int2[]                // ��
        {
            //new Int2(5, 2),
            //new Int2(4, 4),
            //new Int2(4, 4),
            //new Int2(4, 4),
        },
        new Int2[]                // ��
        {
            //new Int2(3, 2),
            //new Int2(2, 4),
        },

        /*========== ���� �̈ʒu ==========*/
        new WindPos[]               // �\
        {
            //new WindPos(Facing.Right, 3, 3),
            //new WindPos(Facing.Down,  3, 0),
        },
        new WindPos[]               // ��
        {
            //new WindPos(Facing.Left, 4, 0),
            //new WindPos(Facing.Up,   3, 3),
        },

        /*======== �V�[�\�[ �̈ʒu ========*/
        new SeeSaw[]                // �\
        {

        },
        new SeeSaw[]                // ��
        {

        },

        /*========== �o�l �̈ʒu ==========*/
        new Int2[]                // ��
        {
        },
        new Int2[]                // ��
        {
        },

        /*========== �n�T�~ �̈ʒu ==========*/
        new Int2[]                // ��
        {
        },
        new Int2[]                // ��
        {
        }
    );

    public static readonly StageData Stage2_Test = new StageData(
        5,                          // �X�e�[�W: ��
        6,                          // �X�e�[�W: �c

        new Int2(0, 3),           // �\: �X�^�[�g�ʒu
        new Int2(5, 4),           // �\: �S�[���ʒu

        new Int2(1, 1),           // ��: �X�^�[�g�ʒu
        new Int2(4, 2),           // ��: �S�[���ʒu

        new SoulPos(false, 3, 4),    // ���̈ʒu: ����, X=2, Y=3

        false,                      // �����ύX�\��?
        false,                       // ���l�͍�������?
        false,                      // �P�l�͍�������?

        new int[] { 99, 99, 99 },            // �D����� �� �D���钷��

        /*========== �S�� �̈ʒu ==========*/
        new Int2[]                // �\
        {
        },
        new Int2[]                // ��
        {
        },

        /*========== �j�R �̈ʒu ==========*/
        new Int2[]                // ��
        {
            new Int2(2, 2)
        },
        new Int2[]                // ��
        {
            new Int2(2, 0)
        },

        /*========== ���� �̈ʒu ==========*/
        new WindPos[]               // �\
        {
        },
        new WindPos[]               // ��
        {
        },

        /*======== �V�[�\�[ �̈ʒu ========*/
        new SeeSaw[]                // �\
        {
            new SeeSaw(false, 1, 3)
        },
        new SeeSaw[]                // ��
        {
            new SeeSaw(true, 2, 3)
        },

        /*========== �o�l �̈ʒu ==========*/
        new Int2[]                // �\
        {
            new Int2(1, 4)
        },
        new Int2[]                // ��
        {
            new Int2(2, 2)
        },

        /*========== �n�T�~ �̈ʒu ==========*/
        new Int2[]                // ��
        {
            new Int2(3, 3)
        },
        new Int2[]                // ��
        {
            new Int2(2, 1)
        }
    );
}
