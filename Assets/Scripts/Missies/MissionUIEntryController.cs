using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MissionUIEntryController : MonoBehaviour
{
    [Header("Koppelingen")]
    public ExplorationMission missionData;
    public Button startButton;
    public Button collectButton;
    public TextMeshProUGUI timerText;

    private bool missionRunning = false;
    private float missionEndTime;

    private List<ResourceDefinition> pendingRewards = new();

    void Start()
    {
        startButton.onClick.AddListener(StartMission);
        collectButton.onClick.AddListener(ClaimReward);
        timerText.text = "";
        collectButton.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!missionRunning) return;

        float remaining = missionEndTime - Time.time;
        if (remaining > 0)
        {
            int minutes = Mathf.FloorToInt(remaining / 60);
            int seconds = Mathf.FloorToInt(remaining % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
        else
        {
            missionRunning = false;
            timerText.text = "Klaar!";
            collectButton.gameObject.SetActive(true);
        }
    }

    void StartMission()
    {
        if (missionRunning) return;
        if (!HasEnoughResources()) return;

        ConsumeResources();
        missionEndTime = Time.time + missionData.durationSeconds;
        missionRunning = true;
        collectButton.gameObject.SetActive(false);
        pendingRewards.Clear();
    }

    void ClaimReward()
    {
        if (pendingRewards.Count == 0)
        {
            foreach (var drop in missionData.dropTable)
            {
                if (Random.value <= drop.chance)
                {
                    pendingRewards.Add(drop.resource);
                }
            }
        }

        bool allDelivered = true;
        List<ResourceDefinition> delivered = new();

        foreach (var item in pendingRewards)
        {
            if (ResourceManager.Instance.CanAddResource(item.resourceName, 1))
            {
                ResourceManager.Instance.AddResource(item.resourceName, 1);
                Debug.Log($"🎁 Ontvangen: {item.resourceName}");
                delivered.Add(item);
            }
            else
            {
                Debug.Log($"❌ Geen ruimte voor {item.resourceName}, probeer later opnieuw.");
                allDelivered = false;
            }
        }

        foreach (var item in delivered)
        {
            pendingRewards.Remove(item);
        }

        if (allDelivered)
        {
            timerText.text = "";
            collectButton.gameObject.SetActive(false);
        }
    }

    bool HasEnoughResources()
    {
        foreach (var req in missionData.inputRequirements)
        {
            if (!ResourceManager.Instance.HasEnough(req.resource.resourceName, req.amount))
            {
                Debug.Log($"Niet genoeg van {req.resource.resourceName}");
                return false;
            }
        }
        return true;
    }

    void ConsumeResources()
    {
        foreach (var req in missionData.inputRequirements)
        {
            ResourceManager.Instance.TryConsumeResource(req.resource.resourceName, req.amount);
        }
    }
}
