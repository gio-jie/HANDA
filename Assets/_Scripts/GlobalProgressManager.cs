using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GlobalProgressManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider progressBar;
    public TextMeshProUGUI progressText;

    [Header("Settings")]
    public int totalLevelsInGame = 25; // 5 Scenarios x 5 Levels each = 25
    // Kung 10 levels bawat scenario, gawin mong 50 ito.

    void OnEnable()
    {
        CalculateTotalProgress();
    }

    void CalculateTotalProgress()
    {
        int levelsCompleted = 0;

        // I-check natin ang Levels 1 hanggang 25
        for (int i = 1; i <= totalLevelsInGame; i++)
        {
            // Check kung may Star na nakuha sa level na 'to
            // (Kahit 1 star lang, considered "Done" na para sa progress)
            if (PlayerPrefs.GetInt("Level" + i + "_Stars", 0) > 0)
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