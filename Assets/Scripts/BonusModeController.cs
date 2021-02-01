using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BonusModeController : MonoBehaviour
{
    public List<FallingObject> tempObjs;
    public bool isBonusModeActive = false;
    public bool isBonusModeStarted = false;

    public CanvasGroup overlayCanvas;
    public GameObject starStream;

    void Start()
    {
        Setup();
    }

    void Setup()
    {
        XPManager.OnReachedXPGoal += OnReachedXPGoal;
        overlayCanvas.DOFade(0f, 0f);
        starStream.SetActive(false);
    }

    void OnReachedXPGoal()
    {
        // Activate UI for bonus
        XPManager.Instance.xpBar.chargedBar.SetActive(true);
        XPManager.Instance.xpBar.iconUI.Play(true);

        if (!isBonusModeActive && !isBonusModeStarted)
        {
            isBonusModeStarted = true;
            StartBonusMode();
        }
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space) && !isBonusModeActive)
        //{
        //    StartBonusMode();
        //}

        if (isBonusModeActive)
        {
            if(SpawnManager.Instance.coinSpawner.coins.Count == 0)
            {
                StopBonusMode();
            }
            //timer += Time.deltaTime;
            //if (timer > interval)
            //{
            //    timer = 0;

            //    // Stop spawning coins, resume normal game play
            //    StopBonusMode();
            //}
        }
    }

    public async void StartBonusMode()
    {
        Debug.Log("starting bonus mode...");

        TrinaxManager.trinaxAudioManager.TransitMusic(TrinaxAudioManager.AUDIOS.BONUS_MODE, 0.5f, 0.5f);
        // freeze player and object movements
        // darken overlay during bonus mode
        // play particle
        // change out objects
        // stop spawning falling objects
        // switch coin to there
        if (AppManager.gameManager.IsGameover) return;

        ScoreManager.Instance.ChangeTextColor(true);

        AppManager.gameManager.PauseNormalPlay();
        overlayCanvas.DOFade(1.0f, 1f).OnComplete(() => { starStream.SetActive(true); });

        tempObjs = new List<FallingObject>(AppManager.gameManager.fallingObjectsList);
        List<GameObject> coinList = new List<GameObject>();

        if (tempObjs.Count > 0)
        {
            for (int i = 0; i < tempObjs.Count; i++)
            {
                if (!tempObjs[i].onGroundHit && !tempObjs[i].hitUmberlla && !tempObjs[i].hitPlayer)
                {
                    FallingObject obj = tempObjs[i];
                    // Spawn particles at falling object positions
                    GameObject particle = ObjectPooler.Instance.GetPooledObject("FeatherExplosion");
                    particle.transform.localPosition = obj.transform.localPosition;
                    particle.SetActive(true);

                    // Spawn coins at falling object positions
                    GameObject coin = ObjectPooler.Instance.GetPooledObject("Coin");
                    Coin c = coin.GetComponent<Coin>();
                    coinList.Add(coin);
                    coin.transform.localPosition = obj.transform.localPosition;
                    coin.SetActive(true);
                    c.shadow.SetShadowOnGround();

                    // Disable all falling objects on screen
                    obj.gameObject.SetActive(false);
                    obj.DeactivateAll();
                }
            }

            for (int i = 0; i < coinList.Count; i++)
            {
                coinList[i].GetComponent<Rigidbody2D>().simulated = false;
            }

            await new WaitForSeconds(3f);

            AppManager.gameManager.player.UnFreezePlayerMovement();

            for (int i = 0; i < coinList.Count; i++)
            {
                coinList[i].GetComponent<Rigidbody2D>().simulated = true;
            }

            // Start spawning coins 
            isBonusModeActive = true;
            isBonusModeStarted = false;

            SpawnManager.Instance.coinSpawner.SetPattern(SpawnManager.Instance.coinSpawner.coinPattern);

            coinList.Clear();
            tempObjs.Clear();
        }
    }

    void StopBonusMode()
    {
        XPManager.Instance.xpBar.Reset();
        XPManager.Instance.xpBar.iconUI.Stop(true);
        isBonusModeActive = false;

        ScoreManager.Instance.ChangeTextColor(false);
        SpawnManager.Instance.coinSpawner.ClearCoinList();

        starStream.SetActive(false);
        overlayCanvas.DOFade(0f, 1f);
        AppManager.gameManager.ResumeNormalPlay();
        TrinaxManager.trinaxAudioManager.TransitMusic(TrinaxAudioManager.AUDIOS.IDLE, 0.5f, 0.5f);
    }
}
