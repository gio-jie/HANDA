using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class StarManagerLevel5 : MonoBehaviour
{
    public static StarManagerLevel5 Instance;

    [Header("Level Info")]
    public int levelIndex;
    public float levelDuration = 90f;
    public float goalMeters = 100f;
    
    [Header("Star UI")]
    public Image[] stars;
    public Slider starSlider;
    public TMP_Text timerText;

    [Header("Meter UI")]
    public Slider meterSlider;
    public TMP_Text meterText;

    [Header("Panels")]
    public GameObject losePanel;
    public GameObject winPanel;

    [Header("Lose Panel UI")]
    public TMP_Text losePanelTimeLeftText;
    public TMP_Text losePanelBestTimeText;

    [Header("Win Panel UI")]
    public TMP_Text winPanelTimeLeftText;
    public TMP_Text winPanelBestTimeText;

    [Header("Colors")]
    public Color activeColor = Color.white;
    public Color inactiveColor = Color.gray;

    [Header("Travel Settings")]
    public float baseTravelTime;
    public int maxWaterHits = 3;

    [Header("Win Animation")]
    public RectTransform flagIcon;
    public float flagAnimDuration = 0.6f;
    public float flagScaleMultiplier;

    private bool isWinning = false;

    private float remainingTime;
    private float timePerStar;
    private int currentStars = 3;

    private float travelTimeAccumulated = 0f;
    private float requiredTravelTime;
    private int waterHitCount = 0;

    private bool levelEnded = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        remainingTime = levelDuration;
        timePerStar = levelDuration / 3f;

        requiredTravelTime = baseTravelTime;

        // Setup UI
        UpdateStars(currentStars);
        UpdateTimerText();

        InventoryUI.Instance.RefreshUI(InventoryManager.Instance.GetSavedInventory());

        if (meterSlider != null)
        {
            meterSlider.maxValue = goalMeters;
            meterSlider.value = 0;
        }

        if (losePanel) losePanel.SetActive(false);
        if (winPanel) winPanel.SetActive(false);
    }

    void Update()
    {
        if (levelEnded) return;

        remainingTime -= Time.deltaTime;
        remainingTime = Mathf.Max(0f, remainingTime);
        UpdateTimerText();

        UpdateStarCalculation();

        UpdateMeterProgress();

        if (remainingTime <= 0f)
            TriggerLose();
    }

    #region Meter / Travel System

    void UpdateMeterProgress()
    {
        travelTimeAccumulated += Time.deltaTime;

        float progress = Mathf.Clamp01(travelTimeAccumulated / requiredTravelTime);

        if (meterSlider != null)
            meterSlider.value = progress * goalMeters;

        if (meterText != null)
            meterText.text = Mathf.FloorToInt(progress * goalMeters) + "m";
        
        if (travelTimeAccumulated >= requiredTravelTime && !isWinning)
        {
            StartCoroutine(AnimateFlagAndWin());
        }
    }

    IEnumerator AnimateFlagAndWin()
    {
        isWinning = true;

        Time.timeScale = 0f;

        Vector2 startPos = flagIcon.anchoredPosition;
        Vector3 startScale = flagIcon.localScale;

        Vector2 centerPos = Vector2.zero;

        float timer = 0f;

        while (timer < flagAnimDuration)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / flagAnimDuration;

            t = 1f - Mathf.Pow(1f - t, 3f);

            flagIcon.anchoredPosition = Vector2.Lerp(startPos, centerPos, t);

            flagIcon.localScale = Vector3.Lerp(startScale, Vector3.one * 3f, t);

            yield return null;
        }

        flagIcon.anchoredPosition = centerPos;
        flagIcon.localScale = Vector3.one * 3f;

        yield return new WaitForSecondsRealtime(0.2f);

        TriggerWin();
    }

    #endregion

    #region Penalties

    public void ApplyPenalty(float seconds)
    {
        if (levelEnded) return;

        // 1️⃣ Increase required travel time
        requiredTravelTime += seconds;

        // 2️⃣ Subtract from remaining level timer
        remainingTime -= seconds;
        remainingTime = Mathf.Max(0f, remainingTime);

        UpdateTimerText();
        UpdateStarCalculation();
    }

    public void RegisterWaterHit()
    {
        if (levelEnded) return;

        waterHitCount++;

        if (waterHitCount >= maxWaterHits)
            TriggerLose();
    }

    #endregion

    #region Star System

    void UpdateStarCalculation()
    {
        int newStars = Mathf.Clamp(Mathf.CeilToInt(remainingTime / timePerStar), 0, 3);
        if (newStars != currentStars)
            UpdateStars(newStars);
    }

    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);
        if (timerText != null)
            timerText.text = $"{minutes:0}:{seconds:00}";
    }

    void UpdateStars(int newCount)
    {
        currentStars = newCount;

        if (stars != null)
        {
            for (int i = 0; i < stars.Length; i++)
                stars[i].color = i < newCount ? activeColor : inactiveColor;
        }

        if (starSlider != null)
        {
            switch (currentStars)
            {
                case 3: starSlider.value = 3f; break;
                case 2: starSlider.value = 1.5f; break;
                case 1: starSlider.value = 0.5f; break;
                default: starSlider.value = 0f; break;
            }
        }
    }

    #endregion

    #region Win / Lose

    void TriggerWin()
    {
        if (levelEnded) return;

        levelEnded = true;
        Time.timeScale = 0f;

        SaveStars();
        SaveBestTime();

        ShowWinPanel();
    }

    void ShowWinPanel()
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

    void TriggerLose()
    {
        if (levelEnded) return;

        levelEnded = true;
        Time.timeScale = 0f;
        currentStars = 0;

        SaveStars();

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PauseBGM();
            AudioManager.instance.PlaySFX(AudioManager.instance.loseSound);
        }

        if (losePanel != null)
        {
            int remainingSeconds = Mathf.CeilToInt(remainingTime);
            int minutes = remainingSeconds / 60;
            int seconds = remainingSeconds % 60;

            int bestSeconds = PlayerPrefs.GetInt("Level_" + levelIndex + "_BestTime", 0);
            int bestMin = bestSeconds / 60;
            int bestSec = bestSeconds % 60;

            if (AudioManager.instance != null) AudioManager.instance.PauseBGM();
            if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.loseSound);
            losePanel.SetActive(true);

            if (losePanelTimeLeftText != null)
                losePanelTimeLeftText.text = $"Time Left: {minutes:0}:{seconds:00}";

            if (losePanelBestTimeText != null)
                losePanelBestTimeText.text = $"Best Record: {bestMin:0}:{bestSec:00}";
        }

        // if (losePanel)
        //     losePanel.SetActive(true);
    }

    #endregion

    #region Save

    void SaveStars()
    {
        int previous = PlayerPrefs.GetInt("Level_" + levelIndex, 0);
        if (currentStars > previous)
            PlayerPrefs.SetInt("Level_" + levelIndex, currentStars);

        PlayerPrefs.Save();
    }

    void SaveBestTime()
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

    #region Getters

    public int GetCurrentStars() => currentStars;
    public int GetRemainingSeconds() => Mathf.CeilToInt(remainingTime);
    public int GetSavedStars() => PlayerPrefs.GetInt("Level_" + levelIndex, 0);
    public int GetBestTime() => PlayerPrefs.GetInt("Level_" + levelIndex + "_BestTime", 0);

    #endregion
}