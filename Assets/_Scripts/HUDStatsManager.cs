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
            progress = (float)lvl4.plugsConnected / 3f; 
        }

        // --- LEVEL 6 (Conveyor) ---
        Level6Manager lvl6 = FindFirstObjectByType<Level6Manager>();
        if (lvl6 != null) 
        {
            if (lvl6.itemsNeeded > 0)
            {
                progress = (float)lvl6.currentScore / lvl6.itemsNeeded;
            }
        }

        // --- LEVEL 7 PHASE 1 (Mosquito) ---
        Level7Manager lvl7 = FindFirstObjectByType<Level7Manager>();
        if (lvl7 != null) 
        {
            if (lvl7.targetKills > 0)
            {
                progress = (float)lvl7.currentKills / lvl7.targetKills;
            }
        }

        // --- LEVEL 7 PART 2 (Go Bag) ---
        Level7Part2Manager lvl7Part2 = FindFirstObjectByType<Level7Part2Manager>();
        if (lvl7Part2 != null)
        {
            if (lvl7Part2.itemsNeeded > 0)
            {
                progress = (float)lvl7Part2.currentScore / lvl7Part2.itemsNeeded;
            }
        }

        // ==========================================
        // --- BAGONG DAGDAG: LEVEL 8 PART 1 ---
        // ==========================================
        Level8Part1Manager lvl8Part1 = FindFirstObjectByType<Level8Part1Manager>();
        if (lvl8Part1 != null)
        {
            if (lvl8Part1.totalFamiliesToServe > 0)
            {
                progress = (float)lvl8Part1.familiesServed / lvl8Part1.totalFamiliesToServe;
            }
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