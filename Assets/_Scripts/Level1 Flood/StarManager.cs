using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class StarManager : MonoBehaviour
{
    public static StarManager Instance;

    [Header("Level Configuration")]
    public string stagePrefix = "";
    public int levelIndex = 1;
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

    private float remainingTime;
    private float timePerStar;
    private int currentStars = 3;

    private Coroutine sliderCoroutine;
    private bool levelEnded = false;
    
    // --- BAGONG DAGDAG PARA SA PHASE 2 ---
    public bool isTimerPaused = false; 

    [Header("Penalty Settings")]
    public float wrongItemPenalty = 5f;
    public int maxConsecutiveWrongs = 3;

    private int consecutiveWrongCount = 0;

    void Start()
    {
        Instance = this;

        remainingTime = levelDuration;
        timePerStar = levelDuration / 3f;

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.ClearRuntimeInventory();
            if (InventoryUI.Instance != null)
            {
                InventoryUI.Instance.RefreshUI(InventoryManager.Instance.runtimeItems);
            }
        }

        UpdateStars(3);
        UpdateSliderImmediate();
        UpdateTimerText();
        RefreshStars();

        if (losePanel != null) losePanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
    }

    void Update()
    {
        // --- UPDATED: Wag magbawas ng oras kapag naka-pause na (Phase 2) ---
        if (levelEnded || isTimerPaused) return;

        remainingTime -= Time.deltaTime;
        remainingTime = Mathf.Max(0f, remainingTime);

        UpdateTimerText();

        int newStars = Mathf.Clamp(Mathf.CeilToInt(remainingTime / timePerStar), 0, 3);
        if (newStars != currentStars) UpdateStars(newStars);

        if (remainingTime <= 0f)
            TriggerLose();
    }

    // ==========================================
    // --- MGA BAGONG DAGDAG PARA SA HEARTS ---
    // ==========================================
    public void PauseTimer()
    {
        isTimerPaused = true;
    }

    public void SetStarsByHearts(int hearts)
    {
        int newStars = 0;
        
        if (hearts >= 4) newStars = 3;      // 4 to 5 Hearts = 3 Stars
        else if (hearts >= 2) newStars = 2; // 2 to 3 Hearts = 2 Stars
        else if (hearts == 1) newStars = 1; // 1 Heart = 1 Star
        else newStars = 0;                  // 0 Hearts = 0 Stars

        // Optional: Para hindi tumaas ang stars kung mababa na nakuha niya sa Phase 1
        newStars = Mathf.Min(newStars, currentStars);

        if (newStars != currentStars)
        {
            UpdateStars(newStars);
        }
    }
    // ==========================================

    public void RefreshStars()
    {
        int savedStars = PlayerPrefs.GetInt(stagePrefix + "Level_" + levelIndex, 0);
    }

    public void RegisterCorrectItem()
    {
        consecutiveWrongCount = 0;
    }

    public void RegisterWrongItem()
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
        {
            TriggerLose();
        }
    }
    //Bonus on level 8 EQ-----
    public void AddBonusTime(float bonusSeconds)
    {
        if (levelEnded) return;

        remainingTime += bonusSeconds;
        UpdateTimerText();
        
        int newStars = Mathf.Clamp(Mathf.CeilToInt(remainingTime / timePerStar), 0, 3);
        if (newStars != currentStars) UpdateStars(newStars);
    }
    //--------------------
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

    private IEnumerator SlideSlider(float targetValue)
    {
        float startValue = starSlider.value;

        if (targetValue < startValue)
        {
            if (AudioManager.instance != null)
                AudioManager.instance.PlaySFX(AudioManager.instance.starReducedSound);
        }

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
    }

    public void EndLevel(bool isWin)
    {
        if (levelEnded) return;
        levelEnded = true;
        consecutiveWrongCount = 0;

        SaveStars();

        if (!isWin)
        {
            StartCoroutine(FinishSliderAndShowLosePanel());
        }
        else
        {
            ShowWinPanel();
        }
    }

    private IEnumerator FinishSliderAndShowLosePanel()
    {
        if (starSlider != null)
        {
            if (sliderCoroutine != null) StopCoroutine(sliderCoroutine);
            yield return StartCoroutine(SlideSlider(0f));
        }

        currentStars = 0;
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].color = inactiveColor;
        }

        yield return new WaitForSeconds(0.1f);

        if (losePanel != null)
        {
            int remainingSeconds = Mathf.CeilToInt(remainingTime);
            int minutes = remainingSeconds / 60;
            int seconds = remainingSeconds % 60;

            int bestSeconds = PlayerPrefs.GetInt(stagePrefix + "Level_" + levelIndex + "_BestTime", 0);
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
    }

    private void ShowWinPanel()
    {
        if (winPanel != null)
        {
            int remainingSeconds = Mathf.CeilToInt(remainingTime);
            int minutes = remainingSeconds / 60;
            int seconds = remainingSeconds % 60;

            int bestSeconds = PlayerPrefs.GetInt(stagePrefix + "Level_" + levelIndex + "_BestTime", 0);
            int bestMin = bestSeconds / 60;
            int bestSec = bestSeconds % 60;

            if (AudioManager.instance != null) AudioManager.instance.PauseBGM();
            if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.winSound);
            winPanel.SetActive(true);

            if (winPanelTimeLeftText != null)
                winPanelTimeLeftText.text = $"Time Left: {minutes:0}:{seconds:00}";

            if (winPanelBestTimeText != null)
                winPanelBestTimeText.text = $"Best Record: {bestMin:0}:{bestSec:00}";

            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.SaveInventorySet();
            }
            SaveStars();
        }
    }

    public void SaveStars()
    {
        int previous = PlayerPrefs.GetInt(stagePrefix + "Level_" + levelIndex, 0);
        if (currentStars > previous)
            PlayerPrefs.SetInt(stagePrefix + "Level_" + levelIndex, currentStars);

        PlayerPrefs.Save();
        RefreshStars();
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
            RefreshStars();
        }
    }

    private void TriggerLose()
    {
        currentStars = 0;
        SaveStars();
        EndLevel(false);
    }

    public void RetryLevel() { Time.timeScale = 1f; SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }
    public void LoadNextLevel() { Time.timeScale = 1f; int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1; SceneManager.LoadScene(nextSceneIndex); }
    public int GetCurrentStars() => currentStars;
    public int GetRemainingSeconds() => Mathf.CeilToInt(remainingTime);
    public int GetSavedStars() => PlayerPrefs.GetInt(stagePrefix + "Level_" + levelIndex, 0);
    public int GetBestTime() => PlayerPrefs.GetInt(stagePrefix + "Level_" + levelIndex + "_BestTime", 0);
}