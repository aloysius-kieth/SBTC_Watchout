using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Shadow))]
public abstract class FallingObject : MonoBehaviour
{
    [Header("Object type")]
    public FALLING_TYPES type;
    [Header("Object Speed scale")]
    public float timeScale = 1f;
    [Header("Damage to player")]
    public float damage = 1;
    [Header("Blink Duration")]
    public float blinkDuration = 0.07f;

    public int PointsPerObject { get; set; }

    public Vector3 UmbrellaCollisionBounds { get; set; }
    public bool hitUmberlla = false;

    Vector3 objColBounds;
    public Vector3 GetObjColBounds() { return objColBounds; }

    public bool hitPlayer = false;

    float timeToDisappear = 1f;
    float distanceToFloor = 0;

    public bool onGroundHit = false;
    bool awardPoint = true;

    // Component cache
    Collider2D umbrellaCollider; // use for umbrella only
    public Collider2D objectCollider;
    Rigidbody2D rb2d;
    SpriteRenderer sprite;
    public Shadow shadow;
    Animator animator;

    //bool first = true;
    //public float _timeScale = 1f;
    //public float timeScale
    //{
    //    get { return _timeScale; }
    //    set
    //    {
    //        if (!first)
    //        {
    //            rb2d.mass *= timeScale;
    //            rb2d.velocity /= timeScale;
    //            rb2d.angularVelocity /= timeScale;
    //        }
    //        first = false;

    //        _timeScale = Mathf.Abs(value);

    //        rb2d.mass /= timeScale;
    //        rb2d.velocity *= timeScale;
    //        rb2d.angularVelocity *= timeScale;
    //    }
    //}

    async void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        umbrellaCollider = GetComponent<Collider2D>();
        objectCollider = GetComponentInChildren<BoxCollider2D>();
        shadow = GetComponent<Shadow>();

        objColBounds = objectCollider.bounds.size;
        gameObject.SetActive(false);
    }

    async void Start()
    {
        await new WaitUntil(() => TrinaxGlobal.Instance.isReady);
        if (TrinaxGlobal.Instance.scene == SCENE.MAIN)
            ObjectPropertyManager.GetFallingObjectProperties(this);
        else ObjectPropertyManager.GetTrainingFallingObjectProperties(this);

        UmbrellaCollisionBounds = AppManager.gameManager.player.umbrella.minColBounds;
    }

    void OnEnable()
    {
        //shadow.SetShadowOnGround();
        Reset();

        transform.rotation = Quaternion.identity;
    }

    void OnDisable()
    {
        AppManager.gameManager.fallingObjectsList.Remove(this);
    }

    public void PopulateValues(GameSettings setting)
    {
        PointsPerObject = setting.pointsPerObject;
    }

    public void RigidbodySimulation(bool enable)
    {
        rb2d.simulated = enable;
    }

    void DoRaycast()
    {
        RaycastHit2D hit;
        hit = Physics2D.Raycast(transform.localPosition, Vector2.down, transform.localPosition.y - AppManager.gameManager.environment.floor.localPosition.y, 1 << 11);

        distanceToFloor = Mathf.Floor(hit.distance);
        //Debug.DrawRay(transform.localPosition, Vector2.down * hit.distance, Color.red);
        //Debug.Log(transform.name + " distance: " + distanceToFloor);
    }

    void Update()
    {
        if (!onGroundHit)
        {
            DoRaycast();
            if (shadow != null)
                shadow.sprite.LerpScale(distanceToFloor);
        }

        if (transform.position.y <= UmbrellaCollisionBounds.y && PlayerStatus.Instance.GetUmbrellaStatusOpened())
            umbrellaCollider.enabled = false;
    }

    void FixedUpdate()
    {
        if (AppManager.gameManager.IsGameover) return;

        //float dt = Time.fixedDeltaTime * timeScale;
        //rb2d.velocity += Physics2D.gravity * dt;
        if (!onGroundHit/* && !uCol.hitUmberlla*/)
        {
            float temp = timeScale * Time.fixedDeltaTime;
            rb2d.velocity += Physics2D.gravity * temp;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col != null && col.tag == "Player" && !hitPlayer && !col.GetComponent<Player>().isHurt)
        {
            OnHitPlayer();
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Umbrella" && !hitUmberlla && col != null)
        {
            ContactPoint2D contactPt = col.contacts[0];
            umbrellaCollider.enabled = false;
            rb2d.AddRelativeForce(Vector2.up * 0.5f, ForceMode2D.Impulse);
            if (contactPt.point.x >= 0)
            {
                rb2d.AddTorque(1f, ForceMode2D.Impulse);
            }

            else
                rb2d.AddTorque(-1f, ForceMode2D.Impulse);

            StartCoroutine(OnHitUmbrella());
        }
    }

    IEnumerator OnHitUmbrella()
    {
        hitUmberlla = true;
        shadow.Deactivate();

        yield return new WaitForSeconds(0.7f);
        DoBlink(0.05f, 1, false);
    }

    void OnHitPlayer()
    {
        hitPlayer = true;
        awardPoint = false;
        OnGroundHit(false);
    }

    // Different effect for different objects that hit player
    public virtual void ApplyStatusEffect(Player player)
    {

    }

    public async void OnGroundHit(bool delayBlink = true)
    {
        RigidbodySimulation(false);
        onGroundHit = true;

        if (!gameObject.activeSelf) return;

        if (animator != null) animator.SetBool("OnGroundHit", onGroundHit);
        else Debug.LogWarning("Could not find " + name + " animator!");

        if (awardPoint)
        {
            if (TrinaxGlobal.Instance.scene == SCENE.MAIN)
            {
                if (ScoreManager.Instance != null) ScoreManager.Instance.AddScore(PointsPerObject);
                XPManager.Instance.GainXP();
            }
            else
            {
                if (TrainingRoomManager.Instance != null) TrainingRoomManager.Instance.AddScore(1);
            }
        }

        if (delayBlink)
        {
            await new WaitForSeconds(timeToDisappear);
        }

        DoBlink(blinkDuration, 3, true);
    }

    public async void DoBlink(float durationToBlink, int loop, bool applyStatusEffect)
    {
        for (int i = 0; i < loop; i++)
        {
            sprite.DOColor(Color.clear, durationToBlink);
            await new WaitForSeconds(durationToBlink);
            sprite.DOColor(Color.white, durationToBlink);
            await new WaitForSeconds(durationToBlink);
        }

        DeactivateAll();

        if (applyStatusEffect)
        {
            ApplyStatusEffect(AppManager.gameManager.player);
        }
    }

    public void DeactivateAll()
    {
        shadow.Deactivate();
        gameObject.SetActive(false);
    }

    void Reset()
    {
        RigidbodySimulation(true);
        hitUmberlla = false;
        hitPlayer = false;
        umbrellaCollider.enabled = true;
        onGroundHit = false;
        awardPoint = true;
    }
}
