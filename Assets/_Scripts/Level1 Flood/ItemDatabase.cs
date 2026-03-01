using UnityEngine;
using System.Collections.Generic;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;

    public List<ItemData> items;

    Dictionary<string, ItemData> itemLookup;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            transform.SetParent(null);

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        //Instance = this;

        itemLookup = new Dictionary<string, ItemData>();

        foreach (var item in items)
        {
            //itemLookup[item.itemID] = item;
            if (!itemLookup.ContainsKey(item.itemID))
            {
                itemLookup.Add(item.itemID, item);
            }
            else
            {
                Debug.LogWarning("Duplicate item ID found: " + item.itemID);
            }
        }
    }

    public ItemData GetItem(string id)
    {
        //return itemLookup[id];
        
        if (itemLookup.ContainsKey(id))
        {
            return itemLookup[id];
        }
        else
        {
            Debug.LogWarning("Item ID not found in database: " + id);
            return null;
        }
    }
}