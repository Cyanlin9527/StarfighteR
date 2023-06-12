using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]public class Pool 
{
    public GameObject Prefab
    {
        get 
        {
            return prefab;
        }
    }
    public int Size
    {
        get
        {
            return size;
        }
    }
    public int RuntimeSize
    {
        get
        {
            return queue.Count;
        
        }
    }
    [SerializeField] GameObject prefab;
    [SerializeField] int size = 1;
    Queue<GameObject> queue;
    Transform parent;

    public void Initialize(Transform parent)
    {
        queue = new Queue<GameObject>();

        this.parent = parent;
        for(var i = 0; i < size; i ++)
        {
            queue.Enqueue(Copy());
        }
    }
    GameObject Copy()
    {
        var copy = GameObject.Instantiate(prefab, parent);
        copy.SetActive(false);
        return copy;
    }
    GameObject AvailableObject()
    {
        GameObject availableObject = null;
        if(queue.Count > 0 && !queue.Peek().activeSelf)
        {
            availableObject = queue.Dequeue();
        }
        else
        {
            availableObject = Copy();
        }
        queue.Enqueue(availableObject);
        return availableObject;
    }
    public GameObject PreparedObjext()
    { 
        GameObject preparedObject = AvailableObject();

        preparedObject.SetActive(true);

        return preparedObject;
    }
    public GameObject PreparedObjext(Vector3 position)
    { 
        GameObject preparedObject = AvailableObject();

        preparedObject.SetActive(true);
        preparedObject.transform.position = position;

        return preparedObject;
    }
     public GameObject PreparedObjext(Vector3 position, Quaternion rotation)
    { 
        GameObject preparedObject = AvailableObject();

        preparedObject.SetActive(true);
        preparedObject.transform.position = position;
        preparedObject.transform.rotation = rotation;

        return preparedObject;
    }
    public GameObject PreparedObjext(Vector3 position, Quaternion rotation, Vector3 localScale)
    { 
        GameObject preparedObject = AvailableObject();

        preparedObject.SetActive(true);
        preparedObject.transform.position = position;
        preparedObject.transform.rotation = rotation;
        preparedObject.transform.localScale = localScale; 

        return preparedObject;
    }
}
