using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // Function para lumipat sa Loading Screen
    public void GoToLoadingScreen()
    {
        SceneManager.LoadScene("LoadingScreen");
    }

    // Function para lumipat sa Scenario Selection
    public void GoToScenarios()
    {
        SceneManager.LoadScene("ScenarioSelection");
    }

    // Function para bumalik sa Main Menu (Example for back buttons)
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
    // Function para mag-quit sa game
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game is exiting..."); // Para makita sa editor na gumagana
    }
    // Papunta sa Typhoon levels
    public void GoToTyphoonLevels()
    {
        SceneManager.LoadScene("TyphoonLevelSelect");
    }
}