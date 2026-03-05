using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConstructionUI : MonoBehaviour
{
    [SerializeField]
    private InformationView _infView;
    [SerializeField]
    private BuildView _buildView;
    [SerializeField]
    private UpgradeView _upgradeView;

    private Construction _cons;

    public Action _onUnlockStart;
    public Action _onUnlockEnd;

    public void SetConstruction(Construction cons)
    {
        _cons = cons;
        _infView.SetResourceType(cons);
        UpdateBuildView();
    }

    public void OnShowUI()
    {
        if (_cons == null) return;
        if (_cons.state == 0)
        {
            _buildView.gameObject.SetActive(true);
            _upgradeView.gameObject.SetActive(false);
        }
        else if (_cons.state == 1)
        {
            _buildView.gameObject.SetActive(false);
            _upgradeView.gameObject.SetActive(true);
        }
    }

    public void OnHideUI()
    {
        if (_cons == null) return;
        _buildView.gameObject.SetActive(false);
        _upgradeView.gameObject.SetActive(false);
    }

    public void OnShowInfo()
    {
        if (_cons == null) return;
        _infView.gameObject.SetActive(true);
        UpdateInformationView();
    }

    public void UpdateBuildView()
    {
        _buildView.SetData(_cons);
    }

    public void UpdateUpgradeView()
    {
        _upgradeView.SetData(_cons);
    }

    public void UpdateInformationView()
    {
        _infView.UpdateProfit(_cons);
    }

    public void OnUnlock()
    {
        if (_cons == null) return;
        if (_cons.data.Unlock > ResourceManager.Instance.GetGold())
        {
            Debug.Log("Not enough gold to unlock!");
            return;
        }
        ResourceManager.Instance.AddGold(-_cons.data.Unlock);
        OnHideUI();
        _onUnlockStart.Invoke();
        _onUnlockEnd = () =>
        {
            _cons.Unlock();
            UpdateBuildView();
            UpdateUpgradeView();
            UpdateInformationView();
            _onUnlockEnd = null;
        };

    }

    public void OnUpgrade()
    {
        if (_cons == null) return;
        if (_cons.GetCurrentUpgradeCost() > ResourceManager.Instance.GetGold())
        {
            Debug.Log("Not enough gold to upgrade!");
            return;
        }
        ResourceManager.Instance.AddGold(-_cons.GetCurrentUpgradeCost());
        _cons.Upgrade();
        UpdateUpgradeView();
        UpdateInformationView();

    }
}
