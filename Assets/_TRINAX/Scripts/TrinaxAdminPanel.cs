using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using System.IO;
using System.Threading.Tasks;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

/// <summary>
/// IDs of all inputFields
/// </summary>
/// 
public enum FIELD_ID
{
    // Adjust as needed
    SERVER_IP,
    IDLE_INTERVAL,
    SPAWN_INTERVAL,
    MIN_POWERUP_SPAWN,
    MAX_POWERUP_SPAWN,

    POINTS_PER_OBJECT,
    XP_PER_OBJECT,
    XP_PER_OBJECT_MULTIPLER,
    MAX_XP_REQUIRED_PER_BONUS,
    PATTERN_INTERVAL,
    DIFFICULTY_INCREASE_INTERVAL,
    // for increasing probability over game duration
    PROBABILITY_SCALE,
    FIRSTAID_RECOVER,
    POINTS_PER_COIN,

    COIN_FALL_SPEED,
    INVUNERABLE_DURATION,
    UMBRELLA_CHANCE,
}

/// <summary>
/// IDs of all toggles
/// </summary>
public enum TOGGLE_ID
{
    // Adjust as needed
    USE_SERVER,
    USE_MOCKY,
    USE_KEYBOARD,
    MUTE_SOUND,
}

/// <summary>
/// IDs of all sliders
/// </summary>
public enum SLIDER_ID
{
    // Adjust as needed
    MASTER,
    MUSIC,
    SFX,
    SFX2,
    SFX3,
    SFX4,
    UI_SFX,
    UI_SFX2,

    BANANA_DROP_SPEED,
    SLIPPER_DROP_SPEED,
    SOCK_DROP_SPEED,
    BOTTLE_DROP_SPEED,
    FLOWER_DROP_SPEED,
    NEWSPAPER_DROP_SPEED,
    MICROWAVE_DROP_SPEED,

    BANANA_DAMAGE,
    SLIPPER_DAMAGE,
    SOCK_DAMAGE,
    BOTTLE_DAMAGE,
    FLOWERPOT_DAMAGE,
    NEWSPAPER_DAMAGE,
    MICROWAVE_DAMAGE,

    KINECT_MIN_USER_DISTANCE,
    KINECT_MAX_USER_DISTANCE,
    KINECT_RIGHTLEFT_USER_DISTANCE,
}

/// <summary>
/// Admin Panel
/// </summary>
public class TrinaxAdminPanel : MonoBehaviour, IManager
{
    int executionPriority = 300;
    object type;
    public int ExecutionPriority
    {
        get { return executionPriority; }
        set { value = executionPriority; }
    }
    public object Type
    {
        get { return type; }
        set { value = type; }
    }
    public bool IsReady { get; set; }

    [Header("Adminpanel Pages")]
    public CanvasGroup[] pages;

    [Header("Display result feedback")]
    public TextMeshProUGUI result;
    public GameObject resultOverlay;

    [Header("InputFields")]
    public TMP_InputField[] inputFields;

    [Header("Toggles")]
    public Toggle[] toggles;

    [Header("Sliders")]
    public Slider[] sliders;
    public TextMeshProUGUI[] sliderValue;

    [Header("Panel Buttons")]
    public Button closeBtn;
    public Button submitBtn;
    public Button pageBtn;
    public Button reporterBtn;
    //public Button clearLB;
    public Button trainingRoomBtn;
    public Button mainBtn;

    Color red = Color.red;
    Color green = Color.green;

    //int selected = 0;
    //int maxInputFieldCount;
    int pageSelected = 0;

    float hideResultText = 0f;
    const float DURATION_RESULT = 5f;

    private void Start(){}

    public async Task Init()
    {
        type = GetType();
        await new WaitUntil(() => TrinaxGlobal.Instance.loadNow);
        Debug.Log("Loading Admin panel...");
        IsReady = false;

        resultOverlay.SetActive(false);

        PopulateCurrentValues();

        toggles[(int)TOGGLE_ID.MUTE_SOUND].onValueChanged.AddListener(delegate { OnMuteAllSounds(toggles[(int)TOGGLE_ID.MUTE_SOUND]); });

        InitButtonListeners();
        IsReady = true;
        Debug.Log("Admin panel is loaded!");

        CycleThroughPages(pageSelected);
        gameObject.SetActive(false);
    }

