using System.Collections.Generic;
using UnityEngine;

public class BuildingUpgradeManager : MonoBehaviour
{
    public static BuildingUpgradeManager Instance { get; private set; }

    private Dictionary<ScriptableObject, int> levels = new();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public int GetLevel(ScriptableObject building)
    {
        return levels.TryGetValue(building, out int level) ? level : 0;
    }

    public bool CanUpgrade(ScriptableObject building, int maxLevel)
    {
        int current = GetLevel(building);
        return current < maxLevel - 1;
    }

    public void Upgrade(ScriptableObject building)
    {
        int current = GetLevel(building);
        if (CanUpgrade(building, int.MaxValue)) // maxLevel wordt gecheckt in UI
        {
            levels[building] = current + 1;
            Debug.Log($"[UpgradeManager] {building.name} is nu level {levels[building]}");
        }
    }
}
