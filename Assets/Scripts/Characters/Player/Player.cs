using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : Character
{
    [SerializeField]StatsBar_HUD statsBar_HUD;
    [SerializeField]bool regenerateHealth = true;
    [SerializeField]float healthRegenerateTime;
    [SerializeField, Range(0, 1)]float healthRengeneratePercent;

    [Header("------INPUT------")]
    [SerializeField]PlayerInput input;
    [Header("------MOVE------")]
    [SerializeField]float moveSpeed = 10f;//移动速度
    [SerializeField]float accelerationTime = 3f , decelerationTime = 3f;//加速时间、减速时间
    [SerializeField]float moveRotationAngle = 30f;//旋转角度
    [Header("------FIRE------")]
    [SerializeField]GameObject projectile1;
    [SerializeField]GameObject projectile2;
    [SerializeField]GameObject projectile3;
    [SerializeField]GameObject projectileOverdrive;
    [SerializeField]ParticleSystem muzzleVFX; 
    [SerializeField, Range(0, 2)] int weaponPower = 0;
    [SerializeField]Transform muzzleMiddle;
    [SerializeField]Transform muzzleTop;
    [SerializeField]Transform muzzleBottom;
    [SerializeField]float fireInterval = 0.2f;
    [SerializeField]AudioDate projectileLaunchSFX;

    [Header("------DODGE------")]
    [SerializeField, Range(0, 100)]int dodgeEnergyCost = 25;
    [SerializeField]float maxRoll = 720f;
    [SerializeField]float rollSpeed = 360f;
    [SerializeField]Vector3 dodgeScale = new Vector3(0.5f, 0.5f, 0.5f);
    [SerializeField]AudioDate dodgeSFX;

    [Header("------OVERDRIVE------")]

    [SerializeField]int overdriveDodgeFactor = 2;
    [SerializeField]float overdriveSpeedFactor = 1.2f;
    [SerializeField]float overdriveFireFactor = 1.2f;
    bool isDodging = false;
    bool isOverdriving = false;

    readonly float slowMotionDuration = 1f;
    readonly float InvicibleTime = 1f;
    float dodgeDurtion;
    float paddingX, paddingY;
    float currentRoll;
    

    Vector2 moveDirection; 
    Vector2 previousVelocity;
    Quaternion previousRotation;

    WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();
    WaitForSeconds waitForFireInterval;
    WaitForSeconds waitForOverdriveFireInterval;
    WaitForSeconds wairHealthregenerateTime;
    WaitForSeconds waitInvincibleTime;

    Coroutine moveCoroutine;
    Coroutine healthRegenerateCoroutine;
    new Rigidbody2D rigidbody;//添加刚体
    new Collider2D collider;

    MissileSystem missile;

    public bool IsFullHealth => health == maxHealth;
    public bool IsFullPower => weaponPower == 2; 

    #region UNITY EVENT FUNCTION
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        missile = GetComponent<MissileSystem>();
        dodgeDurtion = maxRoll / rollSpeed;

        var size = transform.GetChild(0).GetComponent<Renderer>().bounds.size;
        paddingX = size.x / 2f;
        paddingY = size.y / 2f;

        rigidbody.gravityScale = 0f;
        waitForFireInterval = new WaitForSeconds(fireInterval); 
        waitForOverdriveFireInterval = new WaitForSeconds(fireInterval / overdriveFireFactor);
        wairHealthregenerateTime = new WaitForSeconds(healthRegenerateTime);
        waitInvincibleTime = new WaitForSeconds(InvicibleTime);
    }
    protected override void OnEnable() 
    {
        base.OnEnable();

        input.onMove += Move;
        input.onStopMove += StopMove;
        input.onFire += Fire;
        input.onStopFire += StopFire;
        input.onDodge += Dodge;
        input.onLaunchMissile += LaunchMissile;
        input.onOverdrive += Overdrive;
        PlayerOverdrive.on += OverdriveOn;
        PlayerOverdrive.off += OverdriveOff;

    }
    void OnDisable() 
    {
        input.onMove -= Move;
        input.onStopMove -= StopMove;
        input.onFire -= Fire;
        input.onStopFire -= StopFire;
        input.onDodge -= Dodge;
        input.onLaunchMissile -= LaunchMissile;
        input.onOverdrive -= Overdrive;
        PlayerOverdrive.on -= OverdriveOn;
        PlayerOverdrive.off -= OverdriveOff;
    }
    // Start is called before the first frame update
    void Start()
    {
        

        statsBar_HUD.Initialize(health, maxHealth);
        input.EnableGameplayInput();

        StartCoroutine(nameof(MoveRangeLimatationCoroutine));
    }



    #endregion

    #region HEALTH
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        PowerDown();
        statsBar_HUD.UpdateStats(health, maxHealth);
        if(gameObject.activeSelf)
        {
            if(regenerateHealth)
            {
                Move(moveDirection);
                StartCoroutine(InvicibleTCoroutine());
                if(healthRegenerateCoroutine != null)
                {
                    StopCoroutine(healthRegenerateCoroutine);
                }
                healthRegenerateCoroutine = StartCoroutine(HealthRegenerateCoroutine(wairHealthregenerateTime, healthRengeneratePercent));
            }
        }
    }

    public override void RestoreHealth(float value)
    {
        base.RestoreHealth(value);
        statsBar_HUD.UpdateStats(health, maxHealth);
    }
    public override void Die()
    {
        GameManager.onGameOver?.Invoke();
        GameManager.GameState = GameState.GameOver;
        statsBar_HUD.UpdateStats(0f, maxHealth);
        base.Die();
    }

    IEnumerator InvicibleTCoroutine()
    {
        collider.isTrigger = true;
        yield return waitInvincibleTime;
        collider.isTrigger = false;
    }
    #endregion

    #region MOVE

    /// <summary>
    /// 移动函数
    /// </summary>
    /// <param name="moveInput"></param>
    void Move(Vector2 moveInput)
    {
        //Vector2 moveAmount = moveInput * moveSpeed;
        //rigidbody.velocity = moveAmount; 
        if(moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveDirection = moveInput.normalized;
        Quaternion moveRotation = Quaternion.AngleAxis(moveRotationAngle * moveInput.y, Vector3.right);
        moveCoroutine = StartCoroutine(MoveCoroutine(accelerationTime, moveDirection * moveSpeed, moveRotation));
        StartCoroutine(nameof(MoveRangeLimatationCoroutine));
    }

    /// <summary>
    /// 停止移动函数
    /// </summary>
    void StopMove()
    {
        if(moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        moveDirection = Vector2.zero;
        StartCoroutine(MoveCoroutine(decelerationTime, Vector2.zero, Quaternion.identity));
        //StopCoroutine(nameof(MoveRangeLimatationCoroutine));
    }

    /// <summary>
    /// 开始移动、结束移动协程
    /// 通过vector2的线性插值函数Lerp来每秒增加速度值。
    /// 通过Quaternion的lerp函数来每秒增加旋转角度。
    /// </summary>
    /// <param name="moveVelocity"></param>
    /// <returns></returns>
    IEnumerator MoveCoroutine(float time, Vector2 moveVelocity, Quaternion moveRotation)
    {
        float t = 0f;
        previousVelocity = rigidbody.velocity;
        previousRotation = transform.rotation;
        while(t < 1f)
        {
            t += Time.fixedDeltaTime / time;
            rigidbody.velocity = Vector2.Lerp(previousVelocity, moveVelocity, t );
            transform.rotation = Quaternion.Lerp(previousRotation, moveRotation, t);
            yield return WaitForFixedUpdate;
        }
    }


    /// <summary>
    /// 由于使用Update函数每一帧都会运行，消耗的性能过于大，所以使用线程来限制移动的范围。
    /// </summary>
    /// <returns></returns>
    IEnumerator MoveRangeLimatationCoroutine()
    {
        while(true)
        {
            transform.position = Viewport.Instance.PlayerMoveablePosition(transform.position,paddingX,paddingY);
            yield return null;
        }
    }
    #endregion

    #region FIRE

    void Fire()
    {
        muzzleVFX.Play();
        StartCoroutine(nameof(FireCoroutine));
    }

    void StopFire()
    {
        muzzleVFX.Stop();
        StopCoroutine(nameof(FireCoroutine));
    }

    IEnumerator FireCoroutine()
    {
        
        while(true)
        {
            /*
            switch(weaponPower)
            {
                case 0:
                    Instantiate(projectile2, muzzleMiddle.position, Quaternion.identity);
                    break;
                case 1:
                    Instantiate(projectile2, muzzleTop.position, Quaternion.identity);
                    Instantiate(projectile2, muzzleBottom.position, Quaternion.identity);
                    break;
                case 2:
                    Instantiate(projectile1, muzzleTop.position, Quaternion.identity);
                    Instantiate(projectile2, muzzleMiddle.position, Quaternion.identity);
                    Instantiate(projectile3, muzzleBottom.position, Quaternion.identity);
                    break;
            }
            */
            switch(weaponPower)
            {
                case 0:
                    PoolManager.Release(isOverdriving ? projectileOverdrive : projectile1, muzzleMiddle.position);
                    break;
                case 1:
                    PoolManager.Release(isOverdriving ? projectileOverdrive : projectile2, muzzleTop.position);
                    PoolManager.Release(isOverdriving ? projectileOverdrive : projectile2, muzzleBottom.position);
                    break;
                case 2:
                    PoolManager.Release(isOverdriving ? projectileOverdrive : projectile1, muzzleTop.position);
                    PoolManager.Release(isOverdriving ? projectileOverdrive : projectile2, muzzleMiddle.position);
                    PoolManager.Release(isOverdriving ? projectileOverdrive : projectile3, muzzleBottom.position);
                    break;
            }

            AutoManager.Instance.PlayRandomSFX(projectileLaunchSFX);
            //yield return waitForFireInterval;
            yield return isOverdriving ? waitForOverdriveFireInterval : waitForFireInterval;
            /*
            if(isOverdriving)
            {
                yield return waitForOverdriveFireInterval;
            }
            else
            {
                yield return waitForFireInterval;
            }*/
        }
    }


    #endregion
    
    #region DODGE

    void Dodge()
    {
        if(isDodging || !PlayerEnergy.Instance.IsEnough(dodgeEnergyCost)) return;
        StartCoroutine(nameof(DodgeCoroutine));
       
    }

    IEnumerator DodgeCoroutine()
    {
        isDodging = true;
        AutoManager.Instance.PlayRandomSFX(dodgeSFX);
        //能量值消耗
        PlayerEnergy.Instance.Use(dodgeEnergyCost);
        //让玩家无敌
        collider.isTrigger = true;
        //让玩家围绕X轴翻转
        currentRoll = 0f;
        TimeController.Instance.BulletTime(slowMotionDuration, slowMotionDuration);
        
        var scale = transform.localScale;
        while(currentRoll < maxRoll)
        {
            currentRoll += rollSpeed * Time.deltaTime;
            transform.rotation = Quaternion.AngleAxis(currentRoll, Vector3.right);
            //改变缩放位置
            if(currentRoll < maxRoll / 2)
            {
                scale.x = Mathf.Clamp(scale.x - Time.deltaTime / dodgeDurtion, dodgeScale.x, 1f);
                scale.y = Mathf.Clamp(scale.y - Time.deltaTime / dodgeDurtion, dodgeScale.y, 1f);
                scale.z = Mathf.Clamp(scale.z - Time.deltaTime / dodgeDurtion, dodgeScale.z, 1f);
            }
            else
            {
                scale.x = Mathf.Clamp(scale.x + Time.deltaTime / dodgeDurtion, dodgeScale.x, 1f);
                scale.y = Mathf.Clamp(scale.y +  Time.deltaTime / dodgeDurtion, dodgeScale.y, 1f);
                scale.z = Mathf.Clamp(scale.z + Time.deltaTime / dodgeDurtion, dodgeScale.z, 1f);
            }

            transform.localScale = scale;
            yield return null;
        }

        collider.isTrigger = false;
        isDodging = false;

    }
    #endregion

    #region OVERDRIVE

    void Overdrive()
    {
        if(!PlayerEnergy.Instance.IsEnough(PlayerEnergy.MAX))
        {
            return;
        }
        PlayerOverdrive.on.Invoke();
    }

    void OverdriveOn()
    {
        isOverdriving = true;
        dodgeEnergyCost *= overdriveDodgeFactor;
        moveSpeed *= overdriveSpeedFactor; 
        TimeController.Instance.BulletTime(slowMotionDuration, slowMotionDuration);
    }

    void OverdriveOff()
    {
        isOverdriving = false;
        dodgeEnergyCost /= overdriveDodgeFactor;
        moveSpeed /= overdriveSpeedFactor; 
    }

    #endregion

    #region  MISSILE
    void LaunchMissile()
    {
        missile.Launch(muzzleMiddle);
    }

    public void PickUpMissile() 
    {
        missile.PickUp();    
    }
    #endregion

    #region WEAPON POWER

    public void PowerUp() 
    {

        weaponPower = Mathf.Min(weaponPower + 1, 2);
    }

    public void PowerDown() 
    {
        weaponPower = Mathf.Max(--weaponPower, 0);
    }

    #endregion

}
