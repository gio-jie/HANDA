using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class StarManagerLevel6 : MonoBehaviour
{
    public static StarManagerLevel6 Instance;

    [Header("Level Info")]
    public int levelIndex;
    public float levelDuration = 90f;
    public float goalMeters;

    [Header("UI")]
    public Image[] stars;
    public Slider starSlider;
    public TMP_Text timerText;
    public Slider meterSlider;
    public TMP_Text meterText;

    [Header("Panels")]
    public GameObject losePanel;
    public GameObject winPanel;
    public GameObject powerUpPanel;

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
    public float baseTravelTime = 25f;
    public int maxWaterHits = 3;

    [Header("Power-Up Settings")]
    public float slowMotionScale = 0.3f;
    public float slowMotionDuration = 1.5f;
    public float highJumpDuration = 5f;
    public float powerUpDistanceBonus = 10f;

    [Header("Player Reference")]
    public Transform player;

    private float startY;

    private float remainingTime;
    private float timePerStar;
    private int currentStars = 3;
    private float travelTimeAccumulated = 0f;
    private float requiredTravelTime;
    private int waterHitCount = 0;
    private bool levelEnded = false;
    private bool highJumpActive = false;

    void Awake() => Instance = this;

    void Start()
    {
        remainingTime = levelDuration;
        timePerStar = levelDuration / 3f;
        requiredTravelTime = baseTravelTime;

        if (meterSlider != null)
        {
            meterSlider.maxValue = goalMeters;
            meterSlider.value = 0;
        }

        UpdateStars(currentStars);
        UpdateTimerText();

        losePanel?.SetActive(false);
        winPanel?.SetActive(false);
        powerUpPanel?.SetActive(false);

        if (player != null)
            startY = player.position.y;
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

   void UpdateMeterProgress()
    {
        if (player == null) return;

        float heightTravelled = player.position.y - startY;

        heightTravelled = Mathf.Max(0f, heightTravelled);

        meterSlider.value = heightTravelled;
        meterText.text = Mathf.FloorToInt(heightTravelled) + "m";

        if (heightTravelled >= goalMeters)
            TriggerWin();
    }

    void UpdateStarCalculation()
    {
        int newStars = Mathf.Clamp(Mathf.CeilToInt(remainingTime / timePerStar), 0, 3);
        if (newStars != currentStars)
            UpdateStars(newStars);
    }

    void UpdateStars(int newCount)
    {
        currentStars = newCount;
        for (int i = 0; i < stars.Length; i++)
            stars[i].color = i < newCount ? activeColor : inactiveColor;

        if (starSlider != null)
        {
            starSlider.value = currentStars switch
            {
                3 => 3f,
                2 => 1.5f,
                1 => 0.5f,
                _ => 0f
            };
        }
    }

    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);
        if (timerText != null) timerText.text = $"{minutes:0}:{seconds:00}";
    }

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

    public void TriggerLose()
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

    #region Power-Ups
    public void ActivatePowerUp()
    {
        if (levelEnded) return;
        StartCoroutine(PowerUpDistanceRoutine());
    }

    private IEnumerator PowerUpDistanceRoutine()
    {
        if (levelEnded) yield break;

        yield return AnimatePowerUpPanel();

        Time.timeScale = slowMotionScale;
        yield return new WaitForSecondsRealtime(slowMotionDuration);
        Time.timeScale = 1f;
        powerUpPanel.SetActive(false);

        PlayerBounce player = FindObjectOfType<PlayerBounce>();
        if (player != null)
            player.StartPowerUp();

        startY -= powerUpDistanceBonus;
    }

    private IEnumerator AnimatePowerUpPanel()
    {
        powerUpPanel.SetActive(true);
        RectTransform panel = powerUpPanel.GetComponent<RectTransform>();

        float duration = 0.4f;
        float timer = 0f;
        Vector3 overshoot = Vector3.one * 1.15f;

        while (timer < duration)
        {
            float t = timer / duration;
            float scale = Mathf.Lerp(0f, 1.15f, Mathf.Sin(t * Mathf.PI * 0.5f));
            panel.localScale = Vector3.one * scale;
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        panel.localScale = overshoot;

        float bounceDuration = 0.15f;
        timer = 0f;
        while (timer < bounceDuration)
        {
            float t = timer / bounceDuration;
            panel.localScale = Vector3.Lerp(overshoot, Vector3.one, t);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        panel.localScale = Vector3.one;
    }

    public void ActivateHighJump()
    {
        if (levelEnded) return;
        StartCoroutine(HighJumpRoutine());
    }

    private IEnumerator HighJumpRoutine()
    {
        highJumpActive = true;
        yield return new WaitForSeconds(highJumpDuration);
        highJumpActive = false;
    }

    public bool IsHighJumpActive() => highJumpActive;
    #endregion

    #region Penalties
    public void ApplyPenalty(float seconds)
    {
        if (levelEnded) return;
        requiredTravelTime += seconds;
        remainingTime -= seconds;
        remainingTime = Mathf.Max(0f, remainingTime);
        UpdateTimerText();
        UpdateStarCalculation();
    }

    public void RegisterWaterHit()
    {
        if (levelEnded) return;
        waterHitCount++;
        if (waterHitCount >= maxWaterHits) TriggerLose();
    }
    #endregion

    #region Save
    void SaveStars()
    {
        int prev = PlayerPrefs.GetInt("Level_" + levelIndex, 0);
        if (currentStars > prev) PlayerPrefs.SetInt("Level_" + levelIndex, currentStars);
        PlayerPrefs.Save();
    }

    void SaveBestTime()
    {
        int remainingSec = Mathf.CeilToInt(remainingTime);
        int prevBest = PlayerPrefs.GetInt("Level_" + levelIndex + "_BestTime", 0);
        if (remainingSec > prevBest) PlayerPrefs.SetInt("Level_" + levelIndex + "_BestTime", remainingSec);
        PlayerPrefs.Save();
    }
    #endregion

    public int GetCurrentStars() => currentStars;
    public int GetRemainingSeconds() => Mathf.CeilToInt(remainingTime);
    public int GetSavedStars() => PlayerPrefs.GetInt("Level_" + levelIndex, 0);
    public int GetBestTime() => PlayerPrefs.GetInt("Level_" + levelIndex + "_BestTime", 0);
}