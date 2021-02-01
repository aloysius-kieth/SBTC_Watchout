using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class PrestartCountdownFinish : MonoBehaviour
{
    public static System.Action OnCountdownFinished;

    public bool changeImage;
    //public Text counter;
    public TextMeshProUGUI counter;
    public Sprite[] images;
    public Image targetImage;

    int count;
    const string READY = "3";
    const string GO = "1";
    int startCountFrom = 3;
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();

        //gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (TrinaxGlobal.Instance.state != STATES.GAME) return;

        TrinaxManager.trinaxAudioManager.PlaySFX(TrinaxAudioManager.AUDIOS.COUNT3, TrinaxAudioManager.AUDIOPLAYER.SFX);
        //count = startCountFrom + 1; // Add one here because each countdown will decrement FIRST, then show on the display.
        //counter.text = READY;
    }

    public void SetCountAndStart(int cnt = 3)
    {
        startCountFrom = cnt;
        count = startCountFrom + 1; // Add one here because each countdown will decrement FIRST, then show on the display.
        counter.text = cnt.ToString();
        if (changeImage && (startCountFrom - 1) < images.Length && (startCountFrom - 1) >= 0)
        {
            targetImage.sprite = images[startCountFrom - 1];
        }

        Invoke("CountingDown", 1f);
    }



    // NOTE: All the below functions will be called by Animation Events.
    public void CountingDown()
    {
        --count;

        counter.text = count.ToString();
        if (changeImage && (count - 1) < images.Length && (count - 1) >= 0)
        {
            targetImage.sprite = images[count - 1];
        }

        if (count < startCountFrom)
        {
            //if (TrinaxGlobal.Instance.state == GAMESTATES.PHOTOTAKING)
            //{
            //Debug.Log(count);
            //}


            if (count == 2)
            {
                TrinaxManager.trinaxAudioManager.PlaySFX(TrinaxAudioManager.AUDIOS.COUNT2, TrinaxAudioManager.AUDIOPLAYER.SFX);
            }
            else if (count == 1)
            {
                TrinaxManager.trinaxAudioManager.PlaySFX(TrinaxAudioManager.AUDIOS.COUNT1, TrinaxAudioManager.AUDIOPLAYER.SFX);
            }

        }
        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }

        anim.SetTrigger(count == 1 ? "exit" : "pulse");
    }

    public void PlayTick()
    {
        //TrinaxManager.trinaxAudioManager.PlayUISFX(TrinaxAudioManager.AUDIOS.TICK);
    }

    public void CountGo()
    {
        counter.text = "1";
        anim.SetTrigger("exit");
        //TrinaxManager.trinaxAudioManager.PlaySound(TrinaxAudioManager.CLIPS.SWOOSH);
    }

    public void CountdownFinished()
    {
        OnCountdownFinished?.Invoke();
    }

    public void DeactivateSelf()
    {
        gameObject.SetActive(false);
    }
}
