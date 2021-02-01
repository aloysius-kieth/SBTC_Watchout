using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public enum SLIDER_BEHAVIOUR
{
    GENERAL,
    TRAINING,
}

public enum SLIDER_TYPE
{
    NONE,
    DROP_SPEED,
    DAMAGE,
}

public class SliderManager : MonoBehaviour, IManager
{
    #region SINGLETON
    public static SliderManager Instance { get; set; }
    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }
    #endregion

    public List<AdminSlider> adminSliders;
    public List<GameSlider> gameSliders;
    public List<GameSlider> trainingGameSliders;

    int executionPriority = 400;
    public int ExecutionPriority
    {
        get { return executionPriority; }
        set { value = executionPriority; }
    }
    public bool IsReady { get; set; }

    public async Task Init()
    {
        await new WaitUntil(() => TrinaxGlobal.Instance.loadNow);
        Debug.Log("Loading SliderManager...");
        IsReady = false;

        if (GameObject.FindGameObjectsWithTag("TrainingSlider") != null)
        {
            GameObject[] tSliders = GameObject.FindGameObjectsWithTag("TrainingSlider");
            if (tSliders.Length > 0)
            {
                for (int i = 0; i < tSliders.Length; i++)
                {
                    GameSlider t = tSliders[i].GetComponent<GameSlider>();
                    trainingGameSliders.Add(t);
                }
            }
        }

        //GameObject[] aSliders = GameObject.FindGameObjectsWithTag("AdminSlider");
        //if (aSliders.Length > 0)
        //{
        //    for (int i = 0; i < aSliders.Length; i++)
        //    {
        //        AdminSlider s = aSliders[i].GetComponent<AdminSlider>();
        //        adminSliders.Add(s);
        //    }
        //}
        //else { Debug.LogWarning("No Admin Sliders to be found in scene!"); }

        //GameObject[] sSliders = GameObject.FindGameObjectsWithTag("GameSlider");
        //if (sSliders.Length > 0)
        //{
        //    for (int i = 0; i < sSliders.Length; i++)
        //    {
        //        GameSlider s = sSliders[i].GetComponent<GameSlider>();
        //        gameSliders.Add(s);
        //    }
        //}
        //else { Debug.LogWarning("No Game Sliders to be found in scene!"); }

        IsReady = true;
        Debug.Log("SliderManager is loaded!");
    }
}
