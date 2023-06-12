using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("------DEATH------")]
    [SerializeField]GameObject deathVFX;
    [SerializeField]AudioDate[] deathSFX;

    [Header("------HEALTH------")]
    [SerializeField]protected float  maxHealth;
    [SerializeField]bool showOnheadHealthBar = true;
    [SerializeField]StatsBar onHeadHealthBar;
    protected float health;
    protected virtual void OnEnable()
    {
        health = maxHealth;
        if(showOnheadHealthBar)
        {
            ShowOnHeadHealthBar();
        }
        else
        {
            HideOnHeadHelthBar();
        }
    }

    public void ShowOnHeadHealthBar()
    {
        onHeadHealthBar.gameObject.SetActive(true);
        onHeadHealthBar.Initialize(health, maxHealth);
    }

    public void HideOnHeadHelthBar()
    {
        onHeadHealthBar.gameObject.SetActive(false);
    }
    public virtual void TakeDamage(float damage)
    {
        if(health == 0f) return;

        health -= damage;
        if(showOnheadHealthBar )
        {
            onHeadHealthBar.UpdateStats(health, maxHealth);
        }
        if(health <= 0)
        {
            Die();
        }
    }
    public virtual void Die()
    {
        health = 0f;
        AutoManager.Instance.PlayRandomSFX(deathSFX);
        PoolManager.Release(deathVFX, transform.position);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 回血函数
    /// </summary>
    /// <param name="value"></param>
    public virtual void RestoreHealth(float value)
    {
        if(health == maxHealth)
            return;
        health = Mathf.Clamp(health + value, 0f, maxHealth);

        if(showOnheadHealthBar)
        {
            onHeadHealthBar.UpdateStats(health, maxHealth);
        }
    }

    /// <summary>
    /// 持续回血功能线程：一定时间内回复固定的百分比生命值。
    /// </summary>
    /// <param name="waitTime"></param>
    /// <param name="percent"></param>
    /// <returns></returns>
    protected IEnumerator HealthRegenerateCoroutine(WaitForSeconds waitTime, float percent)
    {
        while(health < maxHealth)
        {
            yield return waitTime;
            
            RestoreHealth(maxHealth * percent);
        }
    }

    /// <summary>
    /// 持续性伤害线程
    /// </summary>
    /// <param name="waitTime"></param>
    /// <param name="percent"></param>
    /// <returns></returns>
    protected IEnumerator DamageOvertimeCoroutine(WaitForSeconds waitTime, float percent)
    {
        while(health > 0f)
        {
            yield return waitTime;
            
            TakeDamage(maxHealth * percent);
        }
    }
}
