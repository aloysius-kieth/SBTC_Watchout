using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingSpawner : MonoBehaviour
{
    float timer = 0;
    float timeToSpawn;
    float minSpawnTime;
    float maxSpawnTime;

    FALLING_TYPES _maxDrop;

    async void Start ()
    {
        await new WaitUntil(() => TrainingRoomManager.Instance.roomControls.doneLoadSettings);
        TrainingRoomManager.Instance.SetSpawnTime();
        SetSpawnTime();
    }
	
	void Update ()
    {
        if (!TrainingRoomManager.Instance.roomControls.trainingStarted || TrinaxGlobal.Instance.isChangingLevels) return;

        timer += Time.deltaTime;
        if (timer > timeToSpawn)
        {
            timer = 0;
            SpawnElement();
        }
    }

    public void SetMinMaxSpawnTime(float min, float max)
    {
        minSpawnTime = min;
        maxSpawnTime = max;
    }

    void SetSpawnTime()
    {
        timeToSpawn = Random.Range(minSpawnTime, maxSpawnTime);
    }

    void SpawnElement()
    {
        if(TrainingRoomManager.Instance.fallingList.Count != 0)
        {
            FALLING_TYPES typeToSpawn = FALLING_TYPES.Banana;
            int index = Random.Range(0, TrainingRoomManager.Instance.fallingList.Count);
            for (int i = 0; i < TrainingRoomManager.Instance.fallingList.Count; i++)
            {
                typeToSpawn = TrainingRoomManager.Instance.fallingList[index];
            }

            string typeStr = typeToSpawn.ToString();
            GameObject objectToSpawn = ObjectPooler.Instance.GetPooledObject(typeStr);
            if (objectToSpawn != null)
            {
                objectToSpawn.transform.position = transform.position;
                objectToSpawn.SetActive(true);
                //Debug.Log("Spawned: " + typeToSpawn);
            }
            else
                Debug.LogWarning(objectToSpawn + " does not exist!");
        }
        else
            Debug.Log("Falling list is empty!");

        SetSpawnTime();
    }
}
