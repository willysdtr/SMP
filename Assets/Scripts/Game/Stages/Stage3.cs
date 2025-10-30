using StageInfo;
using System.ComponentModel;

public static class Stage3
{
    public static readonly StageData Stage3_1 = new StageData(
        4,                          // �X�e�[�W: ��
        5,                          // �X�e�[�W: �c

        new Int2(2, 2),           // �\: �X�^�[�g�ʒu
        new Int2(2, 4),           // �\: �S�[���ʒu

        new Int2(0, 1),           // ��: �X�^�[�g�ʒu
        new Int2(3, 4),           // ��: �S�[���ʒu

        new SoulPos(true, 0, 0),    // ���̈ʒu: ����, X=0, Y=3

        false,                      // �����ύX�\��?
        false,                       // ���l�͍�������?
        false,                      // �P�l�͍�������?

        new int[] { 3, 3 },            // �D����� �� �D���钷��

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
            new Int2(3, 2)
        },
        new Int2[]                // ��
        {
            new Int2(3, 3)
        },

        /*========== ���� �̈ʒu ==========*/
        new WindPos[]               // �\
        {

        },
        new WindPos[]               // ��
        {

        },

        /*======== �V�[�\�[ �̈ʒu ========*/
        new SeeSaw[]
        {

        },
        new SeeSaw[]
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


    public static readonly StageData Stage3_2 = new StageData(
    5,                          // �X�e�[�W: ��
    4,                          // �X�e�[�W: �c

    new Int2(4, 2),           // �\: �X�^�[�g�ʒu
    new Int2(4, 0),           // �\: �S�[���ʒu

    new Int2(3, 2),           // ��: �X�^�[�g�ʒu
    new Int2(0, 3),           // ��: �S�[���ʒu

    new SoulPos(true, 0, 0),    // ���̈ʒu: ����, X=0, Y=3

    false,                      // �����ύX�\��?
    true,                       // ���l�͍�������?
    false,                      // �P�l�͍�������?

    new int[] { 1, 2 },            // �D����� �� �D���钷��

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
        new Int2 (3, 1),
        new Int2 (3, 3)
    },
    new Int2[]                // ��
    {
        new Int2 (1, 3)
    },

    /*========== ���� �̈ʒu ==========*/
    new WindPos[]               // �\
    {
    },
    new WindPos[]               // ��
    {
    },

    /*======== �V�[�\�[ �̈ʒu ========*/
    new SeeSaw[]
    {
        new SeeSaw(true, 1, 2)
    },
    new SeeSaw[]
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


    public static readonly StageData Stage3_3 = new StageData(
    5,                          // �X�e�[�W: ��
    5,                          // �X�e�[�W: �c
    
    new Int2(2, 4),           // �\: �X�^�[�g�ʒu
    new Int2(1, 1),           // �\: �S�[���ʒu
    
    new Int2(3, 2),           // ��: �X�^�[�g�ʒu
    new Int2(0, 0),           // ��: �S�[���ʒu
    
    new SoulPos(true, 0, 0),    // ���̈ʒu: ����, X=0, Y=3
    
    false,                      // �����ύX�\��?
    true,                       // ���l�͍�������?
    false,                      // �P�l�͍�������?
    
    new int[] { 3, 1, 1, 2 },            // �D����� �� �D���钷��
    
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
        new Int2 (0, 1)
    },
    new Int2[]                // ��
    {
    },
    
    /*========== ���� �̈ʒu ==========*/
    new WindPos[]               // �\
    {
    },
    new WindPos[]               // ��
    {
    },
    
    /*======== �V�[�\�[ �̈ʒu ========*/
    new SeeSaw[]
    {
        new SeeSaw(false, 3, 2)
    },
    new SeeSaw[]
    {
    
    },
    
    /*========== �o�l �̈ʒu ==========*/
    new Int2[]                // ��
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
    },
    new Int2[]                // ��
    {
    }
    );



    public static readonly StageData Stage3_4 = new StageData(
    5,                          // �X�e�[�W: ��
    5,                          // �X�e�[�W: �c

    new Int2(0, 1),           // �\: �X�^�[�g�ʒu
    new Int2(2, 4),           // �\: �S�[���ʒu

    new Int2(2, 4),           // ��: �X�^�[�g�ʒu
    new Int2(0, 1),           // ��: �S�[���ʒu

    new SoulPos(true, 0, 0),    // ���̈ʒu: ����, X=0, Y=3

    false,                      // �����ύX�\��?
    true,                       // ���l�͍�������?
    true,                      // �P�l�͍�������?

    new int[] { 6, 2, 2, 3 },            // �D����� �� �D���钷��

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
        new Int2 (2, 2),
        new Int2 (3, 4)
    },
    new Int2[]                // ��
    {
        new Int2 (2, 2)
    },

    /*========== ���� �̈ʒu ==========*/
    new WindPos[]               // �\
    {
    },
    new WindPos[]               // ��
    {
    },

    /*======== �V�[�\�[ �̈ʒu ========*/
    new SeeSaw[]
    {
        
    },
    new SeeSaw[]
    {

    },

    /*========== �o�l �̈ʒu ==========*/
    new Int2[]                // ��
    {
        new Int2(1, 4)
    },
    new Int2[]                // ��
    {
        new Int2(3, 3)
    },

    /*========== �n�T�~ �̈ʒu ==========*/
    new Int2[]                // ��
    {
        new Int2 (2, 1),
    },
    new Int2[]                // ��
    {
        new Int2 (1, 4)
    }
    );



    public static readonly StageData Stage3_5 = new StageData(
    6,                          // �X�e�[�W: ��
    6,                          // �X�e�[�W: �c

    new Int2(2, 4),           // �\: �X�^�[�g�ʒu
    new Int2(0, 0),           // �\: �S�[���ʒu

    new Int2(0, 1),           // ��: �X�^�[�g�ʒu
    new Int2(3, 4),           // ��: �S�[���ʒu

    new SoulPos(false, 0, 0),    // ���̈ʒu: ����, X=0, Y=3

    false,                      // �����ύX�\��?
    true,                       // ���l�͍�������?
    true,                      // �P�l�͍�������?

    new int[] { 1, 4, 1, 3, 2 },            // �D����� �� �D���钷��

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
        new Int2 (5, 3)
    },
    new Int2[]                // ��
    {
        new Int2 (1, 4)
    },

    /*========== ���� �̈ʒu ==========*/
    new WindPos[]               // �\
    {
    },
    new WindPos[]               // ��
    {
    },

    /*======== �V�[�\�[ �̈ʒu ========*/
    new SeeSaw[]
    {
        new SeeSaw(true, 3, 1)
    },
    new SeeSaw[]
    {
        new SeeSaw(false, 3, 1)
    },

    /*========== �o�l �̈ʒu ==========*/
    new Int2[]                // ��
    {
        new Int2(1, 2),
        new Int2(5, 2)
    },
    new Int2[]                // ��
    {
        new Int2(2, 3),
        new Int2(5, 4)
    },

    /*========== �n�T�~ �̈ʒu ==========*/
    new Int2[]                // ��
    {
        new Int2(4, 4)
    },
    new Int2[]                // ��
    {
        new Int2(1, 4),
        new Int2(3, 2)
    }
    );
}
