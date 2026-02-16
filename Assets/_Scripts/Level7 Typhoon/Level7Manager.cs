using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; 

public class Level7Manager : MonoBehaviour
{
    public static Level7Manager instance; 

    [Header("Phase 1 Settings (No Timer)")]
    public int targetKills = 20;    
    public int maxHealth = 5;       

    [HideInInspector] public bool isGameActive = true; 
    private int currentKills = 0;
    private int currentHealth;

    [Header("Jobert's Visuals")]
    public SpriteRenderer jobertRenderer; 
    public Sprite[] jobertPantalSprites; 
    public Sprite jobertHappySprite; 

    [Header("UI Panels")]
    // Inalis na natin ang Win Panel dito dahil Scene transition ang kapalit
    public GameObject losePanel; 
    public GameObject pausePanel;
    
    [Header("In-Game UI")]
    public TMP_Text scoreTextUI; 

    void Awake()
    {
        instance = this; 
        Time.timeScale = 1;
    }

    void Start()
    {
        currentHealth = maxHealth;
        UpdateScoreDisplay();
        isGameActive = true;

        if (jobertPantalSprites.Length > 0) jobertRenderer.sprite = jobertPantalSprites[0];

        if (AudioManager.instance != null) AudioManager.instance.ResumeBGM();
    }

    // --- GAMEPLAY LOGIC ---

    public void AddScore()
    {
        if (!isGameActive) return; 

        currentKills++;
        UpdateScoreDisplay();

        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.correctSound);

        if (currentKills >= targetKills)
        {
            isGameActive = false; 

            // Palitan si Jobert ng Happy Sprite
            if (jobertHappySprite != null) jobertRenderer.sprite = jobertHappySprite;

            // Imbes na Win Panel, tatawagin natin ang Next Scene pagkalipas ng 1.5 seconds
            Invoke("LoadPhase2", 1.5f);
        }
    }

    public void TakeDamage()
    {
        if (!isGameActive) return;

        currentHealth--; 
        int damageTaken = maxHealth - currentHealth; 

        if (damageTaken < jobertPantalSprites.Length)
        {
            jobertRenderer.sprite = jobertPantalSprites[damageTaken];
        }

        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.wrongSound);

        // GAME OVER: Naubos ang buhay ni Jobert
        if (currentHealth <= 0)
        {
            isGameActive = false;
            if (losePanel != null) losePanel.SetActive(true);
            if (AudioManager.instance != null) 
            {
                AudioManager.instance.PlaySFX(AudioManager.instance.loseSound);
                AudioManager.instance.PauseBGM();
            }
        }
    }

    void UpdateScoreDisplay() { if(scoreTextUI != null) scoreTextUI.text = currentKills + " / " + targetKills; }

    // --- SCENE TRANSITION ---
    void LoadPhase2()
    {
        // Ito ang pangalan ng susunod na Scene na gagawin natin
        SceneManager.LoadScene("Level7_Part2"); 
    }

    // --- BUTTON CONTROLS ---
    public void RetryLevel()
    {
        Time.timeScale = 1; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void PauseGame()
    {
        if (pausePanel != null) pausePanel.SetActive(true); 
        Time.timeScale = 0; 
        if (AudioManager.instance != null) AudioManager.instance.PauseBGM();
    }
    public void ResumeGame()
    {
        if (pausePanel != null) pausePanel.SetActive(false); 
        Time.timeScale = 1; 
        if (AudioManager.instance != null) AudioManager.instance.ResumeBGM();
    }
    public void QuitToLevelSelect()
    {
        Time.timeScale = 1; 
        SceneManager.LoadScene("TyphoonLevelSelect");
    }
}