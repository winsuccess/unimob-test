using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _gemText;
    [SerializeField]
    private TextMeshProUGUI _coinText;

    [SerializeField]
    private List<ConstructionUI> _consView;
    [SerializeField]
    private UpgradePopup _upgradeView;


    public void OnShowUpgradeView()
    {
        _upgradeView.gameObject.SetActive(true);
    }

    public void OnHideUpgradeView()
    {
        _upgradeView.gameObject.SetActive(false);
    }

    private void Start()
    {
        ResourceManager.Instance.AddGoldAction(() => {
            _coinText.text = FormatResource.FormatNumber(ResourceManager.Instance.GetGold());
        });

        ResourceManager.Instance.AddGold(0);
    }

    public void AddGold(float amount)
    {
        ResourceManager.Instance.AddGold(amount);
    }
}