    void InitButtonListeners()
    {
        closeBtn.onClick.AddListener(Close);
        submitBtn.onClick.AddListener(Submit);
        //trainingRoomBtn.onClick.AddListener(ToTrainingRoom);
        //mainBtn.onClick.AddListener(ToMain);

        pageBtn.onClick.AddListener(() =>
        {
            pageSelected++;
            if (pageSelected >= pages.Length)
            {
                pageSelected = 0;
            }

            CycleThroughPages(pageSelected);
        });

        reporterBtn.onClick.AddListener(TrinaxManager.trinaxCanvas.reporter.doShow);

        //clearLB.onClick.AddListener(() => { LocalLeaderboardJson.Instance.Clear(); });
        //reporterBtn.onClick.AddListener(TrinaxCanvas.Instance.reporter.doShow);
    }

    void CycleThroughPages(int page)
    {
        int num = page;
        for (int i = 0; i < pages.Length; i++)
        {
            CanvasGroup cGrp = pages[i];
            if (i == num)
            {
                cGrp.interactable = true;
                cGrp.blocksRaycasts = true;
                cGrp.DOFade(1.0f, 0.25f);
            }
            else
            {
                cGrp.interactable = false;
                cGrp.blocksRaycasts = false;
                cGrp.DOFade(0.0f, 0.25f);
            }
        }
    }

    void Update()
    {
        HandleInputs();
        if (hideResultText > 0)
            hideResultText -= Time.deltaTime;
        else
        {
            result.gameObject.SetActive(false);
            hideResultText = 0f;
        }
    }

    GameObj UpdateSavedGameObjects(string type)
    {
        ObjectProperties properties = new ObjectProperties { };
        GameObj obj = new GameObj { };
        if (type == FALLING_TYPES.Banana.ToString())
        {
            properties.dropSpeed = TrinaxHelperMethods.RoundDecimal(sliders[(int)SLIDER_ID.BANANA_DROP_SPEED].value, 1);
            properties.damage = TrinaxHelperMethods.RoundDecimal(sliders[(int)SLIDER_ID.BANANA_DAMAGE].value, 1);
            obj.name = type;
            obj.objectProperties = properties;
            return obj;
        }
        else if (type == FALLING_TYPES.Slipper.ToString())
        {
            properties.dropSpeed = TrinaxHelperMethods.RoundDecimal(sliders[(int)SLIDER_ID.SLIPPER_DROP_SPEED].value, 1);
            properties.damage = TrinaxHelperMethods.RoundDecimal(sliders[(int)SLIDER_ID.SLIPPER_DAMAGE].value, 1);
            obj.name = type;
            obj.objectProperties = properties;
            return obj;
        }
        else if (type == FALLING_TYPES.Sock.ToString())
        {
            properties.dropSpeed = TrinaxHelperMethods.RoundDecimal(sliders[(int)SLIDER_ID.SOCK_DROP_SPEED].value, 1);
            properties.damage = TrinaxHelperMethods.RoundDecimal(sliders[(int)SLIDER_ID.SOCK_DAMAGE].value, 1);
            obj.name = type;
            obj.objectProperties = properties;
            return obj;
        }
        else if (type == FALLING_TYPES.Bottle.ToString())
        {
            properties.dropSpeed = TrinaxHelperMethods.RoundDecimal(sliders[(int)SLIDER_ID.BOTTLE_DROP_SPEED].value, 1);
            properties.damage = TrinaxHelperMethods.RoundDecimal(sliders[(int)SLIDER_ID.BOTTLE_DAMAGE].value, 1);
            obj.name = type;
            obj.objectProperties = properties;
            return obj;
        }
        else if (type == FALLING_TYPES.Flowerpot.ToString())
        {
            properties.dropSpeed = TrinaxHelperMethods.RoundDecimal(sliders[(int)SLIDER_ID.FLOWER_DROP_SPEED].value, 1);
            properties.damage = TrinaxHelperMethods.RoundDecimal(sliders[(int)SLIDER_ID.FLOWERPOT_DAMAGE].value, 1);
            obj.name = type;
            obj.objectProperties = properties;
            return obj;
        }
        else if (type == FALLING_TYPES.Newspaper.ToString())
        {
            properties.dropSpeed = TrinaxHelperMethods.RoundDecimal(sliders[(int)SLIDER_ID.NEWSPAPER_DROP_SPEED].value, 1);
            properties.damage = TrinaxHelperMethods.RoundDecimal(sliders[(int)SLIDER_ID.NEWSPAPER_DAMAGE].value, 1);
            obj.name = type;
            obj.objectProperties = properties;
            return obj;
        }
        else if (type == FALLING_TYPES.Microwave.ToString())
        {
            properties.dropSpeed = TrinaxHelperMethods.RoundDecimal(sliders[(int)SLIDER_ID.MICROWAVE_DROP_SPEED].value, 1);
            properties.damage = TrinaxHelperMethods.RoundDecimal(sliders[(int)SLIDER_ID.MICROWAVE_DAMAGE].value, 1);
            obj.name = type;
            obj.objectProperties = properties;
            return obj;
        }
        return obj;
    }

