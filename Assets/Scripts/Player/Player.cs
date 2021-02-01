using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;

public enum PLAYER_ANIM_STATE
{
    DEFAULT,
    UMBRELLA,
    UMBRELLA_BROKEN_HALF,
    UMBRELLA_BROKEN_QUARTER,
}

public enum PLAYER_PARTICLE_SFX
{
    HEAL,
}

public class Player : MonoBehaviour
{
    public System.Action<float> OnHurt;
    public System.Action<float> OnRecoverHP;

    public System.Action OnDeath;
    public System.Action OnTrainingDeath;

    [Header("Animators")]
    public RuntimeAnimatorController[] animators;
    [Header("Special Effects")]
    public ParticleSystem[] particles;

    [Header("Keyboard movement")]
    public bool useKeyboard = false;
    [Header("Check this to bestow HP")]
    public bool enableLifePoints = true;
    public bool isStunned = false;
    public bool isUmbrellaOpened = false;

    public bool isDead = false;
    bool isInvunerable = false;
    [HideInInspector]
    public bool isHurt = false;
    bool isFreezed = false;

    [Header("Player HP")]
    public float lifePoints = 3f;
    [Header("For keyboard")]
    public float runSpeed = 25f;
    [Header("For Kinect")]
    public float kinectMovementSensitivity = 10f;
    [Header("Duration Of Stun Recovery (NOT IN USE)")]
    public float stunDuration = 3f;
    [Header("Duration Of invunerability")]
    public int invunerableDuration = 5;
    [Header("Hurt Color")]
    public Color hurtColor;
    [Header("Spawn position")]
    public Vector3 spawnPosition;

    float originalSpeed;
    float slowedSpeed = 0;

    float timeToRecoverStun = 0;

    float horizontalMove = 0f;

    // Component caching
    CharacterController2D controller;
    SpriteRenderer sprite;
    Animator animator;
    Collider2D playerCol;

    public UmbrellaController umbrella;
    public VirtualController virtualController;

    void Start()
    {
        //Setup();
    }

    public void Setup()
    {
        controller = GetComponent<CharacterController2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        umbrella = GetComponentInChildren<UmbrellaController>();
        playerCol = GetComponent<Collider2D>();

        ChangeAnimator(PLAYER_ANIM_STATE.DEFAULT);

        umbrella.OnUmbrellaBroken += OnUmbrellaBroken;
        umbrella.OnUmbrellaBrokenHalf += OnUmbrellaBrokenHalf;
        umbrella.OnUmbrellaBrokenQuarter += OnUmbrellaBrokenQuarter;

        DeactivateUmbrellaBehaviour();
    }

    public void FreezePlayerMovement()
    {
        horizontalMove = 0f;
        isFreezed = true;
        controller.m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezePosition;
    }

    public void UnFreezePlayerMovement()
    {
        isFreezed = false;
        controller.m_Rigidbody2D.constraints = RigidbodyConstraints2D.None;
        controller.m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        //if (isFreezed)
        //    return;

        Animate();

        if (TrinaxGlobal.Instance.state != STATES.GAME || AppManager.gameManager.IsGameover) return;

        if (isHurt || isDead || AppManager.gameManager.bonusModeController.isBonusModeStarted)
        {
            FreezePlayerMovement();
            return;
        }

        //if (Input.GetKeyDown(KeyCode.Space)/* && !isStunned*/)
        //{
        //    //ApplyStun(3);
        //    PlayerRecoverHP(0.5f);
        //}

        //if (isStunned)
        //{
        //    RecoverFromStun();
        //}

        //if (Input.GetKeyDown(KeyCode.V) && !isUmbrellaOpened && !isDead)
        //{
        //    isUmbrellaOpened = true;
        //    OpenUmbrella();
        //}

        //if (Input.GetKeyDown(KeyCode.M) && isUmbrellaOpened)
        //{
        //    isUmbrellaOpened = false;
        //    OnUmbrellaBroken();
        //    DeactivateUmbrellaBehaviour();
        //}

#if UNITY_STANDALONE_WIN

        if (useKeyboard) KeyboardMovement();
        else KinectMovement();
#endif
        VirtualButtonMovement(virtualController.PlayerDirection);
    }

#region UMBRELLA BEHAVIOUR
    public void OpenUmbrella()
    {
        isUmbrellaOpened = true;
        umbrella.Reset();
        ChangeAnimator(PLAYER_ANIM_STATE.UMBRELLA);
    }

