using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ResourceNodeUIController : MonoBehaviour
{
    [Header("Definities")]
    public ResourceNodeDefinition nodeDefinition;
    private int currentLevel = 0;

    [Header("UI elementen")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bufferText;
    public TextMeshProUGUI amountText;
    public TextMeshProUGUI timerText;
    public Button collectButton;
    public Button automateButton;
    public Image iconImage;

    private int currentBuffer = 0;
    private bool isAutomated = false;
    private bool isReadyToCollect = false;
    private Color defaultCollectColor;

    private ResourceNodeLevelData LevelData => nodeDefinition.levelData[currentLevel];

    void Start()
    {
        if (nodeDefinition == null)
        {
            Debug.LogError($"[ResourceNodeUIController] Geen nodeDefinition gekoppeld aan: {gameObject.name}");
            return;
        }

        defaultCollectColor = collectButton.image.color;

        SetupUI();

        collectButton.onClick.RemoveAllListeners(); // prevent dubbele bindings
        collectButton.onClick.AddListener(OnCollectClicked);

        automateButton.onClick.RemoveAllListeners(); // prevent dubbele bindings
        automateButton.onClick.AddListener(OnAutomateClicked);

        Debug.Log($"[Start] Listeners gekoppeld voor: {nodeDefinition.resource.resourceName}");

        StartCoroutine(ProductionLoop());
    }

    private void SetupUI()
    {
        titleText.text = nodeDefinition.resource.resourceName;
        iconImage.sprite = nodeDefinition.resource.icon;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (isAutomated)
        {
            bufferText.text = $"Buffer: {currentBuffer}/{LevelData.maxOutput}";
            collectButton.image.color = currentBuffer > 0 ? Color.green : defaultCollectColor;
        }
        else
        {
            bufferText.text = ""; // geen “Gereed: +1” tekst meer
            collectButton.image.color = isReadyToCollect ? Color.green : defaultCollectColor;
        }

        amountText.text = $"+{LevelData.amountPerClick}/Click";

        // Zet de knoptekst (TMP of legacy Text)
        bool labelSet = false;

        var tmpLabel = automateButton.GetComponentInChildren<TextMeshProUGUI>();
        if (tmpLabel != null)
        {
            tmpLabel.text = isAutomated ? "Automated" : "Automate";
            labelSet = true;
        }

        var legacyLabel = automateButton.GetComponentInChildren<Text>();
        if (legacyLabel != null)
        {
            legacyLabel.text = isAutomated ? "Automated" : "Automate";
            labelSet = true;
        }

        if (!labelSet)
        {
            Debug.LogWarning($"[UpdateUI] Geen tekstlabel gevonden voor automateButton ({nodeDefinition.resource.resourceName})");
        }
    }

    public void OnCollectClicked()
    {
        if (isAutomated)
        {
            if (currentBuffer > 0)
            {
                Debug.Log($"[Collect] {nodeDefinition.resource.resourceName}: buffer geleegd ({currentBuffer})");
                ResourceManager.Instance.AddResource(nodeDefinition.resource.resourceName, currentBuffer);
                currentBuffer = 0;
            }
            else
            {
                Debug.Log($"[Collect] {nodeDefinition.resource.resourceName}: buffer leeg.");
            }
        }
        else
        {
            if (isReadyToCollect)
            {
                bool added = ResourceManager.Instance.AddResource(nodeDefinition.resource.resourceName, LevelData.amountPerClick);
                if (added)
                {
                    Debug.Log($"[Collect] {nodeDefinition.resource.resourceName}: handmatig opgehaald (+{LevelData.amountPerClick})");
                    isReadyToCollect = false;
                }
                // Als niet toegevoegd: geen log, geen timer-reset
            }
            else
            {
                Debug.Log($"[Collect] {nodeDefinition.resource.resourceName}: nog niet klaar.");
            }
        }

        UpdateUI();
    }

    public void OnAutomateClicked()
    {
        isAutomated = !isAutomated;
        Debug.Log($"[Automate] {nodeDefinition.resource.resourceName} => {isAutomated}");
        UpdateUI();
    }

    private IEnumerator ProductionLoop()
    {
        while (true)
        {
            bool canProduce =
                (isAutomated && currentBuffer < LevelData.maxOutput) ||
                (!isAutomated && !isReadyToCollect);

            if (canProduce)
            {
                float t = LevelData.cooldown;
                while (t > 0f)
                {
                    timerText.text = $"{Mathf.CeilToInt(t)}s";
                    t -= Time.deltaTime;
                    yield return null;
                }

                timerText.text = "";

                if (isAutomated)
                {
                    currentBuffer += LevelData.amountPerClick;
                    currentBuffer = Mathf.Min(currentBuffer, LevelData.maxOutput);
                    Debug.Log($"[AutoProd] {nodeDefinition.resource.resourceName} => +{LevelData.amountPerClick} (buffer={currentBuffer})");
                }
                else
                {
                    isReadyToCollect = true;
                    Debug.Log($"[ManualProd] {nodeDefinition.resource.resourceName} => klaar om op te halen");
                }

                UpdateUI();
            }

            yield return null;
        }
    }
}
