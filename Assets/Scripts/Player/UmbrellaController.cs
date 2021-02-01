using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UmbrellaController : MonoBehaviour
{
    public System.Action OnUmbrellaBroken;
    public System.Action OnUmbrellaBrokenHalf;
    public System.Action OnUmbrellaBrokenQuarter;

    [Header("Invunerability of umbrella")]
    public bool isInvunerable = false;

    public Vector3 minColBounds { get; set; }
    int totalLifePoints = 3;
    int lifePoints;
    bool isDestroyed = false;
    bool isDestroyedHalf = false;
    bool isDestroyedQuarter = false;

    Collider2D col;

    private void Awake()
    {
        col = GetComponentInChildren<Collider2D>();
        minColBounds = col.bounds.min;
    }

    private void OnEnable()
    {
        lifePoints = totalLifePoints;
    }

    public void TakeDamage(int amt)
    {
        if (isInvunerable) return;

        lifePoints -= amt;

        if (lifePoints == 2 && !isDestroyedHalf)
        {
            isDestroyedHalf = true;
            OnUmbrellaBrokenHalf?.Invoke();
        }

        if (lifePoints == 1 && !isDestroyedQuarter)
        {
            isDestroyedQuarter = true;
            OnUmbrellaBrokenQuarter?.Invoke();
        }

        if (lifePoints == 0)
        {
            lifePoints = 0;
            isDestroyed = true;
            // go back to default boy animator
            OnUmbrellaBroken?.Invoke();
        }

        //Debug.Log("Current Umbrella lifePoints " + lifePoints);
    }

    void OnCollisionEnter2D(Collision2D col2D)
    {
        if(col2D.gameObject.tag == "FallingObject")
        {
            TrinaxManager.trinaxAudioManager.PlaySFX(TrinaxAudioManager.AUDIOS.UMBRELLA_HIT, TrinaxAudioManager.AUDIOPLAYER.SFX2);
            //Debug.Log(col2D.gameObject.GetComponent<FallingObject>().damage);
            TakeDamage(1);
        }
    }

    public void Reset()
    {
        isDestroyed = false;
        isDestroyedHalf = false;
        isDestroyedQuarter = false;
        //TODO: put value in admin panel
        lifePoints = 3;
    }

}
