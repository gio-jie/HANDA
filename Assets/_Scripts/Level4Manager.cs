using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class Level4Manager : MonoBehaviour
{
    public static Level4Manager instance;

    [Header("Game Settings")]
    public int plugsConnected = 3;
    public SwipeInteraction breakerScript;
    
    [Header("Game State")]
    public bool isGameActive = true;

    [Header("UI Panels")]
    public GameObject pausePanel;
    public GameObject safetyPanel; 

    [Header("In-Game UI")]
    public TMP_Text statusText; // "Plugs Left: 3"

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
        UpdateStatusDisplay();
        UpdateToggleVisuals();
        
        // Resume Music
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

    // --- LEVEL 4 SPECIFIC LOGIC (Electrical) ---

    public void UnplugDevice(GameObject plugObject)
    {
        if (!isGameActive) return;

        plugsConnected--;
        UpdateStatusDisplay();

        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.clickSound);

        // Ire-reset natin ang "consecutive wrongs" sa Star Manager kasi gumawa siya ng tamang action
        if (StarManager.Instance != null) StarManager.Instance.RegisterCorrectItem();
    }

    public void TrySwitchBreaker()
    {
        if (!isGameActive) return;

        if (plugsConnected == 0)
        {
            // WIN!
            Debug.Log("LEVEL COMPLETE! Power Safe.");
            isGameActive = false;
            
            // Switch Sound
            if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.correctSound);
            
            StartCoroutine(LevelCompleteDelay());
        }
        else
        {
            if (breakerScript != null)
            {
                breakerScript.ResetToActive(); 
            }
            
            Debug.Log("Danger! May nakasaksak pa.");

            if (PlayerPrefs.GetInt("VibrationOn", 1) == 1)
            {
                Handheld.Vibrate(); 
            }
            
            if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.warningSound);

            // --- TAWAGIN ANG STAR MANAGER PARA SA PENALTY ---
            if (StarManager.Instance != null)
            {
                StarManager.Instance.RegisterWrongItem();

                // KUNG NA-GAME OVER NA DAHIL SA PENALTY, WAG NANG ILABAS ANG SAFETY PANEL PARA DI MAG-FREEZE
                if (StarManager.Instance.GetRemainingSeconds() <= 0 || StarManager.Instance.GetCurrentStars() == 0)
                {
                    isGameActive = false;
                    return; 
                }
            }

            // KUNG BUHAY PA, ILABAS ANG SAFETY PANEL AT I-PAUSE
            if (safetyPanel != null)
            {
                safetyPanel.SetActive(true);
                Time.timeScale = 0; // Pause game while reading warning
                if (AudioManager.instance != null) AudioManager.instance.PauseBGM();
            }
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

    public void CloseSafetyWarning()
    {
        if (safetyPanel != null) safetyPanel.SetActive(false);
        
        // I-resume lang ang time kung hindi pa game over
        if (isGameActive)
        {
            Time.timeScale = 1;
            if (AudioManager.instance != null) AudioManager.instance.ResumeBGM();
        }
    }

    void UpdateStatusDisplay()
    {
        if(statusText != null) statusText.text = "Plugs Left: " + plugsConnected;
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