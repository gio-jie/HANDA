using UnityEngine;
using System.Collections.Generic;

public class Level8BagManager : MonoBehaviour
{
    public GameObject itemPrefab;
    public Transform itemsParent;
    public BagAnimator bagAnimator;
    void Start()
    {
        foreach (var item in InventoryManager.Instance.GetSavedInventory())
        {
            GameObject obj = Instantiate(itemPrefab, itemsParent);

            Level8DragItem dragItem = obj.GetComponent<Level8DragItem>();

            dragItem.itemID = item.itemID;

            obj.GetComponent<UnityEngine.UI.Image>().sprite = item.icon;
        }

        if (bagAnimator != null)
            bagAnimator.AnimateBag();
    }
}