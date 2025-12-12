using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using System.Collections;
using UnityEngine.SceneManagement; 

public class Level1Manager : MonoBehaviour
{
    [Header("Game Settings")]
    public int itemsNeeded = 7;
    public float timeLimit = 60f; 
    public float penaltyTime = 5f; 

    [Header("Star System")]
    public float goldStarThreshold = 40f; 
    public float silverStarThreshold = 20f; 

    [Header("Data (Do not edit)")]
    public int currentScore = 0;
    private bool isGameActive = true; 
    private float finalTimeRecorded = 0f; 

    [Header("UI Panels")]
    public GameObject winPanel;
    public GameObject losePanel; 
    public GameObject pausePanel;
    
    [Header("Win Panel Elements")]
    public Image star1;
    public Image star2;
    public Image star3;
    public TMP_Text timeFinishedText; 
    public TMP_Text bestScoreText;    
    
    // --- ITO ANG BAGO: LOSE PANEL ELEMENTS ---
    [Header("Lose Panel Elements")]
    public Image loseStar1;
    public Image loseStar2;
    public Image loseStar3;
    public TMP_Text loseTimeText;
    public TMP_Text loseBestScoreText;
    // -----------------------------------------

    public Color earnedColor = Color.yellow;
    public Color missingColor = Color.gray;

    [Header("In-Game UI")]
    public TMP_Text scoreTextUI;
    public TMP_Text timerTextUI; 
    public GameObject checkIcon;
    public GameObject xIcon;

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
        UpdateScoreDisplay();
        UpdateToggleVisuals();
        isGameActive = true;

