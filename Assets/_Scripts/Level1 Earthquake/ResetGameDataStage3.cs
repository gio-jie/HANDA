using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetGameDataStage3 : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject confirmationPanel;
    public GameObject resetButton;

    [Header("Scene Control")]
    public string levelSelectSceneName;

    [Header("Stage Settings")]
    public string stagePrefix = ""; // MUST MATCH THE GAME
    public int totalLevels = 5;

    void OnEnable()
    {
        CheckIfButtonShouldShow();
    }

    void CheckIfButtonShouldShow()
    {
        if (resetButton == null) return;

        string currentScene = SceneManager.GetActiveScene().name;
        resetButton.SetActive(currentScene == levelSelectSceneName);
    }

    public void AskForConfirmation()
    {
        if (confirmationPanel != null)
            confirmationPanel.SetActive(true);
    }

    public void CancelReset()
    {
        if (confirmationPanel != null)
            confirmationPanel.SetActive(false);
    }

    public void ConfirmReset()
    {
        PlayerPrefs.DeleteKey("EarthquakeIntroSeen");

        for (int i = 1; i <= totalLevels; i++)
        {
            PlayerPrefs.DeleteKey(stagePrefix + "Level_" + i);
            PlayerPrefs.DeleteKey(stagePrefix + "Level_" + i + "_BestTime");
            PlayerPrefs.DeleteKey(stagePrefix + "Level_" + i + "_Stars"); // optional
        }

        PlayerPrefs.Save();

        Debug.Log(stagePrefix + " Reset Successful!");

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}