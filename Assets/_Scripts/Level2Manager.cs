using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; 

public class Level2Manager : MonoBehaviour
{
    [Header("Game Settings")]
    public int totalTasks = 5; 
    private int tasksDone = 0;
    public float timeLimit = 60f;
    public float penaltyTime = 5f;

    [Header("Star System")]
    public float goldStarThreshold = 40f; 
    public float silverStarThreshold = 20f; 

    [Header("Game State")]
    public string currentSelectedTool = "None";
    private bool isGameActive = true;
    private float finalTimeRecorded = 0f;

    [Header("UI Panels")]
    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject pausePanel;
    public GameObject hazardPanel; // (Optional kung meron)

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
    public TMP_Text scoreText;
    public TMP_Text timerText;
    public GameObject xIcon; // Penalty Feedback

    [Header("Tools Setup")]
    public GameObject[] toolButtons; 

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
        if(scoreText != null) scoreText.text = "Repairs: 0/" + totalTasks;
        
        // Resume Music
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

    // --- LEVEL 2 SPECIFIC LOGIC (Repair) ---

    public void SelectTool(GameObject clickedButton)
    {
        if (!isGameActive) return;

        string toolName = "";
        if (clickedButton.name.Contains("Hammer")) toolName = "Hammer";
        else if (clickedButton.name.Contains("Tape")) toolName = "Tape";
        else if (clickedButton.name.Contains("Phone")) toolName = "Phone";
        else if (clickedButton.name.Contains("Toy")) toolName = "Toy";
        else if (clickedButton.name.Contains("Remote")) toolName = "Remote";

        currentSelectedTool = toolName;
        
        // Visual Reset
        foreach (GameObject btn in toolButtons)
        {
            if (btn != null)
            {
                btn.transform.localScale = Vector3.one;
                Outline outline = btn.GetComponent<Outline>();
                if (outline != null) outline.enabled = false;
            }
        }

        // Visual Highlight
        clickedButton.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        Outline clickedOutline = clickedButton.GetComponent<Outline>();
        if (clickedOutline != null) clickedOutline.enabled = true;
        
        // Play Sound
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.clickSound);
    }

    public void TaskCompleted()
    {
        if (!isGameActive) return;

        tasksDone++;
        if(scoreText != null) scoreText.text = "Repairs: " + tasksDone + "/" + totalTasks;

        // Play Correct Sound
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.correctSound);

        if (tasksDone >= totalTasks)
        {
            // WIN!
            isGameActive = false;
            finalTimeRecorded = timeLimit;
            UpdateTimerDisplay(finalTimeRecorded);
            Invoke("ShowWinScreen", 1.0f);
        }
    }

    public void ApplyPenalty()
    {
        if (!isGameActive) return;
        
        // Play Wrong Sound
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.wrongSound);

        timeLimit -= penaltyTime; 
        
        if(xIcon != null)
        {
            xIcon.SetActive(true);
            Invoke("HideX", 1f);
        }

        if (timeLimit <= 0) FinalizeGameOver();
    }

    void HideX() { if(xIcon != null) xIcon.SetActive(false); }

    // --- WIN/LOSE LOGIC (Copied & Adapted from Level 1) ---

    void ShowWinScreen()
    {
        winPanel.SetActive(true);
        if (AudioManager.instance != null) 
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.winSound);
            AudioManager.instance.PauseBGM();
        }

        float scoreTime = finalTimeRecorded; 

        // Star Logic
        if(star1) star1.color = earnedColor; // Star 1 always
        if(star2) star2.color = (scoreTime >= silverStarThreshold) ? earnedColor : missingColor;
        if(star3) star3.color = (scoreTime >= goldStarThreshold) ? earnedColor : missingColor;

        // Time Text
        int seconds = Mathf.FloorToInt(scoreTime);
        int milliseconds = Mathf.FloorToInt((scoreTime * 100) % 100);
        if(timeFinishedText != null) timeFinishedText.text = string.Format("Time Left: {0:00}.{1:00}s", seconds, milliseconds);

        // High Score (Note: Gamit tayo ng ibang Key "Level2_BestTime")
        float currentBest = PlayerPrefs.GetFloat("Level2_BestTime", 0);

        if (scoreTime > currentBest)
        {
            currentBest = scoreTime;
            PlayerPrefs.SetFloat("Level2_BestTime", currentBest);
            PlayerPrefs.Save();
            if(bestScoreText != null) { bestScoreText.text = "NEW BEST RECORD!"; bestScoreText.color = Color.yellow; }
        }
        else
        {
            int bestSec = Mathf.FloorToInt(currentBest);
            int bestMs = Mathf.FloorToInt((currentBest * 100) % 100);
            if(bestScoreText != null) { bestScoreText.text = string.Format("Best Record: {0:00}.{1:00}s", bestSec, bestMs); bestScoreText.color = Color.white; }
        }
    }

    void GameOver()
    {
        isGameActive = false;
        if(losePanel != null) losePanel.SetActive(true);
        
        if (AudioManager.instance != null) 
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.loseSound);
            AudioManager.instance.PauseBGM();
        }

        // Lose Panel UI Update
        if(loseTimeText != null) loseTimeText.text = "Time Left: 00:00";
        if(loseStar1) loseStar1.color = missingColor;
        if(loseStar2) loseStar2.color = missingColor;
        if(loseStar3) loseStar3.color = missingColor;

        float currentBest = PlayerPrefs.GetFloat("Level2_BestTime", 0);
        int bestSec = Mathf.FloorToInt(currentBest);
        int bestMs = Mathf.FloorToInt((currentBest * 100) % 100);
        if(loseBestScoreText != null) loseBestScoreText.text = string.Format("Best Record: {0:00}.{1:00}s", bestSec, bestMs);
    }

    // --- STANDARD BUTTONS ---
    public void RetryLevel() { if (AudioManager.instance != null) AudioManager.instance.ResumeBGM(); SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void QuitToLevelSelect() { if (AudioManager.instance != null) AudioManager.instance.ResumeBGM(); SceneManager.LoadScene("TyphoonLevelSelect"); }
    public void PauseGame() { pausePanel.SetActive(true); Time.timeScale = 0; if (AudioManager.instance != null) AudioManager.instance.PauseBGM(); }
    public void ResumeGame() { pausePanel.SetActive(false); Time.timeScale = 1; if (AudioManager.instance != null) AudioManager.instance.ResumeBGM(); }
    
    public void ToggleSound() { isSoundOn = !isSoundOn; AudioListener.volume = isSoundOn ? 1 : 0; UpdateToggleVisuals(); }
    public void ToggleMusic() { isMusicOn = !isMusicOn; UpdateToggleVisuals(); }
    void UpdateToggleVisuals() { if(soundButtonImage) soundButtonImage.color = isSoundOn ? onColor : offColor; if(musicButtonImage) musicButtonImage.color = isMusicOn ? onColor : offColor; }
}