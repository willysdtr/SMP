using System.Collections.Generic;

using UnityEngine;
// SMP関連全てのステータス
public class SMPState : MonoBehaviour
{
    public static SMPState Instance { get; private set; }
    // セレクトステージ
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
        // Singletonチェック
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // シーンをまたいでも破棄しない
    }
}