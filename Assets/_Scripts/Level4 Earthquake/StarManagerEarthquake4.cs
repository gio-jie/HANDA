using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class StarManagerEarthquake4 : MonoBehaviour
{
    public static StarManagerEarthquake4 Instance;

    [Header("Level Info")]
    public string stagePrefix = "";
    public int levelIndex;
    public float levelDuration;

    [Header("UI Elements")]
    public Image[] stars;
    public Slider starSlider;
    public TMP_Text timerText;

    public GameObject losePanel;
    public GameObject endPanel;
    public GameObject winPanel;

    [Header("Lose Panel UI")]
    public TMP_Text losePanelTimeLeftText;
    public TMP_Text losePanelBestTimeText;

    [Header("Win Panel UI")]
    public TMP_Text winPanelTimeLeftText;
    public TMP_Text winPanelBestTimeText;

    public Color activeColor = Color.white;
    public Color inactiveColor = Color.gray;

    [Header("Penalty Settings")]
    public float wrongItemPenalty;
    public bool useConsecutiveWrongs = false;
    public int maxConsecutiveWrongs;

    private float remainingTime;
    private float timePerStar;
    private int currentStars = 3;

    private bool levelEnded = false;
    private int consecutiveWrongCount = 0;

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

        HideAllPanels();
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
        {
            TriggerLose(); // ✅ only runs once now
        }
    }

    void HideAllPanels()
    {
        if (losePanel) losePanel.SetActive(false);
        if (winPanel) winPanel.SetActive(false);
        if (endPanel) endPanel.SetActive(false);
    }

    #region TIMER

    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);
        timerText.text = $"{minutes:0}:{seconds:00}";
    }

    #endregion

    #region STARS

    private void UpdateStars(int newCount)
    {
        currentStars = newCount;

        for (int i = 0; i < stars.Length; i++)
            stars[i].color = i < newCount ? activeColor : inactiveColor;

        UpdateSliderImmediate();
    }

    private void UpdateSliderImmediate()
    {
        if (starSlider)
            starSlider.value = GetSliderValueForStars(currentStars);
    }

    private float GetSliderValueForStars(int starsCount)
    {
        return starsCount switch
        {
            3 => 3f,
            2 => 1.5f,
            1 => 0.5f,
            _ => 0f
        };
    }

    #endregion

    #region GAMEPLAY

    public void RegisterCorrectItem()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.PlaySFX(AudioManager.instance.correctSound);

        consecutiveWrongCount = 0;
    }

    public void RegisterWrongItem()
    {
        if (levelEnded) return;

        if (AudioManager.instance != null)
            AudioManager.instance.PlaySFX(AudioManager.instance.warningSound);

        remainingTime -= wrongItemPenalty;
        remainingTime = Mathf.Max(0f, remainingTime);

        UpdateTimerText();

        if (useConsecutiveWrongs)
        {
            consecutiveWrongCount++;

            if (consecutiveWrongCount >= maxConsecutiveWrongs)
            {
                TriggerLose();
                return;
            }
        }

        if (remainingTime <= 0f)
        {
            TriggerLose();
        }
    }

    #endregion

    #region LOSE (FIXED)

    private void TriggerLose()
    {
        // ✅ CRITICAL FIX: prevents double trigger
        if (levelEnded) return;

        levelEnded = true;
        currentStars = 0;

        // ✅ Ensure ONLY lose panel shows
        if (endPanel) endPanel.SetActive(false);
        if (winPanel) winPanel.SetActive(false);

        if (losePanel != null)
        {
            losePanel.SetActive(true);

            int remainingSeconds = Mathf.CeilToInt(remainingTime);
            int minutes = remainingSeconds / 60;
            int seconds = remainingSeconds % 60;

            int bestSeconds = PlayerPrefs.GetInt(stagePrefix + "Level_" + levelIndex + "_BestTime", 0);
            int bestMin = bestSeconds / 60;
            int bestSec = bestSeconds % 60;

            if (AudioManager.instance != null)
            {
                AudioManager.instance.PauseBGM();
                AudioManager.instance.PlaySFX(AudioManager.instance.loseSound);
            }

            if (losePanelTimeLeftText != null)
                losePanelTimeLeftText.text = $"Time Left: {minutes:0}:{seconds:00}";

            if (losePanelBestTimeText != null)
                losePanelBestTimeText.text = $"Best Record: {bestMin:0}:{bestSec:00}";
        }

        SaveStars();
    }

    #endregion

    #region WIN FLOW

    public void ShowEndPanel()
    {
        // ❗ Only allow if NOT already ended (prevents conflict with lose)
        if (levelEnded) return;

        levelEnded = true;

        HideAllPanels();

        if (endPanel)
            endPanel.SetActive(true);

        SaveStars();
    }

    public void OnProceedFromEndPanel()
    {
        ShowWinPanel();
    }

    private void ShowWinPanel()
    {
        if (!winPanel) return;

        if (losePanel) losePanel.SetActive(false);
        if (endPanel) endPanel.SetActive(false);

        winPanel.SetActive(true);

        int remainingSeconds = Mathf.CeilToInt(remainingTime);
        int minutes = remainingSeconds / 60;
        int seconds = remainingSeconds % 60;

        int bestSeconds = PlayerPrefs.GetInt(stagePrefix + "Level_" + levelIndex + "_BestTime", 0);
        int bestMin = bestSeconds / 60;
        int bestSec = bestSeconds % 60;

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PauseBGM();
            AudioManager.instance.PlaySFX(AudioManager.instance.winSound);
        }

        if (winPanelTimeLeftText != null)
            winPanelTimeLeftText.text = $"Time Left: {minutes:0}:{seconds:00}";

        if (winPanelBestTimeText != null)
            winPanelBestTimeText.text = $"Best Record: {bestMin:0}:{bestSec:00}";

        SaveStars();
    }

    #endregion

    #region SAVE

    public void SaveStars()
    {
        int previous = PlayerPrefs.GetInt(stagePrefix + "Level_" + levelIndex, 0);

        if (currentStars > previous)
            PlayerPrefs.SetInt(stagePrefix + "Level_" + levelIndex, currentStars);

        PlayerPrefs.Save();
        SaveBestTime();
    }

    private void SaveBestTime()
    {
        int remainingSeconds = Mathf.CeilToInt(remainingTime);
        int previousBest = PlayerPrefs.GetInt(stagePrefix + "Level_" + levelIndex + "_BestTime", 0);

        if (remainingSeconds > previousBest)
        {
            PlayerPrefs.SetInt(stagePrefix + "Level_" + levelIndex + "_BestTime", remainingSeconds);
            PlayerPrefs.Save();
        }
    }

    #endregion

    #region PUBLIC

    public void RetryLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public int GetCurrentStars() => currentStars;

    #endregion
}