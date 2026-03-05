using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class HumanManager : MonoBehaviour
{
    private static HumanManager _instance;

    public static HumanManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<HumanManager>();

                if (_instance == null)
                {
                    GameObject obj = new GameObject("HumanManager");
                    _instance = obj.AddComponent<HumanManager>();
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

    [SerializeField]
    private GameObject _deliveryGO;
    [SerializeField]
    private GameObject _customerGO;

    [SerializeField] private int _poolSize = 5;

    private Queue<GameObject> _deliveryPool = new Queue<GameObject>();
    private Queue<GameObject> _customerPool = new Queue<GameObject>();

    private void Start()
    {
        CreatePool(_deliveryGO, _deliveryPool);
        CreatePool(_customerGO, _customerPool);
    }

    private void CreatePool(GameObject prefab, Queue<GameObject> pool)
    {
        for (int i = 0; i < _poolSize; i++)
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.name = prefab.name + "_" + i;   // set index name
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject SpawnDelivery(Vector3 position)
    {
        return SpawnFromPool(_deliveryPool, _deliveryGO, position);
    }

    public GameObject SpawnCustomer(Vector3 position)
    {
        return SpawnFromPool(_customerPool, _customerGO, position);
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
            // Optional: expand pool automatically
            obj = Instantiate(prefab, transform);
        }

        obj.transform.position = position;
        obj.SetActive(true);

        return obj;
    }

    public void ReturnToPool(GameObject obj, bool isDelivery)
    {
        obj.SetActive(false);

        if (isDelivery)
            _deliveryPool.Enqueue(obj);
        else
            _customerPool.Enqueue(obj);
    }
    public void PlayIdle(GameObject go)
    {
        var content = go.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Animator>();
        content.Play("Idle");
    }

    public void PlayMove(GameObject go)
    {
        var content = go.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Animator>();
        content.Play("Move");
    }


    public void PlayCarryIdle(GameObject go)
    {
        var content = go.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Animator>();
        content.Play("CarryIdle");
    }

    public void PlayCarryMove(GameObject go)
    {
        var content = go.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Animator>();
        content.Play("CarryMove");
    }
}