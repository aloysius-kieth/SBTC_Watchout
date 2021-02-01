using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        if (AppManager.gameManager == null || AppManager.gameManager != this)
            AppManager.gameManager = this;

    }

    public bool IsReady { get; set; }
    public bool IsGameover { get; set; }
    public bool IsTop10 { get; set; }

    public bool SkipToGame = false;

    public int lastPlaceScore = 0;

    [Header("Component Reference")]
    public HUD hud;
    public CameraFade cameraFade;
    public Player player;
    public LevelManager levelManager;
    public Environment environment;
    public PrestartCountdownFinish countdownFinish;
    public Screensaver screensaver;
    public BonusModeController bonusModeController;
    public CameraProperties cameraProperties;
    public List<FallingObject> fallingObjectsList = new List<FallingObject>();

    private void Start()
    {
        IsReady = false;
    }

    public void Init()
    {
        IsGameover = true;

        //Debug.Log("setting up player");
        player.Setup();
        SubscribeEventlisteners();

        IsReady = true;
        Debug.Log("<color=green> GameManager is ready! </color>");
    }

    void SubscribeEventlisteners()
    {
        GameEvents.OnGameStart += OnGameStart;

        cameraFade.FadeInCallback += FadeInCallback;
        cameraFade.FadeOutCallback += FadeOutCallback;
        player.OnDeath += OnDeath;
        PrestartCountdownFinish.OnCountdownFinished += OnCountdownFinished;

#if UNITY_STANDALONE_WIN
        KinectController.GameStarting += OnGameStart;
#endif
    }

    void Update()
    {
        if (!IsReady) return;

        //if (SkipToGame && TrinaxGlobal.Instance.state != PAGES.CALIBRATION)
        //{
        //    background.alpha = 1;
        //    SkipToGame = false;
        //    ToGame();
        //}
    }

    public void PauseNormalPlay()
    {
        player.FreezePlayerMovement();
        SpawnManager.Instance.DeactivateBehaviour();
        if (cameraProperties.redVignetteOverlay.enabled)
        {
            cameraProperties.redVignetteOverlay.enabled = false;
        }

        for (int i = 0; i < ObjectPooler.Instance.pooledObjects.Count; i++)
        {
            if (ObjectPooler.Instance.pooledObjects[i].GetComponent<FallingObject>() != null)
            {
                FallingObject obj = ObjectPooler.Instance.pooledObjects[i].GetComponent<FallingObject>();

                obj.RigidbodySimulation(false);
            }
        }
    }

    public void ResumeNormalPlay()
    {
        player.UnFreezePlayerMovement();
        SpawnManager.Instance.ActivateBehaviour();
        levelManager.StartGameTimer();

        if (hud.heartController.lastLife)
        {
            cameraProperties.redVignetteOverlay.enabled = true;
        }

        for (int i = 0; i < ObjectPooler.Instance.pooledObjects.Count; i++)
        {
            if (ObjectPooler.Instance.pooledObjects[i].GetComponent<FallingObject>() != null)
            {
                FallingObject obj = ObjectPooler.Instance.pooledObjects[i].GetComponent<FallingObject>();

                obj.RigidbodySimulation(true);
            }
        }
    }

    void InitGameEnvironment()
    {
        cameraFade.FadeIn();
    }

    #region Callbacks   
    void FadeInCallback()
    {
        environment.Activate(true);
        cameraFade.FadeOut();
    }

    void FadeOutCallback()
    {
        countdownFinish.gameObject.SetActive(true);
        countdownFinish.SetCountAndStart();
    }

    private void OnGameStart()
    {
        if (AppManager.uiManager.background.alpha >= 1) 
            AppManager.uiManager.background.alpha = 0;

        TrinaxManager.trinaxAudioManager.ImmediateStopMusic();
        InitGameEnvironment();
        TrinaxGlobal.Instance.state = STATES.GAME;
    }

    void OnDeath()
    {
        #if UNITY_STANDALONE_WIN
        KinectManager.Instance.StopKinect();
#endif

        hud.heartController.StopPulse();
        hud.Show(false);
        IsGameover = true;
        ObjectPooler.Instance.ReturnAllToPool();
        SpawnManager.Instance.spawner.StopSpawning();

        ScoreManager.Instance.CheckEnterTop10();

        TrinaxManager.trinaxAudioManager.ImmediateStopMusic();
        TrinaxManager.trinaxAudioManager.PlaySFXCallback(TrinaxAudioManager.AUDIOS.PLAYER_HURT, TrinaxAudioManager.AUDIOPLAYER.SFX2, () =>
        {
            TrinaxManager.trinaxAudioManager.PlaySFXCallback(TrinaxAudioManager.AUDIOS.GAME_END, TrinaxAudioManager.AUDIOPLAYER.SFX2, async () =>
            {
                await new WaitForSeconds(1.5f);
                AppManager.uiManager.ToResults();
                TrinaxManager.trinaxAudioManager.PlaySFXCallback(TrinaxAudioManager.AUDIOS.RESULT_ENTRY_SOUND, TrinaxAudioManager.AUDIOPLAYER.SFX2, ()=> 
                {
                    TrinaxManager.trinaxAudioManager.RestoreIdleBGM();
                });
            });
        });
    }

    void OnCountdownFinished()
    {
        TrinaxManager.trinaxAudioManager.PlayMusic(TrinaxAudioManager.AUDIOS.IDLE, true);

        hud.Show(true);
        IsGameover = false;
        levelManager.StartGameTimer();

        SpawnManager.Instance.StartTimer = true;
        SpawnManager.Instance.SetPattern(SpawnManager.Instance.currentSpawnMode);
    }
#endregion


    public void ResetValues()
    {
        Debug.Log("Resetting game values");
        ScoreManager.Instance.Reset();
        levelManager.Reset();
        player.Reset();
        hud.Reset();

#if UNITY_STANDALONE_WIN
        KinectController.Instance.ResetValues();
#endif
        SpawnManager.Instance.Reset();
        XPManager.Instance.xpBar.Reset();
        XPManager.Instance.Reset();
        XPManager.Instance.PopulateValues(TrinaxGlobal.Instance.gameSettings);

        cameraProperties.redVignetteOverlay.enabled = false;
        Debug.Log("All values reset");
    }

    [ContextMenu("AUTOWIN")]
    void AUTOWIN()
    {
        player.MinusLifePoints(100);
    }

}
