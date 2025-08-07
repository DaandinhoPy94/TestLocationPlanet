using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryProcessor : MonoBehaviour
{
    public FactoryDefinition definition;
    public int currentLevel = 0;

    private Queue<int> processingQueue = new();
    private List<int> readyBuffer = new();

    private float timer = 0f;
    private bool isProcessing = false;

    private FactoryLevelData LevelData => definition.levels[currentLevel];

    void Update()
    {
        if (processingQueue.Count == 0 || readyBuffer.Count >= LevelData.maxBuffer)
            return;

        if (!isProcessing)
        {
            isProcessing = true;
            timer = LevelData.productionTime;
        }

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            CompleteProcessing();
            isProcessing = false;
        }
    }

    public bool TryAddToQueue()
    {
        if (processingQueue.Count >= LevelData.maxQueue)
        {
            Debug.Log($"[Queue] Vol voor {definition.factoryName}");
            return false;
        }

        string resourceName = definition.inputResource.resourceName;
        if (!ResourceManager.Instance.HasEnough(resourceName, 1))
        {
            Debug.Log($"[Queue] Niet genoeg {resourceName} om toe te voegen aan wachtrij.");
            return false;
        }

        ResourceManager.Instance.TryConsumeResource(resourceName, 1);
        processingQueue.Enqueue(1);

        Debug.Log($"[Queue] {definition.factoryName}: item toegevoegd aan wachtrij.");
        return true;
    }

    private void CompleteProcessing()
    {
        if (processingQueue.Count == 0) return;

        processingQueue.Dequeue();

        if (readyBuffer.Count < LevelData.maxBuffer)
        {
            readyBuffer.Add(LevelData.outputAmount);
            Debug.Log($"[Process] {definition.factoryName}: productie voltooid.");
        }
        else
        {
            Debug.LogWarning($"[Process] {definition.factoryName}: buffer vol, kan niet verwerken.");
        }
    }

    public bool TryCollect()
    {
        if (readyBuffer.Count == 0)
        {
            Debug.Log($"[Collect] {definition.factoryName}: niets om op te halen.");
            return false;
        }

        if (!ResourceManager.Instance.CanAddResource(definition.outputResource.resourceName, 1))
        {
            Debug.Log($"[Collect] {definition.factoryName}: geen ruimte in opslag.");
            return false;
        }

        int amount = readyBuffer[0];
        readyBuffer.RemoveAt(0);
        ResourceManager.Instance.AddResource(definition.outputResource.resourceName, amount);
        Debug.Log($"[Collect] {definition.factoryName}: opgehaald ({amount}).");
        return true;
    }

    public int GetQueueSize() => processingQueue.Count;
    public int GetBufferSize() => readyBuffer.Count;
    public float GetTimer() => isProcessing ? timer : 0f;
}
