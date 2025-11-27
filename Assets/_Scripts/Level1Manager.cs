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

    [Header("Data (Do not edit)")]
    public int currentScore = 0;
    private bool isGameActive = true; // Para malaman kung tuloy pa ang timer
    
    [Header("UI Panels")]
    public GameObject winPanel;
    public GameObject losePanel; // BAGONG PANEL
    public GameObject pausePanel;
    public TMP_Text scoreTextUI;
    public TMP_Text timerTextUI; // BAGONG TEXT
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

    // --- ITO ANG BAGONG UPDATE LOOP PARA SA TIMER ---
    void Update()
    {
        // Kung active ang game, bawasan ang oras
        if (isGameActive)
        {
            if (timeLimit > 0)
            {
                timeLimit -= Time.deltaTime; // Bawasan base sa real time seconds
                UpdateTimerDisplay();
            }
            else
            {
                // Pag naubos na (naging 0 or less)
                timeLimit = 0;
                UpdateTimerDisplay();
                GameOver(); // Talo na
            }
        }
    }

    void UpdateTimerDisplay()
    {
        // Gawing whole number (wag may decimals)
        timerTextUI.text = Mathf.CeilToInt(timeLimit).ToString();
        
        // Optional: Gawing pula pag 10 seconds na lang
        if(timeLimit <= 10)
        {
            timerTextUI.color = Color.red;
        }
    }

    public void GameOver()
    {
        isGameActive = false; // Tigil ang logic
        losePanel.SetActive(true); // Labas ang talo screen
    }

    public void RetryLevel()
    {
        // I-reload ang scene para umulit sa simula
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // --- GAMEPLAY FUNCTIONS ---

    public void AddScore()
    {
        if (!isGameActive) return; // Wag magbilang pag game over na

        currentScore++;
        UpdateScoreDisplay();
        StartCoroutine(ShowFeedback(checkIcon));

        if (currentScore >= itemsNeeded)
        {
            isGameActive = false; // TIGIL NA ANG TIMER KASI NANALO NA
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
        Debug.Log("Mali!");
        StartCoroutine(ShowFeedback(xIcon));
        
        // Optional: Pwede ka magbawas ng 5 seconds bilang penalty pag nagkamali!
        // timeLimit -= 5; 
    }

    void ShowWinScreen()
    {
        winPanel.SetActive(true);
    }

    IEnumerator ShowFeedback(GameObject icon)
    {
        icon.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        icon.SetActive(false);
    }

    // --- PAUSE & TOGGLE FUNCTIONS (SAME AS BEFORE) ---

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