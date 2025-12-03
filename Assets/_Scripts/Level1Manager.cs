using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using System.Collections;
using UnityEngine.SceneManagement; 

public class Level1Manager : MonoBehaviour
{
    [Header("Game Settings")]
    public int itemsNeeded = 7;
    public float timeLimit = 60f; // 60 Seconds countdown
    public float penaltyTime = 5f; // Bawas oras pag mali

    [Header("Data (Do not edit)")]
    public int currentScore = 0;
    private bool isGameActive = true; 
    
    [Header("UI Panels")]
    public GameObject winPanel;
    public GameObject losePanel; 
    public GameObject pausePanel;
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

    void Start()
    {
        UpdateScoreDisplay();
        UpdateToggleVisuals();
        Time.timeScale = 1;
        isGameActive = true;
    }

    void Update()
    {
        if (isGameActive)
        {
            if (timeLimit > 0)
            {
                timeLimit -= Time.deltaTime;
                
                // --- MILLISECONDS WITH FIX ---
                int seconds = Mathf.FloorToInt(timeLimit);
                int milliseconds = Mathf.FloorToInt((timeLimit * 100) % 100);

                // GINAMITAN NATIN NG <mspace> PARA HINDI MANGINIG
                // Ang 0.6em ay ang lapad ng bawat number.
                if(timerTextUI != null) 
                    timerTextUI.text = string.Format("<mspace=0.6em>{0:00}:{1:00}</mspace>", seconds, milliseconds);
                
                // Change color if running out (10 seconds left)
                if(timeLimit <= 10 && timerTextUI != null)
                {
                    timerTextUI.color = Color.red;
                }
            }
            else
            {
                FinalizeGameOver();
            }
        }
    }

    void FinalizeGameOver()
    {
        timeLimit = 0;
        if(timerTextUI != null) timerTextUI.text = "00:00";
        GameOver(); 
    }

    public void GameOver()
    {
        isGameActive = false; 
        losePanel.SetActive(true); 
    }

    public void RetryLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // --- GAMEPLAY FUNCTIONS ---

    public void AddScore()
    {
        if (!isGameActive) return; 

        currentScore++;
        UpdateScoreDisplay();
        StartCoroutine(ShowFeedback(checkIcon));

        if (currentScore >= itemsNeeded)
        {
            isGameActive = false; 
            Invoke("ShowWinScreen", 1.5f);
        }
    }

    void UpdateScoreDisplay()
    {
        scoreTextUI.text = "Items: " + currentScore + "/" + itemsNeeded;
    }

    public void WrongItem()
    {
        if (!isGameActive) return;
        
        Debug.Log("Mali! Penalty applied.");
        StartCoroutine(ShowFeedback(xIcon));
        
        // PENALTY LOGIC
        timeLimit -= penaltyTime; 
        
        if (timeLimit <= 0)
        {
            FinalizeGameOver();
        }
    }

    void ShowWinScreen()
    {
        winPanel.SetActive(true);
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.winSound);
        }
    }

    IEnumerator ShowFeedback(GameObject icon)
    {
        icon.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        icon.SetActive(false);
    }

    // --- PAUSE & TOGGLE FUNCTIONS ---

    public void PauseGame()
    {
        pausePanel.SetActive(true); 
        Time.timeScale = 0; 
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false); 
        Time.timeScale = 1; 
    }

    public void QuitToLevelSelect()
    {
        Time.timeScale = 1; 
        SceneManager.LoadScene("TyphoonLevelSelect");
    }

    public void ToggleSound()
    {
        isSoundOn = !isSoundOn;
        AudioListener.volume = isSoundOn ? 1 : 0;
        UpdateToggleVisuals();
    }

    public void ToggleMusic()
    {
        isMusicOn = !isMusicOn;
        UpdateToggleVisuals();
    }

    void UpdateToggleVisuals()
    {
        soundButtonImage.color = isSoundOn ? onColor : offColor;
        musicButtonImage.color = isMusicOn ? onColor : offColor;
    }
}