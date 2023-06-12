using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("--------MOVE--------")]
    protected float paddingX; 
    protected float paddingY;
    protected Vector3 targetPositon;
    [SerializeField]float moveSpeed = 2f;
    [SerializeField]float moveRotationAngle = 25f;

    [Header("--------FIRE--------")]
    [SerializeField]protected GameObject[] projectiles;
    [SerializeField]protected AudioDate[] projectilesLaunchSFX;     
    [SerializeField]protected Transform muzzle;
    [SerializeField]protected ParticleSystem muzzleVFX;
    [SerializeField]protected float minFireInterval;
    [SerializeField]protected float maxFireInterval;

    WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();

    protected virtual void Awake() 
    {
        var size = transform.GetChild(0).GetComponent<Renderer>().bounds.size;
        paddingX = size.x / 2f;
        paddingY = size.y / 2f;
    }
    protected virtual void OnEnable() 
    {
        StartCoroutine(nameof(RandomlyMovingCoroutine));
        StartCoroutine(nameof(RandomFireCoroutine));
    }
    void OnDisable() 
    {
        StopAllCoroutines();
    }
    IEnumerator RandomlyMovingCoroutine()
    {
        transform.position = Viewport.Instance.RandomEnemySpawnPosition(paddingX, paddingY);

        targetPositon = Viewport.Instance.RandomRightHalfPosition(paddingX, paddingY);

        while(gameObject.activeSelf)
        {
            if(Vector3.Distance(transform.position, targetPositon) >= moveSpeed * Time.fixedDeltaTime)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPositon, moveSpeed * Time.fixedDeltaTime);
                transform.rotation = Quaternion.AngleAxis((targetPositon - transform.position).normalized.y * moveRotationAngle, Vector3.right);
            }
            else
            {
                targetPositon = Viewport.Instance.RandomRightHalfPosition(paddingX, paddingY);
            }

            yield return WaitForFixedUpdate;
        }
    }
    protected virtual IEnumerator RandomFireCoroutine()
    {
        while(gameObject.activeSelf)
        {
            yield return new WaitForSeconds(Random.Range(minFireInterval, maxFireInterval));

            if(GameManager.GameState == GameState.GameOver)
            {
                yield break;
            }

            foreach(var projectile in projectiles)
            {
                PoolManager.Release(projectile, muzzle.position);
            }

            AutoManager.Instance.PlayRandomSFX(projectilesLaunchSFX);
            muzzleVFX.Play();
        }
    }
}
