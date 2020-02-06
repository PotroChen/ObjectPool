using UnityEngine;

/// <summary>
/// GameObject对象池被管理对象
/// </summary>
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