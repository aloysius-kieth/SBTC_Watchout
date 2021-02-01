using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraProperties : MonoBehaviour
{
    public float CameraWidth { get; set; }
    public float CameraHeight { get; set; }
    Camera cam;

    [HideInInspector]
    public Vector3 cameraScreenToWorldPointPosition;
    public RedVignetteOverlay redVignetteOverlay;

    private void Awake()
    {
        cam = Camera.main;
        float height = cam.orthographicSize;
        float width = height * Screen.width / Screen.height;
        CameraWidth = Mathf.Floor(width);
        CameraHeight = Mathf.Floor(height);

        ScreenToWorldPointPosition();
    }

    void ScreenToWorldPointPosition()
    {
        cameraScreenToWorldPointPosition = cam.ScreenToWorldPoint(new Vector3(0, Screen.height));
    }
}