    void PopulateSavedGameObjects(string type, GameObj obj)
    {
        if (obj.name == FALLING_TYPES.Banana.ToString())
        {
            sliders[(int)SLIDER_ID.BANANA_DROP_SPEED].value = obj.objectProperties.dropSpeed;
            sliders[(int)SLIDER_ID.BANANA_DAMAGE].value = obj.objectProperties.damage;
        }
        else if (obj.name == FALLING_TYPES.Slipper.ToString())
        {
            sliders[(int)SLIDER_ID.SLIPPER_DROP_SPEED].value = obj.objectProperties.dropSpeed;
            sliders[(int)SLIDER_ID.SLIPPER_DAMAGE].value = obj.objectProperties.damage;
        }
        else if (obj.name == FALLING_TYPES.Sock.ToString())
        {
            sliders[(int)SLIDER_ID.SOCK_DROP_SPEED].value = obj.objectProperties.dropSpeed;
            sliders[(int)SLIDER_ID.SOCK_DAMAGE].value = obj.objectProperties.damage;
        }
        else if (obj.name == FALLING_TYPES.Bottle.ToString())
        {
            sliders[(int)SLIDER_ID.BOTTLE_DROP_SPEED].value = obj.objectProperties.dropSpeed;
            sliders[(int)SLIDER_ID.BOTTLE_DAMAGE].value = obj.objectProperties.damage;
        }
        else if (obj.name == FALLING_TYPES.Flowerpot.ToString())
        {
            sliders[(int)SLIDER_ID.FLOWER_DROP_SPEED].value = obj.objectProperties.dropSpeed;
            sliders[(int)SLIDER_ID.FLOWERPOT_DAMAGE].value = obj.objectProperties.damage;
        }
        else if (obj.name == FALLING_TYPES.Newspaper.ToString())
        {
            sliders[(int)SLIDER_ID.NEWSPAPER_DROP_SPEED].value = obj.objectProperties.dropSpeed;
            sliders[(int)SLIDER_ID.NEWSPAPER_DAMAGE].value = obj.objectProperties.damage;
        }
        else if (obj.name == FALLING_TYPES.Microwave.ToString())
        {
            sliders[(int)SLIDER_ID.MICROWAVE_DROP_SPEED].value = obj.objectProperties.dropSpeed;
            sliders[(int)SLIDER_ID.MICROWAVE_DAMAGE].value = obj.objectProperties.damage;
        }
        PopulateSlidersText();
    }

