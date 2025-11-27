using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Level3Manager : MonoBehaviour
{
    [Header("Game Settings")]
    public int itemsNeeded = 5; // 5 Hazards
    public int currentScore = 0;
    public float timeLimit = 60f;

    [Header("UI References")]
    public TMP_Text scoreText;
    public TMP_Text timerText;
    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject pausePanel;
    public GameObject checkIcon; // Visual feedback

    // Toggles logic (Optional, copy from prev levels if needed)
    public Image soundButtonImage;
    public Image musicButtonImage;
    public Color onColor = Color.green;
    public Color offColor = Color.gray;
    private bool isSoundOn = true;
    private bool isMusicOn = true;

    private bool isGameActive = true;

    void Start()
    {
        Time.timeScale = 1;
        UpdateScoreDisplay();
        UpdateToggleVisuals();
    }

    void Update()
    {
        if (isGameActive)
        {
            if (timeLimit > 0)
            {
                timeLimit -= Time.deltaTime;
                if(timerText != null) timerText.text = Mathf.CeilToInt(timeLimit).ToString();
            }
            else
            {
                timeLimit = 0;
                GameOver();
            }
        }
    }

    public void HazardCollected()
    {
        if (!isGameActive) return;

        currentScore++;
        UpdateScoreDisplay();
        
        // Show Check Icon feedback
        if(checkIcon != null)
        {
            checkIcon.SetActive(true);
            Invoke("HideCheck", 1f);
        }

        if (currentScore >= itemsNeeded)
        {
            isGameActive = false;
            Debug.Log("LEVEL COMPLETE!");
            Invoke("ShowWinScreen", 1.0f);
        }
    }

    void UpdateScoreDisplay()
    {
        if(scoreText != null) scoreText.text = "Hazards: " + currentScore + "/" + itemsNeeded;
    }

    void HideCheck()
    {
        if(checkIcon != null) checkIcon.SetActive(false);
    }

    void GameOver()
    {
        isGameActive = false;
        if(losePanel != null) losePanel.SetActive(true);
    }

    void ShowWinScreen()
    {
        if(winPanel != null) winPanel.SetActive(true);
    }

    // --- PAUSE & NAV ---
    public void PauseGame() { pausePanel.SetActive(true); Time.timeScale = 0; }
    public void ResumeGame() { pausePanel.SetActive(false); Time.timeScale = 1; }
    public void QuitToLevelSelect() { Time.timeScale = 1; SceneManager.LoadScene("TyphoonLevelSelect"); }
    public void RetryLevel() { Time.timeScale = 1; SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    
    // --- TOGGLES ---
    public void ToggleSound() { isSoundOn = !isSoundOn; AudioListener.volume = isSoundOn ? 1 : 0; UpdateToggleVisuals(); }
    public void ToggleMusic() { isMusicOn = !isMusicOn; UpdateToggleVisuals(); }
    void UpdateToggleVisuals() { 
        if(soundButtonImage) soundButtonImage.color = isSoundOn ? onColor : offColor;
        if(musicButtonImage) musicButtonImage.color = isMusicOn ? onColor : offColor;
    }
}