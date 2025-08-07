using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [Header("Alle grondstoffen (in te stellen via Inspector)")]
    public List<ResourceDefinition> allResources;

    private Dictionary<string, int> resourceAmounts = new();

    public event Action OnResourceChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        foreach (var def in allResources)
        {
            resourceAmounts[def.resourceName] = def.startAmount;
        }
    }

    public bool AddResource(string name, int amount)
    {
        if (!CanAddResource(name, amount))
        {
            Debug.LogWarning($"[ResourceManager] Niet genoeg opslagruimte voor {amount} {name}!");
            return false;
        }
        if (!resourceAmounts.ContainsKey(name))
        {
            Debug.LogWarning($"[ResourceManager] Onbekende grondstof: {name}");
            return false;
        }
        resourceAmounts[name] += amount;
        Debug.Log($"[ResourceManager] +{amount} {name} (totaal = {resourceAmounts[name]})");
        OnResourceChanged?.Invoke();
        return true;
    }

    public int GetAmount(string name)
    {
        return resourceAmounts.ContainsKey(name) ? resourceAmounts[name] : 0;
    }

    public bool HasEnough(string name, int amount)
    {
        return GetAmount(name) >= amount;
    }

    public bool TryConsumeResource(string name, int amount)
    {
        if (!resourceAmounts.ContainsKey(name)) return false;
        if (resourceAmounts[name] < amount) return false;

        resourceAmounts[name] -= amount;
        Debug.Log($"[ResourceManager] -{amount} {name} (totaal = {resourceAmounts[name]})");
        OnResourceChanged?.Invoke();
        return true;
    }

    public bool CanAddResource(string name, int amount)
    {
        int total = 0;
        foreach (var def in allResources)
            total += GetAmount(def.resourceName);
        int capacity = StorageUpgradeManager.Instance.GetCurrentCapacity();
        return resourceAmounts.ContainsKey(name) && (total + amount <= capacity);
    }

    public List<(ResourceDefinition, int)> GetAllResources()
    {
        var result = new List<(ResourceDefinition, int)>();
        foreach (var def in allResources)
        {
            result.Add((def, GetAmount(def.resourceName)));
        }
        return result;
    }
}
