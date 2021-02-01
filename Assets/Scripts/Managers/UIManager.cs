using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public bool IsReady { get; set; }

    [Header("Buttons")]
    public Button startGameBtn;
    public Button declineTermsBtn;
    public Button acceptTermsBtn;
    public Button nextResultsBtn;

    [Header("Text")]
    public TextMeshProUGUI resultScore;

    float idleTimer;
    float idleDuration;
    float durationToTransit = 0.5f;

    [Header("UI Background")]
    public CanvasGroup background;

    [Header("Component References")]

    private CanvasController canvasController;

    private void Awake()
    {
        if (AppManager.uiManager == null || AppManager.uiManager != this)
        {
            AppManager.uiManager = this;
        }
    }

    private void Start()
    {
        IsReady = false;
        canvasController = GetComponent<CanvasController>();
    }

    public void Init()
    {
        InitButtonListeners();

        // Initialization ready
        IsReady = true;
        Debug.Log("<color=green> UIManager is ready! </color>");

        if (!background.gameObject.activeSelf) background.gameObject.SetActive(true);
        canvasController.OnStartup((int)STATES.SCREENSAVER, 0.25f, () =>
        {
            TrinaxGlobal.Instance.state = STATES.SCREENSAVER;
            AppManager.gameManager.ResetValues();
            ObjectPropertyManager.PopulateObjectProperties();
            TrinaxManager.trinaxAudioManager.PlayMusic(TrinaxAudioManager.AUDIOS.IDLE, true);
        });
    }

    public void PopulateSettings(GlobalSettings settings)
    {
        idleDuration = settings.idleInterval;
    }

    private void InitButtonListeners()
    {
        startGameBtn.onClick.AddListener(() =>
        {
#if UNITY_STANDALONE_WIN
            if (TrinaxManager.trinaxAsyncServerManager.loadingCircle.activeSelf) return;
            APICalls.RunStartInteraction().WrapErrors();
#endif
            ToInstructions();
        });

        declineTermsBtn.onClick.AddListener(() => { ToScreensaver(); });
        acceptTermsBtn.onClick.AddListener(() => { ToInstructions(); });
        nextResultsBtn.onClick.AddListener(() => {
            if (AppManager.gameManager.IsTop10) ToEnterDetails();
            else ToScreensaver();
        });
    }

    private void Update()
    {
        if (!IsReady) return;

        IdleChecker();
    }

    private void IdleChecker()
    {
        // Include here whatever state to check for idle!
        if (TrinaxGlobal.Instance.state == STATES.INSTRUCTIONS)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > idleDuration)
            {
                Debug.Log("Idled too long! Returning to screensaver...");
                idleTimer = 0;
                ToScreensaver();
            }
        }

        if (Input.anyKeyDown)
            idleTimer = 0;
    }

    #region UI
    public void ToScreensaver()
    {
        TrinaxGlobal.Instance.state = STATES.SCREENSAVER;
        AppManager.gameManager.ResetValues();
        ObjectPropertyManager.PopulateObjectProperties();

        // Put 0 for transit duration due to animation being played on this canvas
        canvasController.TransitToCanvas((int)STATES.SCREENSAVER, 0);

        //APICalls.RunUpdateLeaderboard(lb).WrapErrors();
    }

    void ToInstructions()
    {
        background.alpha = 1;
        TrinaxGlobal.Instance.state = STATES.INSTRUCTIONS;
        canvasController.TransitToCanvas((int)STATES.INSTRUCTIONS, durationToTransit, () => { /*APICalls.RunAddInteraction().WrapErrors();*/ });
    }

   public async void ToGame()
    {
        TrinaxGlobal.Instance.state = STATES.CALIBRATION;

#if UNITY_STANDALONE_WIN
        KinectManager.Instance.StartKinect();

        await new WaitUntil(() => KinectManager.Instance.kinectStarted);

        canvasController.TransitToCanvas((int)STATES.GAME, durationToTransit, () =>
        {
            KinectController.Instance.startCalibration();
        });
#endif

        canvasController.TransitToCanvas((int)STATES.GAME, durationToTransit, (()=> { GameEvents.OnGameStart?.Invoke(); }));
    }

   public void ToResults()
    {
        background.alpha = 1;

        AppManager.gameManager.environment.Activate(false);
        resultScore.text = ScoreManager.Instance.Score.ToString();
        TrinaxGlobal.Instance.state = STATES.RESULT;
        canvasController.TransitToCanvas((int)STATES.RESULT, durationToTransit);
    }

    void ToEnterDetails()
    {
        TrinaxGlobal.Instance.state = STATES.ENTER_DETAILS;
        canvasController.TransitToCanvas((int)STATES.ENTER_DETAILS, durationToTransit);
    }
    #endregion
}
