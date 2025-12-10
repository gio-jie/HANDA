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

    // Tinanggal natin yung "private Level1Manager levelManager" kasi masyadong specific yun.

    void OnEnable()
    {
        UpdateSurvivalStats();
    }

    void UpdateSurvivalStats()
    {
        float progress = 0f;

        // --- LEVEL 1 (Items) ---
        Level1Manager lvl1 = FindFirstObjectByType<Level1Manager>();
        if (lvl1 != null) 
        {
            // Divide Current / Total
            progress = (float)lvl1.currentScore / lvl1.itemsNeeded;
        }

        // --- LEVEL 2 (Repairs) ---
        Level2Manager lvl2 = FindFirstObjectByType<Level2Manager>();
        if (lvl2 != null) 
        {
            // ITO ANG FIX: Tanggalin ang 0.5f, gamitin ang totoong formula!
            if (lvl2.totalTasks > 0) // Safety check para iwas error
            {
                progress = (float)lvl2.tasksDone / lvl2.totalTasks; 
            }
        }

        // --- LEVEL 3 (Cleaning) ---
        Level3Manager lvl3 = FindFirstObjectByType<Level3Manager>();
        if (lvl3 != null) 
        {
            if (lvl3.itemsNeeded > 0)
            {
                progress = (float)lvl3.currentScore / lvl3.itemsNeeded;
            }
        }

        // --- LEVEL 4 (Plugs) ---
        Level4Manager lvl4 = FindFirstObjectByType<Level4Manager>();
        if (lvl4 != null) 
        {
            // Total plugs is usually 3 based on your design
            // Progress = (Total - Left) / Total
            progress = (float)(3 - lvl4.plugsConnected) / 3;
        }

        // ---------------------------------------------------------

        // Update UI
        if (statsFillImage != null) statsFillImage.fillAmount = progress;
        if (statsText != null) statsText.text = Mathf.RoundToInt(progress * 100) + "%";
    }

    // --- BUTTON FUNCTIONS (INDEPENDENT NA SILA!) ---
    
    public void ResumeGame()
    {
        // FIX: Hindi na tayo tatawag sa LevelManager. Tayo na gagawa!
        Time.timeScale = 1; // Padaluyin ang oras
        gameObject.SetActive(false); // Itago ang sarili (Pause Panel)
        
        // Gisingin si DJ
        if (AudioManager.instance != null) AudioManager.instance.ResumeBGM();
    }

    public void OpenSettings()
    {
        optionsPanel.SetActive(true); 
        if (AudioManager.instance != null) AudioManager.instance.ResumeBGM();
    }

    public void QuitGame()
    {
        Time.timeScale = 1;
        if (AudioManager.instance != null) AudioManager.instance.ResumeBGM();
        SceneManager.LoadScene("TyphoonLevelSelect"); 
    }
}