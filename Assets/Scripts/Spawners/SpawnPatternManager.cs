using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// default pattern
// run timer
// select pattern
// forewarn incoming pattern (VO and visual)
// run pattern
// default pattern
public enum SPAWN_PATTERNS
{
    SPAWN_RANDOMLY,
    SPAWN_IN_A_ROW,
    SPAWN_IN_A_ROW_DELAYED,
    TOTAL,
    DEFAULT = SPAWN_RANDOMLY,
};

public class SpawnPatternManager : MonoBehaviour
{
    //#region SINGLETON
    //public static SpawnPatternManager Instance { get; set; }
    //private void Awake()
    //{
    //    //DontDestroyOnLoad(this);
    //    if (Instance != null && Instance != this) Destroy(gameObject);
    //    else Instance = this;
    //}
    //#endregion

    //public SPAWN_PATTERNS currentPattern;
    //public List<SPAWN_PATTERNS> PatternsList;

    //const int PREDEFINED_NUM_OF_PATTERN = 100;
    //float[] offset;

    //float timer;
    //float interval = 9f;

    //bool startTimer = false;
    //public void StartTimer() { startTimer = true; }
    //public void StopTimer() { startTimer = false; }

    //async void Start()
    //{
    //    await new WaitUntil(() => TrinaxGlobal.Instance.isReady);
    //    Setup();
    //}

    //void Setup()
    //{
    //    // first pattern will always be default
    //    currentPattern = SPAWN_PATTERNS.DEFAULT;

    //    // random a list of patterns
    //    InitPatternList();
    //}

    //public void Reset()
    //{
    //    StopTimer();
    //    currentPattern = SPAWN_PATTERNS.DEFAULT;
    //    timer = 0;

    //    PatternsList.Clear();

    //    InitPatternList();
    //}

    //void InitPatternList()
    //{
    //    PatternsList = new List<SPAWN_PATTERNS>();
    //    for (int i = 0; i < PREDEFINED_NUM_OF_PATTERN; i++)
    //    {
    //        int index = Random.Range((int)SPAWN_PATTERNS.SPAWN_IN_A_ROW, (int)SPAWN_PATTERNS.SPAWN_IN_A_ROW);
    //        SPAWN_PATTERNS pattern = (SPAWN_PATTERNS)index;
    //        PatternsList.Add(pattern);
    //    }
    //    PatternsList.Shuffle();
    //}

    //async void PickPattern()
    //{
    //    StopTimer();
    //    SPAWN_PATTERNS pattern = PatternsList.First();
    //    PatternsList.RemoveAt(0);

    //    Debug.Log("Do VO");
    //    await new WaitForSeconds(3f);
    //    Debug.Log("After VO");
    //    SetPattern(pattern);
    //}

    //void Update()
    //{
    //    if (GameManager.Instance.IsGameover || TrinaxGlobal.Instance.state != PAGES.GAME) return;
    //    //if (currentPattern == SPAWN_PATTERNS.SPAWN_IN_A_ROW || currentPattern == SPAWN_PATTERNS.SPAWN_IN_A_ROW_DELAYED) return;

    //    if (startTimer)
    //    {
    //        timer += Time.deltaTime;
    //        if (timer > interval)
    //        {
    //            timer = 0;
    //            PickPattern();
    //        }
    //    }
    //}

    //int numOfTimes;
    //int intervalBetweenSpawn;
    //float delaySpawnInterval;
    //public async void SetPattern(SPAWN_PATTERNS pattern)
    //{
    //    if(SpawnManager.Instance.spawner.spawnTimer.StartSpawn)SpawnManager.Instance.spawner.StopSpawning();

    //    await new WaitForSeconds(0.5f);

    //    RandomizeValues();
    //    currentPattern = pattern;
    //    switch (pattern)
    //    {
    //        case SPAWN_PATTERNS.SPAWN_RANDOMLY:
    //            SpawnRandomly();
    //            break;
    //        case SPAWN_PATTERNS.SPAWN_IN_A_ROW:
    //            SpawnInARow();
    //            break;
    //        case SPAWN_PATTERNS.SPAWN_IN_A_ROW_DELAYED:
    //            SpawnInARowDelayed();
    //            break;
    //        default:
    //            Debug.LogWarning("No such pattern exist!");
    //            break;
    //    }
    //}

    //void RandomizeValues()
    //{
    //    numOfTimes = Random.Range(1,1);
    //    intervalBetweenSpawn = Random.Range(1, 1);
    //    delaySpawnInterval = Random.Range(0.1f, 0.4f);
    //}

    //void SpawnRandomly()
    //{
    //    Debug.Log("spawning randomly");
    //    SpawnManager.Instance.spawner.StartSpawning();
    //}

    //async void SpawnInARow()
    //{
    //    Debug.Log("Spawning in a row...");
    //    int _numOfTimes = 0;
    //    Spawner spawner = SpawnManager.Instance.spawner;
    //    string typeStr = RandomWeightsManager.Instance.GetRandomObject();

    //    GameObject obj = ObjectPooler.Instance.GetPooledObject(typeStr);
    //    float objboundX = obj.GetComponent<FallingObject>().GetObjColBounds().x;
    //    //Debug.Log(obj.name + " | " + objboundX);

    //    int objectBounds = Mathf.RoundToInt(objboundX);
    //    //Debug.Log(objectBounds);
    //    Vector3 calcuSpawnBounds = spawner.GetBounds();
    //    int segmentVal;
    //    segmentVal = Mathf.RoundToInt(Mathf.RoundToInt(calcuSpawnBounds.x) / objectBounds);
    //    //Debug.Log(segmentVal);
    //    GameObject[] objs = new GameObject[segmentVal + 1];
    //    int[] skipIndexes = new int[3];
    //    while (_numOfTimes < numOfTimes)
    //    {
    //        skipIndexes[0] = Random.Range(0, objs.Length);

