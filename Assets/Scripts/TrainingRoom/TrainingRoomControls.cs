using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using TMPro;

[System.Serializable]
public class TraininglobalSettings
{
    //public bool trainingStarted;
    public bool infiniteLife;

    public float minSpawnTime;
    public float maxSpawnTime;

    public DIFFICULTY_LEVELS difficulty;
}

public class TrainingRoomControls : MonoBehaviour
{
    public bool trainingStarted = false;
    public bool doneLoadSettings = false;

    public TraininglobalSettings traininglobalSettings;
    public CanvasGroup[] pages;
    int currentPage = 0;

    public Vector2 moveToOpen;
    public Vector2 moveToClose;

    bool isOpened = true;

    [Header("Buttons")]
    public Button openControlsBtn;
    public Button closeControlsBtn;
    public Button saveBtn;
    public Button nextPageBtn;

    [Header("Settings")]
    public Toggle trainingStart_Toggle;
    public Toggle infiniteLife_Toggle;
    public Toggle useKinect_Toggle;

    public Toggle banana_Toggle;
    public Toggle slipper_Toggle;
    public Toggle sock_Toggle;
    public Toggle bottle_Toggle;
    public Toggle flowerpot_Toggle;
    public Toggle newspaper_Toggle;
    public Toggle microwave_Toggle;

    public TMP_InputField minSpawnTimeIF;
    public TMP_InputField maxSpawnTimeIF;

    const string SAVEFILENAME = "traininglobalSettings.json";

    async void Start()
    {
        CloseControlPanel(0.00001f);
        //await new WaitUntil(() => TrinaxGlobal.Instance.doneLoadComponentReferences);
        Init();
    }

    void Init()
    {
        doneLoadSettings = false;
        trainingStart_Toggle.isOn = false;
        useKinect_Toggle.isOn = false;
        banana_Toggle.isOn = false;
        slipper_Toggle.isOn = false;
        sock_Toggle.isOn = false;
        bottle_Toggle.isOn = false;
        flowerpot_Toggle.isOn = false;
        newspaper_Toggle.isOn = false;
        microwave_Toggle.isOn = false;
        KinectController.GameStarting += OnCalibrationFinished;

        LoadSettings();
        useKinect_Toggle.onValueChanged.AddListener(delegate { OnUsekinectChanged(useKinect_Toggle); });
        openControlsBtn.onClick.AddListener(() => { OpenControlPanel(0.25f); });
        closeControlsBtn.onClick.AddListener(() => { CloseControlPanel(0.25f); });
        saveBtn.onClick.AddListener(SaveValues);
        nextPageBtn.onClick.AddListener(NextPage);
        PopulateValues();

        doneLoadSettings = true;

        SetPage(0);
    }

    void PopulateValues()
    {
        infiniteLife_Toggle.isOn = traininglobalSettings.infiniteLife;

        minSpawnTimeIF.text = traininglobalSettings.minSpawnTime.ToString();
        maxSpawnTimeIF.text = traininglobalSettings.maxSpawnTime.ToString();
    }

    void SaveValues()
    {
        trainingStarted = trainingStart_Toggle.isOn;

        traininglobalSettings.infiniteLife = infiniteLife_Toggle.isOn;

        traininglobalSettings.minSpawnTime = float.Parse(minSpawnTimeIF.text.Trim());
        traininglobalSettings.maxSpawnTime = float.Parse(maxSpawnTimeIF.text.Trim());

        TrainingRoomManager.Instance.SetSpawnTime();

        SaveSettings();
    }

    void OnUsekinectChanged(Toggle toggle)
    {
        if (KinectController.Instance != null)
        {
            if (toggle.isOn)
                KinectController.Instance.startCalibration();
            else
                KinectController.Instance.stopKinect();
        }
    }

    void OnCalibrationFinished()
    {
        AppManager.gameManager.player.useKeyboard = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9))
        {
            if (!isOpened) OpenControlPanel(0.25f);
            else CloseControlPanel(0.25f);
        }

    }

    TraininglobalSettings CreateTrainingSaveFile()
    {
        TraininglobalSettings saveTraininglobalSettings = new TraininglobalSettings
        {
            //trainingStarted = TrinaxGlobal.Instance.traininglobalSettings.trainingStarted,
            infiniteLife = traininglobalSettings.infiniteLife,
            minSpawnTime = traininglobalSettings.minSpawnTime,
            maxSpawnTime = traininglobalSettings.maxSpawnTime,
            difficulty = traininglobalSettings.difficulty,
        };

        return saveTraininglobalSettings;
    }

    void SaveSettings()
    {
        TraininglobalSettings save = CreateTrainingSaveFile();
        string jsonStr = JsonUtility.ToJson(save, true);

        JsonFileUtility.WriteJsonToFile(SAVEFILENAME, jsonStr, JSONSTATE.PERSISTENT_DATA_PATH);

        Debug.Log("Saving as JSON " + jsonStr);
    }

    void LoadSettings()
    {
        string jsonStr = JsonFileUtility.LoadJsonFromFile(SAVEFILENAME, JSONSTATE.PERSISTENT_DATA_PATH);

        TraininglobalSettings loadObj = JsonUtility.FromJson<TraininglobalSettings>(jsonStr);

        if (loadObj != null)
        {
            traininglobalSettings.infiniteLife = loadObj.infiniteLife;
            traininglobalSettings.minSpawnTime = loadObj.minSpawnTime;
            traininglobalSettings.maxSpawnTime = loadObj.maxSpawnTime;
        }
        else
        {
            Debug.Log("Json file is empty! Creating a new training save file...");
            loadObj = CreateTrainingSaveFile();
        }
    }

    void NextPage()
    {
        currentPage++;
        if (currentPage > pages.Length - 1) currentPage = 0;


        SetPage(currentPage);
    }

    void SetPage(int pageNum)
    {
        for (int i = 0; i < pages.Length; i++)
        {
            CanvasGroup canvas = pages[i];
            if (i == pageNum)
            {
                canvas.interactable = true;
                canvas.blocksRaycasts = true;
                canvas.DOFade(1f, 0.1f);
            }
            else
            {
                canvas.interactable = false;
                canvas.blocksRaycasts = false;
                canvas.DOFade(0f, 0.1f);
            }
        }
    }

    void OpenControlPanel(float duration)
    {
        if (isOpened) return;
        isOpened = true;
        transform.DOLocalMove(moveToOpen, duration, false);
    }

    void CloseControlPanel(float duration)
    {
        if (!isOpened) return;
        isOpened = false;
        transform.DOLocalMove(moveToClose, duration, false);
    }
}
