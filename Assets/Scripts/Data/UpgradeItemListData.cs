using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeItemListData", menuName = "Game/UpgradeItemListData")]
public class UpgradeItemListData : ScriptableObject
{
    public List<UpgradeItemData> UpgradeItems;
}

[Serializable]
public class UpgradeItemData
{
    public int Id;
    public string Title;
    public string Desc;
    public double Cost;
    public Sprite Icon;
    public int Amount;
    public int TargetConstructionId;
    public UpgradeType Type;

    public enum UpgradeType
    {
        ProfitOne,
        ProfitAll,
        AddCustomer,

    };
}