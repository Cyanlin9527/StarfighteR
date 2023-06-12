using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileSystem : MonoBehaviour
{
    [SerializeField] int defaultAmount = 3;
    [SerializeField] float cooldownTime = 1f; 
    [SerializeField]GameObject missilePrefab = null;
    [SerializeField]AudioDate launchSFX = null;

    bool isReady = true;
    int amount;

    void Awake()
    {
        amount = defaultAmount;
    }

    void Start()
    {
        MissileDisplay.UpdateAmountText(amount);
    }

    public void PickUp () 
    {
        amount++;
        MissileDisplay.UpdateAmountText(amount);

        if(amount == 1)
        {
            MissileDisplay.UpdateAmountText(amount);
            isReady = true;
        }
    }

    public void Launch (Transform muzzleTransform) 
    {
        if (amount == 0 || !isReady) return; 

        isReady = false; 
        PoolManager.Release(missilePrefab, muzzleTransform.position);
        AutoManager.Instance.PlayRandomSFX(launchSFX);
        amount--;
        MissileDisplay.UpdateAmountText(amount);

        if (amount == 0)
        {
            MissileDisplay.UpdateCooldownImage(1f);
        }
        else
        {
            StartCoroutine(CooldownCoroutine());
        }
    }

    IEnumerator CooldownCoroutine()
    {
        var cooldownValue = cooldownTime;

        while (cooldownValue > 0f)
        {
            MissileDisplay.UpdateCooldownImage(cooldownValue / cooldownTime);
            cooldownValue = Mathf.Max(cooldownValue - Time.deltaTime, 0f);

            yield return null;
        }

        isReady = true;
    }
}
