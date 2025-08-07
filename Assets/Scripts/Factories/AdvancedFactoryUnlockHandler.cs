using System.Collections.Generic;
using UnityEngine;

public class AdvancedFactoryUnlockHandler : MonoBehaviour
{
    [Header("Referenties")]
    public LevelManager levelManager;
    public LevelUnlocks levelUnlocks;

    [Header("Alle advanced factory UI controllers in de scene")]
    public List<AdvancedFactoryUIController> allFactoryUIControllers;

    void Start()
    {
        if (levelManager == null || levelUnlocks == null)
        {
            Debug.LogError("[AdvancedFactoryUnlockHandler] LevelManager of LevelUnlocks niet gekoppeld!");
            return;
        }

        UpdateUnlockedFactories(levelManager.CurrentLevel);
        levelManager.OnLevelChanged += UpdateUnlockedFactories;
    }

    void OnDestroy()
    {
        levelManager.OnLevelChanged -= UpdateUnlockedFactories;
    }

    private void UpdateUnlockedFactories(int currentLevel)
    {
        HashSet<AdvancedFactoryDefinition> unlockedFactories = new();

        // Verzamel alle advanced factories die unlocked zijn
        foreach (var entry in levelUnlocks.unlocksPerLevel)
        {
            if (entry.level <= currentLevel)
            {
                foreach (var factory in entry.unlockedAdvancedFactories)
                {
                    unlockedFactories.Add(factory);
                }
            }
        }

        // Activeer alleen de panels waarvan de factory unlocked is
        foreach (var controller in allFactoryUIControllers)
        {
            bool isUnlocked = unlockedFactories.Contains(controller.advancedFactoryDefinition);
            controller.gameObject.SetActive(isUnlocked);
        }

        Debug.Log($"[AdvancedFactoryUnlockHandler] Geactiveerde advanced factories voor level {currentLevel}: {unlockedFactories.Count}");
    }
}
