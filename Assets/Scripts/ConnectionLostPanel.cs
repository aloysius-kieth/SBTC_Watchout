using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConnectionLostPanel : MonoBehaviour
{
    public TextMeshProUGUI text;

    private void Awake()
    {
        TrinaxAsyncServerManager.OnMaxRetriesReached += OnMaxRetriesReached;
        TrinaxAsyncServerManager.OnConnectionLost += OnConnectionLost;
        gameObject.SetActive(false);
    }

    void OnMaxRetriesReached()
    {
        text.text = "Returning to Home";
    }

    void OnConnectionLost()
    {
        text.text = "Connection Lost!" + "\n" + "Please wait...";
    }

}
