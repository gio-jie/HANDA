using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenarioLoader : MonoBehaviour
{
    // Ikabit ito sa Typhoon Button sa Scenario Selection Scene
    public void LoadTyphoonScenario()
    {
        // RESET: Gawing "0" ulit para lumabas si Jobert
        PlayerPrefs.SetInt("JobertTriviaSeen", 0);
        PlayerPrefs.Save();

        // Load Scene (Siguraduhing tama ang spelling ng scene mo!)
        SceneManager.LoadScene("TyphoonLevelSelect");
    }

    public void LoadFloodScenario()
    {
        //change 0-1 here to not flash the dialogue
        int floodSeen = PlayerPrefs.GetInt("FloodIntroSeen", 1);

        if (floodSeen == 0)
        {
            PlayerPrefs.SetInt("FloodIntroSeen", 0);
            PlayerPrefs.Save();
        }

        SceneManager.LoadScene("FloodLevelSelect");
    }
}