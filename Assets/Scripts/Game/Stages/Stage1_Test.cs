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
        true,                       // ���l�͍�������?
        false,                      // �P�l�͍�������?

        new int[] { 5 },            // �D����� �� �D���钷��

        /*========== �S�� �̈ʒu ==========*/
        new Int2[]                // �\
        {
        },
        new Int2[]                // ��
        {
            //new Int2(4, 4),
        },

        /*========== ���� �̈ʒu ==========*/
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
        }
    );


    public static readonly StageData Stage1_2 = new StageData(
        8,                          // �X�e�[�W: ��
        8,                          // �X�e�[�W: �c

        new Int2(2, 0),           // �\: �X�^�[�g�ʒu
        new Int2(3, 4),           // �\: �S�[���ʒu

        new Int2(5, 5),           // ��: �X�^�[�g�ʒu
        new Int2(2, 4),           // ��: �S�[���ʒu

        new SoulPos(true, 0, 3),    // ���̈ʒu: ����, X=0, Y=3

        false,                      // �����ύX�\��?
        true,                       // ���l�͍�������?
        false,                      // �P�l�͍�������?

        new int[] { 5 },            // �D����� �� �D���钷��

        /*========== �S�� �̈ʒu ==========*/
        new Int2[]                // �\
        {
            new Int2(1, 4),
            new Int2(2, 4),
            new Int2(3, 4),
            new Int2(4, 4),
        },
        new Int2[]                // ��
        {
            new Int2(4, 4),
        },

        /*========== ���� �̈ʒu ==========*/
        new Int2[]                // ��
        {
            new Int2(5, 2),
            new Int2(4, 4),
            new Int2(4, 4),
            new Int2(4, 4),
        },
        new Int2[]                // ��
        {
            new Int2(3, 2),
            new Int2(2, 4),
        },

        /*========== ���� �̈ʒu ==========*/
        new WindPos[]               // �\
        {
            new WindPos(Facing.Left, 0, 0),
            new WindPos(Facing.Left, 0, 1),
            new WindPos(Facing.Left, 1, 0),

            new WindPos(Facing.Left, 0, 6),
            new WindPos(Facing.Left, 0, 7),
            new WindPos(Facing.Left, 1, 7),
        },
        new WindPos[]               // ��
        {
            new WindPos(Facing.Left, 7, 7),
            new WindPos(Facing.Left, 6, 7),
            new WindPos(Facing.Left, 7, 6),
        }
    );
}
