using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public List<ItemData> collectedItems = new List<ItemData>();

    void Awake()
    {
        //Instance = this;

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddItem(string id)
    {
        ItemData data = ItemDatabase.Instance.GetItem(id);

        if (data == null)
        {
            Debug.LogWarning("Item not found in database: " + id);
            return;
        }

        if (!collectedItems.Contains(data))
        {
            collectedItems.Add(data);
            //InventoryUI.Instance.AddItemToUI(data);
            if (InventoryUI.Instance != null)
                InventoryUI.Instance.AddItemToUI(data);
        }
    }

    public void ClearInventory()
    {
        collectedItems.Clear();
    }
}