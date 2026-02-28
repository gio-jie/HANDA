using UnityEngine;

[System.Serializable]
public class SortingItemData
{
    public string itemName;
    public Sprite itemSprite;
    [TextArea] public string description;
    public bool shouldSave;
}