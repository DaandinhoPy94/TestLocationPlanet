using UnityEngine;

[CreateAssetMenu(fileName = "FactoryDefinition", menuName = "Factories/FactoryDefinition")]
public class FactoryDefinition : ScriptableObject, IUpgradeableBuilding
{
    public string factoryName;
    public Sprite icon;

    public ResourceDefinition inputResource;
    public ResourceDefinition outputResource; // ← moet op dit niveau staan

    public FactoryLevelData[] levels; // ← hernoem van `levelData` naar `levels`

    public string GetDisplayName() => factoryName;

    public int GetMaxLevel() => levels.Length;

    public string GetLevelInfo(int level)
    {
        level = Mathf.Clamp(level, 0, levels.Length - 1);
        var data = levels[level];
        return $"Time: {data.productionTime}s\n" +
            $"Output: {data.outputAmount}\n" +
            $"Queue: {data.maxQueue}, Buffer: {data.maxBuffer}";
    }

}

[System.Serializable]
public class FactoryLevelData
{
    public float productionTime;
    public int outputAmount;

    public int maxQueue;    // ← hernoem van `maxQueueSize`
    public int maxBuffer;   // ← hernoem van `maxReadyBuffer`

    [Header("Upgrade kosten naar volgend level")]
    public UpgradeCost[] upgradeCosts;
}
