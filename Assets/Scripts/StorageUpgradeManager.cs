using UnityEngine;
using System.Collections.Generic;

public class StorageUpgradeManager : MonoBehaviour
{
    public static StorageUpgradeManager Instance;

    public List<ResourceDefinition> upgradeResources; // Sleep hier de resource types in de Inspector
    public int baseCost = 1; // Startkosten per resource
    public int costStep = 1; // Verhoging per upgrade
    public int baseCapacity = 100;
    public int capacityStep = 10;

    private int currentLevel = 0;
    private int currentCapacity;

    void Awake()
    {
        Instance = this;
        currentCapacity = baseCapacity;
    }

    public int GetCurrentLevel() => currentLevel;
    public int GetCurrentCapacity() => currentCapacity;

    // Kosten berekenen voor de volgende upgrade
    public Dictionary<ResourceDefinition, int> GetNextUpgradeCosts()
    {
        var costs = new Dictionary<ResourceDefinition, int>();
        foreach (var res in upgradeResources)
        {
            costs[res] = baseCost + currentLevel * costStep;
        }
        return costs;
    }

    // Geeft de upgrade kosten als string terug voor UI
    public string GetNextUpgradeCostsString()
    {
        var costs = GetNextUpgradeCosts();
        var parts = new List<string>();
        foreach (var kvp in costs)
        {
            parts.Add($"{kvp.Value} {kvp.Key.resourceName}");
        }
        return string.Join(", ", parts);
    }

    public int GetNextCapacity() => currentCapacity + capacityStep;

    public bool TryUpgrade()
    {
        var costs = GetNextUpgradeCosts();
        Debug.Log($"[StorageUpgradeManager] Probeer upgrade. Huidig level: {currentLevel}, Huidige capaciteit: {currentCapacity}");
        foreach (var kvp in costs)
        {
            Debug.Log($"[StorageUpgradeManager] Nodig: {kvp.Value} van {kvp.Key.resourceName}, Beschikbaar: {ResourceManager.Instance.GetAmount(kvp.Key.resourceName)}");
            if (!ResourceManager.Instance.HasEnough(kvp.Key.resourceName, kvp.Value))
            {
                Debug.LogWarning($"[StorageUpgradeManager] Niet genoeg {kvp.Key.resourceName} voor upgrade!");
                return false;
            }
        }
        // Trek resources af
        foreach (var kvp in costs)
        {
            ResourceManager.Instance.TryConsumeResource(kvp.Key.resourceName, kvp.Value);
            Debug.Log($"[StorageUpgradeManager] -{kvp.Value} {kvp.Key.resourceName} afgetrokken.");
        }
        currentLevel++;
        currentCapacity += capacityStep;
        Debug.Log($"[StorageUpgradeManager] Upgrade uitgevoerd! Nieuw level: {currentLevel}, Nieuwe capaciteit: {currentCapacity}");
        // Eventueel: UI update event triggeren
        return true;
    }

    public void UpgradeButtonClicked()
    {
        Debug.Log("[StorageUpgradeManager] Upgrade-knop geklikt.");
        bool success = TryUpgrade();
        if (success)
            Debug.Log("[StorageUpgradeManager] Upgrade succesvol!");
        else
            Debug.LogWarning("[StorageUpgradeManager] Upgrade mislukt: niet genoeg resources.");
        // Eventueel: UI update event triggeren
    }
}