using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class StarManagerLevel8 : MonoBehaviour
{
    public static StarManagerLevel8 Instance;

    [Header("Level Info")]
    public int levelIndex;
    public float levelDuration = 90f;

    [Header("UI Elements")]
    public Image[] stars;
    public Slider starSlider;
    public TMP_Text timerText;
    public GameObject losePanel;
    public GameObject endPanel;
    public GameObject winPanel;

    [Header("Panels UI")]
    public TMP_Text losePanelTimeLeftText;
    public TMP_Text losePanelBestTimeText;
    public TMP_Text winPanelTimeLeftText;
    public TMP_Text winPanelBestTimeText;

    [Header("Penalty Settings")]
    public float wrongItemPenalty = 5f;
    public int maxConsecutiveWrongs = 3;

    public Color activeColor = Color.white;
    public Color inactiveColor = Color.gray;

    private float remainingTime;
    private float timePerStar;
    private int currentStars = 3;
    private bool levelEnded = false;
    private int consecutiveWrongCount = 0;
    private int consecutiveCorrectCount = 0;
    private Coroutine sliderCoroutine;
    private bool isPaused = false;
    public void PauseTimer() => isPaused = true;
    public void ResumeTimer() => isPaused = false;
    public int GetConsecutiveCorrect() => consecutiveCorrectCount;

    [Header("Gameplay Answer Records")]
    public List<Level8AnswerRecord> answerRecords = new List<Level8AnswerRecord>();

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

        InventoryUI.Instance.RefreshUI(InventoryManager.Instance.GetSavedInventory());

        if (losePanel) losePanel.SetActive(false);
        if (endPanel) endPanel.SetActive(false);
        if (winPanel) winPanel.SetActive(false);
    }

    void Update()
    {
        if (levelEnded || isPaused) return;

        remainingTime -= Time.deltaTime;
        remainingTime = Mathf.Max(0f, remainingTime);

        UpdateTimerText();

        int newStars = Mathf.Clamp(Mathf.CeilToInt(remainingTime / timePerStar), 0, 3);
        if (newStars != currentStars)
            UpdateStars(newStars);

        if (remainingTime <= 0f)
            TriggerLose();
    }

    public void RegisterCorrect(bool hasNextCard)
    {
        consecutiveWrongCount = 0;
        consecutiveCorrectCount++;

        if (!hasNextCard)
            return;

        if (consecutiveCorrectCount >= 3)
        {
            PowerupManager.Instance.QueueRandomPowerup();
            consecutiveCorrectCount = 0;
            return;
        }

        float chance = Random.value;
        if (chance < 0.3f)
        {
            PowerupManager.Instance.QueueRandomPowerup();
        }
    }

    public void RegisterWrong()
    {
        if (levelEnded) return;

        remainingTime -= wrongItemPenalty;
        remainingTime = Mathf.Max(0f, remainingTime);
        UpdateTimerText();

        consecutiveWrongCount++;

        if (consecutiveWrongCount >= maxConsecutiveWrongs)
        {
            TriggerLose();
            return;
        }

        if (remainingTime <= 0f)
            TriggerLose();
    }

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

        UpdateSliderSmooth();
    }

    private void UpdateSliderImmediate()
    {
        if (starSlider) starSlider.value = GetSliderValue(currentStars);
    }

    private void UpdateSliderSmooth()
    {
        if (starSlider == null) return;

        float target = GetSliderValue(currentStars);

        if (sliderCoroutine != null)
            StopCoroutine(sliderCoroutine);

        sliderCoroutine = StartCoroutine(SlideSlider(target));
    }

    private float GetSliderValue(int starCount)
    {
        switch (starCount)
        {
            case 3: return 3f;
            case 2: return 1.5f;
            case 1: return 0.5f;
            default: return 0f;
        }
    }

    private IEnumerator SlideSlider(float target)
    {
        float start = starSlider.value;
        float duration = 0.4f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float factor = Mathf.Sin((t / duration) * Mathf.PI * 0.5f);
            starSlider.value = Mathf.Lerp(start, target, factor);
            yield return null;
        }

        starSlider.value = target;
    }

    public void ShowEndPanel()
    {
        if (levelEnded) return;
        levelEnded = true;

        SaveStars();

        if (endPanel)
            endPanel.SetActive(true);

        Level8ResultsUI.Instance.ShowResults(answerRecords);
    }
    public void OnProceedFromEndPanel()
    {
        ShowWinPanel();
    }

    private void ShowWinPanel()
    {
        if (!winPanel) return;

        int remainingSeconds = Mathf.CeilToInt(remainingTime);
        int minutes = remainingSeconds / 60;
        int seconds = remainingSeconds % 60;

        int bestSeconds = PlayerPrefs.GetInt("Level_" + levelIndex + "_BestTime", 0);
        int bestMin = bestSeconds / 60;
        int bestSec = bestSeconds % 60;

        winPanel.SetActive(true);

        if (AudioManager.instance != null) AudioManager.instance.PauseBGM();
            if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.winSound);

        if (winPanelTimeLeftText)
            winPanelTimeLeftText.text = $"Time Left: {minutes:0}:{seconds:00}";

        if (winPanelBestTimeText)
            winPanelBestTimeText.text = $"Best Record: {bestMin:0}:{bestSec:00}";
    }

    private void TriggerLose()
    {
        levelEnded = true;
        currentStars = 0;

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

        SaveStars();
    }

    public void SaveStars()
    {
        int previous = PlayerPrefs.GetInt("Level_" + levelIndex, 0);

        if (currentStars > previous)
            PlayerPrefs.SetInt("Level_" + levelIndex, currentStars);

        PlayerPrefs.Save();
        SaveBestTime();
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

    public int GetCurrentStars() => currentStars;
    public int GetRemainingSeconds() => Mathf.CeilToInt(remainingTime);
    public int GetSavedStars() => PlayerPrefs.GetInt("Level_" + levelIndex, 0);
    public int GetBestTime() => PlayerPrefs.GetInt("Level_" + levelIndex + "_BestTime", 0);
}