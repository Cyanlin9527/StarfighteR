using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [SerializeField]Pool[] enemyPools;
    [SerializeField]Pool[] playerProjectilePools;
    [SerializeField]Pool[] enemyProjectilePools;
    [SerializeField]Pool[] vFXPools;
    [SerializeField]Pool[] lootItemPools;

    static Dictionary<GameObject, Pool> dictionary;

    void Awake()
    {
        dictionary = new Dictionary<GameObject, Pool>();
        Initallize(playerProjectilePools);
        Initallize(enemyProjectilePools);
        Initallize(vFXPools);
        Initallize(enemyPools);
        Initallize(lootItemPools);
    }

    #if UNITY_EDITOR
    void OnDestroy() 
    {
        CheckPoolSize(playerProjectilePools);
        CheckPoolSize(enemyProjectilePools);
        CheckPoolSize(vFXPools); 
        CheckPoolSize(enemyPools);
        CheckPoolSize(lootItemPools);
    }
    #endif
    void CheckPoolSize(Pool[] pools)
    {
        foreach(var pool in pools)
        {
            if(pool.RuntimeSize > pool.Size)
            {
                Debug.LogWarning(string.Format("Pool: {0}has a runtime size {1} bihher than its inital size {2}!",
                pool.Prefab.name, pool.RuntimeSize, pool.Size));
            }
        }
    }
    void Initallize(Pool[] pools)
    {
        foreach(var pool in pools)
        {
        #if UNITY_EDITOR    
            if(dictionary.ContainsKey(pool.Prefab))
            {
                Debug.LogError("相同的预制体：" + pool.Prefab.name);
                continue;
            }
        #endif            
            dictionary.Add(pool.Prefab, pool);
            
            Transform poolParent = new GameObject("Pool: " + pool.Prefab.name).transform;
            poolParent.parent = transform;
            
            pool.Initialize(poolParent);
        }
    }

    /// <summary>
    /// <para>根据传入的<preafab name="preafab"></paramref>参数，返回对象池中预备好的游戏对象。</para>
    /// </summary>
    /// <param name="prefab">
    /// <para>指定的游戏对象预制体。</para>
    /// </param>
    /// <returns>
    /// <para>对象池中预备好的游戏对象。</para>
    /// </returns>
    public static GameObject Release(GameObject prefab)
    {
    #if UNITY_EDITOR
        if(!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("对象池中没有找到预制体：" + prefab.name);
            return null;
        }
    #endif
        return dictionary[prefab].PreparedObjext();
    }

    /// <summary>
    /// <para>Release a specified prepared gameObject in the pool at specified position.</para>
    /// <para>根据传入的prefab参数，在position参数位置释放对象池中预备好的游戏对象。</para> 
    /// </summary>
    /// <param name="prefab">
    /// <para>Specified gameObject prefab.</para>
    /// <para>指定的游戏对象预制体。</para>
    /// </param>
    /// <param name="position">
    /// <para>Specified release position.</para>
    /// <para>指定释放位置。</para>
    /// </param>
    /// <returns></returns>
    public static GameObject Release(GameObject prefab, Vector3 position)
    {
    #if UNITY_EDITOR
        if(!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("对象池中没有找到预制体：" + prefab.name);
            return null;
        }
    #endif
        return dictionary[prefab].PreparedObjext(position);
    }
    public static GameObject Release(GameObject prefab, Vector3 position, Quaternion rotation)
    {
    #if UNITY_EDITOR
        if(!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("对象池中没有找到预制体：" + prefab.name);
            return null;
        }
    #endif
        return dictionary[prefab].PreparedObjext(position, rotation);
    }
    public static GameObject Release(GameObject prefab, Vector3 position, Quaternion rotation, Vector3 localScale)
    {
    #if UNITY_EDITOR
        if(!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("对象池中没有找到预制体：" + prefab.name);
            return null;
        }
    #endif
        return dictionary[prefab].PreparedObjext(position, rotation, localScale);
    }
}
