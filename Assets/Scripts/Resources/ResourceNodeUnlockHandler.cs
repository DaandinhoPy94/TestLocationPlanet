using System.Collections.Generic;
using UnityEngine;

public class ResourceNodeUnlockHandler : MonoBehaviour
{
    [Header("Referenties")]
    public LevelManager levelManager;
    public LevelUnlocks levelUnlocks;

    [Header("Alle resource UI controllers in de scene")]
    public List<ResourceNodeUIController> allNodeUIControllers;

    void Start()
    {
        if (levelManager == null || levelUnlocks == null)
        {
            Debug.LogError("[ResourceNodeUnlockHandler] LevelManager of LevelUnlocks niet gekoppeld!");
            return;
        }

        // Bij start meteen unlocks toepassen
        UpdateUnlockedNodes(levelManager.CurrentLevel);

        // Abonneren op level changes
        levelManager.OnLevelChanged += UpdateUnlockedNodes;
    }

    void OnDestroy()
    {
        levelManager.OnLevelChanged -= UpdateUnlockedNodes;
    }

    private void UpdateUnlockedNodes(int currentLevel)
    {
        HashSet<ResourceNodeDefinition> unlockedNodes = new HashSet<ResourceNodeDefinition>();

        // Verzamel alle nodes die unlocked zouden moeten zijn
        foreach (var entry in levelUnlocks.unlocksPerLevel)
        {
            if (entry.level <= currentLevel)
            {
                foreach (var node in entry.unlockedResourceNodes)
                {
                    unlockedNodes.Add(node);
                }
            }
        }

        // Activeer alleen de nodes die unlocked zijn
        foreach (var controller in allNodeUIControllers)
        {
            bool isUnlocked = unlockedNodes.Contains(controller.nodeDefinition);
            controller.gameObject.SetActive(isUnlocked);
        }

        Debug.Log($"[ResourceNodeUnlockHandler] Geactiveerde nodes voor level {currentLevel}: {unlockedNodes.Count}");
    }
}
