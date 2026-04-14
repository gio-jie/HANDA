using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class StarManagerLevel9 : MonoBehaviour
{
    public static StarManagerLevel9 Instance;

    [Header("Level Settings")]
    public int levelIndex;
    public float levelDuration;

    [Header("Task Texts")]
    public TMP_Text wasteText;
    public TMP_Text sanitizerText;
    public TMP_Text waterText;
    public TMP_Text dataText;
    public TMP_Text cookText;

    [Header("UI Elements")]
    public Image[] stars;
    public Slider starSlider;
    public TMP_Text timerText;
    public GameObject losePanel;
    public TMP_Text losePanelTimeLeftText;
    public TMP_Text losePanelBestTimeText;
    public GameObject winPanel;
    public TMP_Text winPanelTimeLeftText;
    public TMP_Text winPanelBestTimeText;
    public GameObject checkmark;

    [Header("Star Settings")]
    public Color activeColor = Color.white;
    public Color inactiveColor = Color.gray;

    [Header("Penalty Settings")]
    public float wrongItemPenalty = 5f;
    public int maxConsecutiveWrongs = 3;

    private float remainingTime;
    private float timePerStar;
    private int currentStars = 3;
    private int consecutiveWrongCount = 0;
    private Coroutine sliderCoroutine;
    private bool levelEnded = false;
    private bool timerRunning = true;

    // Task completion flags
    private bool wasteDone = false;
    private bool sanitizerDone = false;
    private bool waterDone = false;
    private bool dataDone = false;
    private bool cookDone = false;

    void Awake() => Instance = this;

    void Start()
    {
        remainingTime = levelDuration;
        timePerStar = levelDuration / 3f;

        UpdateStars(3);
        //RefreshStars();
        UpdateSliderImmediate();
        UpdateTimerText();

        if (losePanel != null) losePanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
        if (checkmark != null) checkmark.SetActive(false);
    }

    void Update()
    {
        if (levelEnded || !timerRunning) return;

        if (remainingTime > 0f)
        {
            remainingTime -= Time.deltaTime;
            remainingTime = Mathf.Max(0f, remainingTime);
            UpdateTimerText();

            int newStars = Mathf.Clamp(Mathf.CeilToInt(remainingTime / timePerStar), 0, 3);
            if (newStars != currentStars) UpdateStars(newStars);

            if (remainingTime <= 0f) TriggerLose();
        }
    }

    #region Timer & Stars

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
        if (starSlider == null) return;
        starSlider.value = GetSliderValueForStars(currentStars);
    }

    private void UpdateSliderSmooth()
    {
        if (starSlider == null) return;

        float targetValue = GetSliderValueForStars(currentStars);

        if (sliderCoroutine != null)
            StopCoroutine(sliderCoroutine);

        sliderCoroutine = StartCoroutine(SlideSlider(targetValue));
    }

    private float GetSliderValueForStars(int starsCount)
    {
        switch (starsCount)
        {
            case 3: return 3f;
            case 2: return 1.5f;
            case 1: return 0.5f;
            default: return 0f;
        }
    }

    public void RefreshStars()
    {
        int savedStars = PlayerPrefs.GetInt("Level_" + levelIndex, 0);
        for (int i = 0; i < stars.Length; i++)
            stars[i].color = i < savedStars ? activeColor : inactiveColor;
    }

    private IEnumerator SlideSlider(float targetValue)
    {
        float startValue = starSlider.value;
        if (targetValue < startValue && AudioManager.instance != null)
            AudioManager.instance.PlaySFX(AudioManager.instance.starReducedSound);

        float duration = 0.4f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float factor = Mathf.Sin((t / duration) * Mathf.PI * 0.5f);
            starSlider.value = Mathf.Lerp(startValue, targetValue, factor);
            yield return null;
        }

        starSlider.value = targetValue;

        sliderCoroutine = null;
    }

    #endregion

    #region Task Management

    public void SetDataPartial()
    {
        dataText.color = Color.yellow;
        dataText.text = "Download Data (1/2)";
    }

    public void CompleteTask(string taskName)
    {
        StartCoroutine(CompleteTaskRoutine(taskName));
    }

    private IEnumerator CompleteTaskRoutine(string taskName)
    {
        AudioManager.instance.PlaySFX(AudioManager.instance.correctSound);
        StartCoroutine(ShowCheckmark());

        TMP_Text taskText = null;

        switch (taskName)
        {
            case "Waste": wasteDone = true; taskText = wasteText; break;
            case "Sanitizer": sanitizerDone = true; taskText = sanitizerText; break;
            case "Water": waterDone = true; taskText = waterText; break;
            case "Data": dataDone = true; taskText = dataText; break;
            case "Cook": cookDone = true; taskText = cookText; break;
        }

        if (taskText != null)
        {
            float timer = 0f;
            float duration = 0.3f;
            Color startColor = taskText.color;
            Color targetColor = new Color(152f/255f, 152f/255f, 152f/255f);

            while (timer < duration)
            {
                timer += Time.deltaTime;
                taskText.color = Color.Lerp(startColor, targetColor, timer / duration);
                yield return null;
            }
            taskText.color = targetColor;
        }

        yield return new WaitForSeconds(0.1f);

        CheckWin();
    }

    private IEnumerator ShowCheckmark()
    {
        checkmark.SetActive(true);
        yield return new WaitForSeconds(1f);
        checkmark.SetActive(false);
    }

    private void CheckWin()
    {
        if (wasteDone && sanitizerDone && waterDone && dataDone && cookDone)
        {
            levelEnded = true;
            timerRunning = false;
            ShowWinPanel();
        }
    }

    #endregion

    #region Penalty

    public void RegisterWrongItem()
    {
        if (levelEnded) return;

        remainingTime -= wrongItemPenalty;
        remainingTime = Mathf.Max(0f, remainingTime);
        UpdateTimerText();

        consecutiveWrongCount++;

        if (consecutiveWrongCount >= maxConsecutiveWrongs)
            TriggerLose();
    }

    public void RegisterCorrectItem()
    {
        consecutiveWrongCount = 0;
    }

    #endregion

    #region Win / Lose Panels

    private void ShowWinPanel()
    {
        if (winPanel != null)
        {
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

            if (winPanelTimeLeftText != null)
                winPanelTimeLeftText.text = $"Time Left: {minutes:0}:{seconds:00}";

            if (winPanelBestTimeText != null)
                winPanelBestTimeText.text = $"Best Record: {bestMin:0}:{bestSec:00}";

            SaveStars();
        }
    }

    private void TriggerLose()
    {
        if (levelEnded) return;
        StartCoroutine(TriggerLoseRoutine());
    }

    private IEnumerator TriggerLoseRoutine()
    {
        levelEnded = true;
        timerRunning = false;
        currentStars = 0;
        SaveStars();

        while (sliderCoroutine != null)
            yield return null;

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

            losePanel.SetActive(true);

            if (losePanelTimeLeftText != null)
                losePanelTimeLeftText.text = $"Time Left: {minutes:0}:{seconds:00}";

            if (losePanelBestTimeText != null)
                losePanelBestTimeText.text = $"Best Record: {bestMin:0}:{bestSec:00}";
        }
    }

    #endregion

    #region Saving

    public void SaveStars()
    {
        int previous = PlayerPrefs.GetInt("Level_" + levelIndex, 0);
        if (currentStars > previous)
            PlayerPrefs.SetInt("Level_" + levelIndex, currentStars);

        PlayerPrefs.Save();
        RefreshStars();
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
            RefreshStars();
        }
    }

    #endregion

    #region Utility

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
    public int GetSavedStars() => PlayerPrefs.GetInt("Level_" + levelIndex, 0);
    public int GetBestTime() => PlayerPrefs.GetInt("Level_" + levelIndex + "_BestTime", 0);

    #endregion
}