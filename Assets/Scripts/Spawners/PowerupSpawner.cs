using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupSpawner : MonoBehaviour
{
    float timer;
    float interval;

    async void Start()
    {
        await new WaitUntil(() => TrinaxGlobal.Instance.isReady);
        Setup();
    }

    void Setup()
    {
        RandomizeTimeToSpawn();
    }

    void Update()
    {
        if (AppManager.gameManager.IsGameover || TrinaxGlobal.Instance.state != STATES.GAME)
            return;
        if (AppManager.gameManager.bonusModeController.isBonusModeStarted || AppManager.gameManager.bonusModeController.isBonusModeActive) return;

        timer += Time.deltaTime;
        if (timer > interval)
        {
            timer = 0;
            Spawn(RandomPowerupToSpawn());
            RandomizeTimeToSpawn();
        }
    }

    POWERUPS RandomPowerupToSpawn()
    {
        POWERUPS type;
        if (Random.value > TrinaxGlobal.Instance.gameSettings.umbrellaProbabilityChance)
        {
            type = POWERUPS.FirstAid_Powerup;
        }
        else
        {
            type = POWERUPS.Umbrella_Powerup;
        }

        return type;
    }

    void RandomizeTimeToSpawn()
    {
        interval = Random.Range(TrinaxGlobal.Instance.gameSettings.minPowerupSpawn, TrinaxGlobal.Instance.gameSettings.maxPowerupSpawn);
    }

    void Spawn(POWERUPS powerup)
    {
        //Debug.Log("Spawning " + powerup);
        GameObject spawned = ObjectPooler.Instance.GetPooledObject(powerup.ToString());

        if (spawned != null)
        {
            spawned.transform.localPosition = SpawnManager.Instance.spawner.RandomizePositionOnBounds();
            spawned.SetActive(true);
        }
        else { Debug.LogWarning(spawned + " does not exist!"); }
    }

    public void Reset()
    {
        timer = 0;
        RandomizeTimeToSpawn();
    }
}
