using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _buildName;
    [SerializeField]
    private Image _buildIcon;
    [SerializeField]
    private TextMeshProUGUI _buildCost;

    public void SetData(Construction cons)
    {
        _buildName.text = cons.data.Name;
        _buildIcon.sprite = cons.data.Sprite;
        _buildCost.text = FormatResource.FormatNumber(cons.data.Unlock);
    }
}
