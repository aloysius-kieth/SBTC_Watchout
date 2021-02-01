using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TrinaxKey : MonoBehaviour {

    public KeyCode keycode;
    public string custom = "";

    int rnd = 0;

    private void Start() {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(()=> {
            rnd = Random.Range(0, 3);
            //if (rnd == 0) {
            //    TrinaxManager.trinaxAudioManager.PlaySound(TrinaxAudioManager.Clips.TYPING);
            //} else if (rnd == 1) {
            //    TrinaxManager.trinaxAudioManager.PlaySound(TrinaxAudioManager.Clips.TYPING_2);
            //} else {
            //    TrinaxManager.trinaxAudioManager.PlaySound(TrinaxAudioManager.Clips.TYPING_3);
            //}

            TrinaxManager.trinaxAudioManager.PlayUISFX(TrinaxAudioManager.AUDIOS.TYPING, TrinaxAudioManager.AUDIOPLAYER.UI_SFX);
            
            transform.DOScale(Vector3.one * 0.9f, 0.000001f).OnComplete(() => {
                transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
            });

            TrinaxOnScreenKB.Instance.OnKeyDown(keycode, custom);
        });
    }
}
