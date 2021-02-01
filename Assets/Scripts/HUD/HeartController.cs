
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HeartController : MonoBehaviour
{
    public Image[] hearts;

    int currentHeart = 0;
    Player player;
    Transform[] heartParents;

    public bool lastLife { get; set; }

    void Start()
    {
        Setup();
    }

    void Setup()
    {
        heartParents = new Transform[hearts.Length];
        for (int i = 0; i < hearts.Length; i++)
        {
            heartParents[i] = hearts[i].transform.parent.GetComponent<Transform>();
        }

        player = AppManager.gameManager.player;
        player.OnHurt += OnHurt;
        player.OnRecoverHP += OnRecoverHP;
        RestoreFull();
    }

    public void RestoreFull()
    {
        currentHeart = 0;
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].fillAmount = 1f;
        }
    }

    Tweener pulseTween;
    void PlayPulse()
    {
        AppManager.gameManager.cameraProperties.redVignetteOverlay.enabled = true;
        if (pulseTween == null)
        {
            pulseTween = heartParents[hearts.Length - 1].DOScale(0.7f, 0.4f).SetEase(Ease.InOutCubic).SetLoops(-1, LoopType.Yoyo);
        }
    }

    public void StopPulse()
    {
        //Debug.Log("Stop pulsing");
        AppManager.gameManager.cameraProperties.redVignetteOverlay.enabled = false;
        if(pulseTween != null)
        {
            pulseTween.Kill();
            heartParents[hearts.Length - 1].localScale = Vector3.one;
        }

        pulseTween = null;
    }

    void OnHurt(float amt)
    {
        //Debug.Log("amt " + amt);

        float val = hearts[currentHeart].fillAmount - amt;
        hearts[currentHeart].DOFillAmount(val, 0.05f).OnComplete(()=> {

            if (hearts[currentHeart].fillAmount == 0)
                currentHeart++;

            if (currentHeart == 2 && !lastLife)
            {
                lastLife = true;
                PlayPulse();
            }
        });
    }

    void OnRecoverHP(float amt)
    {
        // Full hp
        if (currentHeart == 0 && hearts[currentHeart].fillAmount == 1) return;
        if (hearts[currentHeart].fillAmount == 1) currentHeart--;

        float val = hearts[currentHeart].fillAmount + amt;
        hearts[currentHeart].DOFillAmount(val, 0.05f).OnComplete(()=> {      
            // If more than last heart
            if (currentHeart < hearts.Length - 1 && hearts[hearts.Length - 1].fillAmount == 1)
            {
                lastLife = false;
                StopPulse();           
            }
        });
    }

    public void Reset()
    {
        RestoreFull();
    }
}
