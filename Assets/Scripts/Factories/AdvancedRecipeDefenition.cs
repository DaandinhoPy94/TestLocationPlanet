using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAdvancedRecipe", menuName = "Factories/Advanced Recipe")]
public class AdvancedRecipeDefinition : ScriptableObject
{
    public string recipeName;

    [Header("Input Resources")]
    public List<ResourceCost> inputCosts;

    [Header("Output")]
    public ResourceDefinition outputResource;
    public int outputAmount = 1;

    [Header("Production Settings")]
    public float productionTime = 3f;
    public int unlockLevel = 0; // later gebruiken bij level locks
}

[System.Serializable]
public class ResourceCost
{
    public ResourceDefinition resource;
    public int amount;
}
