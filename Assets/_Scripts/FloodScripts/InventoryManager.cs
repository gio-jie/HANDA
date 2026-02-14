using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public List<ItemData> collectedItems = new List<ItemData>();

    void Awake()
    {
        Instance = this;
    }

    public void AddItem(string id)
    {
        ItemData data = ItemDatabase.Instance.GetItem(id);

        if (!collectedItems.Contains(data))
        {
            collectedItems.Add(data);
            InventoryUI.Instance.AddItemToUI(data);
        }
    }
}