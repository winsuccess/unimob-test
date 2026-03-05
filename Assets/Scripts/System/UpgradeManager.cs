using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    private static UpgradeManager _instance;

    public static UpgradeManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<UpgradeManager>();

                if (_instance == null)
                {
                    GameObject obj = new GameObject("UpgradeManager");
                    _instance = obj.AddComponent<UpgradeManager>();
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
    private UpgradeItemListData _upgradeItemListData;

    private List<UpgradeItemState> _upgradeItems;

    private Action<int, int> _onProfitUpdate;

    private void Start()
    {
        if (_upgradeItemListData != null)
        {
            _upgradeItems = new List<UpgradeItemState>();
            foreach (var itemData in _upgradeItemListData.UpgradeItems)
            {
                _upgradeItems.Add(new UpgradeItemState { Data = itemData });
            }
        }
    }

    public List<UpgradeItemState> GetUpgradeItems()
    {
        return _upgradeItems;
    }

    public bool BuyUpgrade(int upgradeId)
    {
        var isBought = false;
        var upgradeItem = _upgradeItems.Find(item => item.Data.Id == upgradeId);
        if (upgradeItem != null && !upgradeItem.IsPurchased)
        {
            if (ResourceManager.Instance.GetGold() >= upgradeItem.Data.Cost)
            {
                ResourceManager.Instance.AddGold(-upgradeItem.Data.Cost);
                upgradeItem.IsPurchased = true;
                isBought = true;
                if (upgradeItem.Data.Type == UpgradeItemData.UpgradeType.ProfitOne)
                {
                    _onProfitUpdate?.Invoke(upgradeItem.Data.TargetConstructionId, upgradeItem.Data.Amount);
                }
                else if (upgradeItem.Data.Type == UpgradeItemData.UpgradeType.ProfitAll)
                {
                    _onProfitUpdate?.Invoke(0, upgradeItem.Data.Amount);
                }
                else if (upgradeItem.Data.Type == UpgradeItemData.UpgradeType.AddCustomer)
                {
                    ResourceManager.Instance.AddCustomer(upgradeItem.Data.Amount);
                }

            }
            else
            {
                Debug.Log("Not enough gold to buy upgrade!");
            }

        }
        return isBought;
    }

    public void AddProfitUpdateAction(Action<int, int> action)
    {
        _onProfitUpdate += action;
    }

    public void ClearProfitUpdateAction()
    {
        _onProfitUpdate = null;
    }
}

public class UpgradeItemState
{
    public UpgradeItemData Data;
    public bool IsPurchased = false;
}
