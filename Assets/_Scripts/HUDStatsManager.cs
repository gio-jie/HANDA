using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDStatsManager : MonoBehaviour
{
    [Header("UI References")]
    public Image statsFillImage; 
    public TextMeshProUGUI statsText; 

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
                progress = (float)lvl2.tasksDone / lvl2.totalTasks; 
        }

        // --- LEVEL 3 (Cleaning) ---
        Level3Manager lvl3 = FindFirstObjectByType<Level3Manager>();
        if (lvl3 != null) 
        {
            if (lvl3.itemsNeeded > 0)
                progress = (float)lvl3.currentScore / lvl3.itemsNeeded;
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
                progress = (float)lvl6.currentScore / lvl6.itemsNeeded;
        }

        // --- LEVEL 7 PHASE 1 (Mosquito) ---
        Level7Manager lvl7 = FindFirstObjectByType<Level7Manager>();
        if (lvl7 != null) 
        {
            if (lvl7.targetKills > 0)
                progress = (float)lvl7.currentKills / lvl7.targetKills;
        }

        // --- LEVEL 7 PART 2 (Go Bag) ---
        Level7Part2Manager lvl7Part2 = FindFirstObjectByType<Level7Part2Manager>();
        if (lvl7Part2 != null)
        {
            if (lvl7Part2.itemsNeeded > 0)
                progress = (float)lvl7Part2.currentScore / lvl7Part2.itemsNeeded;
        }

        // --- LEVEL 8 PART 1 ---
        Level8Part1Manager lvl8Part1 = FindFirstObjectByType<Level8Part1Manager>();
        if (lvl8Part1 != null)
        {
            if (lvl8Part1.totalFamiliesToServe > 0)
                progress = (float)lvl8Part1.familiesServed / lvl8Part1.totalFamiliesToServe;
        }

        // --- LEVEL 9 PART 1 ---
        Level9Part1Manager lvl9Part1 = FindFirstObjectByType<Level9Part1Manager>();
        if (lvl9Part1 != null)
        {
            if (lvl9Part1.totalHazards > 0)
                progress = (float)lvl9Part1.resolvedHazards / lvl9Part1.totalHazards;
        }

        // --- LEVEL 9 PART 2 ---
        Level9Part2Manager lvl9Part2 = FindFirstObjectByType<Level9Part2Manager>();
        if (lvl9Part2 != null)
        {
            if (lvl9Part2.totalHazards > 0)
            {
                progress = (float)lvl9Part2.clearedHazards / lvl9Part2.totalHazards;
            }
        }

        // --- EARTHQUAKE LEVEL 6 ---
        Level6ManagerEQ lvl6EQ = FindFirstObjectByType<Level6ManagerEQ>();
        if (lvl6EQ != null)
        {
            if (lvl6EQ.buildingsRequired > 0)
            {
                progress = (float)lvl6EQ.buildingsTagged / lvl6EQ.buildingsRequired;
            }
        }

        // --- EARTHQUAKE LEVEL 9 ---
        Level9ManagerEQ lvl9EQ = FindFirstObjectByType<Level9ManagerEQ>();
        if (lvl9EQ != null)
        {
            if (lvl9EQ.totalVehicles > 0)
            {
                progress = (float)lvl9EQ.clearedVehicles / lvl9EQ.totalVehicles;
            }
        }

        // ==========================================
        // --- BAGONG DAGDAG: LEVEL 10 TRIVIA ---
        // ==========================================
        Level10ManagerEQ lvl10 = FindFirstObjectByType<Level10ManagerEQ>();
        if (lvl10 != null)
        {
            if (lvl10.questionPanels != null && lvl10.questionPanels.Length > 0)
            {
                // Progress based on ilang questions na ang nasagutan
                progress = (float)lvl10.currentQuestionIndex / lvl10.questionPanels.Length;
            }
        }

        // --- UPDATE UI ---
        progress = Mathf.Clamp01(progress);

        if (statsFillImage != null) statsFillImage.fillAmount = progress;
        
        if (statsText != null) 
        {
            // Override para sa Level 10: Ipakita ang "0/10" format
            if (lvl10 != null && lvl10.questionPanels != null)
            {
                statsText.text = lvl10.currentQuestionIndex + "/" + lvl10.questionPanels.Length;
            }
            // Para sa lahat ng ibang levels, percentage format pa rin
            else
            {
                statsText.text = Mathf.RoundToInt(progress * 100) + "%";
            }
        }
    }
}