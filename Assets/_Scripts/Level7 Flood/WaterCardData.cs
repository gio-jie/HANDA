using UnityEngine;

public enum WaterType
{
    Clean,
    Dirty
}

[CreateAssetMenu(fileName = "WaterCard", menuName = "Level7/Water Card")]
public class WaterCardData : ScriptableObject
{
    public Sprite image;
    public string waterName;
    [TextArea] public string description;
    public WaterType type;
}