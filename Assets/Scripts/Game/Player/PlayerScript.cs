using StageInfo;
using UnityEngine;
public enum PlayerType
{
    King,
    Queen
}

public class PlayerScript : MonoBehaviour
{
    [Header("プレイヤータイプ")]
    public PlayerType playerType;

    [Header("UI レファレンス")]
    public StageUICanvasLoader loader;

    //player data
    private RectTransform rectTransform;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        if (playerType == PlayerType.King)
        {
            (transform as RectTransform).anchoredPosition = loader.FrontStartPos.anchoredPosition;
        }
        else 
        {
            (transform as RectTransform).anchoredPosition = loader.BackStartPos.anchoredPosition;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
