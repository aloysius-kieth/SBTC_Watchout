using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Every 1000 xp needed to fill bonus bar up
// Scale down xp of each object after each bonus round
public class XPManager : MonoBehaviour
{
    #region SINGLETON 
    public static XPManager Instance { get; set; }
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }
    #endregion

    public static System.Action OnReachedXPGoal;
    public static System.Action OnGainXP;

    int currentXP;
    public int MAX_XP_REQUIRED_PER_BONUS { get; set; }

    int xpPerObject = 18; // starting number, TODO: adminpanel
    public float XPPercent { get; set; }
    public float XPMultipler { get; set; }

    public XPBar xpBar;

    private void Start()
    {
        Setup();
    }

    void Setup()
    {
        PopulateValues(TrinaxGlobal.Instance.gameSettings);

        currentXP = 0;
        XPPercent = 0f;
    }

    public void PopulateValues(GameSettings setting)
    {
        MAX_XP_REQUIRED_PER_BONUS = setting.MAX_XP_REQUIRED_PER_BONUS;
        XPMultipler = setting.xpPerObjectMultipler;
        xpPerObject = setting.xpPerObject;
    }

    public void GainXP()
    {
        if (AppManager.gameManager.bonusModeController.isBonusModeActive
            || AppManager.gameManager.bonusModeController.isBonusModeStarted)
            return;

        currentXP += xpPerObject;
        CalculateXPPercent();
        if (currentXP >= MAX_XP_REQUIRED_PER_BONUS)
        {
            //Debug.Log("Current XP : " + currentXP + " more than " + MAX_XP_REQUIRED_PER_BONUS);
            OnReachedXPGoal?.Invoke();
            ScaleDownXPPerObject();
            Reset();
            return;
        }

        OnGainXP?.Invoke();
        //Debug.Log("Current XP : " + currentXP);
    }

    void CalculateXPPercent()
    {
        XPPercent = ((float)currentXP / (float)MAX_XP_REQUIRED_PER_BONUS);
        XPPercent = XPPercent.Map01(0, 1f);

        //Debug.Log("XPPercent " + XPPercent);
    }

    private void ScaleDownXPPerObject()
    {
        if (xpPerObject <= 1)
        {
            Debug.Log("Max scaling reached!");
            xpPerObject = 1;
            return;
        }
        xpPerObject = xpPerObject - (int)(1 / XPMultipler);
        //Debug.Log(xpPerObject);
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    GainXP();
        //}
    }

    public void Reset()
    {
        currentXP = 0;
        XPPercent = 0;
    }
}