    void PopulateSlidersText()
    {
        for (int i = 0; i < sliders.Length; i++)
        {
            for (int j = 0; j < TrinaxGlobal.Instance.gameSettings.GameObjs.GameObj.Count; j++)
            {
                if (sliders[i].GetComponent<GameSlider>() != null)
                {
                    if (sliders[i].GetComponent<GameSlider>().type.ToString() == TrinaxGlobal.Instance.gameSettings.GameObjs.GameObj[j].name)
                    {
                        if (sliders[i].GetComponent<GameSlider>().slider_type == SLIDER_TYPE.DROP_SPEED)
                        {
                            sliderValue[i].text = TrinaxGlobal.Instance.gameSettings.GameObjs.GameObj[j].objectProperties.dropSpeed.ToString();
                        }
                        else if (sliders[i].GetComponent<GameSlider>().slider_type == SLIDER_TYPE.DAMAGE)
                        {
                            sliderValue[i].text = TrinaxGlobal.Instance.gameSettings.GameObjs.GameObj[j].objectProperties.damage.ToString();
                        }
                    }
                }
            }
        }

        for (int i = 0; i < SliderManager.Instance.adminSliders.Count; i++)
        {
            AdminSlider adminSlider = SliderManager.Instance.adminSliders[i];
            float val = TrinaxHelperMethods.RoundDecimal(TrinaxManager.trinaxAudioManager.audioSettings[i].slider.value, 1);
            adminSlider.sliderValText.text = val.ToString();

        }

    }

    void OnMuteAllSounds(Toggle toggle)
    {
        TrinaxManager.trinaxAudioManager.MuteAllSounds(toggle.isOn);
    }

    //void OnSwitchTrackJoint(Toggle toggle)
    //{
    //    ToggleTrackJoint();
    //}

    //void ToggleTrackJoint()
    //{
    //    if (toggles[(int)TOGGLE_ID.TRACK_HEAD].isOn)
    //    {
    //        TrinaxGlobal.Instance.kinectSettings.isTrackingHead = true;
    //        TrinaxGlobal.Instance.kinectSettings.isTrackingBody = false;
    //    }
    //    else
    //    {
    //        TrinaxGlobal.Instance.kinectSettings.isTrackingBody = true;
    //        TrinaxGlobal.Instance.kinectSettings.isTrackingHead = false;
    //    }
    //    Debug.Log("Tracking head: " + TrinaxGlobal.Instance.kinectSettings.isTrackingHead);
    //    Debug.Log("Tracking body: " + TrinaxGlobal.Instance.kinectSettings.isTrackingBody);
    //}

