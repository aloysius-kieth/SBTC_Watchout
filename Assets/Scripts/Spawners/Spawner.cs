using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This shall only do spawning of falling objects and nothing else. 
public class Spawner : MonoBehaviour
{
    Vector3 spawnPosition;
    [HideInInspector]
    public SpawnTimer spawnTimer;
    public string objToSpawn;

    Collider2D col;
    public float minBoundsX { get; set; }
    public float maxBoundsX { get; set; }
    public float maxBoundsY { get; set; }
    Vector3 boundsSize;

    const int MIN_RANDOM_RANDOMSPAWN = 1;
    const int MAX_RANDOM_RANDOMSPAWN = 3;

    public Vector3 GetBounds()
    {
        //Debug.Log("Spawner bounds: " + boundsSize)
; return boundsSize;
    }

    void Start()
    {
        Setup();
    }

    void Setup()
    {
        col = GetComponent<Collider2D>();
        spawnTimer = GetComponent<SpawnTimer>();

        spawnTimer.OnTimeToSpawn += OnTimeToSpawn;

        minBoundsX = col.bounds.min.x + 1;
        maxBoundsX = col.bounds.max.x - 1;
        maxBoundsY = col.bounds.max.y;
        boundsSize = col.bounds.size;

        //Debug.Log("Min Bounds: " + minBoundsX + " | " + "Max Bounds: " + maxBoundsX);
    }

    public Vector3 RandomizePositionOnBounds()
    {
        float posX = 0;
        float posY = 0;
        posX = Random.Range(minBoundsX, maxBoundsX);
        posY = Random.Range(-1.0f, 1.5f);

        spawnPosition = new Vector3(posX, maxBoundsY + posY);
        return spawnPosition;
    }

    void OnTimeToSpawn()
    {
        if (SpawnManager.Instance.currentSpawnMode == SPAWN_MODE.SPAWN_RANDOMLY || SpawnManager.Instance.currentSpawnMode == SPAWN_MODE.DEFAULT)
        {
            SpawnFromRandom(Random.Range(MIN_RANDOM_RANDOMSPAWN, MAX_RANDOM_RANDOMSPAWN + 1));
        }
        //else if (SpawnManager.Instance.currentSpawnMode == SPAWN_MODE.SPAWN_IN_A_ROW)
        //{
        //    SpawnFromPattern();
        //}
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    SpawnFromRandom(3);
        //}

        //if (Input.GetKeyDown(KeyCode.B))
        //{
        //    ObjectPooler.Instance.ReturnAllToPool();
        //    AppManager.gameManager.fallingObjectsList.Clear();
        //    tempObjects.Clear();
        //}
    }

