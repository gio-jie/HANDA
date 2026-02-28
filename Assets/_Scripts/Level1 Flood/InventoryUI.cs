using UnityEngine;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    public GameObject inventoryPanel;
    public Transform content;
    public GameObject itemSlotPrefab;

    void Awake()
    {
        Instance = this;
        inventoryPanel.SetActive(false);
        RefreshUI(InventoryManager.Instance.GetSavedInventory());

        // foreach (ItemData item in InventoryManager.Instance.collectedItems)
        // {
        //     AddItemToUI(item);
        // }

    }

    public void ToggleInventory()
    {
        //inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        bool isOpen = !inventoryPanel.activeSelf;

        inventoryPanel.SetActive(isOpen);

        if (isOpen)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;
    }

    public void AddItemToUI(ItemData item)
    {
        GameObject slot = Instantiate(itemSlotPrefab, content);
        slot.GetComponent<ItemSlot>().Setup(item);
    }

    public void ClearUI()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
    }

    public void RefreshUI(List<ItemData> items)
    {
        ClearUI();
        foreach (ItemData item in items)
            AddItemToUI(item);
    }
}