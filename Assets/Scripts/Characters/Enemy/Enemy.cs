using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{

    [SerializeField ]int scorePoint = 100;
    [SerializeField]int deathEnergyBouns = 3;

    LootSpawner lootSpawner;

    protected virtual void Awake()
    {
        lootSpawner = GetComponent<LootSpawner>();
    }

    protected override void OnEnable()
    {
        SetHealth();
        base.OnEnable();
    }

    protected virtual void OnCollisionEnter2D(Collision2D other) 
    {
        if(other.gameObject.TryGetComponent<Player>(out Player player))
        {
            player.Die();
            Die();
        }    
    }
    public override void Die()
    {
        ScoreManager.Instance.AddScore(scorePoint);
        PlayerEnergy.Instance.Obtain(deathEnergyBouns);
        EnemyManager.Instance.RemoveFromList(gameObject);
        lootSpawner.Spawn(transform.position);
        base.Die();
    }
    protected virtual void SetHealth()
    {
        maxHealth += EnemyManager.Instance.WaveNumber * 2   ;
    }
}
