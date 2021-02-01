using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TrainingRoomManager : MonoBehaviour
{
    #region SINGLETON
    public static TrainingRoomManager Instance { get; set; }
    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }
    #endregion

    public List<TrainingSpawner> spawnerList = new List<TrainingSpawner>();
    public List<FALLING_TYPES> fallingList = new List<FALLING_TYPES>();


    public int Score { get; set; }
    public TextMeshProUGUI scoreText;

    public HUD hud;
    public TrainingRoomControls roomControls;

    async void Start ()
    {
        //await new WaitUntil(() => TrinaxGlobal.Instance.doneLoadComponentReferences);
        Init();
	}

    void Init()
    {
        TrinaxGlobal.Instance.state = STATES.GAME;
        AppManager.gameManager.IsGameover = false;

        //AppManager.gameManager.environment.Activate(true);
        hud.Show(true);

        Score = 0;
        scoreText.text = "0";

        if(AppManager.gameManager.player != null)
        {
            AppManager.gameManager.player.OnTrainingDeath += OnTrainingDeath;
        }
        else { Debug.LogWarning("<Player> reference not found!"); }

        InitSpawners();
    }

    void OnTrainingDeath()
    {
        AppManager.gameManager.player.TrainingDeathSequence(3);
    }

    void InitSpawners()
    {
        GameObject[] spawnedObjects = GameObject.FindGameObjectsWithTag("SpawnPoint");
        if (spawnedObjects.Length > 0)
        {
            for (int i = 0; i < spawnedObjects.Length; i++)
            {
                TrainingSpawner s = spawnedObjects[i].GetComponent<TrainingSpawner>();
                spawnerList.Add(s);
            }
        }
        else { Debug.LogWarning("No spawn points to be found in scene!"); }
    }

    public void SetSpawnTime()
    {
        for (int i = 0; i < spawnerList.Count; i++)
        {
            spawnerList[i].SetMinMaxSpawnTime(roomControls.traininglobalSettings.minSpawnTime, roomControls.traininglobalSettings.maxSpawnTime);
        }
    }

    public void AddScore(int amt)
    {
        Score += amt;
        scoreText.text = Score.ToString();
    }
}
