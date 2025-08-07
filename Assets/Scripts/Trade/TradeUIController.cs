using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static TradeRequestGenerator;

public class TradeUIController : MonoBehaviour
{
    [Header("Instellingen")]
    public TradeDifficulty difficulty;
    public float rejectCooldownSeconds = 2f;

    [Header("Referenties")]
    public TradeRequestGenerator generator;
    public Button tradeButton;
    public Button rejectButton;
    public TextMeshProUGUI tradeButtonText;
    public TextMeshProUGUI rejectButtonText;

    private TradeRequest currentRequest;
    private bool isWaiting = false;

    void Start()
    {
        tradeButton.onClick.AddListener(OnAcceptTrade);
        rejectButton.onClick.AddListener(OnRejectTrade);
        GenerateNewTrade();
    }

    private void GenerateNewTrade()
    {
        int playerLevel = LevelManager.Instance.CurrentLevel;
        currentRequest = generator.GenerateTradeOfDifficulty(difficulty, playerLevel);

        if (currentRequest == null)
        {
            tradeButtonText.text = "No offer available.";
            tradeButton.interactable = false;
            return;
        }

        tradeButtonText.text = FormatTradeText(currentRequest);
        tradeButton.interactable = true;
    }

    private string FormatTradeText(TradeRequest request)
    {
        string result = $"{request.difficulty} Trade:\n";
        foreach (var item in request.requestedItems)
        {
            result += $"- {item.amount}x {item.resource.resourceName}\n";
        }
        result += $"=> Reward: {request.galactiumReward} Galactium";
        return result;
    }

    private void OnAcceptTrade()
    {
        if (currentRequest == null) return;

        // Check of speler alle items heeft
        foreach (var item in currentRequest.requestedItems)
        {
            if (!ResourceManager.Instance.HasEnough(item.resource.resourceName, item.amount))
            {
                Debug.LogWarning("[Trade] Not enough resources to complete trade.");
                return;
            }
        }

        // Trek resources af
        foreach (var item in currentRequest.requestedItems)
        {
            ResourceManager.Instance.TryConsumeResource(item.resource.resourceName, item.amount);
        }

        // ✅ Voeg Galactium toe via MoneyManager
        MoneyManager.Instance.AddMoney(currentRequest.galactiumReward);
        Debug.Log($"[Trade] Trade completed! +{currentRequest.galactiumReward} Galactium");

        GenerateNewTrade();
    }

    private void OnRejectTrade()
    {
        if (isWaiting) return;
        StartCoroutine(RejectCooldown());
    }

    private IEnumerator RejectCooldown()
    {
        isWaiting = true;
        tradeButtonText.text = "Please wait...";
        tradeButton.interactable = false;
        rejectButton.interactable = false;

        yield return new WaitForSeconds(rejectCooldownSeconds);

        GenerateNewTrade();
        rejectButton.interactable = true;
        isWaiting = false;
    }
}