    void OnUmbrellaBroken()
    {
        ChangeAnimator(PLAYER_ANIM_STATE.DEFAULT);
        DeactivateUmbrellaBehaviour();
        isInvunerable = false;
        isUmbrellaOpened = false;
    }

    void OnUmbrellaBrokenHalf()
    {
        ChangeAnimator(PLAYER_ANIM_STATE.UMBRELLA_BROKEN_HALF);
    }

    void OnUmbrellaBrokenQuarter()
    {
        ChangeAnimator(PLAYER_ANIM_STATE.UMBRELLA_BROKEN_QUARTER);
    }

    void ActivateUmbrellaBehaviour()
    {
        umbrella.gameObject.SetActive(true);
        isInvunerable = true;
    }

    void DeactivateUmbrellaBehaviour()
    {
        umbrella.gameObject.SetActive(false);
    }
#endregion

    void FixedUpdate()
    {
        if (TrinaxGlobal.Instance.state != STATES.GAME || AppManager.gameManager.IsGameover) return;

        if (isFreezed)
            return;

        if (isHurt || isDead)
        {
            horizontalMove = 0f;
            return;
        }

        if (useKeyboard)
            controller.Move(horizontalMove * Time.fixedDeltaTime, false, false);
        else controller.Move(horizontalMove, false, false);
    }

    public void PopulateValues(GlobalSettings globalSettings, GameSettings gameSettings)
    {
        originalSpeed = runSpeed;
        invunerableDuration = gameSettings.invunerableDuration;
        useKeyboard = globalSettings.useKeyboard;
    }

    void KeyboardMovement()
    {
        if (isFreezed) return;

        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(transform.position.x, AppManager.gameManager.environment.minBounds.x, AppManager.gameManager.environment.maxBounds.x);
        transform.position = pos;
    }

    private void VirtualButtonMovement(int direction)
    {
        if (isFreezed) return;

        horizontalMove = direction * runSpeed;
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(transform.position.x, AppManager.gameManager.environment.minBounds.x, AppManager.gameManager.environment.maxBounds.x);
        transform.position = pos;
    }

    void KinectMovement()
    {
        if (isFreezed) return;

        float posX = KinectController.Instance.updatedPositionByKinect().x * kinectMovementSensitivity;
        float currentxPos = transform.localPosition.x;
        posX = Mathf.Clamp(posX, AppManager.gameManager.environment.minBounds.x, AppManager.gameManager.environment.maxBounds.x);
        float diff = posX - currentxPos;
        horizontalMove = diff;
        //Debug.Log("diff " + diff);  
    }

#region ANIMATION
    void Animate()
    {
        if (animator.runtimeAnimatorController == animators[(int)PLAYER_ANIM_STATE.DEFAULT])
        {
            if (animator.runtimeAnimatorController != null)
            {
                animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
                animator.SetFloat("LifePoints", Mathf.Abs(lifePoints));
                animator.SetBool("IsHurt", isHurt);
                animator.SetBool("IsDead", isDead);
            }
            else Debug.LogWarning("Could not find " + name + " animator!");
        }

        if (animator.runtimeAnimatorController == animators[(int)PLAYER_ANIM_STATE.UMBRELLA])
        {
            if (animator.runtimeAnimatorController != null)
            {
                animator.SetFloat("Speed", Mathf.Abs(horizontalMove)); ;
            }
            else Debug.LogWarning("Could not find " + name + " animator!");
        }

        if (animator.runtimeAnimatorController == animators[(int)PLAYER_ANIM_STATE.UMBRELLA_BROKEN_HALF])
        {
            if (animator.runtimeAnimatorController != null)
            {
                animator.SetFloat("Speed", Mathf.Abs(horizontalMove)); ;
            }
            else Debug.LogWarning("Could not find " + name + " animator!");
        }

        if (animator.runtimeAnimatorController == animators[(int)PLAYER_ANIM_STATE.UMBRELLA_BROKEN_QUARTER])
        {
            if (animator.runtimeAnimatorController != null)
            {
                animator.SetFloat("Speed", Mathf.Abs(horizontalMove)); ;
            }
            else Debug.LogWarning("Could not find " + name + " animator!");
        }
    }

