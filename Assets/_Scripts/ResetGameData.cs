using UnityEngine;
using UnityEngine.SceneManagement; // Kailangan ito para malaman ang Scene Name

public class ResetGameData : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject confirmationPanel; // Ang Panel na may "Are you sure?"
    public GameObject resetButton;       // <--- BAGO: Ang RED BUTTON mismo

    // Aalamin nito kung anong Scene tayo ngayon
    // Palitan mo ang "TyphoonLevelSelect" kung iba ang spelling ng Scene mo!
    public string levelSelectSceneName = "TyphoonLevelSelect"; 

    void OnEnable()
    {
        // Tuwing bubuksan ang Options Panel, mag-c-check siya:
        CheckIfButtonShouldShow();
    }

    void CheckIfButtonShouldShow()
    {
        if (resetButton == null) return;

        string currentScene = SceneManager.GetActiveScene().name;

        // LOGIC: Kung nasa Level Selection tayo, IPAKITA. Kung hindi, ITAGO.
        if (currentScene == levelSelectSceneName)
        {
            resetButton.SetActive(true);
        }
        else
        {
            resetButton.SetActive(false);
        }
    }

    // --- EXISTING FUNCTIONS (Wala tayong ginalaw dito) ---

    public void AskForConfirmation()
    {
        if (confirmationPanel != null) confirmationPanel.SetActive(true);
    }

    public void CancelReset()
    {
        if (confirmationPanel != null) confirmationPanel.SetActive(false);
    }

    public void ConfirmReset()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("Game Reset Successful!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}