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