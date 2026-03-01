using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class InventorySaveData
{
    public List<string> itemIDs = new List<string>();
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("Runtime inventory (Level 1 collection)")]
    public List<ItemData> runtimeItems = new List<ItemData>();

    [Header("Saved inventory (official set)")]
    public List<ItemData> savedItems = new List<ItemData>();

    private const string InventoryKey = "PlayerInventory";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            transform.SetParent(null);

            DontDestroyOnLoad(gameObject);
            LoadSavedInventory();
            
            if (InventoryUI.Instance != null)
            {
                InventoryUI.Instance.RefreshUI(GetSavedInventory());
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CollectItem(string id)
    {
        ItemData data = ItemDatabase.Instance.GetItem(id);
        if (data == null) return;

        if (!runtimeItems.Contains(data))
        {
            runtimeItems.Add(data);
            if (InventoryUI.Instance != null)
                InventoryUI.Instance.RefreshUI(runtimeItems); 
        }
    }

    public void SaveInventorySet()
    {
        savedItems.Clear();
        savedItems.AddRange(runtimeItems);

        InventorySaveData saveData = new InventorySaveData();
        foreach (ItemData item in savedItems)
            saveData.itemIDs.Add(item.itemID);

        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString(InventoryKey, json);
        PlayerPrefs.Save();

        Debug.Log("Inventory saved: " + json);
    }

    // --- Load last saved inventory ---
    public void LoadSavedInventory()
    {
        savedItems.Clear();

        if (!PlayerPrefs.HasKey(InventoryKey)) return;

        string json = PlayerPrefs.GetString(InventoryKey);
        InventorySaveData saveData = JsonUtility.FromJson<InventorySaveData>(json);

        foreach (string id in saveData.itemIDs)
        {
            ItemData data = ItemDatabase.Instance.GetItem(id);
            if (data != null)
                savedItems.Add(data);
        }

        Debug.Log("Saved inventory loaded: " + json);
    }

    public List<ItemData> GetSavedInventory() => savedItems;

    public void ClearRuntimeInventory()
    {
        runtimeItems.Clear();
        if (InventoryUI.Instance != null)
            InventoryUI.Instance.ClearUI();
    }

    public void ResetAllInventory()
    {
        runtimeItems.Clear();
        savedItems.Clear();
        PlayerPrefs.DeleteKey(InventoryKey);
        PlayerPrefs.Save();
        if (InventoryUI.Instance != null)
            InventoryUI.Instance.ClearUI();
    }
}