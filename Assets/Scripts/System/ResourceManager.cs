using System;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    private static ResourceManager _instance;

    public static ResourceManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<ResourceManager>();

                if (_instance == null)
                {
                    GameObject obj = new GameObject("ResourceManager");
                    _instance = obj.AddComponent<ResourceManager>();
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

    private double _gold;
    private Action _goldUpdateAction;
    private int _customerCount = 0;

    public double GetGold()
    {
        return _gold;
    }

    public void AddGold(double amount)
    {
        _gold += amount;
        _goldUpdateAction?.Invoke();
    }

    public void AddGoldAction(Action action)
    {
        _goldUpdateAction += action;
    }

    public void ClearGoldAction()
    {
        _goldUpdateAction = null;
    }

    public void AddCustomer(int amount)
    {
        _customerCount += amount;
        TransferManager.Instance.AddCustomer(amount);
    }

    public int GetCustomerCount()
    {
        return _customerCount;
    }

}
