using System.Collections.Generic;
using UnityEngine;

public class FactoryUnlockHandler : MonoBehaviour
{
    [Header("Referenties")]
    public LevelManager levelManager;
    public LevelUnlocks levelUnlocks;

    [Header("Alle factory UI controllers in de scene")]
    public List<FactoryUIController> allFactoryUIControllers;

    void Start()
    {
        if (levelManager == null || levelUnlocks == null)
        {
            Debug.LogError("[FactoryUnlockHandler] LevelManager of LevelUnlocks niet gekoppeld!");
            return;
        }

        // Bij start meteen unlocks toepassen
        UpdateUnlockedFactories(levelManager.CurrentLevel);

        // Abonneren op level changes
        levelManager.OnLevelChanged += UpdateUnlockedFactories;
    }

    void OnDestroy()
    {
        levelManager.OnLevelChanged -= UpdateUnlockedFactories;
    }

    private void UpdateUnlockedFactories(int currentLevel)
    {
        HashSet<FactoryDefinition> unlockedFactories = new();

        // Verzamel alle factories die unlocked zouden moeten zijn
        foreach (var entry in levelUnlocks.unlocksPerLevel)
        {
            if (entry.level <= currentLevel)
            {
                foreach (var factory in entry.unlockedFactories)
                {
                    unlockedFactories.Add(factory);
                }
            }
        }

        // Activeer alleen de panels die bij een unlocked factory horen
        foreach (var controller in allFactoryUIControllers)
        {
            bool isUnlocked = unlockedFactories.Contains(controller.factoryDefinition);
            controller.gameObject.SetActive(isUnlocked);
        }

        Debug.Log($"[FactoryUnlockHandler] Geactiveerde factories voor level {currentLevel}: {unlockedFactories.Count}");
    }
}
