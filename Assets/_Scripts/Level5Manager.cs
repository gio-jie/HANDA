using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class Level5Manager : MonoBehaviour
{
    public static Level5Manager instance;

    [Header("Game State")]
    public bool isGameActive = true;

    [Header("UI Panels")]
    public GameObject pausePanel;
    public GameObject hazardPanel; // Yung "DANGER" warning

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
        UpdateToggleVisuals();
        
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

    // --- LEVEL 5 MECHANICS ---

    public void HitHazard()
    {
        if (!isGameActive) return;

        if (PlayerPrefs.GetInt("VibrationOn", 1) == 1)
        {
            Handheld.Vibrate(); 
        }
        
        // Sound
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.warningSound);

        // --- TAWAGIN ANG STAR MANAGER PARA SA PENALTY ---
        if (StarManager.Instance != null)
        {
            StarManager.Instance.RegisterWrongItem();

            // KUNG NA-GAME OVER NA DAHIL SA PENALTY, WAG NANG ILABAS ANG HAZARD PANEL
            if (StarManager.Instance.GetRemainingSeconds() <= 0 || StarManager.Instance.GetCurrentStars() == 0)
            {
                isGameActive = false;
                return; 
            }
        }

        // Show Warning Panel if hindi pa game over
        if (hazardPanel != null)
        {
            hazardPanel.SetActive(true);
            Time.timeScale = 0; // Pause game
            if (AudioManager.instance != null) AudioManager.instance.PauseBGM();
        }
    }

    public void CloseWarning()
    {
        if (hazardPanel != null) hazardPanel.SetActive(false);
        
        // I-resume lang ang time kung hindi pa game over
        if (isGameActive)
        {
            Time.timeScale = 1; 
            if (AudioManager.instance != null) AudioManager.instance.ResumeBGM();
        }
    }

    public void ReachGoal()
    {
        if (!isGameActive) return;

        // WIN!
        isGameActive = false;

        // Play Success Sound pag dating sa dulo
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.correctSound);

        StartCoroutine(LevelCompleteDelay());
    }

    // --- BAGONG DAGDAG: DELAY COROUTINE ---
    IEnumerator LevelCompleteDelay()
    {
        yield return new WaitForSeconds(1.0f);
        if (StarManager.Instance != null)
        {
            StarManager.Instance.EndLevel(true); 
        }
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