using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour
{
    public Vector2 minBounds;
    public Vector2 maxBounds;
    public Transform floor;

    void Start()
    {
        Activate(false);
    }

    public void Activate(bool active)
    {
        gameObject.SetActive(active);
    }
}
