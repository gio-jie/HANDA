using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("Survival Stats")]
    public Image statsFillImage; 
    public TextMeshProUGUI statsText; 
    
    [Header("Sub-Panels")]
    public GameObject optionsPanel; 

    private Level1Manager levelManager;

    void OnEnable()
    {
        levelManager = FindFirstObjectByType<Level1Manager>();
        UpdateSurvivalStats();
    }

    void UpdateSurvivalStats()
    {
        if (levelManager != null)
        {
            float progress = (float)levelManager.currentScore / levelManager.itemsNeeded;
            statsFillImage.fillAmount = progress;
            statsText.text = Mathf.RoundToInt(progress * 100) + "%";
        }
    }

    // --- BUTTON FUNCTIONS ---
    
    public void ResumeGame()
    {
        // Ito okay na to, kasi tinatawag niya yung LevelManager na may ResumeBGM na
        if(levelManager != null) levelManager.ResumeGame();
    }

    public void OpenSettings()
    {
        optionsPanel.SetActive(true); 
        
        // --- FIX 1: Play Music habang nag-aadjust ng settings ---
        if (AudioManager.instance != null) 
        {
            AudioManager.instance.ResumeBGM();
        }
    }

    public void QuitGame()
    {
        Time.timeScale = 1;
        
        // --- FIX 2: Play Music bago bumalik sa Level Selection ---
        if (AudioManager.instance != null) 
        {
            AudioManager.instance.ResumeBGM();
        }

        SceneManager.LoadScene("TyphoonLevelSelect"); 
    }
}