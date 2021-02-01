using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppManager : MonoBehaviour
{
    public static AppManager Instance { get; set; }
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

    public static GameManager gameManager;
    public static UIManager uiManager;
    public static LeaderboardManager leaderboardManager;


    private async void Start()
    {
        await new WaitUntil(() => TrinaxGlobal.Instance.isReady);

        // App start
        Execute();
    }

    async void Execute()
    {
        Debug.Log("App starting...");
        leaderboardManager.Init();
        await new WaitUntil(() => leaderboardManager.IsReady);
        gameManager.Init();
        await new WaitUntil(() => gameManager.IsReady);
        uiManager.Init();
        await new WaitUntil(() => uiManager.IsReady);
        gameManager.screensaver.Init();
    }
}
