using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AdvancedFactoryUIController : MonoBehaviour
{
    [Header("Factory Config")]
    public AdvancedFactoryDefinition advancedFactoryDefinition;
    public int currentLevel = 0;

    [Header("UI Elements")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI queueText;
    public TextMeshProUGUI bufferText;
    public TextMeshProUGUI timerText;
    public Button fillQueueButton;
    public Button collectButton;

    [Header("Receptselectie via handmatige knoppen")]
    public Button recipeButton1;
    public Button recipeButton2;

    [Header("Visual feedback")]
    public Color selectedColor = Color.yellow;
    public Color defaultColor = Color.white;

    private AdvancedFactoryLevelData LevelData => advancedFactoryDefinition.levelData[currentLevel];
    private AdvancedRecipeDefinition selectedRecipe;
    private Queue<AdvancedRecipeDefinition> processingQueue = new();
    private List<AdvancedRecipeDefinition> readyBuffer = new();
    private bool isProcessing = false;

    void Start()
    {
        Debug.Log("[Start] AdvancedFactoryUIController: Start aangeroepen (eerste regel)");
        titleText.text = advancedFactoryDefinition.factoryName;
        fillQueueButton.onClick.AddListener(OnFillQueueClicked);
        collectButton.onClick.AddListener(OnCollectClicked);

        // Receptknoppen koppelen aan recepten
        recipeButton1.onClick.AddListener(() => SelectRecipe(advancedFactoryDefinition.availableRecipes[0]));
        recipeButton2.onClick.AddListener(() => SelectRecipe(advancedFactoryDefinition.availableRecipes[1]));

        UpdateUI();
        StartCoroutine(ProcessingLoop());
    }

    private void SelectRecipe(AdvancedRecipeDefinition recipe)
    {
        Debug.Log($"[SelectRecipe] Recept geselecteerd: {recipe.recipeName}");
        selectedRecipe = recipe;
        Debug.Log($"[Select] Geselecteerd recept: {recipe.recipeName}");

        // UI feedback – kleuren resetten
        recipeButton1.image.color = (advancedFactoryDefinition.availableRecipes[0] == recipe) ? selectedColor : defaultColor;
        recipeButton2.image.color = (advancedFactoryDefinition.availableRecipes[1] == recipe) ? selectedColor : defaultColor;
    }

    private void OnFillQueueClicked()
    {
        Debug.Log($"[OnFillQueueClicked] Geselecteerd recept: {(selectedRecipe != null ? selectedRecipe.recipeName : "null")}, queue={processingQueue.Count}/{LevelData.maxQueueSize}");
        if (selectedRecipe == null)
        {
            Debug.LogWarning("[FillQueue] Geen recept geselecteerd.");
            return;
        }

        if (processingQueue.Count >= LevelData.maxQueueSize)
        {
            Debug.LogWarning("[FillQueue] Wachtrij is vol.");
            return;
        }

        // Check inputkosten
        foreach (var cost in selectedRecipe.inputCosts)
        {
            if (!ResourceManager.Instance.HasEnough(cost.resource.resourceName, cost.amount))
            {
                Debug.LogWarning($"[FillQueue] Niet genoeg {cost.resource.resourceName}");
                return;
            }
        }

        // Trek resources af
        foreach (var cost in selectedRecipe.inputCosts)
        {
            ResourceManager.Instance.TryConsumeResource(cost.resource.resourceName, cost.amount);
        }

        processingQueue.Enqueue(selectedRecipe);
        Debug.Log($"[FillQueue] {selectedRecipe.recipeName} toegevoegd aan wachtrij. QueueCount={processingQueue.Count}");

        UpdateUI();
        Debug.Log($"[OnFillQueueClicked] UpdateUI aangeroepen na vullen queue");
    }

    private void OnCollectClicked()
    {
        Debug.Log($"[OnCollectClicked] readyBuffer.Count = {readyBuffer.Count}");
        if (readyBuffer.Count == 0)
        {
            Debug.Log("[Collect] Geen items om op te halen.");
            return;
        }

        var recipe = readyBuffer[0];
        if (!ResourceManager.Instance.CanAddResource(recipe.outputResource.resourceName, recipe.outputAmount))
        {
            Debug.LogWarning("[Collect] Niet genoeg ruimte in opslag.");
            return;
        }

        ResourceManager.Instance.AddResource(recipe.outputResource.resourceName, recipe.outputAmount);
        readyBuffer.RemoveAt(0);
        Debug.Log($"[Collect] {recipe.outputAmount}x {recipe.outputResource.resourceName} opgehaald.");

        UpdateUI();
    }

    private IEnumerator ProcessingLoop()
    {
        while (true)
        {
            if (!isProcessing && processingQueue.Count > 0 && readyBuffer.Count < LevelData.maxReadyBuffer)
            {
                Debug.Log($"[ProcessingLoop] {gameObject.name} Start productie voor recept uit queue");
                isProcessing = true;
                var recipe = processingQueue.Dequeue();
                float duration = recipe.productionTime * LevelData.productionSpeedMultiplier;
                float timeRemaining = duration;

                Debug.Log($"[Processing] {gameObject.name} Start productie: {recipe.recipeName} (tijd: {duration})");

                while (timeRemaining > 0f)
                {
                    try
                    {
                        timerText.text = Mathf.CeilToInt(timeRemaining).ToString();
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"[ProcessingLoop] {gameObject.name} Error updating timer: {e.Message}");
                    }
                    
                    timeRemaining -= Time.deltaTime;
                    yield return null;
                }

                readyBuffer.Add(recipe);
                Debug.Log($"[Processing] {gameObject.name} {recipe.recipeName} klaar. Buffer: {readyBuffer.Count}/{LevelData.maxReadyBuffer}");
                isProcessing = false;
                timerText.text = "";
                UpdateUI();
            }

            yield return null;
        } 
    }

    private void UpdateUI()
    {
        queueText.text = $"Queue: {processingQueue.Count}/{LevelData.maxQueueSize}";
        bufferText.text = $"Ready: {readyBuffer.Count}/{LevelData.maxReadyBuffer}";

        if (!isProcessing)
        {
            Debug.Log($"[UpdateUI] timerText.text wordt geleegd omdat isProcessing={isProcessing}");
            timerText.text = "";
        }
    }
}