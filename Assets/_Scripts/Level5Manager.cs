using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Level5Manager : MonoBehaviour
{
    [Header("Game Settings")]
    public float timeLimit = 60f;
    public float penaltyTime = 10f; // Bawas oras pag bumunggo

    [Header("Star System")]
    public float goldStarThreshold = 40f; 
    public float silverStarThreshold = 20f; 

    [Header("Game State")]
    private bool isGameActive = true;
    private float finalTimeRecorded = 0f;

    [Header("UI Panels")]
    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject pausePanel;
    public GameObject hazardPanel; // Yung "DANGER" warning

    [Header("Win Panel Elements")]
    public Image star1;
    public Image star2;
    public Image star3;
    public TMP_Text timeFinishedText; 
    public TMP_Text bestScoreText;
    
    [Header("Lose Panel Elements")]
    public Image loseStar1;
    public Image loseStar2;
    public Image loseStar3;
    public TMP_Text loseTimeText;
    public TMP_Text loseBestScoreText;

    public Color earnedColor = Color.yellow;
    public Color missingColor = Color.gray;

    [Header("In-Game UI")]
    public TMP_Text timerText;

    [Header("Toggle Buttons")]
    public Image soundButtonImage; 
    public Image musicButtonImage;
    public Color onColor = Color.green;
    public Color offColor = Color.gray;
    private bool isSoundOn = true;
    private bool isMusicOn = true;

    void Awake()
    {
        Time.timeScale = 1;
    }

    void Start()
    {
        UpdateToggleVisuals();
        
        if (AudioManager.instance != null) AudioManager.instance.ResumeBGM();
    }

    void Update()
    {
        if (isGameActive)
        {
            if (timeLimit > 0)
            {
                timeLimit -= Time.deltaTime;
                UpdateTimerDisplay(timeLimit);
            }
            else
            {
                FinalizeGameOver();
            }
        }
    }

    void UpdateTimerDisplay(float timeToShow)
    {
        if(timerText != null)
        {
            int seconds = Mathf.FloorToInt(timeToShow);
            int milliseconds = Mathf.FloorToInt((timeToShow * 100) % 100);
            timerText.text = string.Format("<mspace=0.6em>{0:00}:{1:00}</mspace>", seconds, milliseconds);

            if(timeToShow <= 10) timerText.color = Color.red;
        }
    }

    void FinalizeGameOver()
    {
        timeLimit = 0;
        isGameActive = false;
        if(timerText != null) timerText.text = "00:00";
        GameOver();
    }

    // --- LEVEL 5 MECHANICS ---

    public void HitHazard()
    {
        if (!isGameActive) return;

        // Penalty
        timeLimit -= penaltyTime;

        if (PlayerPrefs.GetInt("VibrationOn", 1) == 1)
        {
            // Ito ang utos para yumugyog ang phone
            Handheld.Vibrate(); 
            Debug.Log("Brrrzt! Vibrate dahil mali ang item."); // Para makita mo sa Console
        }
        
        // Sound
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.warningSound);

        // Show Warning
        if (hazardPanel != null)
        {
            hazardPanel.SetActive(true);
            Time.timeScale = 0; // Pause game
            if (AudioManager.instance != null) AudioManager.instance.PauseBGM();
        }

        if (timeLimit <= 0) FinalizeGameOver();
    }

    public void CloseWarning()
    {
        if (hazardPanel != null) hazardPanel.SetActive(false);
        Time.timeScale = 1; 
        if (AudioManager.instance != null) AudioManager.instance.ResumeBGM();
    }

    public void ReachGoal()
    {
        // WIN!
        isGameActive = false;
        finalTimeRecorded = timeLimit;
        UpdateTimerDisplay(finalTimeRecorded);
        Invoke("ShowWinScreen", 1f);
    }

    // --- WIN/LOSE LOGIC ---

    void ShowWinScreen()
    {
        winPanel.SetActive(true);
        if (AudioManager.instance != null) 
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.winSound);
            AudioManager.instance.PauseBGM(); // Ngayon safe na ito!
        }

        float scoreTime = finalTimeRecorded; 

        // Star Logic
        if(star1) star1.color = earnedColor;
        if(star2) star2.color = (scoreTime >= silverStarThreshold) ? earnedColor : missingColor;
        if(star3) star3.color = (scoreTime >= goldStarThreshold) ? earnedColor : missingColor;

        // Time Text
        int seconds = Mathf.FloorToInt(scoreTime);
        int milliseconds = Mathf.FloorToInt((scoreTime * 100) % 100);
        if(timeFinishedText != null) timeFinishedText.text = string.Format("Time Left: {0:00}.{1:00}s", seconds, milliseconds);

        // High Score (Level 5 Key)
        float currentBest = PlayerPrefs.GetFloat("Level5_BestTime", 0);

        if (scoreTime > currentBest)
        {
            currentBest = scoreTime;
            PlayerPrefs.SetFloat("Level5_BestTime", currentBest);
            PlayerPrefs.Save();
            if(bestScoreText != null) { bestScoreText.text = "NEW BEST RECORD!"; bestScoreText.color = Color.yellow; }
        }
        else
        {
            int bestSec = Mathf.FloorToInt(currentBest);
            int bestMs = Mathf.FloorToInt((currentBest * 100) % 100);
            if(bestScoreText != null) { bestScoreText.text = string.Format("Best Record: {0:00}.{1:00}s", bestSec, bestMs); bestScoreText.color = Color.white; }
        }

        // --- SAVE SYSTEM FOR LEVEL 5 ---
        
        int starsEarned = 1; 
        if (scoreTime >= silverStarThreshold) starsEarned = 2;
        if (scoreTime >= goldStarThreshold) starsEarned = 3;

        int currentSavedStars = PlayerPrefs.GetInt("Level5_Stars", 0);
        if (starsEarned > currentSavedStars)
        {
            PlayerPrefs.SetInt("Level5_Stars", starsEarned);
        }

        PlayerPrefs.SetInt("Level6_Unlocked", 1);
        
        PlayerPrefs.Save();
    }

    void GameOver()
    {
        isGameActive = false;
        if(losePanel != null) losePanel.SetActive(true);
        
        if (AudioManager.instance != null) 
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.loseSound);
            AudioManager.instance.PauseBGM(); // Ngayon safe na ito!
        }

        // Lose Panel UI Update
        if(loseTimeText != null) loseTimeText.text = "Time Left: 00:00";
        if(loseStar1) loseStar1.color = missingColor;
        if(loseStar2) loseStar2.color = missingColor;
        if(loseStar3) loseStar3.color = missingColor;

        float currentBest = PlayerPrefs.GetFloat("Level5_BestTime", 0);
        int bestSec = Mathf.FloorToInt(currentBest);
        int bestMs = Mathf.FloorToInt((currentBest * 100) % 100);
        if(loseBestScoreText != null) loseBestScoreText.text = string.Format("Best Record: {0:00}.{1:00}s", bestSec, bestMs);
    }

    // --- BUTTONS ---
    public void RetryLevel() { if (AudioManager.instance != null) AudioManager.instance.ResumeBGM(); SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void QuitToLevelSelect() { if (AudioManager.instance != null) AudioManager.instance.ResumeBGM(); SceneManager.LoadScene("TyphoonLevelSelect"); }
    public void PauseGame() { pausePanel.SetActive(true); Time.timeScale = 0; if (AudioManager.instance != null) AudioManager.instance.PauseBGM(); }
    public void ResumeGame() { pausePanel.SetActive(false); Time.timeScale = 1; if (AudioManager.instance != null) AudioManager.instance.ResumeBGM(); }
    
    public void ToggleSound() { isSoundOn = !isSoundOn; AudioListener.volume = isSoundOn ? 1 : 0; UpdateToggleVisuals(); }
    public void ToggleMusic() { isMusicOn = !isMusicOn; UpdateToggleVisuals(); }
    void UpdateToggleVisuals() { if(soundButtonImage) soundButtonImage.color = isSoundOn ? onColor : offColor; if(musicButtonImage) musicButtonImage.color = isMusicOn ? onColor : offColor; }
}