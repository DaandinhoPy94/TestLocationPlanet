using System.Collections.Generic;
using UnityEngine;

public class MissionUnlockHandler : MonoBehaviour
{
    [Header("Referenties")]
    public LevelManager levelManager;
    public LevelUnlocks levelUnlocks;

    [Header("Alle missie-panels in de scene")]
    public List<MissionUIEntryController> allMissionPanels;

    void Start()
    {
        if (levelManager == null || levelUnlocks == null)
        {
            Debug.LogError("[MissionUnlockHandler] LevelManager of LevelUnlocks niet gekoppeld!");
            return;
        }

        UpdateUnlockedMissions(levelManager.CurrentLevel);
        levelManager.OnLevelChanged += UpdateUnlockedMissions;
    }

    void OnDestroy()
    {
        levelManager.OnLevelChanged -= UpdateUnlockedMissions;
    }

    private void UpdateUnlockedMissions(int currentLevel)
    {
        HashSet<ExplorationMission> unlockedMissions = new();

        foreach (var entry in levelUnlocks.unlocksPerLevel)
        {
            if (entry.level <= currentLevel)
            {
                foreach (var mission in entry.unlockedMissions)
                {
                    unlockedMissions.Add(mission);
                }
            }
        }

        foreach (var panel in allMissionPanels)
        {
            bool isUnlocked = unlockedMissions.Contains(panel.missionData);
            panel.gameObject.SetActive(isUnlocked);
        }

        Debug.Log($"[MissionUnlockHandler] Geactiveerde missiepanels voor level {currentLevel}: {unlockedMissions.Count}");
    }
}
