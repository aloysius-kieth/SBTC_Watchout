using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class BaseSlider : MonoBehaviour
{
    public SLIDER_BEHAVIOUR slider_behaviour;
    public TextMeshProUGUI sliderValText;

    protected Slider slider;
    public float val;

    void Start()
    {
        Init();
    }

    public virtual void Init()
    {
        slider = GetComponent<Slider>();
        val = slider.value;
        slider.onValueChanged.AddListener(delegate { OnValueChanged(slider); });
    }

    public virtual void OnValueChanged(Slider slider)
    {
        val = TrinaxHelperMethods.RoundDecimal(slider.value, 1);
        sliderValText.text = val.ToString();
    }
}
