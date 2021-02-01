using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum SPAWN_MODE
{
    SPAWN_RANDOMLY,
    SPAWN_IN_A_ROW,
    //SPAWN_IN_A_ROW_DELAYED,
    TOTAL,
    DEFAULT = SPAWN_RANDOMLY,
};

public class SpawnManager : MonoBehaviour
{
    #region SINGLETON
    public static SpawnManager Instance { get; set; }
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }
    #endregion

    // Current spawn mode
    public SPAWN_MODE currentSpawnMode;

    // Timer to activate next pattern
    float timer = 0f;
    public float Interval { get; set; }

    const int MAX_PROBABILITY_WEIGHT = 100;
    public int ProbabilityScale { get; set; }

    public bool StartTimer { get; set; }

    public Spawner spawner;
    public CoinSpawner coinSpawner;
    public PowerupSpawner powerupSpawner;

    // Drop table that contains items that can spawn
    public GenericLootDropTableString dropTable;

    void Start()
    {
        Setup();
    }

    public void Init()
    {

    }

    //void OnValidate()
    //{
    //    dropTable.ValidateTable();
    //}

    void Setup()
    {
        dropTable.ValidateTable();
        LevelManager.Instance.OnReachedNextlevel += OnReachedNextlevel;

        //if (GameObject.FindGameObjectWithTag("SpawnPoint") != null)
        //{
        //    GameObject temp = GameObject.FindGameObjectWithTag("SpawnPoint");
        //    spawner = temp.GetComponent<Spawner>();
        //}

        //if (GameObject.FindGameObjectWithTag("CoinSpawner") != null)
        //{
        //    GameObject temp = GameObject.FindGameObjectWithTag("CoinSpawner");
        //    coinSpawner = temp.GetComponent<CoinSpawner>();
        //}
        //if (GameObject.FindGameObjectWithTag("PowerupSpawner") != null)
        //{
        //    GameObject temp = GameObject.FindGameObjectWithTag("PowerupSpawner");
        //    powerupSpawner = temp.GetComponent<PowerupSpawner>();
        //}
        currentSpawnMode = SPAWN_MODE.DEFAULT;
    }

    public void PopulateValues(GameSettings setting)
    {
        ProbabilityScale = setting.probabilityScale;
        Interval = setting.timeToActivateNextPattern;
        spawner.spawnTimer.PopulateValues(setting);
    }

    void OnReachedNextlevel()
    {
        ModifyDropTableProbabilites();
    }

    void ModifyDropTableProbabilites()
    {
        for (int i = 0; i < dropTable.lootDropItems.Count; i++)
        {
            GenericLootDropItemString drop = dropTable.lootDropItems[i];
            if (drop.probabilityWeight >= MAX_PROBABILITY_WEIGHT)
            {
                drop.probabilityWeight = MAX_PROBABILITY_WEIGHT;
                continue;
            }
            else
            {
                drop.probabilityWeight += ProbabilityScale;
            }
        }

        dropTable.ValidateTable();
    }

    // TODO: change this hardcoded shit
    void ResetDropTableProbabilites()
    {
        for (int i = 0; i < dropTable.lootDropItems.Count; i++)
        {
            GenericLootDropItemString drop = dropTable.lootDropItems[i];
            if (drop.item == "Banana")
            {
                drop.probabilityWeight = 70;
            }
            else if (drop.item == "Sock")
            {
                drop.probabilityWeight = 50;
            }
            else if (drop.item == "Slipper")
            {
                drop.probabilityWeight = 50;
            }
            else if (drop.item == "Newspaper")
            {
                drop.probabilityWeight = 35;
            }
            else if (drop.item == "Bottle")
            {
                drop.probabilityWeight = 25;
            }
            else if (drop.item == "Flowerpot")
            {
                drop.probabilityWeight = 20;
            }
            else if (drop.item == "Microwave")
            {
                drop.probabilityWeight = 4;
            }
        }

        dropTable.ValidateTable();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            ResetDropTableProbabilites();
           //SpawnInARow();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            //ObjectPooler.Instance.ReturnAllToPool();
            //AppManager.gameManager.fallingObjectsList.Clear();
            ModifyDropTableProbabilites();
        }

        if (AppManager.gameManager.IsGameover || TrinaxGlobal.Instance.state != STATES.GAME) return;

        if (StartTimer)
        {
            timer += Time.deltaTime;
            if (timer > Interval)
            {
                timer = 0;
                StartTimer = false;
                PlayVoiceOver();
                PickPattern();
            }
        }
    }

    public void SetPattern(SPAWN_MODE mode)
    {
        if (AppManager.gameManager.bonusModeController.isBonusModeActive) return;

        currentSpawnMode = mode;
        switch (mode)
        {
            case SPAWN_MODE.SPAWN_RANDOMLY:
                SpawnRandomly();
                break;
            case SPAWN_MODE.SPAWN_IN_A_ROW:
                SpawnInARow();
                break;
        }
    }

    public void DeactivateBehaviour()
    {
        spawner.StopSpawning();
        StartTimer = false;

        timer = 0;
    }

    public void ActivateBehaviour()
    {
        SetPattern(SPAWN_MODE.DEFAULT);
        StartTimer = true;
    }

    void PlayVoiceOver()
    {
        spawner.StopSpawning();
       //Debug.Log("Play VO");
    }

    void PickPattern()
    {
        int index = Random.Range((int)SPAWN_MODE.SPAWN_IN_A_ROW, (int)SPAWN_MODE.SPAWN_IN_A_ROW);
        SPAWN_MODE mode = (SPAWN_MODE)index;
        SetPattern(mode);
    }

    void SpawnRandomly()
    {
        // activate spawner bool
        spawner.StartSpawning();
    }

    public string GetRandomObject()
    {
        GenericLootDropItemString obj = dropTable.PickLootDropItem();
        //Debug.Log(obj.item);
        return obj.item;
    }

    async void SpawnInARow()
    {
        spawner.StopSpawning();

        Debug.Log("Spawning in a row...");
        string typeStr = GetRandomObject();

        GameObject obj = ObjectPooler.Instance.GetPooledObject(typeStr);
        float objboundX = obj.GetComponent<FallingObject>().GetObjColBounds().x;
        //Debug.Log(obj.name + " | " + objboundX);

        int objectBounds = Mathf.RoundToInt(objboundX);
        //Debug.Log(objectBounds);
        Vector3 calcuSpawnBounds = spawner.GetBounds();
        int segmentVal;
        segmentVal = Mathf.RoundToInt(Mathf.RoundToInt(calcuSpawnBounds.x) / objectBounds);
        //Debug.Log(segmentVal);
        GameObject[] objs = new GameObject[segmentVal]; // 0 - 17
        int[] skipIndexes;
        if (typeStr == FALLING_TYPES.Banana.ToString())
        {
            skipIndexes = new int[5];
        }
        else
        {
            skipIndexes = new int[4];
        }
        skipIndexes[0] = Random.Range(skipIndexes.Length, objs.Length - skipIndexes.Length);
        //Debug.Log(skipIndexes[0]);

        if (skipIndexes[0] == objs.Length - 1)
        {
            Debug.Log("spawning from right");
            int j = 1;
            for (int i = 0; i < skipIndexes.Length; i++)
            {
                skipIndexes[i] = skipIndexes[0] - i;
                j++;
            }
            //skipIndexes[1] = skipIndexes[0] - 1;
            //skipIndexes[2] = skipIndexes[1] - 2;
            //skipIndexes[3] = skipIndexes[1] - 3;
            //skipIndexes[4] = skipIndexes[1] - 4;

            //if (typeStr == FALLING_TYPES.Banana.ToString())
            //{
            //    skipIndexes[2] = skipIndexes[0] - 2;
            //}
        }
        else
        {
            Debug.Log("spawning from left");
            int j = 1;
            for (int i = 0; i < skipIndexes.Length; i++)
            {
                skipIndexes[i] = skipIndexes[0] + i;
                j++;
            }
            //skipIndexes[1] = skipIndexes[0] + 1;
            //skipIndexes[2] = skipIndexes[1] + 2;
            //skipIndexes[3] = skipIndexes[1] + 3;
            //skipIndexes[4] = skipIndexes[1] + 4;
            //if (typeStr == FALLING_TYPES.Banana.ToString())
            //{
            //    skipIndexes[2] = skipIndexes[0] + 2;
            //}
        }
        for (int i = 0; i < skipIndexes.Length; i++)
        {
            Debug.Log(skipIndexes[i]);
        }
        int _numOfTimes = 0;
        while (_numOfTimes < 1 && !AppManager.gameManager.bonusModeController.isBonusModeActive && !AppManager.gameManager.bonusModeController.isBonusModeStarted)
        {
            int startingPos = Mathf.RoundToInt(spawner.minBoundsX);

            int x = startingPos;
            for (int i = 0; i < objs.Length; i++)
            {
                objs[i] = ObjectPooler.Instance.GetPooledObject(typeStr);
                FallingObject falling = objs[i].GetComponent<FallingObject>();
                AppManager.gameManager.fallingObjectsList.Add(objs[i].GetComponent<FallingObject>());

                if (i == 0)
                {
                    x = startingPos;
                    //Debug.Log("Starting pos: " + objs[i].name + x);
                }
                else
                {
                    x += objectBounds;
                    //Debug.Log(objs[i].name + x);
                }

                objs[i].transform.localPosition = new Vector3(x, spawner.maxBoundsY, 0);
                objs[i].SetActive(true);
                falling.shadow.SetShadowOnGround();
                for (int j = 0; j < skipIndexes.Length; j++)
                {
                    if (skipIndexes[j] == i)
                    {
                        objs[i].SetActive(false);
                    }
                }
                _numOfTimes++;
            }

            //Debug.Log("Spawned in a row: " + _numOfTimes);
            //await new WaitForSeconds(0.1f);
            //Debug.Log("waited for " + interval);
        }

        await new WaitUntil(() => AppManager.gameManager.fallingObjectsList.Count <= 1);
        SetPattern(SPAWN_MODE.DEFAULT);
        StartTimer = true;
    }

    public void Reset()
    {
        timer = 0;
        spawner.Reset();
        powerupSpawner.Reset();
        ResetDropTableProbabilites();
    }
}
