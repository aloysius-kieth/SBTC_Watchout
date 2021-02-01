using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    #region SINGLETON
    public static PlayerStatus Instance { get; set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else Instance = this;
    }
    #endregion

    public GameManager gameManager;
    Player player;

    private void Start()
    {
        player = gameManager.player;
    }

    public bool GetUmbrellaStatusOpened()
    {
        return true ? player.isUmbrellaOpened : false;
    }

    public bool GetPlayerStatusDead()
    {
        return true ? player.isDead : false;
    }

}
