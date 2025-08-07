using UnityEngine;

[CreateAssetMenu(fileName = "AdvancedFactoryDefinition", menuName = "Factories/AdvancedFactoryDefinition")]
public class AdvancedFactoryDefinition : ScriptableObject, IUpgradeableBuilding
{
    public string factoryName;

    [Header("Mogelijke recepten")]
    public AdvancedRecipeDefinition[] availableRecipes;

    [Header("Level specifieke data (voor toekomstig gebruik)")]
    public AdvancedFactoryLevelData[] levelData;
    
    public string GetDisplayName() => factoryName;

    public int GetMaxLevel() => levelData.Length;

    public string GetLevelInfo(int level)
    {
        level = Mathf.Clamp(level, 0, levelData.Length - 1);
        var data = levelData[level];
        return $"Speed: x{data.productionSpeedMultiplier:F2}\n" +
            $"Queue: {data.maxQueueSize}, Buffer: {data.maxReadyBuffer}";
    }
}

[System.Serializable]
public class AdvancedFactoryLevelData
{
    [Tooltip("Vermenigvuldiger voor productie. 1.0 = normaal, 0.9 = 10% sneller")]
    public float productionSpeedMultiplier = 1f;

    public int maxQueueSize = 3;
    public int maxReadyBuffer = 5;

    [Tooltip("Welke recepten zijn beschikbaar op dit level")]
    public AdvancedRecipeDefinition[] unlockedRecipes;

    [Header("Upgrade kosten naar volgend level")]
    public UpgradeCost[] upgradeCosts;
}


