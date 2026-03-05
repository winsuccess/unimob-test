using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    private static EffectManager _instance;

    public static EffectManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<EffectManager>();

                if (_instance == null)
                {
                    GameObject obj = new GameObject("EffectManager");
                    _instance = obj.AddComponent<EffectManager>();
                }
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [SerializeField] private GameObject _effectBuild;
    [SerializeField] private GameObject _effectPay;
    [SerializeField] private int _poolSize = 5;

    private Queue<GameObject> _buildPool = new Queue<GameObject>();
    private Queue<GameObject> _payPool = new Queue<GameObject>();

    private void Start()
    {
        CreatePool(_effectBuild, _buildPool);
        CreatePool(_effectPay, _payPool);
    }

    private void CreatePool(GameObject prefab, Queue<GameObject> pool)
    {
        for (int i = 0; i < _poolSize; i++)
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject SpawnBuildEffect(Vector3 position)
    {
        GameObject obj = SpawnFromPool(_buildPool, _effectBuild, position);
        StartCoroutine(AutoReturn(obj, _buildPool));
        return obj;
    }

    public GameObject SpawnPayEffect(Vector3 position)
    {
        GameObject obj = SpawnFromPool(_payPool, _effectPay, position + new Vector3(0f, 1f, 0f));
        StartCoroutine(AutoReturn(obj, _payPool));
        return obj;
    }

    private GameObject SpawnFromPool(Queue<GameObject> pool, GameObject prefab, Vector3 position)
    {
        GameObject obj;

        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
        }
        else
        {
            obj = Instantiate(prefab, transform);
        }

        obj.transform.position = position;
        obj.SetActive(true);

        return obj;
    }

    private IEnumerator AutoReturn(GameObject obj, Queue<GameObject> pool)
    {
        yield return new WaitForSeconds(1f);

        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}