using System.Collections.Generic;

using UnityEngine;
// SMP�֘A�S�ẴX�e�[�^�X
public class SMPState : MonoBehaviour
{
    public static SMPState Instance { get; private set; }
    // �Z���N�g�X�e�[�W
    public static int CURRENT_STAGE = 0;
    public enum GameState
    {
        Title,
        SelectStage,
        PlayGame,
        Pause,
        GameOver
    }
    public GameState m_CurrentGameState = GameState.Title;
    void Awake()
    {
        // Singleton�`�F�b�N
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // �V�[�����܂����ł��j�����Ȃ�
    }
}