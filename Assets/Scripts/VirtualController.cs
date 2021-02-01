using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class VirtualController : MonoBehaviour
{
    public int PlayerDirection { get; private set; }

    public void OnPointerUp()
    {
        PlayerDirection = 0;
    }

    public void OnPointerDown(int direction)
    {
        PlayerDirection = direction;
    }
}
