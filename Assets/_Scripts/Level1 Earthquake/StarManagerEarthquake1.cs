using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class StarManagerEarthquake1 : MonoBehaviour
{
    public static StarManagerEarthquake1 Instance;

    [Header("Level")]
    public string stagePrefix = "";
    public int levelIndex;
    public float levelDuration;

    [Header("UI")]
    public Image[] stars;
    public Slider starSlider;
    public TMP_Text timerText;
    public GameObject losePanel;
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
    public float wrongItemPenalty = 5f;
    public int maxConsecutiveWrongs = 3;

    [Header("Tasks")]
    public int totalTasks;
    public int completedTasks = 0;

    private float remainingTime;
    private float timePerStar;
    private int currentStars = 3;

    private int consecutiveWrongCount = 0;

    private Coroutine sliderCoroutine;

    private bool levelEnded = false;
    private bool gameplayStopped = false;
    private bool waitingForFinishAnimation = false;

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
        if (winPanel) winPanel.SetActive(false);
    }

    void Update()
    {
        if (levelEnded || gameplayStopped) return;

        remainingTime -= Time.deltaTime;
        remainingTime = Mathf.Max(0f, remainingTime);

        UpdateTimerText();

        int newStars = Mathf.Clamp(Mathf.CeilToInt(remainingTime / timePerStar), 0, 3);
        if (newStars != currentStars)
            UpdateStars(newStars);

        if (remainingTime <= 0f)
            TriggerLose();
    }

    public void RegisterTaskComplete()
    {
        if (levelEnded) return;

        completedTasks++;

        if (completedTasks >= totalTasks)
        {
            gameplayStopped = true;
            waitingForFinishAnimation = true;

            StartCoroutine(AutoFinishIfNoAnimation());
        }
    }

    private IEnumerator AutoFinishIfNoAnimation()
    {
        yield return new WaitForSeconds(0.5f);

        if (waitingForFinishAnimation && !levelEnded)
        {
            FinishLevelAfterAnimation();
        }
    }

    public void StartWinSequence()
    {
        if (levelEnded) return;

        gameplayStopped = true;
        waitingForFinishAnimation = true;
    }

    public void FinishLevelAfterAnimation()
    {
        if (!waitingForFinishAnimation || levelEnded) return;

        waitingForFinishAnimation = false;
        EndLevel(true);
    }


    public void RegisterCorrect()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.PlaySFX(AudioManager.instance.correctSound);

        consecutiveWrongCount = 0;
    }

    public void RegisterWrong()
    {
        if (levelEnded) return;

        if (AudioManager.instance != null)
            AudioManager.instance.PlaySFX(AudioManager.instance.warningSound);

        remainingTime -= wrongItemPenalty;
        remainingTime = Mathf.Max(0f, remainingTime);

        UpdateTimerText();

        consecutiveWrongCount++;

        if (consecutiveWrongCount >= maxConsecutiveWrongs || remainingTime <= 0f)
        {
            TriggerLose();
        }
    }

    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);

        if (timerText)
            timerText.text = $"{minutes:0}:{seconds:00}";
    }

    private void UpdateStars(int newCount)
    {
        currentStars = newCount;

        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].color = i < newCount ? activeColor : inactiveColor;
        }

        UpdateSliderSmooth();
    }

    private void UpdateSliderImmediate()
    {
        if (!starSlider) return;
        starSlider.value = GetSliderValueForStars(currentStars);
    }

    private void UpdateSliderSmooth()
    {
        if (!starSlider) return;

        float target = GetSliderValueForStars(currentStars);

        if (sliderCoroutine != null)
            StopCoroutine(sliderCoroutine);

        sliderCoroutine = StartCoroutine(SlideSlider(target));
    }

    private float GetSliderValueForStars(int count)
    {
        switch (count)
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

        if (target < start && AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.starReducedSound);
        }

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

    public void EndLevel(bool isWin)
    {
        if (levelEnded) return;

        levelEnded = true;
        gameplayStopped = true;

        SaveStars();

        if (isWin)
            ShowWinPanel();
        else
            StartCoroutine(ShowLosePanel());
    }

    private IEnumerator ShowLosePanel()
    {
        if (starSlider)
        {
            if (sliderCoroutine != null) StopCoroutine(sliderCoroutine);
            yield return StartCoroutine(SlideSlider(0f));
        }

        currentStars = 0;

        foreach (var star in stars)
            star.color = inactiveColor;

        yield return new WaitForSeconds(0.1f);

        if (losePanel)
        {
            if (AudioManager.instance)
            {
                AudioManager.instance.PauseBGM();
                AudioManager.instance.PlaySFX(AudioManager.instance.loseSound);
            }

            losePanel.SetActive(true);
            UpdateEndPanelTexts(losePanelTimeLeftText, losePanelBestTimeText);
        }
    }

    private void ShowWinPanel()
    {
        if (!winPanel) return;

        if (AudioManager.instance)
        {
            AudioManager.instance.PauseBGM();
            AudioManager.instance.PlaySFX(AudioManager.instance.winSound);
        }

        winPanel.SetActive(true);
        UpdateEndPanelTexts(winPanelTimeLeftText, winPanelBestTimeText);
    }

    private void UpdateEndPanelTexts(TMP_Text timeText, TMP_Text bestText)
    {
        int remaining = Mathf.CeilToInt(remainingTime);
        int min = remaining / 60;
        int sec = remaining % 60;

        int best = GetBestTime();
        int bestMin = best / 60;
        int bestSec = best % 60;

        if (timeText)
            timeText.text = $"Time Left: {min:0}:{sec:00}";

        if (bestText)
            bestText.text = $"Best Record: {bestMin:0}:{bestSec:00}";
    }

    public void SaveStars()
    {
        int prev = GetSavedStars();

        if (currentStars > prev)
            PlayerPrefs.SetInt(GetLevelKey(), currentStars);

        SaveBestTime();
        PlayerPrefs.Save();
    }

    private void SaveBestTime()
    {
        int remaining = Mathf.CeilToInt(remainingTime);
        int prevBest = GetBestTime();

        if (remaining > prevBest)
        {
            PlayerPrefs.SetInt(GetBestTimeKey(), remaining);
        }
    }

    private string GetLevelKey() => stagePrefix + "Level_" + levelIndex;
    private string GetBestTimeKey() => GetLevelKey() + "_BestTime";

    private void TriggerLose()
    {
        currentStars = 0;
        EndLevel(false);
    }

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
    public int GetRemainingSeconds() => Mathf.CeilToInt(remainingTime);
    public int GetSavedStars() => PlayerPrefs.GetInt(GetLevelKey(), 0);
    public int GetBestTime() => PlayerPrefs.GetInt(GetBestTimeKey(), 0);
}