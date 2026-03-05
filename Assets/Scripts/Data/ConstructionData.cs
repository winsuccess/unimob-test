using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConstructionData", menuName = "Game/ConstructionData")]
public class ConstructionData : ScriptableObject
{
    public int Id;
    public string Name;
    public Sprite Sprite;
    public double Unlock;
    public double ProfitBase;
    public int ProfitScale;
    public double UpgradeCostBase;
    public int UpgradeCostScale;
    public int MaxLevel;
}