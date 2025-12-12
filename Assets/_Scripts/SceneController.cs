using UnityEngine;
using UnityEngine.SceneManagement; // Kailangan ito para makalipat ng scene

public class SceneController : MonoBehaviour
{
    // --- MAIN MENU NAVIGATION ---

    public void GoToLoadingScreen()
    {
        ResumeMusic();
        SceneManager.LoadScene("LoadingScreen");
    }

    public void GoToScenarios()
    {
        ResumeMusic();
        SceneManager.LoadScene("ScenarioSelection");
    }

    public void GoToMainMenu()
    {
        ResumeMusic();
        SceneManager.LoadScene("MainMenu");
    }

    // --- LEVEL SELECTION ---

    public void GoToTyphoonLevels()
    {
        ResumeMusic();
        SceneManager.LoadScene("TyphoonLevelSelect");
    }

    public void GoToTyphoonLevelSelect()
    {
        ResumeMusic();
        SceneManager.LoadScene("TyphoonLevelSelect");
    }

    public void GoToTyphoonLevel1()
    {
        ResumeMusic();
        SceneManager.LoadScene("Typhoon_Level1");
    }

    // --- NEXT LEVEL NAVIGATION ---

    public void GoToTyphoonLevel2()
    {
        ResumeMusic();
        SceneManager.LoadScene("Typhoon_Level2");
    }

    public void GoToTyphoonLevel3()
    {
        ResumeMusic();
        SceneManager.LoadScene("Typhoon_Level3");
    }

    public void GoToTyphoonLevel4()
    {
        ResumeMusic();
        SceneManager.LoadScene("Typhoon_Level4");
    }

    public void GoToTyphoonLevel5()
    {
        ResumeMusic();
        SceneManager.LoadScene("Typhoon_Level5");
    }

    public void GoToTyphoonLevel6()
    {
        ResumeMusic();
        SceneManager.LoadScene("Typhoon_Level6");
    }

    //Scenario 2 --- FLOOD ----

    public void GoToFloodLevelSelect()
    {
        ResumeMusic();
        SceneManager.LoadScene("FloodLevelSelect");
    }

    public void GoToFloodLevel1()
    {
        ResumeMusic();
        SceneManager.LoadScene("Flood_Level1");
    }

    //Scenario 3 --- EARTHQUAKE ----

    public void GoToEarthquakeLevelSelect()
    {
        ResumeMusic();
        SceneManager.LoadScene("EarthquakeLevelSelect");
    }
    public void GoToEarthquakeLevel1()
    {
        ResumeMusic();
        SceneManager.LoadScene("Earthquake_Level1");
    }

    // Scenario 4 --- LANDSLIDE ---

    public void GoToLandslideLevelSelect()
    {
        ResumeMusic();
        SceneManager.LoadScene("LandslideLevelSelect");
    }
    
    public void GoToLandslideLevel1()
    {
        ResumeMusic();
        SceneManager.LoadScene("Landslide_Level1");
    }

    // Scenario 5 --- Multi-Disaster ---

    public void GoToMultiDisasterLevelSelect()
    {
        ResumeMusic();
        SceneManager.LoadScene("MultiDisasterLevelSelect");
    }

    public void GoToMultiDisasterLevel1()
    {
        ResumeMusic();
        SceneManager.LoadScene("MultiDisaster_Level1");
    }

    // Pwede mo dagdagan dito para sa Level 4, 5, etc.
    // public void GoToTyphoonLevel4() { ResumeMusic(); SceneManager.LoadScene("Typhoon_Level4"); }

    // --- SYSTEM FUNCTIONS ---

    public void QuitGame()
    {
        Debug.Log("Game is exiting...");
        Application.Quit();
    }

    // --- HELPER FUNCTION (Para hindi paulit-ulit ang code) ---
    void ResumeMusic()
    {
        // Check kung buhay si DJ, tapos utusan mag-resume
        if (AudioManager.instance != null)
        {
            AudioManager.instance.ResumeBGM();
        }
    }
}