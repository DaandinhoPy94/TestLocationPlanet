using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StorageUpgradeUIController : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI statusText; // toont 21/50 status
    public Button upgradeButton;
    public TextMeshProUGUI upgradeButtonText;

    void Start()
    {
        Debug.Log("[StorageUpgradeUIController] Start: UI init en event-subscriptie");
        RefreshUI();
        upgradeButton.onClick.AddListener(OnUpgradeClicked);
        ResourceManager.Instance.OnResourceChanged += RefreshUI;
    }

    void RefreshUI()
    {
        Debug.Log("[StorageUpgradeUIController] RefreshUI aangeroepen");
        int totalResources = 0;
        foreach (var res in ResourceManager.Instance.GetAllResources())
        {
            totalResources += res.Item2;
        }
        int capacity = StorageUpgradeManager.Instance.GetCurrentCapacity();
        Debug.Log($"[StorageUpgradeUIController] Status: {totalResources}/{capacity}");
        if (statusText != null)
            statusText.text = $"{totalResources}/{capacity}";
        upgradeButtonText.text = $"Upgrade ({StorageUpgradeManager.Instance.GetNextUpgradeCostsString()})";
    }

    void OnUpgradeClicked()
    {
        StorageUpgradeManager.Instance.UpgradeButtonClicked();
        RefreshUI();
    }
}