    //        if (skipIndexes[0] >= objs.Length - 1)
    //        {
    //            skipIndexes[1] = skipIndexes[0] - 1;
    //            skipIndexes[2] = skipIndexes[0] - 2;
    //        }
    //        else
    //        {
    //            skipIndexes[1] = skipIndexes[0] + 1;
    //            skipIndexes[2] = skipIndexes[0] + 2;
    //        }

    //        //Debug.Log("Skipped index: " + skipIndex);
    //        //Debug.Log("Skipped index2: " + skipIndex2);
    //        //Debug.Log("Skipped index3: " + skipIndex3);
    //        int startingPos = Mathf.RoundToInt(spawner.minBounds);

    //        int x = startingPos;
    //        for (int i = 0; i < objs.Length; i++)
    //        {
    //            objs[i] = ObjectPooler.Instance.GetPooledObject(typeStr);
    //            GameManager.Instance.fallingObjectsList.Add(objs[i].GetComponent<FallingObject>());

    //            if (i == 0) x = startingPos;
    //            else x += objectBounds;

    //            objs[i].transform.localPosition = new Vector3(x, 11f, 0);
    //            if (i == skipIndexes[0] || i == skipIndexes[1] || i == skipIndexes[2])
    //            {
    //                objs[i].SetActive(false);
    //                //Debug.Log(objs[i]);
    //            }
    //            else
    //            {
    //                objs[i].SetActive(true);
    //            }
    //            //Debug.Log(x);
    //        }
    //        _numOfTimes++;

    //        //Debug.Log("Spawned in a row: " + _numOfTimes);
    //        await new WaitForSeconds(intervalBetweenSpawn);
    //        //Debug.Log("waited for " + interval);
    //    }
    //    SpawnManager.Instance.spawner.StopSpawning();

    //    SetPattern(SPAWN_PATTERNS.DEFAULT);
    //    await new WaitUntil(() => GameManager.Instance.fallingObjectsList.Count > 0);
    //    StartTimer();
    //}

    //GameObject[] objs;
    //async void SpawnInARowDelayed()
    //{
    //    bool startFromLeft = true;
    //    int _numOfTimes = 0;

    //    //Debug.Log("Spawning in a row...");
    //    Spawner spawner = SpawnManager.Instance.spawner;
    //    string typeStr = RandomWeightsManager.Instance.GetRandomObject();
    //    GameObject obj = ObjectPooler.Instance.GetPooledObject(typeStr);
    //    float objboundX = obj.GetComponent<FallingObject>().GetObjColBounds().x;
    //    //Debug.Log(obj.name + " | " + objboundX);

    //    int objectBounds = Mathf.RoundToInt(objboundX);
    //    //Debug.Log(objectBounds);
    //    Vector3 calcuSpawnBounds = spawner.GetBounds();
    //    int segmentVal;
    //    segmentVal = Mathf.RoundToInt(Mathf.RoundToInt(calcuSpawnBounds.x) / objectBounds);
    //    //Debug.Log(segmentVal);
    //    objs = new GameObject[segmentVal + 1];

    //    offset = new float[objs.Length];
    //    SetOffsetTime(delaySpawnInterval);

    //    while (_numOfTimes < numOfTimes)
    //    {
    //        int skipIndex = Random.Range(0, objs.Length);
    //        int skipIndex2;
    //        if (skipIndex >= objs.Length - 1) skipIndex2 = skipIndex - 1;
    //        else { skipIndex2 = skipIndex + 1; }
    //        //Debug.Log("Skipped index: " + skipIndex);
    //       // Debug.Log("Skipped index2: " + skipIndex2);
    //        int startingPos = Mathf.RoundToInt(spawner.minBounds);
    //        int x = 0;
    //        if (Random.value > 0.9f)
    //        {
    //            x = startingPos;
    //            startFromLeft = true;
    //        }
    //        else
    //        {
    //            x = -startingPos;
    //            startFromLeft = false;
    //        }

    //        for (int i = 0; i < objs.Length; i++)
    //        {
    //            objs[i] = ObjectPooler.Instance.GetPooledObject(typeStr);

    //            if (startFromLeft)
    //            {
    //                if (i == 0) x = startingPos;
    //                else x += objectBounds;
    //            }
    //            else
    //            {
    //                if (i == 0) x = -startingPos;
    //                else x -= objectBounds;
    //            }

    //            objs[i].transform.localPosition = new Vector3(x, 11f, 0);
    //            if (i == skipIndex || i == skipIndex2)
    //            {
    //                objs[i].SetActive(false);
    //                Debug.Log(objs[i]);
    //            }
    //            else
    //            {
    //                objs[i].SetActive(true);
    //            }
    //                await new WaitForSeconds(0.5f);
    //            //Debug.Log(x);
    //        }

    //        _numOfTimes++;

    //        // Debug.Log("Spawned in a row: " + _numOfTimes);
    //        await new WaitForSeconds(intervalBetweenSpawn);
    //        //Debug.Log("waited for " + intervalBetweenSpawn);
    //    }
    //    SpawnManager.Instance.spawner.StopSpawning();

    //    SetPattern(SPAWN_PATTERNS.DEFAULT);
    //    await new WaitUntil(() => GameManager.Instance.fallingObjectsList.Count > 0);
    //    StartTimer();
    //}

    //void SetOffsetTime(float factor)
    //{
    //    for (int i = 1; i < offset.Length; i++)
    //    {
    //        offset[i] = (i + factor) / 10;
    //        //Debug.Log(offset[i]);
    //    }
    //}
}
