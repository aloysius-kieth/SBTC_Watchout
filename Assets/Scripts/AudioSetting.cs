using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class AudioSetting
{
    public Slider slider;
    public string exposedParam;

    public void Init()
    {
        TrinaxManager.trinaxAudioManager.masterMixer.SetFloat(exposedParam, Mathf.Log10(slider.value) * 20);
        slider.onValueChanged.AddListener(delegate { SetExposedParam(slider.value); });
    }

    public void SetExposedParam(float value)
    {
        TrinaxManager.trinaxAudioManager.masterMixer.SetFloat(exposedParam, Mathf.Log10(value) * 20);
    }
}