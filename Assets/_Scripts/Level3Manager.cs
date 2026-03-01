using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class Level3Manager : MonoBehaviour
{
    public static Level3Manager instance;

    [Header("Game Settings")]
    public int itemsNeeded = 5; // Total na kalat
    public int currentScore = 0;
    
    [Header("Game State")]
    public bool isGameActive = true;

    [Header("UI Panels")]
    public GameObject pausePanel;

    [Header("In-Game UI")]
    public TMP_Text scoreText;
    public GameObject checkIcon; // Visual feedback pag naligpit

    [Header("Toggle Buttons")]
    public Image soundButtonImage; 
    public Image musicButtonImage;
    public Color onColor = Color.green;
    public Color offColor = Color.gray;
    private bool isSoundOn = true;
    private bool isMusicOn = true;

    void Awake()
    {
        instance = this;
        Time.timeScale = 1;
    }

    void Start()
    {
        UpdateScoreDisplay();
        UpdateToggleVisuals();
        
        // Resume Music pag pasok sa level
        if (AudioManager.instance != null) AudioManager.instance.ResumeBGM();
    }

    void Update()
    {
        if (isGameActive && StarManager.Instance != null)
        {
            // Kung naubos ang oras sa StarManager, I-GAME OVER!
            if (StarManager.Instance.GetRemainingSeconds() <= 0)
            {
                isGameActive = false;
            }
        }
    }

    // --- LEVEL 3 SPECIFIC LOGIC (Cleanup) ---

    public void HazardCollected()
    {
        if (!isGameActive) return;

        currentScore++;
        UpdateScoreDisplay();
        
        // Play Sound
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.correctSound);

        // Show Check Icon feedback
        if(checkIcon != null)
        {
            checkIcon.SetActive(true);
            CancelInvoke("HideCheck");
            Invoke("HideCheck", 1f);
        }

        // --- IPASA SA STAR MANAGER ANG PAGKA-TAMA ---
        if (StarManager.Instance != null)
        {
            StarManager.Instance.RegisterCorrectItem();
        }

        if (currentScore >= itemsNeeded)
        {
            // WIN!
            isGameActive = false;
            StartCoroutine(LevelCompleteDelay());
        }
    }

    // --- BAGONG DAGDAG: DELAY COROUTINE PARA HINDI MABIGLA ---
    IEnumerator LevelCompleteDelay()
    {
        yield return new WaitForSeconds(1.0f);
        if (StarManager.Instance != null)
        {
            StarManager.Instance.EndLevel(true); 
        }
    }

    void UpdateScoreDisplay()
    {
        if(scoreText != null) scoreText.text = "" + currentScore + "/" + itemsNeeded;
    }

    void HideCheck() { if(checkIcon != null) checkIcon.SetActive(false); }

    // --- BUTTONS & TOGGLES ---
    public void RetryLevel() { if (AudioManager.instance != null) AudioManager.instance.ResumeBGM(); SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void QuitToLevelSelect() { if (AudioManager.instance != null) AudioManager.instance.ResumeBGM(); SceneManager.LoadScene("TyphoonLevelSelect"); }
    public void PauseGame() { pausePanel.SetActive(true); Time.timeScale = 0; if (AudioManager.instance != null) AudioManager.instance.PauseBGM(); }
    public void ResumeGame() { pausePanel.SetActive(false); Time.timeScale = 1; if (AudioManager.instance != null) AudioManager.instance.ResumeBGM(); }
    
    public void ToggleSound() { isSoundOn = !isSoundOn; AudioListener.volume = isSoundOn ? 1 : 0; UpdateToggleVisuals(); }
    public void ToggleMusic() { isMusicOn = !isMusicOn; UpdateToggleVisuals(); }
    void UpdateToggleVisuals() { if(soundButtonImage) soundButtonImage.color = isSoundOn ? onColor : offColor; if(musicButtonImage) musicButtonImage.color = isMusicOn ? onColor : offColor; }
}