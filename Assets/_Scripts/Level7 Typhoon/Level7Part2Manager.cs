using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using System.Collections;
using UnityEngine.SceneManagement; 

public class Level7Part2Manager : MonoBehaviour
{
    public static Level7Part2Manager instance; 

    [Header("Game Settings")]
    public int itemsNeeded = 3; // Kulambo, Takip ng Drum, Basura
    public float timeLimit = 45f; 
    public float penaltyTime = 5f; 

    [Header("Star System")]
    public float goldStarThreshold = 25f; 
    public float silverStarThreshold = 10f; 

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
    
    [Header("Lose Panel Elements")]
    public Image loseStar1;
    public Image loseStar2;
    public Image loseStar3;
    public TMP_Text loseTimeText;
    public TMP_Text loseBestScoreText;

    public Color earnedColor = Color.yellow;
    public Color missingColor = Color.gray;

    [Header("In-Game UI")]
    public TMP_Text scoreTextUI; 
    public TMP_Text timerTextUI; 
    public GameObject checkIcon; 
    public GameObject xIcon;

    void Awake()
    {
        instance = this; 
        Time.timeScale = 1;
    }

    void Start()
    {
        UpdateScoreDisplay();
        isGameActive = true;
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
        if (timerTextUI != null)
        {
            if (timeToShow < 0) timeToShow = 0;
            float seconds = Mathf.FloorToInt(timeToShow);
            timerTextUI.text = string.Format("<mspace=0.6em>{0:00}</mspace>", seconds);

            if (timeToShow <= 10) timerTextUI.color = Color.red;
            else timerTextUI.color = Color.white;
        }
    }    

    // --- GAMEPLAY LOGIC ---

    public void AddScore()
    {
        if (!isGameActive) return; 

        currentScore++;
        UpdateScoreDisplay();
        StartCoroutine(ShowFeedback(checkIcon));

        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.correctSound);

        if (currentScore >= itemsNeeded)
        {
            isGameActive = false; 
            finalTimeRecorded = timeLimit; 
            UpdateTimerDisplay(finalTimeRecorded);
            Invoke("ShowWinScreen", 1.5f);
        }
    }

    public void WrongItem() 
    { 
        if (!isGameActive) return; 

        StartCoroutine(ShowFeedback(xIcon)); 
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.wrongSound);
        if (PlayerPrefs.GetInt("VibrationOn", 1) == 1) Handheld.Vibrate(); 
        
        timeLimit -= penaltyTime; // Bawas oras!
        if (timeLimit <= 0) FinalizeGameOver(); 
        UpdateTimerDisplay(timeLimit);
    }

    IEnumerator ShowFeedback(GameObject icon) { if(icon) { icon.SetActive(true); yield return new WaitForSeconds(1.0f); icon.SetActive(false); } }

    void UpdateScoreDisplay() { if(scoreTextUI != null) scoreTextUI.text = currentScore + " / " + itemsNeeded; }

    // --- WIN / LOSE LOGIC ---

    void FinalizeGameOver()
    {
        timeLimit = 0;
        isGameActive = false;
        if(timerTextUI != null) timerTextUI.text = "00";
        
        if(losePanel != null) losePanel.SetActive(true); 
        if (AudioManager.instance != null) { AudioManager.instance.PlaySFX(AudioManager.instance.loseSound); AudioManager.instance.PauseBGM(); }
        
        if(loseTimeText != null) loseTimeText.text = "Time Left: 00:00";
        if(loseStar1) loseStar1.color = missingColor; if(loseStar2) loseStar2.color = missingColor; if(loseStar3) loseStar3.color = missingColor;
    }

    void ShowWinScreen()
    {
        winPanel.SetActive(true);
        if (AudioManager.instance != null) { AudioManager.instance.PlaySFX(AudioManager.instance.winSound); AudioManager.instance.PauseBGM(); }

        float scoreTime = finalTimeRecorded; 

        if(star1) star1.color = missingColor; if(star2) star2.color = missingColor; if(star3) star3.color = missingColor;
        if(star1) star1.color = earnedColor;
        if (scoreTime >= silverStarThreshold && star2) star2.color = earnedColor;
        if (scoreTime >= goldStarThreshold && star3) star3.color = earnedColor;

        float min = Mathf.FloorToInt(scoreTime / 60); float sec = Mathf.FloorToInt(scoreTime % 60);
        if(timeFinishedText != null) timeFinishedText.text = string.Format("Time Left: {0:00}:{1:00}", min, sec);

        // SAVE RECORD & UNLOCK NEXT LEVEL
        float currentBest = PlayerPrefs.GetFloat("Level7Part2_BestTime", 0);
        if (scoreTime > currentBest) { PlayerPrefs.SetFloat("Level7Part2_BestTime", scoreTime); }
        PlayerPrefs.SetInt("Level8_Unlocked", 1);
        PlayerPrefs.Save();
    }

    // --- BUTTONS ---
    public void RetryLevel() { Time.timeScale = 1; SceneManager.LoadScene("Level7_Dengue"); } // Balik sa tap-tap lamok pag umulit!
    public void PauseGame() { pausePanel.SetActive(true); Time.timeScale = 0; }
    public void ResumeGame() { pausePanel.SetActive(false); Time.timeScale = 1; }
    public void QuitToLevelSelect() { Time.timeScale = 1; SceneManager.LoadScene("TyphoonLevelSelect"); }
}