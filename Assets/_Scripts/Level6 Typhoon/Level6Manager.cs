using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using System.Collections;
using UnityEngine.SceneManagement; 

public class Level6Manager : MonoBehaviour
{
    // --- BAGONG DAGDAG: Para madaling tawagin mula sa UIDropZone ---
    public static Level6Manager instance; 

    [Header("Game Settings")]
    public int itemsNeeded = 8; // Ilan lahat ang relief goods na is-sort?
    public float timeLimit = 60f; 
    public float penaltyTime = 5f; // Oras na mababawas pag mali ang kahon

    [Header("Star System")]
    public float goldStarThreshold = 40f; 
    public float silverStarThreshold = 20f; 

    [Header("Data (Do not edit)")]
    public int currentScore = 0;
    private bool isGameActive = true; 
    private float finalTimeRecorded = 0f; 

    [System.Serializable]
    public struct MissionItem
    {
        public string itemName;       // Pangalan ng Item na idi-drag (e.g., "Item_CannedGoods")
        public GameObject checkmarkUI; // Yung Checkmark image sa HUD
    }
    
    [Header("Checklist Configuration")]
    public MissionItem[] missionItems; 

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

    [Header("Toggle Buttons")]
    public Image soundButtonImage; 
    public Image musicButtonImage;
    public Color onColor = Color.green;
    public Color offColor = Color.gray;

    private bool isSoundOn = true;
    private bool isMusicOn = true;

    void Awake()
    {
        instance = this; // SET INSTANCE PARA SA DRAG AND DROP
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
        if (timerTextUI != null)
        {
            if (timeToShow < 0) timeToShow = 0;
            float seconds = Mathf.FloorToInt(timeToShow);
            timerTextUI.text = string.Format("<mspace=0.6em>{0:00}</mspace>", seconds);

            if (timeToShow <= 10) 
            {
                timerTextUI.color = Color.red;
            }
            else 
            {
                Color customGreen;
                if (ColorUtility.TryParseHtmlString("#37B900", out customGreen))
                {
                    timerTextUI.color = customGreen;
                }
            }
        }
    }    

    void FinalizeGameOver()
    {
        timeLimit = 0;
        isGameActive = false;
        if(timerTextUI != null) timerTextUI.text = "00";
        GameOver(); 
    }

    public void GameOver()
    {
        isGameActive = false; 
        if(losePanel != null) losePanel.SetActive(true); 

        if (AudioManager.instance != null) 
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.loseSound);
            AudioManager.instance.PauseBGM(); 
        }
        
        if(loseTimeText != null) loseTimeText.text = "Time Left: 00:00";

        if(loseStar1) loseStar1.color = missingColor;
        if(loseStar2) loseStar2.color = missingColor;
        if(loseStar3) loseStar3.color = missingColor;

        // LEVEL 6 UPDATE
        float currentBest = PlayerPrefs.GetFloat("Level6_BestTime", 0);
        float bestMin = Mathf.FloorToInt(currentBest / 60);
        float bestSec = Mathf.FloorToInt(currentBest % 60);

        if(loseBestScoreText != null)
        {
            loseBestScoreText.text = string.Format("Best Record: {0:00}:{1:00}", bestMin, bestSec);
        }
    }

    public void AddScore(string collectedItemName)
    {
        if (!isGameActive) return; 

        bool foundInList = false;
        foreach (var mission in missionItems)
        {
            if (mission.itemName == collectedItemName)
            {
                if(mission.checkmarkUI != null) 
                {
                    mission.checkmarkUI.SetActive(true); 
                }
                foundInList = true;
                break;
            }
        }

        if (!foundInList)
        {
            Debug.LogWarning("Item na nakuha (" + collectedItemName + ") ay wala sa Mission List! Check names.");
        }

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
            AudioManager.instance.PauseBGM(); 
        }

        float scoreTime = finalTimeRecorded; 

        if(star1) star1.color = missingColor;
        if(star2) star2.color = missingColor;
        if(star3) star3.color = missingColor;

        if(star1) star1.color = earnedColor;
        if (scoreTime >= silverStarThreshold && star2) star2.color = earnedColor;
        if (scoreTime >= goldStarThreshold && star3) star3.color = earnedColor;

        float min = Mathf.FloorToInt(scoreTime / 60);
        float sec = Mathf.FloorToInt(scoreTime % 60);
        
        if(timeFinishedText != null)
            timeFinishedText.text = string.Format("Time Left: {0:00}:{1:00}", min, sec);

        // LEVEL 6 UPDATE
        float currentBest = PlayerPrefs.GetFloat("Level6_BestTime", 0);

        if (scoreTime > currentBest)
        {
            currentBest = scoreTime;
            PlayerPrefs.SetFloat("Level6_BestTime", currentBest);
            PlayerPrefs.Save();
            
            if(bestScoreText != null)
            {
                bestScoreText.text = "NEW BEST RECORD!";
                bestScoreText.color = Color.yellow;
            }
        }
        else
        {
            float bestMin = Mathf.FloorToInt(currentBest / 60);
            float bestSec = Mathf.FloorToInt(currentBest % 60);
            
            if(bestScoreText != null)
            {
                bestScoreText.text = string.Format("Best Record: {0:00}:{1:00}", bestMin, bestSec);
                bestScoreText.color = Color.white;
            }
        }

        int starsEarned = 1; 
        if (scoreTime >= silverStarThreshold) starsEarned = 2;
        if (scoreTime >= goldStarThreshold) starsEarned = 3;

        int currentSavedStars = PlayerPrefs.GetInt("Level6_Stars", 0);
        if (starsEarned > currentSavedStars)
        {
            PlayerPrefs.SetInt("Level6_Stars", starsEarned);
        }

        // UNLOCK LEVEL 7
        PlayerPrefs.SetInt("Level7_Unlocked", 1);
        PlayerPrefs.Save();
    }

    void UpdateScoreDisplay() { if(scoreTextUI != null) scoreTextUI.text = "" + currentScore + "/" + itemsNeeded; }
    
    public void WrongItem() 
    { 
        if (!isGameActive) return; 
        StartCoroutine(ShowFeedback(xIcon)); 
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.wrongSound);
        if (PlayerPrefs.GetInt("VibrationOn", 1) == 1) Handheld.Vibrate(); 
        timeLimit -= penaltyTime; // DITO BINABAWAS ANG ORAS
        if (timeLimit <= 0) FinalizeGameOver(); 
        UpdateTimerDisplay(timeLimit);
    }

    IEnumerator ShowFeedback(GameObject icon) { if(icon) { icon.SetActive(true); yield return new WaitForSeconds(1.0f); icon.SetActive(false); } }
    
    public void RetryLevel()
    {
        if (AudioManager.instance != null) AudioManager.instance.ResumeBGM();
        Time.timeScale = 1; 
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
        if (AudioManager.instance != null) AudioManager.instance.ResumeBGM();
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