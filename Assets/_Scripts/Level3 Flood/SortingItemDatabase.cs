using UnityEngine;
using System.Collections.Generic;

public class SortingItemDatabase : MonoBehaviour
{
    public static SortingItemDatabase Instance;

    public List<SortingItemData> allItems;

    void Awake()
    {
        Instance = this;
    }
}