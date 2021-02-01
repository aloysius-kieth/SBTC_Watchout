using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RANDOM_WEIGHTS
{
    POWERUP,
    FALLING_OBJECT,
}

public class RandomWeightsManager : MonoBehaviour
{
    public static RandomWeightsManager Instance { get; set; }
    public RandomWeight[] weights;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public string GetRandomObject()
    {
        RandomWeight weight;
        weight = weights[(int)RANDOM_WEIGHTS.FALLING_OBJECT];

        int index = weight.GetRandom();
        FALLING_TYPES type = (FALLING_TYPES)index;
        string typeStr = type.ToString();

        return typeStr;
    }
}
