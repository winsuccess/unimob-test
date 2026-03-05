using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeItemView : MonoBehaviour
{
    [SerializeField]
    private Image _Icon;
    [SerializeField]
    private TextMeshProUGUI _TitleText;
    [SerializeField]
    private TextMeshProUGUI _DescriptionText;
    [SerializeField]
    private TextMeshProUGUI _CostText;

    private UpgradeItemData _data;

    public void SetData(UpgradeItemData data)
    {
        _data = data;
        _Icon.sprite = data.Icon;
        _TitleText.text = data.Title;
        _DescriptionText.text = data.Desc;
        _CostText.text = FormatResource.FormatNumber(data.Cost);
    }

    public void OnUpgradeButtonClicked()
    {
        if (_data != null)
        {
            var bought = UpgradeManager.Instance.BuyUpgrade(_data.Id);
            if (bought)
                gameObject.SetActive(false);
        }
    }
}
