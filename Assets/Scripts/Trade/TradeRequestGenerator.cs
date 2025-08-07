using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TradeRequestGenerator : MonoBehaviour
{
    public LevelUnlocks levelUnlocks;

    public List<TradeRequest> GenerateTradeRequests(int playerLevel)
    {
        List<TradeRequest> requests = new();

        if (playerLevel >= 1)
            requests.Add(GenerateRequestByType(playerLevel, ResourceDefinition.ResourceType.Raw, TradeDifficulty.Easy, 1.0f, 1, 10));

        if (playerLevel >= 4)
            requests.Add(GenerateRequestByType(playerLevel, ResourceDefinition.ResourceType.Processed, TradeDifficulty.Medium, 1.5f, 2, 6));

        if (playerLevel >= 7)
            requests.Add(GenerateRequestByType(playerLevel, ResourceDefinition.ResourceType.Advanced, TradeDifficulty.Hard, 2.5f, 1, 4));

        return requests;
    }

    private TradeRequest GenerateRequestByType(int level, ResourceDefinition.ResourceType type, TradeDifficulty difficulty, float factor, int minAmount, int maxAmount)
    {
        List<ResourceDefinition> unlocked = GetUnlockedResources(level)
            .Where(r => r.resourceType == type)
            .ToList();

        if (unlocked.Count == 0) return null;

        List<TradeItem> items = new();
        HashSet<ResourceDefinition> used = new();

        int count = Random.Range(1, Mathf.Min(unlocked.Count, 3)); // max 3 verschillende resources

        while (items.Count < count)
        {
            var res = unlocked[Random.Range(0, unlocked.Count)];
            if (used.Contains(res)) continue;
            used.Add(res);

            int amount = Random.Range(minAmount, maxAmount + 1);
            items.Add(new TradeItem { resource = res, amount = amount });
        }

        int reward = CalculateReward(items, factor);
        return new TradeRequest(difficulty, items, reward);
    }

    private int CalculateReward(List<TradeItem> items, float difficultyFactor)
    {
        int totalAmount = items.Sum(i => i.amount);
        return Mathf.CeilToInt(totalAmount * 3f * difficultyFactor);
    }

    private List<ResourceDefinition> GetUnlockedResources(int level)
    {
        List<ResourceDefinition> all = new();
        foreach (var unlock in levelUnlocks.unlocksPerLevel)
        {
            if (unlock.level <= level)
                all.AddRange(unlock.unlockedResources);
        }
        return all;
    }

    public TradeRequest GenerateTradeOfDifficulty(TradeDifficulty difficulty, int playerLevel)
    {
        switch (difficulty)
        {
            case TradeDifficulty.Easy:
                return GenerateRequestByType(playerLevel, ResourceDefinition.ResourceType.Raw, difficulty, 1.0f, 1, 10);
            case TradeDifficulty.Medium:
                return GenerateRequestByType(playerLevel, ResourceDefinition.ResourceType.Processed, difficulty, 1.5f, 2, 6);
            case TradeDifficulty.Hard:
                return GenerateRequestByType(playerLevel, ResourceDefinition.ResourceType.Advanced, difficulty, 2.5f, 1, 4);
            default:
                Debug.LogWarning("Onbekende trade difficulty!");
                return null;
        }
    }

    // ---------- Embedded klassen en enums ----------
    public enum TradeDifficulty
    {
        Easy,
        Medium,
        Hard
    }

    public class TradeRequest
    {
        public TradeDifficulty difficulty;
        public List<TradeItem> requestedItems;
        public int galactiumReward;

        public TradeRequest(TradeDifficulty difficulty, List<TradeItem> items, int reward)
        {
            this.difficulty = difficulty;
            this.requestedItems = items;
            this.galactiumReward = reward;
        }
    }

    public class TradeItem
    {
        public ResourceDefinition resource;
        public int amount;
    }
}