    public void ChangeAnimator(PLAYER_ANIM_STATE state)
    {
        animator.runtimeAnimatorController = animators[(int)state];
    }
#endregion

    public void MinusLifePoints(float amt)
    {
        if (enableLifePoints)
        {
            PlayerHurt();
            lifePoints -= amt;
            if (lifePoints > 0)
            {
                TrinaxManager.trinaxAudioManager.PlaySFX(TrinaxAudioManager.AUDIOS.OUCH, TrinaxAudioManager.AUDIOPLAYER.SFX3);
            }
            OnHurt?.Invoke(amt);
        }
        else
        {
            isDead = true;
            OnDeath?.Invoke();
            return;
        }

        //Debug.Log("Current lifePoints " + lifePoints);
        if (lifePoints <= 0)
        {
            FreezePlayerMovement();
            lifePoints = 0;
            isDead = true;
            OnDeath?.Invoke();
            return;
        }
    }

#region TRAINING
    // In training mode only
    void TrainingMinusLifePoints(float amt)
    {
        PlayerHurt();
        if (TrainingRoomManager.Instance != null)
        {
            if (TrainingRoomManager.Instance.roomControls.traininglobalSettings.infiniteLife) return;
        }
        else
        {
            Debug.LogWarning("<TrainingRoomManager reference not found!");
            return;
        }

        lifePoints -= amt;
        OnHurt?.Invoke(amt);
        //Debug.Log("Current lifePoints " + lifePoints);
        if (lifePoints <= 0)
        {
            FreezePlayerMovement();
            lifePoints = 0;
            isDead = true;
            OnTrainingDeath?.Invoke();
            return;
        }
    }

    async Task LoopBlink(int num)
    {
        int i = 0;
        while (i < num)
        {
            sprite.DOColor(Color.clear, 0.07f);
            await new WaitForSeconds(0.07f);
            sprite.DOColor(hurtColor, 0.07f);
            await new WaitForSeconds(0.07f);
            sprite.DOColor(Color.clear, 0.07f);

            i++;
        }
    }

    public async void TrainingDeathSequence(float duration)
    {
        await new WaitForSeconds(duration);
        await LoopBlink(5);

        await new WaitForSeconds(0.5f);
        sprite.DOColor(Color.white, 0.07f);
        Debug.Log("Died! Spawning to " + spawnPosition);
        transform.localPosition = spawnPosition;

        lifePoints = 3;
        AppManager.gameManager.hud.Reset();
        isDead = false;
        isHurt = false;
        UnFreezePlayerMovement();
    }
#endregion

    public void ApplyStun(float amt)
    {
        Debug.Log("Player is stunned!");
        isStunned = true;
        timeToRecoverStun = stunDuration;
        if (useKeyboard)
        {
            originalSpeed = runSpeed;
            runSpeed = runSpeed / amt;
            slowedSpeed = Mathf.RoundToInt(runSpeed);
            Debug.Log("Current Speed: " + runSpeed);
        }
        else
        {
            originalSpeed = kinectMovementSensitivity;
            kinectMovementSensitivity = kinectMovementSensitivity / amt;
            slowedSpeed = Mathf.RoundToInt(kinectMovementSensitivity);
            Debug.Log("Current Speed: " + kinectMovementSensitivity);
        }
    }

