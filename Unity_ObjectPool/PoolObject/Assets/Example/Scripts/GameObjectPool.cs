using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO 提供初始化的委托接口，回收的事件
//TODO 销毁这个方法并不希望使用对象池的用户使用，可能应该用基类实现而不是接口
//TODO 再封装一层泛型基类
public class GameObjectPool : IPool<GameObject>
{
    public int MaxSize { get; set; }
    public int TotalCount { get { return objectsInPools.Count + objectsPoped.Count; } }
    private List<IPoolObject<GameObject>> objectsInPools = new List<IPoolObject<GameObject>>();
    private List<IPoolObject<GameObject>> objectsPoped = new List<IPoolObject<GameObject>>();
    private GameObject prefab;
    public GameObjectPool(GameObject prefab, int maxSize)
    {
        this.prefab = prefab;
        this.MaxSize = maxSize;
    }

    public void WarmUp()
    {
        if (TotalCount < MaxSize)
        {
            GameObject content = GameObject.Instantiate(prefab);
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
        return poolObject;
    }

    public void Recycle(IPoolObject<GameObject> poolObject)
    {
        if (poolObject.Pool != this)
            return;

        if (TotalCount > MaxSize)
        {
            poolObject.Dispose();
        }
        else
        {
            objectsPoped.Remove(poolObject);
            objectsInPools.Add(poolObject);
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

public class GameObjectPoolObject : IPoolObject<GameObject>
{
    public IPool<GameObject> Pool { get; set; }
    public GameObject Content { get; set; }

    public GameObjectPoolObject(GameObjectPool pool, GameObject content)
    {
        Pool = pool;
        Content = content;
    }

    public void Recycle()
    {
        Pool.Recycle(this);
    }

    public void Dispose()
    {
        Pool.Dispose(this);
    }
}

public interface IPool<T>
{
    /// <summary>
    /// 预先将池加满（可以不调用）
    /// </summary>
    void WarmUp();

    /// <summary>
    /// 获取IPoolObject
    /// </summary>
    /// <returns></returns>
    IPoolObject<T> Get();

    /// <summary>
    /// 回收IPoolObject
    /// </summary>
    /// <param name="poolObject"></param>
    void Recycle(IPoolObject<T> poolObject);

    /// <summary>
    /// 销毁IPoolObject
    /// TODO 这个方法并不希望使用对象池的用户使用，可能应该用基类实现而不是接口
    /// </summary>
    /// <param name="poolObject"></param>
    void Dispose(IPoolObject<T> poolObject);
}

public interface IPoolObject<T>
{
    /// <summary>
    /// 对象池的引用
    /// </summary>
    /// <value></value>
    IPool<T> Pool { get; }

    /// <summary>
    /// 内容
    /// </summary>
    /// <value></value>
    T Content { get; }

    /// <summary>
    /// 回收自己
    /// </summary>
    /// <param name="poolObject"></param>
    void Recycle();

    /// <summary>
    /// 销毁自己IPoolObject
    /// TODO 这个方法并不希望使用对象池的用户使用，可能应该用基类实现而不是接口
    /// </summary>
    /// <param name="poolObject"></param>
    void Dispose();
}
