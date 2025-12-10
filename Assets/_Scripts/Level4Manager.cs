using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Level4Manager : MonoBehaviour
{
    [Header("Game Settings")]
    public int plugsConnected = 3; // Ilan ang nakasaksak?
    public SwipeInteraction breakerScript;
    public float timeLimit = 60f;
    public float penaltyTime = 5f;

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
    public GameObject safetyPanel; // Specific sa Level 4 (Warning)

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
    public TMP_Text statusText; // "Plugs Left: 3"
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
        UpdateStatusDisplay();
        UpdateToggleVisuals();
        
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

    // --- LEVEL 4 SPECIFIC LOGIC (Electrical) ---

    public void UnplugDevice(GameObject plugObject)
    {
        if (!isGameActive) return;

        plugsConnected--;
        
        // plugObject.SetActive(false); <--- Deleted na to
        
        UpdateStatusDisplay();

        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.clickSound);
    }

    public void TrySwitchBreaker()
    {
        if (!isGameActive) return;

        if (plugsConnected == 0)
        {
            // WIN!
            Debug.Log("LEVEL COMPLETE! Power Safe.");
            isGameActive = false;
            finalTimeRecorded = timeLimit;
            UpdateTimerDisplay(finalTimeRecorded);
            
            // Switch Sound
            if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.correctSound);
            
            Invoke("ShowWinScreen", 1f);
        }
        else
        {
            if (breakerScript != null)
            {
                breakerScript.ResetToActive(); 
            }
            
            // FAIL: Warning!
            Debug.Log("Danger! May nakasaksak pa.");
            
            // Penalty
            timeLimit -= penaltyTime;
            
            // Warning Sound
            if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.warningSound);

            // Show Safety Panel
            if (safetyPanel != null)
            {
                safetyPanel.SetActive(true);
                Time.timeScale = 0; // Pause game while reading warning
                if (AudioManager.instance != null) AudioManager.instance.PauseBGM();
            }
            
            if (timeLimit <= 0) FinalizeGameOver();
        }
    }

    public void CloseSafetyWarning()
    {
        if (safetyPanel != null) safetyPanel.SetActive(false);
        Time.timeScale = 1;
        if (AudioManager.instance != null) AudioManager.instance.ResumeBGM();
    }

    void UpdateStatusDisplay()
    {
        if(statusText != null) statusText.text = "Plugs Left: " + plugsConnected;
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

        // High Score (Level 4 Key)
        float currentBest = PlayerPrefs.GetFloat("Level4_BestTime", 0);

        if (scoreTime > currentBest)
        {
            currentBest = scoreTime;
            PlayerPrefs.SetFloat("Level4_BestTime", currentBest);
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
            AudioManager.instance.PlaySFX(AudioManager.instance.winSound);
            AudioManager.instance.PauseBGM(); // Ngayon safe na ito!
        }

        // Lose Panel UI Update
        if(loseTimeText != null) loseTimeText.text = "Time Left: 00:00";
        if(loseStar1) loseStar1.color = missingColor;
        if(loseStar2) loseStar2.color = missingColor;
        if(loseStar3) loseStar3.color = missingColor;

        float currentBest = PlayerPrefs.GetFloat("Level4_BestTime", 0);
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