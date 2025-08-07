using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceInventoryUI : MonoBehaviour
{
    public GameObject entryPrefab;      // Prefab met Image + Text
    public Transform container;         // Content-container (bv. binnen ScrollView)

    private Dictionary<string, TextMeshProUGUI> entries = new();

    void Start()
    {
        var resources = ResourceManager.Instance.GetAllResources();

        foreach (var (def, amount) in resources)
        {
            GameObject entry = Instantiate(entryPrefab, container);
            entry.transform.Find("Icon").GetComponent<Image>().sprite = def.icon;
            var amountText = entry.transform.Find("Amount").GetComponent<TextMeshProUGUI>();
            amountText.text = amount.ToString();

            entries[def.resourceName] = amountText;
        }

        ResourceManager.Instance.OnResourceChanged += UpdateUI;
    }

    void UpdateUI()
    {
        foreach (var (def, amount) in ResourceManager.Instance.GetAllResources())
        {
            if (entries.ContainsKey(def.resourceName))
            {
                entries[def.resourceName].text = amount.ToString();
            }
        }
    }
}
