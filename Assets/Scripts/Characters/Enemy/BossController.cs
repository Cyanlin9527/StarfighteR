using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : EnemyController
{
    [SerializeField] float continuousFireDuration = 1.5f;
    
    [Header("---- Player Detection ----")]
    [SerializeField] Transform playerDetectionTransform;
    [SerializeField] Vector3 playerDetectionSize;
    [SerializeField] LayerMask playerLayer;
    
    [Header("---- Beam ----")]
    [SerializeField] float beamCooldownTime = 12f;
    [SerializeField] AudioDate beamChargingSFX;
    [SerializeField] AudioDate beamLaunchSFX;
    
    bool isBeamReady;

    int launchBeamID = Animator.StringToHash("launchBeam");

    WaitForSeconds waitForContinuousFireInterval;
    WaitForSeconds waitForFireInterval;
    WaitForSeconds waitBeamCooldownTime;    

    List<GameObject> magazine;
    AudioDate launchSFX;
    Animator animator;
    Transform playerTransform;

    protected override void Awake()
    {
        base.Awake();

        animator = GetComponent<Animator>();

        waitForContinuousFireInterval = new WaitForSeconds(minFireInterval);
        waitForFireInterval = new WaitForSeconds(maxFireInterval);
        waitBeamCooldownTime = new WaitForSeconds(beamCooldownTime);

        magazine = new List<GameObject>(projectiles.Length);
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

    }

    protected override void OnEnable()
    {
        isBeamReady = false;
        muzzleVFX.Stop();
        StartCoroutine(nameof(BeamCooldownCoroutine));
        base.OnEnable();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(playerDetectionTransform.position, playerDetectionSize);
    }

    void ActivateBeamWeapon()
    {
        isBeamReady = false;
        animator.SetTrigger(launchBeamID);
        AutoManager.Instance.PlayRandomSFX(beamChargingSFX);
    }

    void AnimationEventLaunchBeam()
    {
        AutoManager.Instance.PlayRandomSFX(beamLaunchSFX);
    }

    void AnimationEventStopBeam()
    {
        StopCoroutine(nameof(ChasingPlayerCoroutine));
        StartCoroutine(nameof(BeamCooldownCoroutine));
        StartCoroutine(nameof(RandomFireCoroutine));
    }

    void LoadProjectiles()
    {
        magazine.Clear();

        if (Physics2D.OverlapBox(playerDetectionTransform.position, playerDetectionSize, 0f, playerLayer))
        {
            magazine.Add(projectiles[0]);
            launchSFX = projectilesLaunchSFX[0];
        }
        else 
        {
            if (Random.value < 0.5f)
            {
                magazine.Add(projectiles[1]);
                launchSFX = projectilesLaunchSFX[1];
            }
            else 
            {
                for (int i = 2; i < projectiles.Length; i++)
                {
                    magazine.Add(projectiles[i]);
                }

                launchSFX = projectilesLaunchSFX[2];
            }
        }
    }

    protected override IEnumerator RandomFireCoroutine()
    {
        while (isActiveAndEnabled)
        {
            
            if (GameManager.GameState == GameState.GameOver) yield break;

            if (isBeamReady)
            {
                ActivateBeamWeapon();
                StartCoroutine(nameof(ChasingPlayerCoroutine));

                yield break;
            }
            
            yield return waitForFireInterval;
            yield return StartCoroutine(nameof(ContinuousFireCoroutine));
        }
    }

    IEnumerator ContinuousFireCoroutine()
    {
        LoadProjectiles();
        muzzleVFX.Play();

        float continuousFireTimer = 0f;

        while (continuousFireTimer < continuousFireDuration)
        {
            foreach (var projectile in magazine)
            {
                PoolManager.Release(projectile, muzzle.position);
            }

            continuousFireTimer += minFireInterval;
            AutoManager.Instance.PlayRandomSFX(launchSFX);

            yield return waitForContinuousFireInterval;
        }

        muzzleVFX.Stop();
    }

    IEnumerator BeamCooldownCoroutine()
    {
        yield return waitBeamCooldownTime;

        isBeamReady = true;
    }
    IEnumerator ChasingPlayerCoroutine()
    {
        while (isActiveAndEnabled)
        {
            targetPositon.x = Viewport.Instance.MaxX - paddingX;
            targetPositon.y = playerTransform.position.y;

            yield return null;
        }
    }
}
