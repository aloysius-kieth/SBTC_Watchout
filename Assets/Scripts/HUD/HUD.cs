using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HUD : MonoBehaviour
{
    CanvasGroup canvas;
    public HeartController heartController;
    Animator anim;

	void Start ()
    {
        anim = GetComponent<Animator>();
        anim.enabled = false;
        canvas = GetComponent<CanvasGroup>();
        canvas.alpha = 0;
	}

    public void Show(bool show)
    {
        if (show) canvas.DOFade(1.0f, 0).OnComplete(()=> { anim.enabled = true; });
        else canvas.DOFade(0.0f, 0f).OnComplete(() => { anim.enabled = false; }); ;
    }
	
    public void Reset()
    {
        heartController.Reset();
    }
}
