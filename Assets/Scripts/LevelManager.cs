using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Level Config")]
    [SerializeField] private int currentLevel = 0;
    public TextMeshProUGUI levelText; // Sleep hier je level-tekst in
    public LevelUnlocks levelUnlocks;

    // Event voor andere systemen zoals unlock handlers
    public event Action<int> OnLevelChanged;

    // Intern bijgehouden welke dingen al unlocked zijn
    private HashSet<ScriptableObject> unlockedItems = new();

    public int CurrentLevel => currentLevel; // publieke getter

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        UpdateLevelUI();
        UnlockItemsForLevel(currentLevel);
    }

    /// <summary>
    /// Wordt aangeroepen om handmatig level te verhogen
    /// (bijv. via knop)
    /// </summary>
    public void IncreaseLevel()
    {
        Debug.Log("[Button] IncreaseLevel() aangeroepen");
        currentLevel++;
        Debug.Log($"[Level] Speler is nu level {currentLevel}");

        UpdateLevelUI();
        UnlockItemsForLevel(currentLevel);
        OnLevelChanged?.Invoke(currentLevel); // Event afvuren
    }

    private void UpdateLevelUI()
    {
        if (levelText != null)
            levelText.text = $"{currentLevel}";
    }

    private void UnlockItemsForLevel(int level)
    {
        var data = levelUnlocks.unlocksPerLevel.Find(d => d.level == level);
        if (data == null)
        {
            Debug.LogWarning($"[LevelUnlocks] Geen unlocks gevonden voor level {level}");
            return;
        }

        void TryUnlock<T>(List<T> list) where T : ScriptableObject
        {
            foreach (var item in list)
            {
                if (item != null && unlockedItems.Add(item))
                {
                    Debug.Log($"[Unlock] {item.name} beschikbaar op level {level}");
                }
            }
        }

        TryUnlock(data.unlockedResources);
        TryUnlock(data.unlockedFactories);
        TryUnlock(data.unlockedAdvancedFactories);
        TryUnlock(data.unlockedResourceNodes);
    }

    /// <summary>
    /// Check of een item (resource, factory, etc) unlocked is op basis van huidig level.
    /// </summary>
    public bool IsUnlocked(ScriptableObject item)
    {
        return unlockedItems.Contains(item);
    }
}
