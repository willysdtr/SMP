using UnityEngine;
using DG.Tweening;

public class CurtainAnimation : MonoBehaviour
{
    public RectTransform curtainLeft;
    public RectTransform curtainRight;
    public RectTransform topCurtain;
    public RectTransform plaque;

    void Start()
    {
        Sequence openSceneSequence = DOTween.Sequence();

        openSceneSequence
            //最初配置
            .Append(curtainLeft.DOAnchorPosX(curtainLeft.anchoredPosition.x + 50f, 0.2f))
            .Join(curtainRight.DOAnchorPosX(curtainRight.anchoredPosition.x - 50f, 0.2f))
            .Join(topCurtain.DOAnchorPosY(topCurtain.anchoredPosition.y - 50f, 0.2f))
            .Join(plaque.DOAnchorPosY(plaque.anchoredPosition.y - 20f, 0.1f))
            //アニメーション
            .Append(curtainLeft.DOAnchorPosX(-Screen.width, 0.8f).SetEase(Ease.InOutQuad))
            .Join(curtainRight.DOAnchorPosX(Screen.width, 0.8f).SetEase(Ease.InOutQuad))
            .Join(topCurtain.DOAnchorPosY(Screen.height, 1.5f).SetEase(Ease.OutQuad))
            .Join(plaque.DOAnchorPosY(Screen.height, 0.8f).SetEase(Ease.InQuad));
    }
}