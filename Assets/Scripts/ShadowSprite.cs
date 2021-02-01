using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowSprite : MonoBehaviour
{
    [Header("Y position of shadow")]
    public float posY = -7.4f;

    void OnEnable()
    {
        transform.localScale = Vector3.zero;
        transform.localPosition = new Vector3(transform.localPosition.x, posY);
    }

    public void LerpScale(float dist)
    {
        float distanceApart = dist;

        float lerp = TrinaxHelperMethods.mapValue(distanceApart, 0, 18, 1f, 0f);
        transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(1, 0.4f, 0), lerp);
    }
}
