using UnityEngine;
using System.Linq;
using System.Collections.Generic;

// Use this for storing user's data
[System.Serializable]
public class UserData
{
    public string name = "";
    public string score = "";
    public string interactionID = "";

    public void Clear()
    {
        name = "";
        score = "";
    }
}

/// <summary>
/// Global Manager
/// </summary>
public class TrinaxGlobal : MonoBehaviour
{
    #region SINGLETON
    public static TrinaxGlobal Instance { get; set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }
    #endregion

    public string[] AllGameObjTypesStr;

    [Header("Settings")]
    public GlobalSettings globalSettings;
    public GameSettings gameSettings;
    public KinectSettings kinectSettings;

    public bool IsAppPaused { get; set; }
    public bool isReady = false;
    public bool loadNow = false;
    public bool isChangingLevels = false;

    public STATES state;
    public SCENE scene;

    public UserData userData = new UserData();

    private TrinaxAdminPanel aP;

    private IManager[] managers;
    private List<KeyValuePair<IManager, int>> IManagers = new List<KeyValuePair<IManager, int>>();

    private void Start()
    {
#if !UNITY_EDITOR
    Cursor.visible = false;
#endif
        AllGameObjTypesStr = System.Enum.GetNames(typeof(FALLING_TYPES));
        scene = SCENE.MAIN;

        // Assign component references
        SetComponentReferences();
        // Collate all managers that derive from IManager
        AddIManagers();
        // Load our Managers in order
        LoadManagers();
        // Assign settings and finally load game logic
        Init();
    }

    #region Init Managers
    private async void Init()
    {
        if (!loadNow) return;

        Debug.Log("Initializing...");
        await new WaitUntil(() => !loadNow);
        Debug.Log("Initializing Done!");

        // Indicate that everything is ready
        isReady = true;
        Debug.Log("All managers loaded!");

        // *** Here all managers should be fully loaded. Do whatever you want now! *** //

        if (string.IsNullOrEmpty(globalSettings.IP))
        {
            Debug.Log("Mandatory fields in admin panel not filled!" + "\n" + "Opening admin panel...");
            aP.gameObject.SetActive(true);
        }
        else
        {
            aP.gameObject.SetActive(false);
        }

        RefreshSettings();
    }

    private void AddIManagers()
    {
        for (int i = 0; i < managers.Length; i++)
        {
            IManagers.Add(new KeyValuePair<IManager, int>(managers[i], managers[i].ExecutionPriority));
        }

        // Sort by execution order
        IManagers.Sort((order1, order2) => order1.Value.CompareTo(order2.Value));
    }

    async void LoadManagers()
    {
        Debug.Log("Waiting for managers to be loaded...");
        loadNow = true;

        for (int i = 0; i < IManagers.Count; i++)
        {
            await IManagers[i].Key.Init();
            bool ready = IManagers[i].Key.IsReady;
            await new WaitUntil(() => ready);
        }

        loadNow = false;
    }
    #endregion

    private void Update()
    {
        if (Application.isPlaying)
        {
            if (Time.frameCount % 30 == 0)
            {
                System.GC.Collect(1, System.GCCollectionMode.Optimized, false, false);
            }
        }
    }

    private void SetComponentReferences()
    {
        aP = TrinaxManager.trinaxCanvas.adminPanel;
        //if(aP != null)
        //{
        //    if (!aP.gameObject.activeSelf)
        //    {
        //        aP.gameObject.SetActive(true);
        //    }
        //}
        managers = GetComponentsInChildren<IManager>();
    }

    public void RefreshSettings()
    {
#if UNITY_STANDALONE_WIN
        if (KinectController.Instance != null)
            KinectController.Instance.PopulateValues(kinectSettings);
        else Debug.LogWarning("<KinectController> values not populated!");
#endif

        if (LevelManager.Instance != null)
            LevelManager.Instance.PopulateValues(gameSettings);
        else Debug.LogWarning("<LevelManager> values not populated!");

        // TODO: move this to object property manager?
        if (ObjectPooler.Instance.pooledObjects != null && ObjectPooler.Instance.pooledObjects.Count > 0)
        {
            for (int i = 0; i < ObjectPooler.Instance.pooledObjects.Count; i++)
            {
                if (ObjectPooler.Instance.pooledObjects[i].GetComponent<FallingObject>() != null)
                {
                    FallingObject obj = ObjectPooler.Instance.pooledObjects[i].GetComponent<FallingObject>();

                    obj.PopulateValues(gameSettings);
                }
            }
        }
        else
            Debug.LogWarning("<Object properties> values not populated!");

        if (XPManager.Instance != null)
            XPManager.Instance.PopulateValues(gameSettings);
        else
            Debug.LogWarning("<XPManager> values not populated!");

        if (SpawnManager.Instance != null)
            SpawnManager.Instance.PopulateValues(gameSettings);
        else
            Debug.LogWarning("<SpawnManager> values not populated!");

        if (TrinaxManager.trinaxAsyncServerManager != null)
            TrinaxManager.trinaxAsyncServerManager.PopulateValues(globalSettings);
        else
            Debug.LogWarning("<TrinaxAsyncServerManager> values not populated!");

        if (TrinaxManager.trinaxAudioManager != null)
            TrinaxManager.trinaxAudioManager.PopulateValues();
        else
            Debug.LogWarning("<TrinaxAudioManager> values not populated!");

        if (AppManager.uiManager != null)
            AppManager.uiManager.PopulateSettings(globalSettings);
        else
            Debug.LogWarning("<GameManager> values not populated!");

        if (AppManager.gameManager.player != null)
            AppManager.gameManager.player.PopulateValues(globalSettings, gameSettings);
        else
            Debug.LogWarning("<Player> values not populated!");
    }
}
