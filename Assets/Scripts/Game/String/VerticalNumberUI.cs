using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class VerticalNumberUI : MonoBehaviour
{

    [Header("0〜9のスプライトを順に登録")]
    public Sprite[] numberSprites;

    [Header("数字のImageプレハブ（背景付き）")]
    public GameObject digitPrefab; // 子にBackgroundとNumber Imageがある

    [Header("背景スプライト")]
    public Sprite backgroundSprite;

    [Header("縦方向の間隔（px）")]//続きまだある位置が1つめの以降はつめつめ    
    public float spacing;

    private List<GameObject> digitObjects = new List<GameObject>();

    private List<int> currentNumbers = new List<int>();


    public void ShowNumbers(List<int> numbers)
    {
        ClearDigits();

        for (int i = 0; i < numbers.Count; i++)
        {
            int num = numbers[i];
            if (num < 0 || num > 9) continue;

            // Prefab生成
            GameObject digitObj = Instantiate(digitPrefab, transform);
            digitObjects.Add(digitObj);

            // 子オブジェクト取得
            Image bg = digitObj.transform.Find("BackGround")?.GetComponent<Image>();
            Image numImg = digitObj.transform.Find("Number")?.GetComponent<Image>();

            // 数字スプライト設定
            if (numImg != null)
                numImg.sprite = numberSprites[num];

            // 背景制御
            if (bg != null)
            {
                if (i == 0)//消えねえ
                {
                    // 一番上（先頭）は非表示
                    bg.enabled = false;
                }
                else
                {
                    bg.enabled = true;

                    // Prefab側に画像が設定されていれば、上書き不要
                    if (backgroundSprite != null)
                        bg.sprite = backgroundSprite;
                }
            }


            // --- サイズと位置制御 ---
            RectTransform rect = digitObj.GetComponent<RectTransform>();

            // サイズ倍率と間隔を index に応じて調整
            float scale = (i == 0) ? 1.6f : 0.75f;          // 1番目を大きく
            float spacingY = (i == 0) ? spacing * 1.8f : spacing;  // 1番目と2番目の間を広く

            rect.localScale = Vector3.one * scale;

            // 累積位置を計算
            float posY = 0;
            for (int j = 0; j < i; j++)
            {
                posY -= (j == 0) ? spacing * 1.8f : spacing;
            }

            rect.anchoredPosition = new Vector2(0, posY);
        }
    }

    // 先頭が0なら繰り上げ
    public void UpdateNumbers(List<int> numbers)
    {
        // 現在の状態をコピー
        currentNumbers = new List<int>(numbers);

        // 先頭が0なら繰り上げ
        for(int i=0;i<numbers.Count-1;i++)
        {
            if(currentNumbers[0] == 0)
                currentNumbers.RemoveAt(0);
            else
                break;
        }

        ShowNumbers(currentNumbers);
    }

    private void ClearDigits()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        digitObjects.Clear();
    }
}
