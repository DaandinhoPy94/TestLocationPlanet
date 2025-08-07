using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMission", menuName = "Missions/Exploration Mission")]
public class ExplorationMission : ScriptableObject
{
    public string missionName;
    public float durationSeconds;
    public int unlockLevel;

    public List<ItemRequirement> inputRequirements = new();
    public List<RewardChance> dropTable = new();
}

[System.Serializable]
public class ItemRequirement
{
    public ResourceDefinition resource;
    public int amount;
}

[System.Serializable]
public class RewardChance
{
    public ResourceDefinition resource;
    public float chance; // 0.25 = 25% kans
}
