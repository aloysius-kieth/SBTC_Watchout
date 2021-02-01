using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SCREENSAVER_STATE
{
    MAIN,
    QUEUE,
    LEADER,
};

public class Screensaver : MonoBehaviour
{
    public SCREENSAVER_STATE state;

    public Animator animator;
    public Animator queueMsgAnim;
    public Animator leaderboardAnim;

    float timer;
    public float timeInterval = 15f;

    private void Start()
    {

    }

    public void Init()
    {
        state = SCREENSAVER_STATE.MAIN;

        OnScreenEntry();
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    RebindAnim();
        //}

        if (TrinaxGlobal.Instance.state != STATES.SCREENSAVER) return;

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
        { timer += Time.deltaTime; }

        if(timer > timeInterval)
        {
            timer = 0;
            OnScreenExit();
        }

        if (Input.anyKeyDown)
        {
            timer = 0;
            if (state == SCREENSAVER_STATE.QUEUE ||  state == SCREENSAVER_STATE.LEADER)
            {
                state = SCREENSAVER_STATE.MAIN;
                RebindAnim();
            }
        }
    }

    private void OnScreenEntry()
    {
        if (state == SCREENSAVER_STATE.MAIN)
        {
            AppManager.uiManager.background.alpha = 0;
            animator.SetTrigger("ToMain");
        }
        else if(state == SCREENSAVER_STATE.LEADER)
        {
            AppManager.uiManager.background.alpha = 1;
            animator.SetTrigger("ToLeaderboard");
        }
        else
        {
            animator.SetTrigger("ToMain");
        }
    }

    void OnScreenExit()
    {
        if (state == SCREENSAVER_STATE.MAIN)
        {
            AppManager.uiManager.background.alpha = 1;
            animator.SetTrigger("OnMainExit");
        }
        else if (state == SCREENSAVER_STATE.QUEUE)
        {
            AppManager.uiManager.background.alpha = 1;
            animator.SetTrigger("OnQueueMsgExit");
        }
        else if (state == SCREENSAVER_STATE.LEADER)
        {
            animator.SetTrigger("OnLeaderboardExit");
        }
 
    }

    public void SetBackgroundOff()
    {
        AppManager.uiManager.background.alpha = 0;
    }

    public void PlayQueueMsgExit()
    {
        queueMsgAnim.SetTrigger("QueueExit");
    }

    public void PlayLeaderboardExit()
    {
        leaderboardAnim.SetTrigger("LeaderboardExit");
    }

    public void SetState(SCREENSAVER_STATE _state)
    {
        state = _state;
    }

    public void RebindAnim()
    {
        animator.Rebind();
    }

    private void OnDisable()
    {
        timer = 0;
    }

}