    float offsetX;
    GameObject firstObject;
    // Spawns x number of objects at once
    void SpawnFromRandom(int num)
    {
        List<string> spawnList = new List<string>();
        List<GameObject> spawnObjects = new List<GameObject>();
        bool isLeft = false;

        if (SpawnManager.Instance.currentSpawnMode == SPAWN_MODE.SPAWN_RANDOMLY)
        {
            for (int i = 0; i < num; i++)
            {
                // get required number of objects to spawn
                //string objStr = RandomWeightsManager.Instance.GetRandomObject();
                string objStr = SpawnManager.Instance.GetRandomObject();
                spawnList.Add(objStr);
            }

            if (spawnList.Count > 0)
            {
                for (int j = 0; j < spawnList.Count; j++)
                {
                    GameObject spawn = ObjectPooler.Instance.GetPooledObject(spawnList[j]);
                    FallingObject falling = spawn.GetComponent<FallingObject>();
                    spawnObjects.Add(spawn);
                    //Debug.Log(j + " | " + spawn.name);
                    if (spawn != null)
                    {
                        // first object
                        if (j == 0)
                        {
                            firstObject = spawn;
                            spawn.transform.position = RandomizePositionOnBounds();
                            //Debug.Log(spawn.transform.position.x);

                            if (spawn.transform.position.x > maxBoundsX)
                            {
                                isLeft = true;
                                // special offset because image not cropped to object
                                if (spawnList[j] == "Microwave")
                                {
                                    offsetX = spawn.transform.position.x - spawn.GetComponent<SpriteRenderer>().bounds.size.x / 2.5f;
                                    //Debug.Log("offsetx " + offsetX);
                                }
                                else if (spawnList[j] == "Bottle")
                                {
                                    offsetX = spawn.transform.position.x - spawn.GetComponent<SpriteRenderer>().bounds.size.x;
                                }
                                else
                                {
                                    offsetX = spawn.transform.position.x - spawn.GetComponent<SpriteRenderer>().bounds.size.x * 2.0f;
                                }
                            }
                            else if (spawn.transform.position.x < minBoundsX)
                            {
                                isLeft = false;
                                if (spawnList[j] == "Microwave")
                                {
                                    offsetX = spawn.transform.position.x + spawn.GetComponent<SpriteRenderer>().bounds.size.x / 2.5f;
                                }
                                else if (spawnList[j] == "Bottle")
                                {
                                    offsetX = spawn.transform.position.x + spawn.GetComponent<SpriteRenderer>().bounds.size.x;
                                }
                                else
                                {
                                    offsetX = spawn.transform.position.x + spawn.GetComponent<SpriteRenderer>().bounds.size.x * 2.0f;
                                }
                            }
                            else if (spawn.transform.position.x < maxBoundsX && spawn.transform.position.x > minBoundsX)
                            {
                                if (spawn.transform.position.x > 0)
                                {
                                    isLeft = true;
                                    if (spawnList[j] == "Microwave")
                                    {
                                        offsetX = spawn.transform.position.x - spawn.GetComponent<SpriteRenderer>().bounds.size.x / 3.5f;
                                    }
                                    else if (spawnList[j] == "Bottle")
                                    {
                                        offsetX = spawn.transform.position.x - spawn.GetComponent<SpriteRenderer>().bounds.size.x;
                                    }
                                    else
                                    {
                                        offsetX = spawn.transform.position.x - spawn.GetComponent<SpriteRenderer>().bounds.size.x * 2.0f;
                                        //Debug.Log("rest left");
                                    }
                                }
                                else
                                {
                                    isLeft = false;
                                    if (spawnList[j] == "Microwave")
                                    {
                                        offsetX = spawn.transform.position.x + spawn.GetComponent<SpriteRenderer>().bounds.size.x / 3.5f;
                                    }
                                    else if (spawnList[j] == "Bottle")
                                    {
                                        offsetX = spawn.transform.position.x + spawn.GetComponent<SpriteRenderer>().bounds.size.x;
                                    }
                                    else
                                    {
                                        offsetX = spawn.transform.position.x + spawn.GetComponent<SpriteRenderer>().bounds.size.x * 2.0f;
                                        //Debug.Log("rest right");
                                    }
                                }
                            }
                        }
                        else
                        {
                            spawn.transform.position = new Vector3(offsetX, RandomizePositionOnBounds().y);
                            //Debug.Log("before 2nd obj: " + offsetX);
                            if (isLeft)
                            {
                                offsetX = offsetX - spawnObjects[spawnObjects.Count - 2].GetComponent<SpriteRenderer>().bounds.size.x - 0.5f;
                                //Debug.Log(offsetX);
                            }
                            else
                            {
                                offsetX = offsetX + spawnObjects[spawnObjects.Count - 2].GetComponent<SpriteRenderer>().bounds.size.x + 0.5f;
                                //Debug.Log(offsetX);
                            }
     

                            //Debug.Log(spawn.name + " | " + spawn.GetComponent<SpriteRenderer>().bounds.size.x);
                            //Debug.Log("offsetX 2nd " + offsetX);

                            //Debug.Log(j + " | " + spawn.transform.position);
                            //Debug.Log(spawn.name);
                            //Debug.Log(spawn.transform.position);
                        }

                        spawn.SetActive(true);
                        falling.shadow.SetShadowOnGround();
                        AppManager.gameManager.fallingObjectsList.Add(spawn.GetComponent<FallingObject>());
                        //tempObjects = new List<GameObject>(spawnObjects);
                    }
                }
                spawnObjects.Clear();
            }
            else Debug.LogWarning("Cannot spawn 0!");
        }
    }

    //void SpawnFromPattern()
    //{
    //    //if (SpawnManager.Instance.currentSpawnMode == SPAWN_MODE.SPAWN_RANDOMLY || SpawnManager.Instance.currentSpawnMode == SPAWN_MODE.DEFAULT)
    //    //{
    //    //    objToSpawn = RandomWeightsManager.Instance.GetRandomObject();
    //    //}

    //    GameObject spawned = ObjectPooler.Instance.GetPooledObject(objToSpawn);
    //    //Debug.Log(name + " | " + objToSpawn);

    //    //if (spawned != null)
    //    //{
    //    //    spawned.transform.position = RandomizePositionOnBounds();
    //    //    spawned.SetActive(true);
    //    //    AppManager.gameManager.fallingObjectsList.Add(spawned.GetComponent<FallingObject>());
    //    //}

    //    //if (SpawnPatternManager.Instance.currentPattern == SPAWN_PATTERNS.SPAWN_IN_A_ROW || SpawnPatternManager.Instance.currentPattern == SPAWN_PATTERNS.SPAWN_IN_A_ROW_DELAYED)
    //    //{
    //    //    StopSpawning();
    //    //}
    //}

    public void StartSpawning()
    {
        //Debug.Log("START SPAWNING");
        spawnTimer.SetTimer();
        spawnTimer.StartSpawn = true;
    }

    public void StopSpawning()
    {
        //Debug.Log("STOP SPAWNING");
        spawnTimer.StartSpawn = false;
        spawnTimer.SetTimer();
    }

    public void Reset()
    {
        spawnTimer.Reset();
    }

}
