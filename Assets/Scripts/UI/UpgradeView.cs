using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _levelText;
    [SerializeField]
    private TextMeshProUGUI _consName;
    [SerializeField]
    private Slider _slider;
    [SerializeField]
    private TextMeshProUGUI _nextProfitRate;
    [SerializeField]
    private TextMeshProUGUI _upgradeCost;
    [SerializeField]
    private GameObject _max;

    public void SetData(Construction cons)
    {
        _consName.text = cons.data.Name;
        _levelText.text = $"Level {cons.level}";
        _slider.value = (float)cons.level / cons.data.MaxLevel;
        _upgradeCost.text = FormatResource.FormatNumber(cons.GetCurrentUpgradeCost());
        _upgradeCost.gameObject.SetActive(cons.level < cons.data.MaxLevel);
        _max.SetActive(cons.level >= cons.data.MaxLevel);
        _nextProfitRate.text = FormatResource.FormatNumber(cons.GetCurrentProfit());
        _nextProfitRate.gameObject.SetActive(cons.level < cons.data.MaxLevel);
    }
}
