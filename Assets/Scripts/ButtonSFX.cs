using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSFX : MonoBehaviour
{
    public TrinaxAudioManager.AUDIOS clip;
    Button btn;

    private void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(() => { TrinaxManager.trinaxAudioManager.PlayUISFX(clip, TrinaxAudioManager.AUDIOPLAYER.UI_SFX); });
    }

    
}
