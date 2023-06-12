using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMissile : PlayerProjectileOverdrive
{
    [SerializeField]AudioDate targetAcquiredVoice = null;
    [Header("------SPEED CHANGE------")]
    [SerializeField]float lowSpeed = 8f;
    [SerializeField]float highSpeed = 25f;
    [SerializeField]float variableSpeedDelay = 0.5f;

    [Header("==== EXPLOSION ====")]
    [SerializeField]GameObject explosionVFX = null;
    [SerializeField]AudioDate explosionSFX = null;
    WaitForSeconds waitVariableSpeedDelay;
    protected override void Awake()
    {
        base.Awake();
        waitVariableSpeedDelay = new WaitForSeconds(variableSpeedDelay);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(nameof(VariableSpeedCoroutine));
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        // Spawn a explosion VFX
        PoolManager.Release(explosionVFX, transform.position);
        // Play explosion SFX
        AutoManager.Instance.PlayRandomSFX(explosionSFX);
    }
    IEnumerator VariableSpeedCoroutine()
    {
        moveSpeed = lowSpeed;

        yield return waitVariableSpeedDelay;

        moveSpeed = highSpeed; 

        if(target != null)
        {
            AutoManager.Instance.PlayRandomSFX(targetAcquiredVoice);
        }
    }
}
