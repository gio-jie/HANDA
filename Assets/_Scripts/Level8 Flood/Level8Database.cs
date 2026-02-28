using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Level8Database", menuName = "Level8/Level8Database")]
public class Level8Database : ScriptableObject
{
    public List<Level8ScenarioData> allScenarios;
}