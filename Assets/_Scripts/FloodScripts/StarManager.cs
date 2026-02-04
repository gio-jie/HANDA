using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class StarManager : MonoBehaviour
{
    [Header("Level")]
    public int levelIndex = 1;
    public float levelDuration = 180f;

    [Header("UI")]
    public Image[] stars;
    public Slider starSlider;
    public TMP_Text timerText;
    public GameObject losePanel;

    [Header("Panels")]
    public TMP_Text panelTimeLeftText;
    public TMP_Text panelBestTimeText;

    public Color activeColor = Color.white;
    public Color inactiveColor = Color.gray;

    private float remainingTime;
    private float timePerStar;
    private int currentStars = 3;

    private Coroutine sliderCoroutine;
    private bool levelEnded = false;

    void Start()
    {
        remainingTime = levelDuration;
        timePerStar = levelDuration / 3f;

        UpdateStars(3);
        UpdateSliderImmediate();
        UpdateTimerText();

        if (losePanel != null)
            losePanel.SetActive(false);
    }

    void Update()
    {
        if (levelEnded) return;

        remainingTime -= Time.deltaTime;
        remainingTime = Mathf.Max(0f, remainingTime);

        UpdateTimerText();
        UpdatePanelTimeLeft();

        int newStars = Mathf.Clamp(Mathf.CeilToInt(remainingTime / timePerStar), 0, 3);

        if (newStars != currentStars)
            UpdateStars(newStars);

        if (remainingTime <= 0f)
            TriggerLose();
    }

    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);
        timerText.text = $"{minutes:0}:{seconds:00}";
    }

    private void UpdatePanelTimeLeft()
    {
        if (panelTimeLeftText != null)
        {
            int minutes = Mathf.FloorToInt(remainingTime / 60f);
            int seconds = Mathf.FloorToInt(remainingTime % 60f);
            panelTimeLeftText.text = $"Time Left: {minutes:0}:{seconds:00}";
        }
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

    public void SaveStars()
    {
        int previous = PlayerPrefs.GetInt("Level_" + levelIndex, 0);
        if (currentStars > previous)
            PlayerPrefs.SetInt("Level_" + levelIndex, currentStars);

        SaveBestTime();
    }

    public int GetSavedStars()
    {
        return PlayerPrefs.GetInt("Level_" + levelIndex, 0);
    }

    private void SaveBestTime()
    {
        int remainingSeconds = Mathf.CeilToInt(remainingTime);
        int previousBest = PlayerPrefs.GetInt("Level_" + levelIndex + "_BestTime", 0);

        if (remainingSeconds > previousBest)
            PlayerPrefs.SetInt("Level_" + levelIndex + "_BestTime", remainingSeconds);

        UpdatePanelBestTime();
    }

    private void UpdatePanelBestTime()
    {
        if (panelBestTimeText != null)
        {
            int bestSeconds = PlayerPrefs.GetInt("Level_" + levelIndex + "_BestTime", 0);
            int minutes = Mathf.FloorToInt(bestSeconds / 60f);
            int seconds = Mathf.FloorToInt(bestSeconds % 60f);
            panelBestTimeText.text = $"Best Record: {minutes:0}:{seconds:00}";
        }
    }

    private void TriggerLose()
    {
        levelEnded = true;

        if (losePanel != null)
            losePanel.SetActive(true);

        SaveStars();
    }
}