    void RecoverFromStun()
    {
        timeToRecoverStun -= Time.deltaTime;
        if (timeToRecoverStun <= 0)
        {
            timeToRecoverStun = 0;
            isStunned = false;
        }

        float perc = timeToRecoverStun / stunDuration;
        if (useKeyboard)
        {
            runSpeed = Mathf.Lerp(originalSpeed, slowedSpeed, perc);
        }
        else
        {
            kinectMovementSensitivity = Mathf.Lerp(slowedSpeed, originalSpeed, perc);
        }

        Debug.Log("Stun time left: " + timeToRecoverStun);
    }

    // hit
    // freeze
    // unfreeze
    //invunerable and blink
    async void PlayerHurt()
    {
        if (isFreezed) return;

        playerCol.enabled = false;

        isHurt = true;
        isInvunerable = true;

        await new WaitUntil(() => !isHurt);

        int i = 0;
        while (i <= invunerableDuration)
        {
            sprite.DOColor(Color.clear, 0.07f);
            await new WaitForSeconds(0.1f);
            sprite.DOColor(Color.white, 0.07f);
            await new WaitForSeconds(0.1f);

            i++;
        }

        isInvunerable = false;
        playerCol.enabled = true;
    }

    public void OnPlayerHurtAnimOver()
    {
        isHurt = false;
        UnFreezePlayerMovement();
    }

    void PlayParticleEffect(PLAYER_PARTICLE_SFX sfx)
    {
        int index = (int)sfx;
        for (int i = 0; i < particles.Length; i++)
        {
            ParticleSystem p = particles[i];
            //Debug.Log(p.name);
            if (index == i)
            {
                if (!p.isPlaying)
                    p.Play();
                else
                    Debug.LogWarning(p.name + " already playing!");
            }
            else
            {
                Debug.LogWarning(index + " does not exist!");
                return;
            }
        }
    }

    public void PlayerRecoverHP(float amt)
    {
        PlayParticleEffect(PLAYER_PARTICLE_SFX.HEAL);

        if (lifePoints >= 3)
            return;

        //Debug.Log("Recovered " + amt);
        lifePoints += amt;
        //Debug.Log("Player HP " + lifePoints);

        OnRecoverHP?.Invoke(amt);
    }

    public void PlayerGetCoin()
    {
        int rand = Random.Range((int)TrinaxAudioManager.AUDIOS.COIN_PICKUP1, (int)TrinaxAudioManager.AUDIOS.COIN_PICKUP3 + 1);
        TrinaxManager.trinaxAudioManager.PlaySFX((TrinaxAudioManager.AUDIOS)rand, TrinaxAudioManager.AUDIOPLAYER.SFX3);
        GameObject coinParticle = ObjectPooler.Instance.GetPooledObject("CoinParticle");
        if (coinParticle != null)
            coinParticle.SetActive(true);
        else
            Debug.LogWarning(coinParticle + " does not exist!");
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "FallingObject" && !isInvunerable && !isHurt)
        {
            if (TrinaxGlobal.Instance.scene == SCENE.MAIN)
                MinusLifePoints(col.GetComponent<FallingObject>().damage);
            else
                TrainingMinusLifePoints(col.GetComponent<FallingObject>().damage);
        }

        if (col.tag == "Coin")
        {
            PlayerGetCoin();
            col.GetComponent<Coin>().Deactivate();
        }
    }

    public void Reset()
    {
        isDead = false;
        isHurt = false;
        //TODO: adminpanel
        lifePoints = 3;
        transform.position = spawnPosition;
        gameObject.SetActive(true);
        UnFreezePlayerMovement();
    }
}
