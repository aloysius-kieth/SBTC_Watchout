using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    public float smoothTime = 0.125f;
    Vector3 velocity;
    public Vector3 offset;

    public bool maxYEnabled = false;
    public float maxYValue;
    public bool minYEnabled = false;
    public float minYValue;

    public bool maxXEnabled = false;
    public float maxXValue;
    public bool minXEnabled = false;
    public float minXValue;

    void FixedUpdate()
    {
        Vector3 targetPos = target.position + offset;

        if (minYEnabled && maxYEnabled)
        {
            targetPos.y = Mathf.Clamp(target.position.y, minYValue, maxYValue);
        }
        else if(minYEnabled)
        {
            targetPos.y = Mathf.Clamp(target.position.y, minYValue, target.position.y);
        }
        else if(maxYEnabled)
        {
            targetPos.y = Mathf.Clamp(target.position.y, target.position.y, maxYValue);
        }

        if (minXEnabled && maxXEnabled)
        {
            targetPos.x = Mathf.Clamp(target.position.x, minXValue, maxXValue);
        }
        else if (minXEnabled)
        {
            targetPos.x = Mathf.Clamp(target.position.x, minXValue, target.position.x);
        }
        else if (maxXEnabled)
        {
            targetPos.x = Mathf.Clamp(target.position.x, target.position.x, maxXValue);
        }

        targetPos.z = transform.position.z;


        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
    }
}
