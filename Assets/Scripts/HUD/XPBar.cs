using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Coffee.UIExtensions;

using DG.Tweening;

public class XPBar : MonoBehaviour
{
    public Image xpBar;
    public GameObject chargedBar;

    public UIShiny iconUI;

    void Start()
    {
        Setup();
    }

    void Setup()
    {
        iconUI = GetComponentInChildren<UIShiny>();
        chargedBar.SetActive(false);

        xpBar.fillAmount = 0f;
        XPManager.OnGainXP += OnGainXP;
    }

    void OnGainXP()
    {
        //Debug.Log("gained" + XPManager.Instance.XPPercent);
        //xpBar.fillAmount = XPManager.Instance.XPPercent;
        xpBar.DOFillAmount(XPManager.Instance.XPPercent, 0.1f);
    }

    public void Reset()
    {
        xpBar.fillAmount = 0f;
        chargedBar.SetActive(false);
    }
}
