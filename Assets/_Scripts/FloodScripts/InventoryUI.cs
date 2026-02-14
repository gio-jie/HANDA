using UnityEngine;

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
}