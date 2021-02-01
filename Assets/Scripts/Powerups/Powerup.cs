using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum POWERUPS
{
    Umbrella_Powerup,
    FirstAid_Powerup,
}

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Powerup : MonoBehaviour
{
    public POWERUPS powerup;
    public float fallSpeed = 0.7f;

    protected System.Action OnPickup;
    protected SpriteRenderer sprite;

    Rigidbody2D rb2d;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    public virtual void Start()
    {
        OnPickup += OnPickedUp;

        rb2d.gravityScale = fallSpeed;
    }

    public virtual void OnPickedUp()
    {

    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            //Debug.Log(col.gameObject.name + " picked up!");
            OnPickup?.Invoke();
        }
        if(col.gameObject.tag == "Floor" && gameObject.activeSelf)
        {
           DoBlink(0.25f, 3);
        }
    }

    float durationBeforeBlink = 2;
    async void DoBlink(float durationToBlink, int loop)
    {
        await new WaitForSeconds(durationBeforeBlink);
        for (int i = 0; i < loop; i++)
        {
            sprite.DOColor(Color.clear, durationToBlink);
            await new WaitForSeconds(durationToBlink);
            sprite.DOColor(Color.white, durationToBlink);
            await new WaitForSeconds(durationToBlink);
        }

        //castShadow.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        sprite.color = Color.white;
    }
}
