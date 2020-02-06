using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// GameObject对象池
/// </summary>
public class GameObjectPool : IPool<GameObject>
{
    public event Action<GameObject> OnRecycle;

    public int MaxSize { get; set; }
    public int TotalCount { get { return objectsInPools.Count + objectsPoped.Count; } }
    private List<IPoolObject<GameObject>> objectsInPools = new List<IPoolObject<GameObject>>();
    private List<IPoolObject<GameObject>> objectsPoped = new List<IPoolObject<GameObject>>();
    private GameObject prefab;

    private GameObject poolRoot;
    public GameObjectPool(GameObject prefab, int maxSize)
    {
        this.prefab = prefab;
        this.MaxSize = maxSize;
        poolRoot = new GameObject("GameObjectPool");
    }

    public void WarmUp()
    {
        if (TotalCount < MaxSize)
        {
            GameObject content = GameObject.Instantiate(prefab);
            content.transform.SetParent(poolRoot.transform);
            content.SetActive(false);
            IPoolObject<GameObject> poolObject = new GameObjectPoolObject(this, content);
            objectsInPools.Add(poolObject);
        }
    }

    public IPoolObject<GameObject> Get()
    {
        IPoolObject<GameObject> poolObject = null;

        if (objectsInPools.Count == 0)
        {
            GameObject content = GameObject.Instantiate(prefab);
            poolObject = new GameObjectPoolObject(this, content);

            poolObject.Content.SetActive(true);

            objectsPoped.Add(poolObject);
        }
        else
        {
            int lastIndex = objectsInPools.Count - 1;
            poolObject = objectsInPools[lastIndex];

            poolObject.Content.SetActive(true);

            objectsInPools.RemoveAt(lastIndex);
            objectsPoped.Add(poolObject);
        }
        poolObject.Content.transform.SetParent(null);
        return poolObject;
    }

    public void Recycle(IPoolObject<GameObject> poolObject)
    {
        if (poolObject.Pool != this)
            return;

        OnRecycle?.Invoke(poolObject.Content);
        if (TotalCount > MaxSize)
        {
            poolObject.Dispose();
        }
        else
        {
            objectsPoped.Remove(poolObject);
            objectsInPools.Add(poolObject);

            poolObject.Content.transform.SetParent(poolRoot.transform);
            poolObject.Content.SetActive(false);
        }
    }

    public void Dispose(IPoolObject<GameObject> poolObject)
    {
        if (poolObject.Pool != this)
            return;

        GameObject.DestroyImmediate(poolObject.Content);

        if (objectsPoped.Contains(poolObject))
            objectsPoped.Remove(poolObject);

        if (objectsInPools.Contains(poolObject))
            objectsInPools.Remove(poolObject);
    }
}