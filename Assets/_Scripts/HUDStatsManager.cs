using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDStatsManager : MonoBehaviour
{
    [Header("UI References")]
    public Image statsFillImage; 
    public TextMeshProUGUI statsText; 

    // Gagamit tayo ng Update() para real-time ang pag-fill ng bar habang naglalaro
    void Update()
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
            if (lvl1.itemsNeeded > 0)
                progress = (float)lvl1.currentScore / lvl1.itemsNeeded;
        }

        // --- LEVEL 2 (Repairs) ---
        Level2Manager lvl2 = FindFirstObjectByType<Level2Manager>();
        if (lvl2 != null) 
        {
            if (lvl2.totalTasks > 0)
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
            // Note: Siguraduhin na tama ang logic mo dito. 
            // Kung gusto mo mapuno ang bar habang nagkokonek, dapat: plugsConnected / 3
            // Kung gusto mo mabawasan (countdown), gamitin mo yung luma mo: (3 - connected) / 3
            
            // Default assumption (Filling up bar):
            progress = (float)lvl4.plugsConnected / 3f; 
        }

        // --- UPDATE UI ---
        // Clamp para hindi lumampas sa 0 to 1
        progress = Mathf.Clamp01(progress);

        if (statsFillImage != null) statsFillImage.fillAmount = progress;
        
        if (statsText != null) 
        {
            // Format: "50%"
            statsText.text = Mathf.RoundToInt(progress * 100) + "%";
        }
    }
}