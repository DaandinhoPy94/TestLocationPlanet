using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelUnlocks", menuName = "Player/Level Unlocks")]
public class LevelUnlocks : ScriptableObject
{
    [System.Serializable]
    public class LevelUnlockData
    {
        public int level;
        public List<ResourceDefinition> unlockedResources;
        public List<FactoryDefinition> unlockedFactories;
        public List<AdvancedFactoryDefinition> unlockedAdvancedFactories;
        public List<ResourceNodeDefinition> unlockedResourceNodes; // ✅ Nieuw toegevoegd
        public List<ExplorationMission> unlockedMissions;
    }

    public List<LevelUnlockData> unlocksPerLevel;
} 
