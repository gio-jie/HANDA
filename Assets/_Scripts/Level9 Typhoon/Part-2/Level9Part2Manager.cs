using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class Level9Part2Manager : MonoBehaviour
{
    public static Level9Part2Manager instance;

    // ==========================================
    // --- MALINIS NA BAHAY TRANSITION ---
    // ==========================================
    [Header("Clean House Transition")]
    public Image backgroundImage; 
    public Sprite cleanBackgroundSprite; 
    public AudioClip cleanSoundEffect; 
    // ==========================================

    [Header("Player Status")]
    public bool isWearingBoots = false;
    public Image jobertImage; 
    public Sprite jobertWithBootsSprite; 

    [Header("Infection Meter (Parusa)")]
    public float currentInfection = 0f;      
    private float displayedInfection = 0f;   
    public float maxInfection = 100f;
    public Image infectionBarFill;
    public float infectionPenalty = 25f;
    public float barFillSpeed = 30f;         

    [Header("Game Data")]
    public int totalHazards = 3; 
    [HideInInspector] public int clearedHazards = 0;

    [Header("UI Feedback")]
    public TMP_Text statusText;
    public TMP_Text scoreTextUI; 

    [Header("UI Panels")]
    public GameObject pausePanel;

    [HideInInspector] public bool isGameActive = true;

    void Awake()
    {
        instance = this;
        Time.timeScale = 1;
    }

    void Start()
    {
        UpdateScoreDisplay();
        isGameActive = true;
    }

    void Update()
    {
        if (!isGameActive) return;

        // --- CHECK TIMER SA STAR MANAGER ---
        if (StarManager.Instance != null && StarManager.Instance.GetRemainingSeconds() <= 0)
        {
            FinalizeGameOver("Naubusan ng oras!");
            return;
        }

        // --- INFECTION BAR ANIMATION ---
        if (displayedInfection < currentInfection)
        {
            displayedInfection = Mathf.MoveTowards(displayedInfection, currentInfection, barFillSpeed * Time.deltaTime);
        }

        if (infectionBarFill != null)
        {
            infectionBarFill.fillAmount = displayedInfection / maxInfection;
            
            if (displayedInfection > 70f) infectionBarFill.color = Color.red;
            else if (displayedInfection > 40f) infectionBarFill.color = Color.yellow;
            else infectionBarFill.color = Color.green;
        }

        if (displayedInfection >= maxInfection) 
        {
            FinalizeGameOver("Jobert has been infected!");
        }
    }

    void UpdateScoreDisplay()
    {
        if (scoreTextUI != null)
        {
            scoreTextUI.text = clearedHazards + " / " + totalHazards;
        }
    }

    public void WearBoots()
    {
        isWearingBoots = true;
        
        if (jobertImage != null && jobertWithBootsSprite != null)
        {
            jobertImage.sprite = jobertWithBootsSprite;
            jobertImage.SetNativeSize(); 
        }

        if (statusText != null) statusText.text = "It's safe now! We can begin cleaning.";
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.correctSound);
    }

    public void HazardCleaned()
    {
        if (!isGameActive) return;

        clearedHazards++;
        UpdateScoreDisplay(); 

        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.correctSound);

        // --- IPASA SA STAR MANAGER ANG PAGKA-TAMA ---
        if (StarManager.Instance != null)
        {
            StarManager.Instance.RegisterCorrectItem();
        }

        if (clearedHazards >= totalHazards)
        {
            isGameActive = false;
            
            if (statusText != null) statusText.text = "Malinis at ligtas na ang bahay!";
            
            // PALITAN ANG BACKGROUND AT TUMUNOG
            if (backgroundImage != null && cleanBackgroundSprite != null)
            {
                backgroundImage.sprite = cleanBackgroundSprite;
            }

            if (cleanSoundEffect != null)
            {
                AudioSource.PlayClipAtPoint(cleanSoundEffect, Camera.main.transform.position, 1f);
            }

            // 3 SECONDS DELAY BAGO LUMABAS ANG WIN PANEL
            StartCoroutine(LevelCompleteDelay());
        }
    }

    IEnumerator LevelCompleteDelay()
    {
        yield return new WaitForSeconds(3.0f);
        if (StarManager.Instance != null)
        {
            StarManager.Instance.EndLevel(true); 
        }
    }

    public void AddInfection(string warningMessage)
    {
        if (!isGameActive) return;

        currentInfection += infectionPenalty; 
        
        if (statusText != null) statusText.text = warningMessage;
        
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.wrongSound);
        if (PlayerPrefs.GetInt("VibrationOn", 1) == 1) Handheld.Vibrate();

        // --- TAWAGIN ANG STAR MANAGER PARA SA TIME PENALTY ---
        if (StarManager.Instance != null)
        {
            StarManager.Instance.RegisterWrongItem();
            
            if (StarManager.Instance.GetRemainingSeconds() <= 0 || StarManager.Instance.GetCurrentStars() == 0)
            {
                FinalizeGameOver("Naubusan ng oras dahil sa penalty!");
            }
        }
    }

    void FinalizeGameOver(string reason)
    {
        if (!isGameActive) return;

        isGameActive = false;

        if (statusText != null) statusText.text = "GAME OVER: " + reason;
        
        // --- IPASA ANG LOSE PANEL SA STAR MANAGER ---
        if (StarManager.Instance != null)
        {
            StarManager.Instance.EndLevel(false);
        }
    }

    // PAALALA: Parehas ito sa Level 7, kung gusto mong bumalik sa Phase 1 pag natalo, 
    // palitan mo ang SceneManager.GetActiveScene().name ng "Level9_Part1"
    public void RetryLevel() { Time.timeScale = 1; SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void PauseGame() { if (pausePanel != null) pausePanel.SetActive(true); Time.timeScale = 0; }
    public void ResumeGame() { if (pausePanel != null) pausePanel.SetActive(false); Time.timeScale = 1; }
    public void QuitToLevelSelect() { Time.timeScale = 1; SceneManager.LoadScene("TyphoonLevelSelect"); }
}