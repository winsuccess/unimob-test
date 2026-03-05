using UnityEngine;

public class UpgradePopup : MonoBehaviour
{
    [SerializeField]
    private RectTransform _contentHolder;
    [SerializeField]
    private GameObject _upgradeItemPrefab;

    void Start()
    {
        var upgradeItems = UpgradeManager.Instance.GetUpgradeItems();
        for (int i = 0; i < upgradeItems.Count; i++)
        {
            var itemState = upgradeItems[i];
            var itemViewObj = Instantiate(_upgradeItemPrefab, _contentHolder);
            itemViewObj.SetActive(true);
            var itemView = itemViewObj.GetComponent<UpgradeItemView>();
            itemView.SetData(itemState.Data);
        }
    }


    void Update()
    {

    }
}
