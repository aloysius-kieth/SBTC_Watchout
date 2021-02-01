using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveMovement : MonoBehaviour
{
    public Transform[] Points;
    Vector3 controlPoint;
    float currentLerpTime = 0f;
    float lerpTime = 1f;
    float curveAmt = 5f;

    float speed = 1.5f;

    private void Awake()
    {
        Points[0] = AppManager.gameManager.player.transform;
        // Points[1] = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<Transform>();
        Points[2] = ScoreManager.Instance.scoreText.GetComponent<Transform>();
    }

    void Start()
    {
        if (Points.Length > 0)
        {
            controlPoint = Points[0].position + (Points[2].position - Points[0].position) / 2 + Vector3.up * curveAmt;
        }
        transform.position = Points[0].position;
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    currentLerpTime = 0;
        //    transform.position = Points[0].position;
        //}

        if (Points.Length > 0)
        {
            if (currentLerpTime < 1.0f)
            {
                currentLerpTime += speed * Time.deltaTime;
                if (currentLerpTime >= 1.0f)
                {
                    currentLerpTime = lerpTime;
                    ScoreManager.Instance.OnBonusGet(TrinaxGlobal.Instance.gameSettings.pointsPerCoin);
                    Deactivate();
                }

                float perc = currentLerpTime / lerpTime;
                perc = perc * perc;

                Vector3 m1 = Vector3.Lerp(Points[0].position + new Vector3(0, 1.5f), controlPoint, currentLerpTime);
                Vector3 m2 = Vector3.Lerp(controlPoint, Points[2].position, currentLerpTime);
                transform.position = Vector3.Lerp(m1, m2, currentLerpTime);
            }
        }
    }

    void Deactivate()
    {
        currentLerpTime = 0;
        transform.position = Points[0].position;
        gameObject.SetActive(false);
    }
}
