using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using CharTween;

[RequireComponent(typeof(TMP_Text))]
public class TextBubbly : MonoBehaviour
{
    TMP_Text text;
    CharTweener tweener;

    public float valueY = 5.0f;
    public float duration = 0.3f;
    public bool scaleFade = false;

    async void Start()
    {
        text = GetComponent<TMP_Text>();
        await new WaitUntil(() => text.text.Length > 0);
        tweener = text.GetCharTweener();

        ApplyTweenToLine(BubblyText);
    }

    void BubblyText(int start, int end, bool scaleFade)
    {
        var sequence = DOTween.Sequence();

        for (var i = start; i <= end; ++i)
        {
            var timeOffset = Mathf.Lerp(0, 1, (i - start) / (float)(end - start + 1));
            var charSequence = DOTween.Sequence();
            if (scaleFade)
            {
                charSequence.Append(tweener.DOLocalMoveY(i, valueY, duration).SetEase(Ease.InOutCubic))
    //.Join(tweener.DOFade(i, 1.0f, 0.5f).From())
    .Join(tweener.DOScale(i, 0, 0.5f).From().SetEase(Ease.OutBack, 5))
    .Append(tweener.DOLocalMoveY(i, 0, duration).SetEase(Ease.OutBounce));
            }
            else
            {
                charSequence.Append(tweener.DOLocalMoveY(i, valueY, duration).SetEase(Ease.InOutCubic))
    //.Join(tweener.DOFade(i, 1.0f, 0.5f).From())
    //.Join(tweener.DOScale(i, 0, 0.5f).From().SetEase(Ease.OutBack, 5))
    .Append(tweener.DOLocalMoveY(i, 0, duration).SetEase(Ease.OutBounce));
            }

            sequence.Insert(timeOffset, charSequence);
        }

        sequence.SetLoops(-1, LoopType.Yoyo);
    }

    private void ApplyTweenToLine(System.Action<int, int, bool> tween)
    {
        var lineInfo = text.textInfo.lineInfo[0];
        tween(lineInfo.firstCharacterIndex, lineInfo.lastCharacterIndex, scaleFade);
    }
}