    /// <summary>
    /// Handles all inputs.
    /// </summary>
    void HandleInputs()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            Submit();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            resultOverlay.SetActive(false);
            Close();
        }
        //if (Input.GetKeyDown(KeyCode.Tab))
        //CycleThroughInputFields(++selected);
    }

    /// <summary>
    /// Cycles through all inputfields in the admin panel.
    /// </summary>
    /// <param name="index"></param>
    //void CycleThroughInputFields(int index)
    //{
    //    if (index >= maxInputFieldCount)
    //    {
    //        index = 0;
    //        selected = 0;
    //    }

    //    inputFields[index].Select();
    //    inputFields[index].ActivateInputField();
    //}

    void PopulateGlobalValues()
    {
        inputFields[(int)FIELD_ID.SERVER_IP].text = TrinaxGlobal.Instance.globalSettings.IP.ToString();
        inputFields[(int)FIELD_ID.IDLE_INTERVAL].text = TrinaxGlobal.Instance.globalSettings.idleInterval.ToString();

        toggles[(int)TOGGLE_ID.USE_SERVER].isOn = TrinaxGlobal.Instance.globalSettings.useServer;
        toggles[(int)TOGGLE_ID.USE_MOCKY].isOn = TrinaxGlobal.Instance.globalSettings.useMocky;
        toggles[(int)TOGGLE_ID.USE_KEYBOARD].isOn = TrinaxGlobal.Instance.globalSettings.useKeyboard;
        toggles[(int)TOGGLE_ID.MUTE_SOUND].isOn = TrinaxGlobal.Instance.globalSettings.muteAllSounds;
    }

    void PopulateGameValues()
    {
        //inputFields[(int)FIELD_ID.GAME_DURATION].text = TrinaxGlobal.Instance.gameSettings.gameDuration.ToString();

        inputFields[(int)FIELD_ID.SPAWN_INTERVAL].text = TrinaxGlobal.Instance.gameSettings.spawnInterval.ToString();
        inputFields[(int)FIELD_ID.MIN_POWERUP_SPAWN].text = TrinaxGlobal.Instance.gameSettings.minPowerupSpawn.ToString();
        inputFields[(int)FIELD_ID.MAX_POWERUP_SPAWN].text = TrinaxGlobal.Instance.gameSettings.maxPowerupSpawn.ToString();

        inputFields[(int)FIELD_ID.POINTS_PER_OBJECT].text = TrinaxGlobal.Instance.gameSettings.pointsPerObject.ToString();
        inputFields[(int)FIELD_ID.XP_PER_OBJECT].text = TrinaxGlobal.Instance.gameSettings.xpPerObject.ToString();
        inputFields[(int)FIELD_ID.XP_PER_OBJECT_MULTIPLER].text = TrinaxGlobal.Instance.gameSettings.xpPerObjectMultipler.ToString();
        inputFields[(int)FIELD_ID.MAX_XP_REQUIRED_PER_BONUS].text = TrinaxGlobal.Instance.gameSettings.MAX_XP_REQUIRED_PER_BONUS.ToString();
        inputFields[(int)FIELD_ID.PATTERN_INTERVAL].text = TrinaxGlobal.Instance.gameSettings.timeToActivateNextPattern.ToString();
        inputFields[(int)FIELD_ID.DIFFICULTY_INCREASE_INTERVAL].text = TrinaxGlobal.Instance.gameSettings.DifficultyIncreaseInterval.ToString();
        inputFields[(int)FIELD_ID.PROBABILITY_SCALE].text = TrinaxGlobal.Instance.gameSettings.probabilityScale.ToString();
        inputFields[(int)FIELD_ID.FIRSTAID_RECOVER].text = TrinaxGlobal.Instance.gameSettings.firstAidRecover.ToString();
        inputFields[(int)FIELD_ID.POINTS_PER_COIN].text = TrinaxGlobal.Instance.gameSettings.pointsPerCoin.ToString();
        inputFields[(int)FIELD_ID.COIN_FALL_SPEED].text = TrinaxGlobal.Instance.gameSettings.coinFallSpeed.ToString();
        inputFields[(int)FIELD_ID.INVUNERABLE_DURATION].text = TrinaxGlobal.Instance.gameSettings.invunerableDuration.ToString();
        inputFields[(int)FIELD_ID.UMBRELLA_CHANCE].text = TrinaxGlobal.Instance.gameSettings.umbrellaProbabilityChance.ToString();
    }

    void PopulateKinectValues()
    {
        sliders[(int)SLIDER_ID.KINECT_MIN_USER_DISTANCE].GetComponent<AdminSlider>().sliderValText.text = TrinaxGlobal.Instance.kinectSettings.minUserDistance.ToString();
        sliders[(int)SLIDER_ID.KINECT_MAX_USER_DISTANCE].GetComponent<AdminSlider>().sliderValText.text = TrinaxGlobal.Instance.kinectSettings.maxUserDistance.ToString();
        sliders[(int)SLIDER_ID.KINECT_RIGHTLEFT_USER_DISTANCE].GetComponent<AdminSlider>().sliderValText.text = TrinaxGlobal.Instance.kinectSettings.maxLeftRightDistance.ToString();

        sliders[(int)SLIDER_ID.KINECT_MIN_USER_DISTANCE].value = TrinaxGlobal.Instance.kinectSettings.minUserDistance;
        sliders[(int)SLIDER_ID.KINECT_MAX_USER_DISTANCE].value = TrinaxGlobal.Instance.kinectSettings.maxUserDistance;
        sliders[(int)SLIDER_ID.KINECT_RIGHTLEFT_USER_DISTANCE].value = TrinaxGlobal.Instance.kinectSettings.maxLeftRightDistance;
    }

    /// <summary>
    /// Sets current values to fields.
    /// </summary>
    void PopulateCurrentValues()
    {
        PopulateGlobalValues();

        PopulateGameValues();

        PopulateKinectValues();

        //toggles[(int)TOGGLE_ID.TRACK_HEAD].isOn = TrinaxGlobal.Instance.kinectSettings.isTrackingHead;
        //toggles[(int)TOGGLE_ID.TRACK_SPINEBASE].isOn = TrinaxGlobal.Instance.kinectSettings.isTrackingBody;

        for (int i = 0; i < TrinaxGlobal.Instance.gameSettings.GameObjs.GameObj.Count; i++)
        {
            PopulateSavedGameObjects(TrinaxGlobal.Instance.gameSettings.GameObjs.GameObj[i].name, TrinaxGlobal.Instance.gameSettings.GameObjs.GameObj[i]);
        }
    }

    void UpdateSaveValues()
    {
        GlobalSettings globalSettings = new GlobalSettings
        {
            IP = inputFields[(int)FIELD_ID.SERVER_IP].text.Trim(),
            idleInterval = float.Parse(inputFields[(int)FIELD_ID.IDLE_INTERVAL].text),

            useServer = toggles[(int)TOGGLE_ID.USE_SERVER].isOn,
            useMocky = toggles[(int)TOGGLE_ID.USE_MOCKY].isOn,
            useKeyboard = toggles[(int)TOGGLE_ID.USE_KEYBOARD].isOn,
            muteAllSounds = toggles[(int)TOGGLE_ID.MUTE_SOUND].isOn,
        };

        // loop through all the gameobject types in the game
        for (int i = 0; i < TrinaxGlobal.Instance.gameSettings.GameObjs.GameObj.Count; i++)
        {
            TrinaxGlobal.Instance.gameSettings.GameObjs.GameObj[i] = UpdateSavedGameObjects(TrinaxGlobal.Instance.gameSettings.GameObjs.GameObj[i].name);
        }

        float firstAidVal;
        firstAidVal = TrinaxHelperMethods.RoundToNearestMidPoint(float.Parse(inputFields[(int)FIELD_ID.FIRSTAID_RECOVER].text), 2);
        inputFields[(int)FIELD_ID.FIRSTAID_RECOVER].text = firstAidVal.ToString();

        GameSettings gameSettings = new GameSettings
        {
            //gameDuration = int.Parse(inputFields[(int)FIELD_ID.GAME_DURATION].text),
            spawnInterval = float.Parse(inputFields[(int)FIELD_ID.SPAWN_INTERVAL].text),
            minPowerupSpawn = float.Parse(inputFields[(int)FIELD_ID.MIN_POWERUP_SPAWN].text),
            maxPowerupSpawn = float.Parse(inputFields[(int)FIELD_ID.MAX_POWERUP_SPAWN].text),

            pointsPerObject = int.Parse(inputFields[(int)FIELD_ID.POINTS_PER_OBJECT].text),
            xpPerObject = int.Parse(inputFields[(int)FIELD_ID.XP_PER_OBJECT].text),
            xpPerObjectMultipler = float.Parse(inputFields[(int)FIELD_ID.XP_PER_OBJECT_MULTIPLER].text),
            MAX_XP_REQUIRED_PER_BONUS = int.Parse(inputFields[(int)FIELD_ID.MAX_XP_REQUIRED_PER_BONUS].text),
            timeToActivateNextPattern = float.Parse(inputFields[(int)FIELD_ID.PATTERN_INTERVAL].text),
            DifficultyIncreaseInterval = float.Parse(inputFields[(int)FIELD_ID.DIFFICULTY_INCREASE_INTERVAL].text),
            probabilityScale = int.Parse(inputFields[(int)FIELD_ID.PROBABILITY_SCALE].text),
            firstAidRecover = firstAidVal,
            pointsPerCoin = int.Parse(inputFields[(int)FIELD_ID.POINTS_PER_COIN].text),
            coinFallSpeed = float.Parse(inputFields[(int)FIELD_ID.COIN_FALL_SPEED].text),
            invunerableDuration = int.Parse(inputFields[(int)FIELD_ID.INVUNERABLE_DURATION].text),
            umbrellaProbabilityChance = float.Parse(inputFields[(int)FIELD_ID.UMBRELLA_CHANCE].text),

            GameObjs = TrinaxGlobal.Instance.gameSettings.GameObjs,
        };

        KinectSettings kinectSettings = new KinectSettings
        {
            minUserDistance = TrinaxHelperMethods.RoundDecimal(sliders[(int)SLIDER_ID.KINECT_MIN_USER_DISTANCE].value, 1),
            maxUserDistance = TrinaxHelperMethods.RoundDecimal(sliders[(int)SLIDER_ID.KINECT_MAX_USER_DISTANCE].value, 1),
            maxLeftRightDistance = TrinaxHelperMethods.RoundDecimal(sliders[(int)SLIDER_ID.KINECT_RIGHTLEFT_USER_DISTANCE].value, 1),
        };

        TrinaxGlobal.Instance.globalSettings = globalSettings;
        TrinaxGlobal.Instance.gameSettings = gameSettings;
        TrinaxGlobal.Instance.kinectSettings = kinectSettings;
    }

    /// <summary>
    /// Saves the value to respective fields.
    /// </summary>
    void Submit()
    {
        string resultText = "Empty";
        if (string.IsNullOrEmpty(inputFields[(int)FIELD_ID.SERVER_IP].text.Trim())
           /* || string.IsNullOrEmpty(inputFields[(int)FIELD_ID.PHOTO_PATH].text.Trim())*/)
        {
            Debug.Log("Mandatory fields in admin panel is empty!");
            result.color = red;
            resultText = "Need to fill mandatory fields!";
        }
        else
        {
            StartCoroutine(DoSuccessResult(false));
            result.color = green;
            resultText = "Success!";
            //TrinaxGlobal.Instance.kinectSettings.isTrackingHead = toggles[(int)TOGGLE_ID.TRACK_HEAD].isOn;
            //TrinaxGlobal.Instance.kinectSettings.isTrackingBody = toggles[(int)TOGGLE_ID.TRACK_SPINEBASE].isOn;

            //Debug.Log("Tracking head: " + TrinaxGlobal.Instance.kinectSettings.isTrackingHead);
            //Debug.Log("Tracking body: " + TrinaxGlobal.Instance.kinectSettings.isTrackingBody);

            UpdateSaveValues();
            TrinaxManager.trinaxSaveManager.SaveJson();

            TrinaxGlobal.Instance.RefreshSettings();
        }

        result.text = resultText;
        result.gameObject.SetActive(true);
        hideResultText = DURATION_RESULT;
    }

    public IEnumerator DoSuccessResult(bool immediate)
    {
        resultOverlay.SetActive(true);

        if (!immediate)
            yield return new WaitForSeconds(2f);

        resultOverlay.SetActive(false);
    }


    //void ToTrainingRoom()
    //{
    //    trainingRoomBtn.interactable = false;
    //    TrinaxHelperMethods.ChangeLevel(SCENE.TRAINING_ROOM, () =>
    //     {
    //         trainingRoomBtn.interactable = true;
    //         TrinaxGlobal.Instance.scene = SCENE.TRAINING_ROOM;
    //         TrinaxGlobal.Instance.GetComponentReferences();
    //         TrinaxCanvas.Instance.adminPanel.gameObject.SetActive(false);
    //         TrinaxGlobal.Instance.RefreshSettings(TrinaxGlobal.Instance.globalSettings, TrinaxGlobal.Instance.gameSettings, TrinaxGlobal.Instance.kinectSettings);
    //     });
    //}

    //void ToMain()
    //{
    //    mainBtn.interactable = false;
    //    TrinaxHelperMethods.ChangeLevel(SCENE.MAIN, () =>
    //    {
    //        mainBtn.interactable = true;
    //        TrinaxGlobal.Instance.scene = SCENE.MAIN;
    //        TrinaxGlobal.Instance.GetComponentReferences();
    //        TrinaxCanvas.Instance.adminPanel.gameObject.SetActive(false);
    //        TrinaxGlobal.Instance.RefreshSettings(TrinaxGlobal.Instance.globalSettings, TrinaxGlobal.Instance.gameSettings, TrinaxGlobal.Instance.kinectSettings);
    //    });
    //}

    /// <summary>
    /// Closes admin panel.
    /// </summary>
    void Close()
    {
        gameObject.SetActive(false);
    }
}
