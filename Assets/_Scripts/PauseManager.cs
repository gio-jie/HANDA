using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("Survival Stats")]
    public Image statsFillImage; // Yung Green Circle
    public TextMeshProUGUI statsText; // Yung "%" Text
    
    [Header("Sub-Panels")]
    public GameObject optionsPanel; // Yung Options Prefab

    // Reference sa Level Manager para makuha ang score
    private Level1Manager levelManager;

    void OnEnable()
    {
        // Tuwing bubukas ang Pause, hanapin ang Manager at i-update ang stats
        levelManager = FindFirstObjectByType<Level1Manager>();
        UpdateSurvivalStats();
    }

    void UpdateSurvivalStats()
    {
        if (levelManager != null)
        {
            // Calculation: Current Score / Target Score
            float progress = (float)levelManager.currentScore / levelManager.itemsNeeded;
            
            // Update Circle (0.0 to 1.0)
            statsFillImage.fillAmount = progress;
            
            // Update Text (0% to 100%)
            statsText.text = Mathf.RoundToInt(progress * 100) + "%";
        }
    }

    // --- BUTTON FUNCTIONS ---
    
    public void ResumeGame()
    {
        // Tawagin ang Resume ng Level Manager
        if(levelManager != null) levelManager.ResumeGame();
        // O kaya direct logic: gameObject.SetActive(false); Time.timeScale = 1;
    }

    public void OpenSettings()
    {
        optionsPanel.SetActive(true); // Labasin ang Options
    }

    public void QuitGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("TyphoonLevelSelect"); // Balik sa Menu
    }
}