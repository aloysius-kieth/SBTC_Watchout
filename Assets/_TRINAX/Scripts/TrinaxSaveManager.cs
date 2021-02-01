using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TrinaxSaveManager : MonoBehaviour, IManager
{
    private void Awake()
    {
        if(TrinaxManager.trinaxSaveManager == null || TrinaxManager.trinaxSaveManager != this)
            TrinaxManager.trinaxSaveManager = this;
    }

    int executionPriority = 0;
    public int ExecutionPriority
    {
        get { return executionPriority; }
        set { value = executionPriority; }
    }
    public bool IsReady { get; set; }

    public TrinaxSaves saveObj;
    const string ADMINSAVEFILE = "adminsave.json";

    List<GameObj> saveGameObjects = new List<GameObj>();

    public async Task Init()
    {
        await new WaitUntil(() => TrinaxGlobal.Instance.loadNow);
        Debug.Log("Loading TrinaxSaveManager");
        IsReady = false;
        LoadJson();
        IsReady = true;
        Debug.Log("TrinaxSaveManager is loaded");
    }

    TrinaxSaves CreateAdminSave()
    {
        saveGameObjects.Clear();
     
        for (int i = 0; i < TrinaxGlobal.Instance.AllGameObjTypesStr.Length - 1; i++)
        {
            GameObj obj = new GameObj
            {
                name = TrinaxGlobal.Instance.AllGameObjTypesStr[i],
                objectProperties = TrinaxGlobal.Instance.gameSettings.GameObjs.GameObj[i].objectProperties,
            };
            saveGameObjects.Add(obj);
        }

        GameObjs gameObjects = new GameObjs
        {
            GameObj = saveGameObjects,
        };

        GameSettings saveGameSettings = new GameSettings
        {
            //gameDuration = TrinaxGlobal.Instance.gameSettings.gameDuration,

            spawnInterval = TrinaxGlobal.Instance.gameSettings.spawnInterval,
            minPowerupSpawn = TrinaxGlobal.Instance.gameSettings.minPowerupSpawn,
            maxPowerupSpawn = TrinaxGlobal.Instance.gameSettings.maxPowerupSpawn,

            pointsPerObject = TrinaxGlobal.Instance.gameSettings.pointsPerObject,
            xpPerObject = TrinaxGlobal.Instance.gameSettings.xpPerObject,
            xpPerObjectMultipler = TrinaxGlobal.Instance.gameSettings.xpPerObjectMultipler,
            MAX_XP_REQUIRED_PER_BONUS = TrinaxGlobal.Instance.gameSettings.MAX_XP_REQUIRED_PER_BONUS,
            timeToActivateNextPattern = TrinaxGlobal.Instance.gameSettings.timeToActivateNextPattern,
            DifficultyIncreaseInterval = TrinaxGlobal.Instance.gameSettings.DifficultyIncreaseInterval,
            probabilityScale = TrinaxGlobal.Instance.gameSettings.probabilityScale,
            firstAidRecover = TrinaxGlobal.Instance.gameSettings.firstAidRecover,
            pointsPerCoin = TrinaxGlobal.Instance.gameSettings.pointsPerCoin,
            coinFallSpeed = TrinaxGlobal.Instance.gameSettings.coinFallSpeed,
            invunerableDuration = TrinaxGlobal.Instance.gameSettings.invunerableDuration,
            umbrellaProbabilityChance =TrinaxGlobal.Instance.gameSettings.umbrellaProbabilityChance,

            GameObjs = gameObjects,
        };

        GlobalSettings saveGlobalSettings = new GlobalSettings
        {
            IP = TrinaxGlobal.Instance.globalSettings.IP,
            photoPath = TrinaxGlobal.Instance.globalSettings.photoPath,
            idleInterval = TrinaxGlobal.Instance.globalSettings.idleInterval,

            COMPORT1 = TrinaxGlobal.Instance.globalSettings.COMPORT1,

            useServer = TrinaxGlobal.Instance.globalSettings.useServer,
            useMocky = TrinaxGlobal.Instance.globalSettings.useMocky,
            useKeyboard = TrinaxGlobal.Instance.globalSettings.useKeyboard,
            muteAllSounds = TrinaxManager.trinaxAudioManager.muteAllSounds,
        };

        AudioSettings saveAudioSettings = new AudioSettings
        {
            masterVolume = TrinaxManager.trinaxAudioManager.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.MASTER].slider.value,
            musicVolume = TrinaxManager.trinaxAudioManager.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.MUSIC].slider.value,
            SFXVolume = TrinaxManager.trinaxAudioManager.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.SFX].slider.value,
            SFX2Volume = TrinaxManager.trinaxAudioManager.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.SFX2].slider.value,
            SFX3Volume = TrinaxManager.trinaxAudioManager.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.SFX3].slider.value,
            SFX4Volume = TrinaxManager.trinaxAudioManager.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.SFX4].slider.value,
            UI_SFXVolume = TrinaxManager.trinaxAudioManager.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.UI_SFX].slider.value,
            UI_SFX2Volume = TrinaxManager.trinaxAudioManager.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.UI_SFX2].slider.value,
        };

        KinectSettings saveKinectSettings = new KinectSettings
        {
            isTrackingBody = TrinaxGlobal.Instance.kinectSettings.isTrackingBody,
            isTrackingHead = TrinaxGlobal.Instance.kinectSettings.isTrackingHead,
            minUserDistance = TrinaxGlobal.Instance.kinectSettings.minUserDistance,
            maxUserDistance = TrinaxGlobal.Instance.kinectSettings.maxUserDistance,
            maxLeftRightDistance = TrinaxGlobal.Instance.kinectSettings.maxLeftRightDistance,
        };

        TrinaxSaves save = new TrinaxSaves
        {
            gameSettings = saveGameSettings,
            globalSettings = saveGlobalSettings,
            audioSettings = saveAudioSettings,
            kinectSettings  = saveKinectSettings,
        };    

        return save;
    }

    public void SaveJson()
    {
        saveObj = CreateAdminSave();

        string saveJsonString = JsonUtility.ToJson(saveObj, true);

        JsonFileUtility.WriteJsonToFile(ADMINSAVEFILE, saveJsonString, JSONSTATE.PERSISTENT_DATA_PATH);
        Debug.Log("Saving as JSON " + saveJsonString);
    }

    void PopulateGlobalSettings()
    {
        TrinaxGlobal.Instance.globalSettings.IP = saveObj.globalSettings.IP;
        TrinaxGlobal.Instance.globalSettings.photoPath = saveObj.globalSettings.photoPath;
        TrinaxGlobal.Instance.globalSettings.idleInterval = saveObj.globalSettings.idleInterval;

        TrinaxGlobal.Instance.globalSettings.COMPORT1 = saveObj.globalSettings.COMPORT1;

        TrinaxGlobal.Instance.globalSettings.useServer = saveObj.globalSettings.useServer;
        TrinaxGlobal.Instance.globalSettings.useMocky = saveObj.globalSettings.useMocky;
        TrinaxGlobal.Instance.globalSettings.useKeyboard = saveObj.globalSettings.useKeyboard;
        TrinaxGlobal.Instance.globalSettings.muteAllSounds = saveObj.globalSettings.muteAllSounds;
    }

    void PopulateGameSettings()
    {
        //TrinaxGlobal.Instance.gameSettings.gameDuration = saveObj.gameSettings.gameDuration;
        TrinaxGlobal.Instance.gameSettings.spawnInterval = saveObj.gameSettings.spawnInterval;
        TrinaxGlobal.Instance.gameSettings.minPowerupSpawn = saveObj.gameSettings.minPowerupSpawn;
        TrinaxGlobal.Instance.gameSettings.maxPowerupSpawn = saveObj.gameSettings.maxPowerupSpawn;

        TrinaxGlobal.Instance.gameSettings.pointsPerObject = saveObj.gameSettings.pointsPerObject;
        TrinaxGlobal.Instance.gameSettings.xpPerObject = saveObj.gameSettings.xpPerObject;
        TrinaxGlobal.Instance.gameSettings.xpPerObjectMultipler = saveObj.gameSettings.xpPerObjectMultipler;
        TrinaxGlobal.Instance.gameSettings.MAX_XP_REQUIRED_PER_BONUS = saveObj.gameSettings.MAX_XP_REQUIRED_PER_BONUS;
        TrinaxGlobal.Instance.gameSettings.timeToActivateNextPattern = saveObj.
            gameSettings.timeToActivateNextPattern;
        TrinaxGlobal.Instance.gameSettings.DifficultyIncreaseInterval = saveObj.gameSettings.DifficultyIncreaseInterval;
        TrinaxGlobal.Instance.gameSettings.probabilityScale = saveObj.gameSettings.probabilityScale;
        TrinaxGlobal.Instance.gameSettings.firstAidRecover = saveObj.gameSettings.firstAidRecover;
        TrinaxGlobal.Instance.gameSettings.pointsPerCoin = saveObj.gameSettings.pointsPerCoin;
        TrinaxGlobal.Instance.gameSettings.coinFallSpeed = saveObj.gameSettings.coinFallSpeed;
        TrinaxGlobal.Instance.gameSettings.invunerableDuration = saveObj.gameSettings.invunerableDuration;
        TrinaxGlobal.Instance.gameSettings.umbrellaProbabilityChance = saveObj.gameSettings.umbrellaProbabilityChance;

        TrinaxGlobal.Instance.gameSettings.GameObjs = saveObj.gameSettings.GameObjs;
    }

    void PopulateAudioSettings()
    {
        TrinaxManager.trinaxAudioManager.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.MASTER].slider.value = saveObj.audioSettings.masterVolume;
        TrinaxManager.trinaxAudioManager.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.MUSIC].slider.value = saveObj.audioSettings.musicVolume;
        TrinaxManager.trinaxAudioManager.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.SFX].slider.value = saveObj.audioSettings.SFXVolume;
        TrinaxManager.trinaxAudioManager.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.SFX2].slider.value = saveObj.audioSettings.SFX2Volume;
        TrinaxManager.trinaxAudioManager.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.SFX3].slider.value = saveObj.audioSettings.SFX3Volume;
        TrinaxManager.trinaxAudioManager.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.SFX4].slider.value = saveObj.audioSettings.SFX4Volume;
        TrinaxManager.trinaxAudioManager.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.UI_SFX].slider.value = saveObj.audioSettings.UI_SFXVolume;
        TrinaxManager.trinaxAudioManager.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.UI_SFX2].slider.value = saveObj.audioSettings.UI_SFX2Volume;
    }

    void PopulateKinectSettings()
    {
        TrinaxGlobal.Instance.kinectSettings.isTrackingBody = saveObj.kinectSettings.isTrackingBody;
        TrinaxGlobal.Instance.kinectSettings.isTrackingHead = saveObj.kinectSettings.isTrackingHead;
        TrinaxGlobal.Instance.kinectSettings.minUserDistance = saveObj.kinectSettings.minUserDistance;
        TrinaxGlobal.Instance.kinectSettings.maxUserDistance = saveObj.kinectSettings.maxUserDistance;
        TrinaxGlobal.Instance.kinectSettings.maxLeftRightDistance = saveObj.kinectSettings.maxLeftRightDistance;
    }

    public void LoadJson()
    {
        string loadJsonString = JsonFileUtility.LoadJsonFromFile(ADMINSAVEFILE, JSONSTATE.PERSISTENT_DATA_PATH);
        saveObj = JsonUtility.FromJson<TrinaxSaves>(loadJsonString);

        // Assign our values back!
        if (saveObj != null)
        {
            PopulateGlobalSettings();
            PopulateGameSettings();
            PopulateAudioSettings();
            PopulateKinectSettings();
        }
        else
        {
            Debug.Log("Json file is empty! Creating a new save file...");
            saveObj = CreateAdminSave();
        }
    }
}