        if (AudioManager.instance != null) 
        {
            AudioManager.instance.ResumeBGM();
        }
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
        if(timerTextUI != null)
        {
            int seconds = Mathf.FloorToInt(timeToShow);
            int milliseconds = Mathf.FloorToInt((timeToShow * 100) % 100);
            timerTextUI.text = string.Format("<mspace=0.6em>{0:00}:{1:00}</mspace>", seconds, milliseconds);

            if(timeToShow <= 10) timerTextUI.color = Color.red;
        }
    }

    void FinalizeGameOver()
    {
        timeLimit = 0;
        isGameActive = false;
        if(timerTextUI != null) timerTextUI.text = "00:00";
        GameOver(); 
    }

    // --- GAME OVER LOGIC (UPDATED) ---
    public void GameOver()
    {
        isGameActive = false; 
        if(losePanel != null) losePanel.SetActive(true); 

        // Play Sound
        if (AudioManager.instance != null) 
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.loseSound);
            AudioManager.instance.PauseBGM(); // Ngayon safe na ito!
        }
        // 1. Set Time to 00:00 (Kasi natalo)
        if(loseTimeText != null) loseTimeText.text = "Time Left: 00:00";

        // 2. Set ALL Stars to Gray (Kasi talo)
        if(loseStar1) loseStar1.color = missingColor;
        if(loseStar2) loseStar2.color = missingColor;
        if(loseStar3) loseStar3.color = missingColor;

        // 3. Show Best Record (Kinuha sa Memory)
        float currentBest = PlayerPrefs.GetFloat("Level1_BestTime", 0);
        int bestSec = Mathf.FloorToInt(currentBest);
        int bestMs = Mathf.FloorToInt((currentBest * 100) % 100);

        if(loseBestScoreText != null)
        {
            loseBestScoreText.text = string.Format("Best Record: {0:00}.{1:00}s", bestSec, bestMs);
        }
    }

    // --- WIN LOGIC ---
    public void AddScore()
    {
        if (!isGameActive) return; 

        currentScore++;
        UpdateScoreDisplay();
        StartCoroutine(ShowFeedback(checkIcon));

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.correctSound);
        }

        if (currentScore >= itemsNeeded)
        {
            isGameActive = false; 
            finalTimeRecorded = timeLimit; 
            UpdateTimerDisplay(finalTimeRecorded);
            Invoke("ShowWinScreen", 1.5f);
        }
    }

    void ShowWinScreen()
    {
        winPanel.SetActive(true);
        if (AudioManager.instance != null) 
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.winSound);
            AudioManager.instance.PauseBGM(); // Ngayon safe na ito!
        }

        float scoreTime = finalTimeRecorded; 

        if(star1) star1.color = missingColor;
        if(star2) star2.color = missingColor;
        if(star3) star3.color = missingColor;

        if(star1) star1.color = earnedColor;
        if (scoreTime >= silverStarThreshold && star2) star2.color = earnedColor;
        if (scoreTime >= goldStarThreshold && star3) star3.color = earnedColor;

        int seconds = Mathf.FloorToInt(scoreTime);
        int milliseconds = Mathf.FloorToInt((scoreTime * 100) % 100);
        
        if(timeFinishedText != null)
            timeFinishedText.text = string.Format("Time Left: {0:00}.{1:00}s", seconds, milliseconds);

        float currentBest = PlayerPrefs.GetFloat("Level1_BestTime", 0);

        if (scoreTime > currentBest)
        {
            currentBest = scoreTime;
            PlayerPrefs.SetFloat("Level1_BestTime", currentBest);
            PlayerPrefs.Save();
            
            if(bestScoreText != null)
            {
                bestScoreText.text = "NEW BEST RECORD!";
                bestScoreText.color = Color.yellow;
            }
        }
        else
        {
            int bestSec = Mathf.FloorToInt(currentBest);
            int bestMs = Mathf.FloorToInt((currentBest * 100) % 100);
            
            if(bestScoreText != null)
            {
                bestScoreText.text = string.Format("Best Record: {0:00}.{1:00}s", bestSec, bestMs);
                bestScoreText.color = Color.white;
            }
        }

        // 1. Compute Stars Earned
        int starsEarned = 1; // Automatic 1 star pag nanalo
        if (scoreTime >= silverStarThreshold) starsEarned = 2;
        if (scoreTime >= goldStarThreshold) starsEarned = 3;

        // 2. Save Best Star Record (Para sa Level Selection)
        // Note: "Level1_Stars" ang key para sa Level 1
        int currentSavedStars = PlayerPrefs.GetInt("Level1_Stars", 0);
        if (starsEarned > currentSavedStars)
        {
            PlayerPrefs.SetInt("Level1_Stars", starsEarned);
        }

        // 3. Unlock Next Level (Level 2)
        // "Level2_Unlocked" = 1 means BUKAS NA.
        PlayerPrefs.SetInt("Level2_Unlocked", 1);
        
        PlayerPrefs.Save();
    }

    // ... (REST OF THE FUNCTIONS: Copy these exactly as before) ...
    
    void UpdateScoreDisplay() { if(scoreTextUI != null) scoreTextUI.text = "Items: " + currentScore + "/" + itemsNeeded; }
    public void WrongItem() 
    { 
        if (!isGameActive) return; 
        
        // Show Visual Feedback
        StartCoroutine(ShowFeedback(xIcon)); 

        // Play Sound (Huwag i-pause ang music!)
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.wrongSound);
            // TINANGGAL NATIN YUNG "PauseBGM" DITO PARA TULOY-TULOY ANG TUGTOG
        }

        if (PlayerPrefs.GetInt("VibrationOn", 1) == 1)
        {
            // Ito ang utos para yumugyog ang phone
            Handheld.Vibrate(); 
            Debug.Log("Brrrzt! Vibrate dahil mali ang item."); // Para makita mo sa Console
        }

        // Penalty Logic
        timeLimit -= penaltyTime; 
        
        if (timeLimit <= 0) 
        {
            FinalizeGameOver(); 
        }
    }
    IEnumerator ShowFeedback(GameObject icon) { if(icon) { icon.SetActive(true); yield return new WaitForSeconds(1.0f); icon.SetActive(false); } }
    public void RetryLevel()
    {
        // --- DAGDAG MO ITO: Resume bago mag-reload ---
        if (AudioManager.instance != null) 
        {
            AudioManager.instance.ResumeBGM();
        }
        // ---------------------------------------------

        Time.timeScale = 1; // Siguraduhing umaandar ang oras
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void PauseGame()
    {
        pausePanel.SetActive(true); 
        Time.timeScale = 0; 

        if (AudioManager.instance != null) AudioManager.instance.PauseBGM();
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false); 
        Time.timeScale = 1; 
        
        if (AudioManager.instance != null) AudioManager.instance.ResumeBGM();
    }
    public void QuitToLevelSelect()
    {
        // --- DAGDAG MO ITO: Resume bago bumalik sa menu ---
        if (AudioManager.instance != null) 
        {
            AudioManager.instance.ResumeBGM();
        }
        // --------------------------------------------------

        Time.timeScale = 1; 
        SceneManager.LoadScene("TyphoonLevelSelect");
    }
    public void ToggleSound() { isSoundOn = !isSoundOn; AudioListener.volume = isSoundOn ? 1 : 0; UpdateToggleVisuals(); }
    public void ToggleMusic() { isMusicOn = !isMusicOn; UpdateToggleVisuals(); }
    void UpdateToggleVisuals() { 
        if(soundButtonImage) soundButtonImage.color = isSoundOn ? onColor : offColor; 
        if(musicButtonImage) musicButtonImage.color = isMusicOn ? onColor : offColor; 
    }
}