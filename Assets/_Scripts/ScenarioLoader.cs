using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Kailangan ito para makontrol natin ang mga Buttons!

public class ScenarioLoader : MonoBehaviour
{
    [Header("Scenario Buttons")]
    public Button floodButton; // I-drag ang Flood Button dito
    public Button earthquakeButton; // I-drag ang Earthquake Button dito

    [Header("Lock Visuals (Optional)")]
    public GameObject floodLockIcon; // Kung may padlock image ka, i-drag dito
    public GameObject earthquakeLockIcon;

    void Start()
    {
        // --- LOCKING LOGIC PARA SA FLOOD (STAGE 2) ---
        // I-check kung na-unlock na ang Flood. Default ay 0 (Locked).
        int isFloodUnlocked = PlayerPrefs.GetInt("Flood_Unlocked", 0);

        if (isFloodUnlocked == 1)
        {
            floodButton.interactable = true; // Pwedeng pindutin
            if (floodLockIcon != null) floodLockIcon.SetActive(false); // Itago ang padlock
        }
        else
        {
            floodButton.interactable = false; // Naka-lock, hindi mapipindot
            if (floodLockIcon != null) floodLockIcon.SetActive(true); // Ipakita ang padlock
        }

        // --- LOCKING LOGIC PARA SA EARTHQUAKE (STAGE 3) ---
        int isEarthquakeUnlocked = PlayerPrefs.GetInt("Earthquake_Unlocked", 0);

        if (isEarthquakeUnlocked == 1)
        {
            earthquakeButton.interactable = true;
            if (earthquakeLockIcon != null) earthquakeLockIcon.SetActive(false);
        }
        else
        {
            earthquakeButton.interactable = false;
            if (earthquakeLockIcon != null) earthquakeLockIcon.SetActive(true);
        }
    }

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

    public void LoadEarthquakeScenario()
    {
        SceneManager.LoadScene("EarthquakeLevelSelect"); // Palitan mo ng tamang scene name
    }
}