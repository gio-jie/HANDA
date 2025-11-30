using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Level4Manager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject safetyPanel;
    [Header("Game State")]
    public int plugsConnected = 3; // Ilan ang nakasaksak?
    public float timeLimit = 60f;
    private bool isGameActive = true;

    [Header("UI References")]
    public TMP_Text timerText;
    public TMP_Text statusText; // Yung dating ScoreText
    public GameObject warningText; // Yung "DANGER" text
    
    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject pausePanel;

    // Toggles for Settings (Copy standard logic)
    public Image soundButtonImage;
    public Image musicButtonImage;
    public Color onColor = Color.green;
    public Color offColor = Color.gray;
    private bool isSoundOn = true;
    private bool isMusicOn = true;

    void Start()
    {
        Time.timeScale = 1;
        UpdateStatusDisplay();
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

    // Function kapag binunot ang plug
    public void UnplugDevice(GameObject plugObject)
    {
        if (!isGameActive) return;

        // 1. Bawasan ang bilang
        plugsConnected--;
        
        // 2. Hide the plug (Kunwari nabunot na)
        plugObject.SetActive(false);
        
        // 3. Update Text
        UpdateStatusDisplay();

        Debug.Log("Plug removed. Remaining: " + plugsConnected);
    }

    // Function kapag pinindot ang Breaker
    public void TrySwitchBreaker()
    {
        if (!isGameActive) return;

        if (plugsConnected == 0)
        {
            // SUCCESS
            Debug.Log("LEVEL COMPLETE! Power Safe.");
            isGameActive = false;
            Invoke("ShowWinScreen", 1f);
        }
        else
        {
            // FAIL: May nakasaksak pa!
            Debug.Log("Danger! May nakasaksak pa.");
            
            // Penalty
            timeLimit -= 5f;
            if(timerText != null) timerText.text = Mathf.CeilToInt(timeLimit).ToString();

            // SHOW SAFETY PANEL
            if (safetyPanel != null)
            {
                safetyPanel.SetActive(true); // Labas panel
                Time.timeScale = 0; // PAUSE GAME
            }
        }
    }

    public void CloseSafetyWarning()
    {
        if (safetyPanel != null) safetyPanel.SetActive(false); // Tago panel
        Time.timeScale = 1; // RESUME GAME
    }

    void UpdateStatusDisplay()
    {
        if(statusText != null) 
            statusText.text = "Plugs Left: " + plugsConnected;
    }

    void ShowWarning()
    {
        if(warningText != null)
        {
            warningText.SetActive(true);
            CancelInvoke("HideWarning"); // Reset timer kung sakaling nag spam click
            Invoke("HideWarning", 2f); // Tago after 2 seconds
        }
    }

    void HideWarning()
    {
        if(warningText != null) warningText.SetActive(false);
    }

    void GameOver() { isGameActive = false; if(losePanel != null) losePanel.SetActive(true); }
    void ShowWinScreen() { if(winPanel != null) winPanel.SetActive(true); }

    // --- STANDARD UI FUNCTIONS ---
    public void PauseGame() { pausePanel.SetActive(true); Time.timeScale = 0; }
    public void ResumeGame() { pausePanel.SetActive(false); Time.timeScale = 1; }
    public void QuitToLevelSelect() { Time.timeScale = 1; SceneManager.LoadScene("TyphoonLevelSelect"); }
    public void RetryLevel() { Time.timeScale = 1; SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void ToggleSound() { isSoundOn = !isSoundOn; AudioListener.volume = isSoundOn ? 1 : 0; UpdateToggleVisuals(); }
    public void ToggleMusic() { isMusicOn = !isMusicOn; UpdateToggleVisuals(); }
    void UpdateToggleVisuals() { 
        if(soundButtonImage) soundButtonImage.color = isSoundOn ? onColor : offColor;
        if(musicButtonImage) musicButtonImage.color = isMusicOn ? onColor : offColor;
    }
}