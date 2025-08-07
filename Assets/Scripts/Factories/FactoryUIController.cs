using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class FactoryUIController : MonoBehaviour
{
    void OnEnable()
    {
        StartCoroutine(ProcessingLoop());
    }
    [Header("Definities")]
    public FactoryDefinition factoryDefinition;
    private int currentLevel = 0;

    [Header("UI-elementen")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI queueText;
    public TextMeshProUGUI bufferText;
    public TextMeshProUGUI timerText;
    public Button fillQueueButton;
    public Button collectButton;
    public Image iconImage;

    private int currentQueue = 0;
    private int currentBuffer = 0;
    private bool isProcessing = false;

    private FactoryLevelData LevelData => factoryDefinition.levels[currentLevel];

    void Start()
    {
        SetupUI();
        fillQueueButton.onClick.AddListener(OnFillQueueClicked);
        collectButton.onClick.AddListener(OnCollectClicked);
        StartCoroutine(ProcessingLoop());
    }

    private void SetupUI()
    {
        titleText.text = factoryDefinition.factoryName;
        iconImage.sprite = factoryDefinition.icon;
        UpdateUI();
    }

    private void UpdateUI()
    {
        queueText.text = $"Queue: {currentQueue}/{LevelData.maxQueue}";
        bufferText.text = $"Ready: {currentBuffer}/{LevelData.maxBuffer}";
        // Laat timer leeg als er niks geproduceerd wordt
        if (!isProcessing)
            timerText.text = "";
    }

    private void OnFillQueueClicked()
    {
        if (currentQueue >= LevelData.maxQueue)
        {
            return;
        }

        bool resourceConsumed = ResourceManager.Instance.TryConsumeResource(factoryDefinition.inputResource.resourceName, 1);
        if (resourceConsumed)
        {
            currentQueue++;
            Debug.Log($"[FillQueue] +1 item in wachtrij bij {factoryDefinition.factoryName}. Queue nu: {currentQueue}");
        }
        else
        {
            Debug.Log($"[FillQueue] Niet genoeg input ({factoryDefinition.inputResource.resourceName})");
        }

        UpdateUI();
    }

    private void OnCollectClicked()
    {
        if (currentBuffer <= 0)
        {
            return;
        }

        bool canAdd = ResourceManager.Instance.CanAddResource(factoryDefinition.outputResource.resourceName, 1);
        if (!canAdd)
        {
            return;
        }

        ResourceManager.Instance.AddResource(factoryDefinition.outputResource.resourceName, 1);
        currentBuffer--;
        Debug.Log($"[Collect] 1 item opgehaald bij {factoryDefinition.factoryName}");

        UpdateUI();
    }

    private IEnumerator ProcessingLoop()
    {
        while (true)
        {
            if (!isProcessing && currentQueue > 0 && currentBuffer < LevelData.maxBuffer)
            {
                isProcessing = true;
                float remaining = LevelData.productionTime;
                Debug.Log($"[Process] Start productie bij {factoryDefinition.factoryName}");
                while (remaining > 0f)
                {
                    timerText.text = Mathf.CeilToInt(remaining).ToString();
                    remaining -= Time.deltaTime;
                    yield return null;
                }
                timerText.text = "";
                currentQueue--;
                currentBuffer++;
                isProcessing = false;
                Debug.Log($"[Process] Product klaar bij {factoryDefinition.factoryName}");
                UpdateUI();
            }
            yield return null;
        }
    }
}
