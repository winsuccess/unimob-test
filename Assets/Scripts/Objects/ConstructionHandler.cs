using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ConstructionHandler : MonoBehaviour
{
    [SerializeField]
    private Construction _cons;
    [SerializeField]
    private int _side;
    [SerializeField]
    private ConstructionUI constructionUI;
    [SerializeField]
    private Animation _boxAnim;
    [SerializeField]
    private GameObject _box;
    [SerializeField]
    private GameObject _tree;
    [SerializeField]
    private Transform _deliveryStart;
    [SerializeField]
    private Transform _deliveryEnd;

    private BoxCollider boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        constructionUI.SetConstruction(_cons);
        constructionUI._onUnlockStart += () =>
        {
            StartCoroutine(HandlerUnlock());
        };
        UpgradeManager.Instance.AddProfitUpdateAction((id, amount) =>
        {
            if (id == 0 || id == _cons.data.Id)
            {
                _cons.UpdateProfitMultiplier(amount);
                constructionUI.UpdateInformationView();
            }
        });
    }

    IEnumerator HandlerUnlock()
    {
        _cons.state = -1; // isBuilding
        _boxAnim.Play("BoxOpen");

        yield return new WaitForSeconds(_boxAnim["BoxOpen"].length);

        _boxAnim.Play("BoxIdle");
        _box.SetActive(false);
        _tree.SetActive(true);
        constructionUI._onUnlockEnd?.Invoke();
        constructionUI.OnShowInfo();
        TransferManager.Instance.AddConstruction(this);
        EffectManager.Instance.SpawnBuildEffect(transform.position);
    }

    public ConstructionUI GetConstructionUI()
    {
        return constructionUI;
    }

    public Vector3 PositionStart()
    {
        return _deliveryStart.position;
    }

    public Vector3 PositionEnd()
    {
        return _deliveryEnd.position;
    }
    public int GetSide()
    {
        return _side;
    }

    public double GetCurrentProfit()
    {
        return _cons.GetCurrentProfit();
    }
    public double GetNextProfit()
    {
        return _cons.GetNextProfit();
    }
}

[Serializable]
public class Construction
{
    public string name;
    public int state; // 0 : not built, 1: built, -1: isBuilding
    public int level;
    public int profitMultiplier = 1;
    public ConstructionData data;

    public void Unlock()
    {
        if (state == 0 || state == -1)
        {
            state = 1;
            level = 1;
        }
    }

    public void Upgrade()
    {
        if (state == 1 && level < data.MaxLevel)
        {
            level++;
        }
    }

    public void UpdateProfitMultiplier(int amount)
    {
        profitMultiplier *= amount;
    }

    public double GetCurrentProfit()
    {
        if (state == 0) return 0;
        return data.ProfitBase * Math.Pow(data.ProfitScale, level - 1) * profitMultiplier;
    }

    public double GetNextProfit()
    {
        if (state == 0) return 0;
        return data.ProfitBase * Math.Pow(data.ProfitScale, level) * profitMultiplier;
    }

    public double GetCurrentUpgradeCost()
    {
        if (state == 0) return 0;
        return data.UpgradeCostBase * Math.Pow(data.UpgradeCostScale, level - 1);
    }
}
