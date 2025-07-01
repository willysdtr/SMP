using StageInfo;

public static class Stage3
{
    public static readonly StageData Stage3_1 = new StageData(
        25,                          // �X�e�[�W: ��
        25,                          // �X�e�[�W: �c

        new Int2(11, 0),           // �\: �X�^�[�g�ʒu
        new Int2(3, 4),           // �\: �S�[���ʒu

        new Int2(5, 5),           // ��: �X�^�[�g�ʒu
        new Int2(2, 14),           // ��: �S�[���ʒu

        new SoulPos(true, 0, 3),    // ���̈ʒu: ����, X=0, Y=3

        false,                      // �����ύX�\��?
        true,                       // ���l�͍�������?
        false,                      // �P�l�͍�������?

        new int[] { 5 },            // �D����� �� �D���钷��

        /*========== �S�� �̈ʒu ==========*/
        new Int2[]                // �\
        {
        },
        new Int2[]                // ��
        {
            new Int2(4, 4),
        },

        /*========== ���� �̈ʒu ==========*/
        new Int2[]                // ��
        {
            new Int2(5, 2),
            new Int2(14, 4),
            new Int2(4, 4),
            new Int2(4, 14),
        },
        new Int2[]                // ��
        {
            new Int2(3, 2),
            new Int2(12, 4),
        },

        /*========== ���� �̈ʒu ==========*/
        new WindPos[]               // �\
        {
            new WindPos(Facing.Right, 3, 3),
            new WindPos(Facing.Down,  13, 0),
        },
        new WindPos[]               // ��
        {
            new WindPos(Facing.Left, 4, 10),
            new WindPos(Facing.Up,   3, 3),
        }
    );


    public static readonly StageData Stage3_2 = new StageData(
    50,                          // �X�e�[�W: ��
    50,                          // �X�e�[�W: �c

    new Int2(11, 0),           // �\: �X�^�[�g�ʒu
    new Int2(3, 34),           // �\: �S�[���ʒu

    new Int2(25, 5),           // ��: �X�^�[�g�ʒu
    new Int2(49, 49),           // ��: �S�[���ʒu
     
    new SoulPos(true, 0, 3),    // ���̈ʒu: ����, X=0, Y=3

    false,                      // �����ύX�\��?
    true,                       // ���l�͍�������?
    false,                      // �P�l�͍�������?

    new int[] { 5 },            // �D����� �� �D���钷��

    /*========== �S�� �̈ʒu ==========*/
    new Int2[]                // �\
    {
    },
    new Int2[]                // ��
    {
            new Int2(34, 4),
    },

    /*========== ���� �̈ʒu ==========*/
    new Int2[]                // ��
    {
            new Int2(5, 22),
            new Int2(24, 4),
            new Int2(4, 24),
            new Int2(34, 4),
    },
    new Int2[]                // ��
    {
            new Int2(13, 2),
            new Int2(2, 24),
    },

    /*========== ���� �̈ʒu ==========*/
    new WindPos[]               // �\
    {
            new WindPos(Facing.Right, 13, 3),
            new WindPos(Facing.Down,  3, 10),
    },
    new WindPos[]               // ��
    {
            new WindPos(Facing.Left, 4, 0),
            new WindPos(Facing.Up,   13, 3),
    }
);
}
