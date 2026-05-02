using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Scenario")]
public class ScenarioData : ScriptableObject
{
    public string mission;
    public Sprite image;

    public List<string> correctToolIDs;
}