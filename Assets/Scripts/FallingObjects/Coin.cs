using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    bool groundHit = false;

    float distToFloor = 0;
    float timeScale = 0.1f;

    public Shadow shadow;
    Rigidbody2D rb2d;

    void Awake()
    {
        shadow = GetComponent<Shadow>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        //shadow.SetShadowOnGround();
        groundHit = false;

        timeScale = TrinaxGlobal.Instance.gameSettings.coinFallSpeed;
    }

    void DistanceFromGround()
    {
        RaycastHit2D hit;
        hit = Physics2D.Raycast(transform.localPosition, Vector2.down, transform.localPosition.y - AppManager.gameManager.environment.floor.localPosition.y, 1 << 11);

        distToFloor = Mathf.Floor(hit.distance);
    }

    void FixedUpdate()
    {
        if (!AppManager.gameManager.IsGameover && !groundHit)
        {
            float temp = timeScale * Time.fixedDeltaTime;
            rb2d.velocity += Physics2D.gravity * temp;
        }
    }

    void Update()
    {
        if(!AppManager.gameManager.IsGameover && !groundHit)
        {
            DistanceFromGround();
            if (shadow != null)
            {
                shadow.sprite.LerpScale(distToFloor);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Floor" && !groundHit)
        {
            groundHit = true;
            Deactivate();
        }
    }

    public void Deactivate()
    {
        shadow.Deactivate();
        SpawnManager.Instance.coinSpawner.RemoveCoinsFromList(gameObject);
        gameObject.SetActive(false);
    }
}
