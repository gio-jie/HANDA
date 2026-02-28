using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Level9Part2Manager : MonoBehaviour
{
    public static Level9Part2Manager instance;

    // ==========================================
    // --- BAGONG DAGDAG: MALINIS NA BAHAY TRANSITION ---
    // ==========================================
    [Header("Clean House Transition")]
    public Image backgroundImage; // Ang mismong Background natin
    public Sprite cleanBackgroundSprite; // Ang malinis na version ng bahay
    public AudioClip cleanSoundEffect; // Tunog na "Ting!" o kislap
    // ==========================================

    [Header("Player Status")]
    public bool isWearingBoots = false;
    public Image jobertImage; 
    public Sprite jobertWithBootsSprite; 

    [Header("Infection Meter (Parusa)")]
    public float currentInfection = 0f;      
    private float displayedInfection = 0f;   
    public float maxInfection = 100f;
    public Image infectionBarFill;
    public float infectionPenalty = 25f;
    public float barFillSpeed = 30f;         

    [Header("Game Timer (Countdown Lang)")]
    public float timeLimit = 60f; 
    public int totalHazards = 3; 
    [HideInInspector] public int clearedHazards = 0;

    [Header("UI Feedback")]
    public TMP_Text statusText;
    public TMP_Text timerTextUI;
    public TMP_Text scoreTextUI; 

    private Color originalTimerColor;

    [Header("Star System")]
    public float goldStarThreshold = 30f; 
    public float silverStarThreshold = 15f; 

    [Header("UI Panels")]
    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject pausePanel;

    [Header("Win Panel Elements")]
    public Image star1; public Image star2; public Image star3;
    public TMP_Text timeFinishedText;
    public Color earnedColor = Color.yellow;

    [HideInInspector] public bool isGameActive = true;

    void Awake()
    {
        instance = this;
        Time.timeScale = 1;

        if (timerTextUI != null)
        {
            originalTimerColor = timerTextUI.color; 
        }
    }

    void Start()
    {
        UpdateScoreDisplay();
    }

    void Update()
    {
        if (!isGameActive) return;

        if (timeLimit > 0)
        {
            timeLimit -= Time.deltaTime;
            UpdateTimerDisplay(timeLimit);
            if (timeLimit <= 0) FinalizeGameOver("The time is over.");
        }

        if (displayedInfection < currentInfection)
        {
            displayedInfection = Mathf.MoveTowards(displayedInfection, currentInfection, barFillSpeed * Time.deltaTime);
        }

        if (infectionBarFill != null)
        {
            infectionBarFill.fillAmount = displayedInfection / maxInfection;
            
            if (displayedInfection > 70f) infectionBarFill.color = Color.red;
            else if (displayedInfection > 40f) infectionBarFill.color = Color.yellow;
            else infectionBarFill.color = Color.green;
        }

        if (displayedInfection >= maxInfection) 
        {
            FinalizeGameOver("Jobert has been infected!");
        }
    }

    void UpdateTimerDisplay(float time)
    {
        if (timerTextUI != null)
        {
            if (time < 0) time = 0;

            int minutes = Mathf.FloorToInt(time / 60); 
            int seconds = Mathf.FloorToInt(time % 60); 

            timerTextUI.text = string.Format("<mspace=0.6em>{0:00}:{1:00}</mspace>", minutes, seconds);
            timerTextUI.color = (time <= 10) ? Color.red : originalTimerColor;
        }
    }

    void UpdateScoreDisplay()
    {
        if (scoreTextUI != null)
        {
            scoreTextUI.text = clearedHazards + " / " + totalHazards;
        }
    }

    public void WearBoots()
    {
        isWearingBoots = true;
        
        if (jobertImage != null && jobertWithBootsSprite != null)
        {
            jobertImage.sprite = jobertWithBootsSprite;
            jobertImage.SetNativeSize(); 
        }

        if (statusText != null) statusText.text = "It's safe now! We can begin cleaning.";
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.correctSound);
    }

    public void HazardCleaned()
    {
        clearedHazards++;
        UpdateScoreDisplay(); 

        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.correctSound);

        if (clearedHazards >= totalHazards)
        {
            isGameActive = false;
            
            // --- BINAGO: PALITAN ANG TEXT PARA MAG-MATCH SA MALINIS NA BAHAY ---
            if (statusText != null) statusText.text = "Malinis at ligtas na ang bahay!";
            
            // --- BAGONG DAGDAG: PALITAN ANG BACKGROUND AT TUMUNOG ---
            if (backgroundImage != null && cleanBackgroundSprite != null)
            {
                backgroundImage.sprite = cleanBackgroundSprite;
            }

            if (cleanSoundEffect != null)
            {
                AudioSource.PlayClipAtPoint(cleanSoundEffect, Camera.main.transform.position, 1f);
            }
            // ---------------------------------------------------------

            Invoke("ShowWinScreen", 3.0f); 
        }
    }

    public void AddInfection(string warningMessage)
    {
        if (!isGameActive) return;

        currentInfection += infectionPenalty; 
        
        if (statusText != null) statusText.text = warningMessage;
        
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.wrongSound);
        if (PlayerPrefs.GetInt("VibrationOn", 1) == 1) Handheld.Vibrate();
    }

    void FinalizeGameOver(string reason)
    {
        timeLimit = 0;
        isGameActive = false;

        if (timerTextUI != null) timerTextUI.text = "00:00";

        if (statusText != null) statusText.text = "GAME OVER: " + reason;
        if (losePanel != null) losePanel.SetActive(true);
        if (AudioManager.instance != null) { AudioManager.instance.PlaySFX(AudioManager.instance.loseSound); AudioManager.instance.PauseBGM(); }
    }

    void ShowWinScreen()
    {
        if (winPanel != null) winPanel.SetActive(true);
        if (AudioManager.instance != null) { AudioManager.instance.PlaySFX(AudioManager.instance.winSound); AudioManager.instance.PauseBGM(); }

        float scoreTime = timeLimit;
        if (star1) star1.color = earnedColor;
        if (scoreTime >= silverStarThreshold && star2) star2.color = earnedColor;
        if (scoreTime >= goldStarThreshold && star3) star3.color = earnedColor;

        float min = Mathf.FloorToInt(scoreTime / 60); float sec = Mathf.FloorToInt(scoreTime % 60);
        if (timeFinishedText != null) timeFinishedText.text = string.Format("Time Left: {0:00}:{1:00}", min, sec);

        PlayerPrefs.SetInt("Level10_Unlocked", 1);
        PlayerPrefs.Save();
    }

    public void RetryLevel() { Time.timeScale = 1; SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void PauseGame() { if (pausePanel != null) pausePanel.SetActive(true); Time.timeScale = 0; }
    public void ResumeGame() { if (pausePanel != null) pausePanel.SetActive(false); Time.timeScale = 1; }
    public void QuitToLevelSelect() { Time.timeScale = 1; SceneManager.LoadScene("TyphoonLevelSelect"); }
}