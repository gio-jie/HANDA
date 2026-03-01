using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GlobalProgressManager : MonoBehaviour
{
    // --- BAGONG DAGDAG: Para mahiwalay ang Stage 1 at Stage 2 ---
    public string stagePrefix = ""; 

    [Header("UI Elements")]
    public Slider progressBar;
    public TextMeshProUGUI progressText;

    [Header("Settings")]
    public int totalLevelsInGame = 10; // Napansin ko sa console mo 0/10 siya, kaya ginawa kong 10 ang default.

    void OnEnable()
    {
        CalculateTotalProgress();
    }

    void CalculateTotalProgress()
    {
        int levelsCompleted = 0;

        for (int i = 1; i <= totalLevelsInGame; i++)
        {
            // --- BINAGO: Binabasa na ngayon ang bagong StarManager save format! ---
            int savedStars = PlayerPrefs.GetInt(stagePrefix + "Level_" + i, 0);

            // Backup logic in case may lumang save data ka pa rin
            int oldSavedStars = PlayerPrefs.GetInt("Level" + i + "_Stars", 0);

            if (savedStars > 0 || oldSavedStars > 0)
            {
                levelsCompleted++;
            }
        }

        // COMPUTE PERCENTAGE
        float progress = (float)levelsCompleted / totalLevelsInGame;

        // UPDATE UI
        if (progressBar != null)
        {
            progressBar.value = progress; // 0.0 to 1.0
        }

        if (progressText != null)
        {
            // Convert to 0% - 100% format
            progressText.text = Mathf.RoundToInt(progress * 100) + "%";
        }

        Debug.Log("Total Levels Done: " + levelsCompleted + " / " + totalLevelsInGame);
    }
}