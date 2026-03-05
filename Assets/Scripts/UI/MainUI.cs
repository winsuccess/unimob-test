using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

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

    private void Update()
    {
        // Mobile
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                return;

            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            ProcessRay(ray);
        }

        // PC
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            ProcessRay(ray);
        }
    }

    void ProcessRay(Ray ray)
    {
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.TryGetComponent(out ConstructionHandler construction) ||
                hit.collider.GetComponentInParent<ConstructionHandler>() != null)
            {
                construction.GetConstructionUI().OnShowUI();
            }
        }
    }
}
