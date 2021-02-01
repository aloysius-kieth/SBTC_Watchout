using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class handles timer portion of spawning only
public class SpawnTimer : MonoBehaviour
{
    public System.Action OnTimeToSpawn;

    float timer;
    public float Timer { get { return timer; } set { timer = value; } }
    public void SetTimer()
    {
        if (SpawnManager.Instance.currentSpawnMode == SPAWN_MODE.SPAWN_RANDOMLY)
            Timer = interval;
        else if (SpawnManager.Instance.currentSpawnMode == SPAWN_MODE.SPAWN_IN_A_ROW)
            Timer = 0;
        //else if (SpawnManager.Instance.currentSpawnMode == SPAWN_MODE.SPAWN_IN_A_ROW_DELAYED)
        //    return;
    }

    public float interval;

    public bool StartSpawn;

    async void Start()
    {
        await new WaitUntil(() => TrinaxGlobal.Instance.isReady);
        Setup();
    }

    void Setup()
    {

        SetTimer();
    }

    public void PopulateValues(GameSettings setting)
    {
        interval = setting.spawnInterval;
    }

    void Update()
    {
        if (AppManager.gameManager.IsGameover || TrinaxGlobal.Instance.state != STATES.GAME) return;
        if (AppManager.gameManager.bonusModeController.isBonusModeActive || AppManager.gameManager.bonusModeController.isBonusModeStarted) return;

        if (StartSpawn)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                SetTimer();
                // Do spawn
                OnTimeToSpawn?.Invoke();
            }
        }
    }

    public void Reset()
    {
        timer = 0;
        PopulateValues(TrinaxGlobal.Instance.gameSettings);
    }
  
}
