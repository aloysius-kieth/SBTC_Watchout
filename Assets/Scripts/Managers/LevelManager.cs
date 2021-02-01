using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DIFFICULTY_LEVELS
{
    BEGINNER,
    INTERMEDIATE,
    ADVANCED,
}

public class LevelManager : MonoBehaviour
{
    #region SINGLETON 
    public static LevelManager Instance { get; set; }
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }
    #endregion

    public System.Action OnReachedNextlevel;

    public DIFFICULTY_LEVELS difficulty;

    const float BASE_DIFFICULTY = 1f;
    const float DIFFICULTY_MULTIPLIER = 1.025f;
    const float SPAWN_TIME_MULTIPLIER = 1.105f;
    const int MAX_DIFFICULTY = 10; // max level
    const float MAX_SPAWN_INTERVAL = 0.5f; // max spawn timer interval    

    public float DifficultyIncreaseInterval { get; set; }

    [Range(1, MAX_DIFFICULTY)]
    public int currentLevel = 1;
    float percent = 0.1f;
    int gameTimer = 0;

    float difficultyMultiplier = 0f;
    float spawnTimerMultiplier = 0f;

    bool timerStarted = false;
    public void StartGameTimer()
    {
        timerStarted = true;
    }

    public bool reachedMaxDifficulty = false;

    void Start()
    {
        Setup();
    }

    void Setup()
    {
        SetDifficulty(currentLevel);
    }

    public void PopulateValues(GameSettings setting)
    {
        // TODO: adminpanel
        DifficultyIncreaseInterval = setting.DifficultyIncreaseInterval;
    }

    void SetDifficulty(int level)
    {
        int multiple = MAX_DIFFICULTY / 10;

        if (level >= 1 && level <= MAX_DIFFICULTY / multiple)
        {
            difficulty = DIFFICULTY_LEVELS.BEGINNER;
        }
        else if (level > MAX_DIFFICULTY / 3 && level <= MAX_DIFFICULTY - (MAX_DIFFICULTY / multiple))
        {
            difficulty = DIFFICULTY_LEVELS.INTERMEDIATE;
        }
        else
        {
            difficulty = DIFFICULTY_LEVELS.ADVANCED;
        }
    }

    void ReachedNextDifficultyLevel()
    {
        if (currentLevel >= MAX_DIFFICULTY)
        {
            reachedMaxDifficulty = true;
            Debug.Log("Max difficulty reached!");
            return;
        }
        currentLevel++;

        OnReachedNextlevel?.Invoke();
        SetDifficulty(currentLevel);
        difficultyMultiplier = BASE_DIFFICULTY * Mathf.Pow(DIFFICULTY_MULTIPLIER, currentLevel);
        //Debug.Log(difficultyMultiplier);
        IncreaseDifficulty();

        if (SpawnManager.Instance.spawner.spawnTimer.interval <= MAX_SPAWN_INTERVAL)
        {
            Debug.Log("Max spawn timer reached!");
            SpawnManager.Instance.spawner.spawnTimer.interval = MAX_SPAWN_INTERVAL;
            return;
        }
        spawnTimerMultiplier = 1.0f * Mathf.Pow(SPAWN_TIME_MULTIPLIER, currentLevel);
        //Debug.Log(spawnTimerMultiplier);
        SpawnManager.Instance.spawner.spawnTimer.interval /= spawnTimerMultiplier;
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    ReachedNextDifficultyLevel();
        //}

        if (AppManager.gameManager.IsGameover) return;

        if (timerStarted)
        {
            StartCoroutine(RunLevelTimer());
            timerStarted = false;
        }

        if (gameTimer > DifficultyIncreaseInterval && gameTimer != 0 && difficulty != DIFFICULTY_LEVELS.ADVANCED)
        {
            gameTimer = 0;
            ReachedNextDifficultyLevel();
        }

    }

    void IncreaseDifficulty()
    {
        for (int i = 0; i < ObjectPooler.Instance.pooledObjects.Count; i++)
        {
            if (ObjectPooler.Instance.pooledObjects[i].GetComponent<FallingObject>() != null)
            {
                FallingObject obj = ObjectPooler.Instance.pooledObjects[i].GetComponent<FallingObject>();

                //float temp = obj.timeScale;
                //float scaled = temp / difficultyMultiplier;
                obj.timeScale *= difficultyMultiplier;
            }
        }
    }

    IEnumerator RunLevelTimer()
    {
        while (!AppManager.gameManager.IsGameover && difficulty != DIFFICULTY_LEVELS.ADVANCED)
        {
            if (AppManager.gameManager.bonusModeController.isBonusModeStarted) yield break;

            gameTimer++;
            //Debug.Log(gameTimer);
            yield return new WaitForSeconds(1);
        }

        if (difficulty == DIFFICULTY_LEVELS.ADVANCED) Debug.Log("Reached max difficulty!");

        Debug.Log("Level timer stopped!");
    }

    public void Reset()
    {
        difficulty = DIFFICULTY_LEVELS.BEGINNER;
        gameTimer = 0;
        currentLevel = 1;
        reachedMaxDifficulty = false;
    }

}
