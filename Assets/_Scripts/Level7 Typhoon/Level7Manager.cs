using UnityEngine;
using UnityEngine.UI; // --- BAGONG DAGDAG: Kailangan para sa Image ---
using TMPro;
using System.Collections; // --- BAGONG DAGDAG: Kailangan para sa IEnumerator (Animation) ---
using UnityEngine.SceneManagement; 

public class Level7Manager : MonoBehaviour
{
    public static Level7Manager instance; 

    [Header("Phase 1 Settings (No Timer)")]
    public int targetKills = 20;    
    public int maxHealth = 5;       

    [HideInInspector] public bool isGameActive = true; 
    public int currentKills = 0;
    private int currentHealth;

    [Header("Jobert's Visuals")]
    public SpriteRenderer jobertRenderer; 
    public Sprite[] jobertPantalSprites; 
    public Sprite jobertHappySprite; 

    // ==========================================
    // --- BAGONG DAGDAG: HEALTH UI (HEARTS) ---
    // ==========================================
    [Header("Health UI (Hearts)")]
    public Image[] heartIcons; 
    public Sprite emptyHeartSprite; 
    public Image fallingHeartPrefab; 
    public float fallSpeed = 200f;
    public float fadeDuration = 1f;

    [Header("UI Panels")]
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

        // Siguraduhing tago ang falling heart sa simula
        if (fallingHeartPrefab != null) fallingHeartPrefab.gameObject.SetActive(false);

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

            if (jobertHappySprite != null) jobertRenderer.sprite = jobertHappySprite;

            Invoke("LoadPhase2", 1.5f);
        }
    }

    public void TakeDamage()
    {
        if (!isGameActive) return;

        currentHealth--; 

        // --- BAGONG DAGDAG: HEART ANIMATION LOGIC ---
        // Kapag nabawasan ng buhay, hahanapin nito yung tamang heart index para palitan at ihulog
        if (currentHealth >= 0 && currentHealth < heartIcons.Length)
        {
            Vector3 lostHeartPos = heartIcons[currentHealth].rectTransform.position;
            
            if (emptyHeartSprite != null)
            {
                heartIcons[currentHealth].sprite = emptyHeartSprite;
            }

            StartCoroutine(AnimateFallingHeart(lostHeartPos));
        }
        // ---------------------------------------------

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

    // --- BAGONG DAGDAG: COROUTINE PARA SA PAGHULOG NG PUSO ---
    IEnumerator AnimateFallingHeart(Vector3 startPos)
    {
        fallingHeartPrefab.gameObject.SetActive(true);
        fallingHeartPrefab.rectTransform.position = startPos;

        Color c = fallingHeartPrefab.color; 
        c.a = 1f; 
        fallingHeartPrefab.color = c;
        
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fallingHeartPrefab.rectTransform.position += Vector3.down * fallSpeed * Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            fallingHeartPrefab.color = c;
            yield return null;
        }

        fallingHeartPrefab.gameObject.SetActive(false);
    }
    // ---------------------------------------------------------

    void UpdateScoreDisplay() { if(scoreTextUI != null) scoreTextUI.text = currentKills + " / " + targetKills; }

    // --- SCENE TRANSITION ---
    void LoadPhase2()
    {
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