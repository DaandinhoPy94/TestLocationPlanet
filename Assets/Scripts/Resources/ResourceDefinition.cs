using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Resources/Resource Definition")]
public class ResourceDefinition : ScriptableObject
{
    public string resourceName;   // bijv. "Steen"
    public Sprite icon;
    public int startAmount = 0;
    public enum ResourceType { Raw, Processed, Advanced, Special }
    public ResourceType resourceType;

}
