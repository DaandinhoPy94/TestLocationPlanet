using System.Collections.Generic;
using UnityEngine;

public class ResearchPanelController : MonoBehaviour
{
    public Transform contentRoot; // ScrollView content
    public GameObject upgradeEntryPrefab;

    private List<GameObject> activeEntries = new();

    void Start()
    {
        LevelManager.Instance.OnLevelChanged += UpdateUI;
        UpdateUI(LevelManager.Instance.CurrentLevel);
    }

    void OnDestroy()
    {
        if (LevelManager.Instance != null)
            LevelManager.Instance.OnLevelChanged -= UpdateUI;
    }

    private void UpdateUI(int playerLevel)
    {
        // Verwijder oude entries
        foreach (var entry in activeEntries)
            Destroy(entry);
        activeEntries.Clear();

        // Gebruik een HashSet om dubbele gebouwen te voorkomen
        HashSet<ScriptableObject> uniqueBuildings = new();

        foreach (var unlock in LevelManager.Instance.levelUnlocks.unlocksPerLevel)
        {
            if (unlock.level <= playerLevel)
            {
                foreach (var building in unlock.unlockedFactories)
                    uniqueBuildings.Add(building);
                foreach (var building in unlock.unlockedAdvancedFactories)
                    uniqueBuildings.Add(building);
                foreach (var building in unlock.unlockedResourceNodes)
                    uniqueBuildings.Add(building);
                // Voeg hier andere types toe indien nodig
            }
        }

        // Instantieer een UI-kaart voor elk uniek gebouw
        foreach (var so in uniqueBuildings)
        {
            if (so is IUpgradeableBuilding)
            {
                var entry = Instantiate(upgradeEntryPrefab, contentRoot);
                entry.GetComponent<UpgradeUIEntry>().Init(so);
                activeEntries.Add(entry);
            }
        }
    }
}