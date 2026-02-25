using UnityEngine;
using UnityEngine.UI; 
using TMPro;
using System.Collections; 
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
    // --- BAGONG DAGDAG: VOLUME SLIDER ---
    // ==========================================
    [Header("Audio (SFX)")]
    public AudioClip biteSound; 
    [Range(0f, 1f)] // Gagawin nitong slider ang volume sa Inspector!
    public float biteVolume = 0.5f; // Default ay kalahati (50%)
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

        // --- DITO NATIN INAPPLY YUNG VOLUME ---
        if (biteSound != null)
        {
            // Babasahin na niya yung biteVolume na sinet mo sa Inspector!
            AudioSource.PlayClipAtPoint(biteSound, Camera.main.transform.position, biteVolume);
        }
        else if (AudioManager.instance != null) 
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.wrongSound);
        }
        // ----------------------------------------

        if (currentHealth >= 0 && currentHealth < heartIcons.Length)
        {
            Vector3 lostHeartPos = heartIcons[currentHealth].rectTransform.position;
            
            if (emptyHeartSprite != null)
            {
                heartIcons[currentHealth].sprite = emptyHeartSprite;
            }

            StartCoroutine(AnimateFallingHeart(lostHeartPos));
        }

        int damageTaken = maxHealth - currentHealth; 

        if (damageTaken < jobertPantalSprites.Length)
        {
            jobertRenderer.sprite = jobertPantalSprites[damageTaken];
        }

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

    void UpdateScoreDisplay() { if(scoreTextUI != null) scoreTextUI.text = currentKills + " / " + targetKills; }

    void LoadPhase2()
    {
        SceneManager.LoadScene("Level7_Part2"); 
    }

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