using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Example : MonoBehaviour
{
    public GameObject prefab;

    public float destroyTimeMin = 0f;
    public float destroyTimeMax = 2f;
    public float spawnTime = 1f;

    private IPool<GameObject> pool;
    private float spawnTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        pool = new GameObjectPool(prefab, 10);
        pool.OnRecycle += (GameObject content) =>
        {
            content.transform.position = Vector3.zero;
        };
    }

    // Update is called once per frame
    void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnTime)
        {
            spawnTimer = 0f;
            IPoolObject<GameObject> poolObject = pool.Get();
            StartCoroutine(UpdateInstance(poolObject, Random.Range(destroyTimeMin, destroyTimeMax)));
        }
    }

    IEnumerator UpdateInstance(IPoolObject<GameObject> poolObject, float destroyTime)
    {
        float timer = 0;
        Vector3 direction = Random.onUnitSphere;
        while (timer < destroyTime)
        {
            poolObject.Content.transform.Translate(direction * 1f * Time.deltaTime);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
        poolObject.Recycle();
    }
}
