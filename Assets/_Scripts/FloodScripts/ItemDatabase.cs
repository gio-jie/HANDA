using UnityEngine;
using System.Collections.Generic;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;

    public List<ItemData> items;

    Dictionary<string, ItemData> itemLookup;

    void Awake()
    {
        Instance = this;

        itemLookup = new Dictionary<string, ItemData>();

        foreach (var item in items)
        {
            itemLookup[item.itemID] = item;
        }
    }

    public ItemData GetItem(string id)
    {
        return itemLookup[id];
    }
}