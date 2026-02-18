using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class StarManagerLevel4 : MonoBehaviour
{
    public static StarManagerLevel4 Instance;

    [Header("Level Info")]
    public int levelIndex;
    public float levelDuration = 120f;

    [Header("UI Elements")]
    public Image[] stars;
    public Slider starSlider;
    public TMP_Text timerText;

    public GameObject losePanel;
    public GameObject completionPanel; // your pop panel
    public GameObject winPanel;

    [Header("Lose Panel UI")]
    public TMP_Text losePanelTimeLeftText;
    public TMP_Text losePanelBestTimeText;

    [Header("Win Panel UI")]
    public TMP_Text winPanelTimeLeftText;
    public TMP_Text winPanelBestTimeText;

    public Color activeColor = Color.white;
    public Color inactiveColor = Color.gray;

    private float remainingTime;
    private float timePerStar;
    private int currentStars = 3;
    private bool levelEnded = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        remainingTime = levelDuration;
        timePerStar = levelDuration / 3f;

        UpdateStars(3);
        UpdateSliderImmediate();
        UpdateTimerText();

        if (losePanel) losePanel.SetActive(false);
        if (completionPanel) completionPanel.SetActive(false);
        if (winPanel) winPanel.SetActive(false);
    }

    void Update()
    {
        if (levelEnded) return;

        remainingTime -= Time.deltaTime;
        remainingTime = Mathf.Max(0f, remainingTime);

        UpdateTimerText();

        int newStars = Mathf.Clamp(Mathf.CeilToInt(remainingTime / timePerStar), 0, 3);
        if (newStars != currentStars)
            UpdateStars(newStars);

        if (remainingTime <= 0f)
            TriggerLose();
    }

    #region Puzzle Completion

    public void OnPuzzleCompleted()
    {
        if (levelEnded) return;

        levelEnded = true;      // stop timer
        SaveStars();            // lock star result immediately
        SaveBestTime();

        // Show animated completion panel
        if (completionPanel != null)
            completionPanel.SetActive(true);
    }

    public void OnProceedFromCompletionPanel()
    {
        ShowWinPanel();
    }

    #endregion

    #region Win / Lose

    private void ShowWinPanel()
    {
        if (!winPanel) return;

        int remainingSeconds = Mathf.CeilToInt(remainingTime);
        int minutes = remainingSeconds / 60;
        int seconds = remainingSeconds % 60;

        int bestSeconds = PlayerPrefs.GetInt("Level_" + levelIndex + "_BestTime", 0);
        int bestMin = bestSeconds / 60;
        int bestSec = bestSeconds % 60;

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PauseBGM();
            AudioManager.instance.PlaySFX(AudioManager.instance.winSound);
        }

        winPanel.SetActive(true);

        if (winPanelTimeLeftText)
            winPanelTimeLeftText.text = $"Time Left: {minutes:0}:{seconds:00}";

        if (winPanelBestTimeText)
            winPanelBestTimeText.text = $"Best Record: {bestMin:0}:{bestSec:00}";
    }

    private void TriggerLose()
    {
        if (levelEnded) return;

        levelEnded = true;
        currentStars = 0;
        SaveStars();

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PauseBGM();
            AudioManager.instance.PlaySFX(AudioManager.instance.loseSound);
        }

        if (losePanel)
            losePanel.SetActive(true);
    }

    #endregion

    #region Stars & Timer

    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);
        timerText.text = $"{minutes:0}:{seconds:00}";
    }

    private void UpdateStars(int newCount)
    {
        currentStars = newCount;

        for (int i = 0; i < stars.Length; i++)
            stars[i].color = i < newCount ? activeColor : inactiveColor;

        UpdateSliderImmediate();
    }

    private void UpdateSliderImmediate()
    {
        if (!starSlider) return;

        switch (currentStars)
        {
            case 3: starSlider.value = 3f; break;
            case 2: starSlider.value = 1.5f; break;
            case 1: starSlider.value = 0.5f; break;
            default: starSlider.value = 0f; break;
        }
    }

    #endregion

    #region Save

    private void SaveStars()
    {
        int previous = PlayerPrefs.GetInt("Level_" + levelIndex, 0);
        if (currentStars > previous)
            PlayerPrefs.SetInt("Level_" + levelIndex, currentStars);

        PlayerPrefs.Save();
    }

    private void SaveBestTime()
    {
        int remainingSeconds = Mathf.CeilToInt(remainingTime);
        int previousBest = PlayerPrefs.GetInt("Level_" + levelIndex + "_BestTime", 0);

        if (remainingSeconds > previousBest)
        {
            PlayerPrefs.SetInt("Level_" + levelIndex + "_BestTime", remainingSeconds);
            PlayerPrefs.Save();
        }
    }

    #endregion

    #region Controls

    public void RetryLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    #endregion

    public int GetCurrentStars()
    {
        return currentStars;
    }

    public int GetRemainingSeconds()
    {
        return Mathf.CeilToInt(remainingTime);
    }

    public int GetSavedStars()
    {
        return PlayerPrefs.GetInt("Level_" + levelIndex, 0);
    }

    public int GetBestTime()
    {
        return PlayerPrefs.GetInt("Level_" + levelIndex + "_BestTime", 0);
    }
}