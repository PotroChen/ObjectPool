/// <summary>
/// 对象池被管理单位
/// </summary>
/// <typeparam name="T"></typeparam>
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
    /// </summary>
    /// <param name="poolObject"></param>
    void Dispose();
}