using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // For Toggles

public class Level5Manager : MonoBehaviour
{
    public float timeLimit = 60f;
    public TMP_Text timerText;
    
    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject pausePanel;

    private bool isGameActive = true;

    // Toggles UI (Optional copy)
    public Image soundButtonImage;
    public Image musicButtonImage;
    public Color onColor = Color.green;
    public Color offColor = Color.gray;
    private bool isSoundOn = true, isMusicOn = true;

    void Start() { Time.timeScale = 1; UpdateToggleVisuals(); }

    void Update()
    {
        if (isGameActive && timeLimit > 0)
        {
            timeLimit -= Time.deltaTime;
            if(timerText != null) timerText.text = Mathf.CeilToInt(timeLimit).ToString();
            
            if (timeLimit <= 0) GameOver();
        }
    }

   [Header("Warning UI")]
    public GameObject hazardPanel; 

    public void HitHazard()
    {
        // 1. Penalty Logic (Same pa rin)
        timeLimit -= 10f; 
        if(timerText != null) timerText.text = Mathf.CeilToInt(timeLimit).ToString();

        if (timeLimit <= 0)
        {
            timeLimit = 0;
            if(timerText != null) timerText.text = "0";
            GameOver();
            return; // Tigil na dito kung game over
        }

        // 2. SHOW WARNING PANEL (Bago!)
        if (hazardPanel != null)
        {
            hazardPanel.SetActive(true); // Labas ang warning
            Time.timeScale = 0; // PAUSE GAME (Para mabasa nila)
        }
    }

    // Function para sa "OKAY" button
    public void CloseWarning()
    {
        if (hazardPanel != null) hazardPanel.SetActive(false); // Tago ang warning
        Time.timeScale = 1; // RESUME GAME
    }

    public void ReachGoal()
    {
        isGameActive = false;
        Debug.Log("LEVEL COMPLETE!");
        Invoke("ShowWin", 1f);
    }

    void ShowWin() { if(winPanel) winPanel.SetActive(true); }
    void GameOver() 
    { 
        isGameActive = false; 
        
        // IMPORTANT: I-freeze ang oras para hindi na makagalaw ang player
        Time.timeScale = 0; 

        if(losePanel != null) losePanel.SetActive(true); 
    }

    // STANDARD FUNCTIONS
    public void PauseGame() { pausePanel.SetActive(true); Time.timeScale = 0; }
    public void ResumeGame() { pausePanel.SetActive(false); Time.timeScale = 1; }
    public void QuitToLevelSelect() { Time.timeScale = 1; SceneManager.LoadScene("TyphoonLevelSelect"); }
    public void RetryLevel() { Time.timeScale = 1; SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    
    // Toggles
    public void ToggleSound() { isSoundOn = !isSoundOn; AudioListener.volume = isSoundOn ? 1 : 0; UpdateToggleVisuals(); }
    public void ToggleMusic() { isMusicOn = !isMusicOn; UpdateToggleVisuals(); }
    void UpdateToggleVisuals() { 
        if(soundButtonImage) soundButtonImage.color = isSoundOn ? onColor : offColor;
        if(musicButtonImage) musicButtonImage.color = isMusicOn ? onColor : offColor;
    }
    


}