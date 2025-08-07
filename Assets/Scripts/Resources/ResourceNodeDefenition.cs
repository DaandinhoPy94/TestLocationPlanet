using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Resources/Resource Node Definition")]
public class ResourceNodeDefinition : ScriptableObject, IUpgradeableBuilding
{
    public ResourceDefinition resource;  // Verwijzing naar de bijbehorende resource

    public ResourceNodeLevelData[] levelData;
    public string GetDisplayName() => resource != null ? resource.resourceName : "Onbekend";

    public int GetMaxLevel() => levelData.Length;

    public string GetLevelInfo(int level)
    {
        level = Mathf.Clamp(level, 0, levelData.Length - 1);
        var data = levelData[level];
        return $"Cooldown: {data.cooldown}s\n" +
            $"Per click: {data.amountPerClick}, Max output: {data.maxOutput}";
    }

}
[Serializable]
public class ResourceNodeLevelData
{
    public int amountPerClick;
    public float cooldown;
    public int maxOutput;
    [Header("Upgrade kosten naar volgend level")]
    public UpgradeCost[] upgradeCosts;
}

