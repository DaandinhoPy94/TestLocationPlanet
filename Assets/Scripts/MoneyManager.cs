using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;

    [Header("UI Element")]
    public TextMeshProUGUI moneyText; // Sleep hier je MoneyTxt in

    [Header("Startwaarde")]
    public int startingMoney = 0;

    private int currentMoney;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        currentMoney = startingMoney;
        UpdateMoneyUI();
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        UpdateMoneyUI();
    }

    public bool TrySpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            UpdateMoneyUI();
            return true;
        }

        Debug.LogWarning($"[MoneyManager] Niet genoeg geld. Nodig: {amount}, Beschikbaar: {currentMoney}");
        return false;
    }

    public bool HasEnough(int amount) => currentMoney >= amount;

    public int GetCurrentMoney() => currentMoney;

    private void UpdateMoneyUI()
    {
        if (moneyText != null)
            moneyText.text = $"{currentMoney}";
    }
}
