using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainingToggle : MonoBehaviour
{
    public FALLING_TYPES type;
    Toggle toggle;

    void Start()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(delegate { OnActivate(toggle); });

    }

    void OnActivate(Toggle toggle)
    {
        if(TrainingRoomManager.Instance != null)
        {
            if (toggle.isOn)
                TrainingRoomManager.Instance.fallingList.Add(type);
            else
                TrainingRoomManager.Instance.fallingList.Remove(type);
        }
    }
}
