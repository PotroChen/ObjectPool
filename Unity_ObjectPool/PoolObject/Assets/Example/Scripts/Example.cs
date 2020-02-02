using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Example : MonoBehaviour
{
    public GameObject prefab;

    public float destroyTimeMin = 0f;
    public float destroyTimeMax = 2f;
    public float spawnTime = 1f;

    private float spawnTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnTime)
        {
            spawnTimer = 0f;
            GameObject instance = GameObject.Instantiate(prefab);
            StartCoroutine(UpdateInstance(instance, Random.Range(destroyTimeMin, destroyTimeMax)));
        }
    }

    IEnumerator UpdateInstance(GameObject instance, float destroyTime)
    {
        float timer = 0;
        Vector3 direction = Random.onUnitSphere;
        while (timer < destroyTime)
        {
            instance.transform.Translate(direction * 1f * Time.deltaTime);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
        GameObject.Destroy(instance);
    }
}
