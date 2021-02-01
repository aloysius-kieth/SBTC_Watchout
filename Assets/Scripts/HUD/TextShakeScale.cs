using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using CharTween;

public class TextShakeScale : MonoBehaviour
{ 
    TMP_Text text;
    CharTweener tweener;

    public float duration = 0.5f;
    public float shakeStrength = 0.3f;
    public int vibrato = 7;

    async void Start()
    {
        text = GetComponent<TMP_Text>();
        await new WaitUntil(() => text.text.Length > 0);
        tweener = text.GetCharTweener();

        ApplyTween(Wave);
    }
    void Wave(int start, int end)
    {
        var sequence = DOTween.Sequence();

        for (var i = start; i <= end; ++i)
        {
            var timeOffset = Mathf.Lerp(0, 1, (i - start) / (float)(end - start + 1));
            var charSequence = DOTween.Sequence();
            charSequence.Append(tweener.DOShakeScale(i, 0.5f, shakeStrength, vibrato, 90, true));/*(tweener.DOLocalMoveY(i, radius, 0.5f).SetEase(Ease.InOutCubic))*/
                                                                                      //.Join(tweener.DOPunchScale(i, new Vector3(0.5f, 0.5f), 0.5f).From())
                                                                                      //.Join(tweener.DOFade(i, 0, 0.5f).From())
                                                                                      //.Join(tweener.DOScale(i, 0, 0.5f).From().SetEase(Ease.OutBack, 5))
                /*.Append(tweener.DOLocalMoveY(i, 0, 0.5f).SetEase(Ease.OutBounce));*/
            sequence.Insert(timeOffset, charSequence);
        }

        sequence.SetLoops(-1, LoopType.Restart);
    }

    private void ApplyTween(System.Action<int, int> tween)
    {
        var lineInfo = text.textInfo.lineInfo[0];
        tween(lineInfo.firstCharacterIndex, lineInfo.lastCharacterIndex);
    }
}
