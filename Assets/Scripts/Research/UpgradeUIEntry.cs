using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeUIEntry : MonoBehaviour
{
    private ScriptableObject buildingData;
    private IUpgradeableBuilding upgradeable;

    [Header("UI")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI infoText;
    public Button upgradeButton;

    public void Init(ScriptableObject data)
    {
        buildingData = data;
        upgradeable = data as IUpgradeableBuilding;

        if (upgradeable == null)
        {
            Debug.LogError($"[UpgradeUIEntry] {data.name} implementeert IUpgradeableBuilding niet.");
            gameObject.SetActive(false);
            return;
        }

        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(HandleUpgrade);

        Refresh();
    }

    private void HandleUpgrade()
    {
        int level = BuildingUpgradeManager.Instance.GetLevel(buildingData);
        if (level >= upgradeable.GetMaxLevel() - 1)
        {
            Refresh();
            return;
        }

        UpgradeCost[] costs = GetUpgradeCosts(level);
        bool canAfford = true;
        foreach (var cost in costs)
        {
            if (cost == null || cost.resource == null) continue;
            if (!ResourceManager.Instance.HasEnough(cost.resource.resourceName, cost.amount))
            {
                canAfford = false;
                break;
            }
        }

        if (!canAfford)
        {
            Debug.Log("Niet genoeg resources voor upgrade!");
            // Hier kun je eventueel een UI-melding tonen
            Refresh();
            return;
        }

        // Trek resources af
        foreach (var cost in costs)
        {
            if (cost == null || cost.resource == null) continue;
            ResourceManager.Instance.TryConsumeResource(cost.resource.resourceName, cost.amount);
        }

        BuildingUpgradeManager.Instance.Upgrade(buildingData);
        Refresh();
    }

    private UpgradeCost[] GetUpgradeCosts(int level)
    {
        // FactoryDefinition
        if (buildingData is FactoryDefinition factory && level < factory.levels.Length)
            return factory.levels[level].upgradeCosts;
        // AdvancedFactoryDefinition
        if (buildingData is AdvancedFactoryDefinition advFactory && level < advFactory.levelData.Length)
            return advFactory.levelData[level].upgradeCosts;
        // ResourceNodeDefinition
        if (buildingData is ResourceNodeDefinition node && level < node.levelData.Length)
            return node.levelData[level].upgradeCosts;
        return new UpgradeCost[0];
    }

    private string FormatCosts(UpgradeCost[] costs)
    {
        if (costs == null || costs.Length == 0) return "Gratis";
        var parts = new System.Collections.Generic.List<string>();
        foreach (var cost in costs)
        {
            if (cost == null || cost.resource == null) continue;
            parts.Add($"{cost.amount} {cost.resource.resourceName}");
        }
        return string.Join(", ", parts);
    }

    private void Refresh()
    {
        if (upgradeable == null || buildingData == null)
        {
            Debug.LogError("[UpgradeUIEntry] Kan niet verversen: upgradeable of buildingData is null.");
            return;
        }
        int level = BuildingUpgradeManager.Instance.GetLevel(buildingData);
        nameText.text = upgradeable.GetDisplayName();
        levelText.text = $"Level {level + 1}";
        infoText.text = upgradeable.GetLevelInfo(level);
        // Kosten ophalen voor de volgende upgrade
        UpgradeCost[] costs = (level < upgradeable.GetMaxLevel() - 1) ? GetUpgradeCosts(level) : null;
        string costString = (level < upgradeable.GetMaxLevel() - 1) ? FormatCosts(costs) : "Max level bereikt";
        // Toon kosten in de button zelf
        if (upgradeButton != null)
        {
            var btnText = upgradeButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (btnText != null)
                btnText.text = (level < upgradeable.GetMaxLevel() - 1) ? $"Upgrade ({costString})" : "Max level";
        }
        upgradeButton.interactable = (level < upgradeable.GetMaxLevel() - 1);
    }
}