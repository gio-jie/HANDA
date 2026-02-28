using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "WaterCardDatabase", menuName = "Level7/WaterCardDatabase")]
public class WaterCardDatabase : ScriptableObject
{
    public List<WaterCardData> allCards;
    
    public static WaterCardDatabase Instance;

    private void OnEnable() { Instance = this; }
}