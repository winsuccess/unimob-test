using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InformationView : MonoBehaviour
{
    [SerializeField]
    private Image _itemIcon;
    [SerializeField]
    private TextMeshProUGUI _profitText;
    [SerializeField]
    private TextMeshProUGUI _rateText;


    public void SetResourceType(Construction cons)
    {
        _itemIcon.sprite = cons.data.Sprite;
    }

    public void UpdateProfit(Construction cons)
    {
        _profitText.text = FormatResource.FormatNumber(cons.GetCurrentProfit());
    }
}
