using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetGameDataStage2 : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject confirmationPanel;
    public GameObject resetButton;
    
    public string levelSelectSceneName; 

    void OnEnable()
    {
        CheckIfButtonShouldShow();
    }

    void CheckIfButtonShouldShow()
    {
        if (resetButton == null) return;

        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == levelSelectSceneName)
        {
            resetButton.SetActive(true);
        }
        else
        {
            resetButton.SetActive(false);
        }
    }

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
        int totalFloodLevels = 10;

        PlayerPrefs.DeleteKey("FloodIntroSeen");

        for (int i = 1; i <= totalFloodLevels; i++)
        {
            PlayerPrefs.DeleteKey("Level_" + i);
            PlayerPrefs.DeleteKey("Level_" + i + "_BestTime");
        }

        PlayerPrefs.Save();

        Debug.Log("Flood Stage Reset Successful!");

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}