using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; 
using System.Collections; // Kailangan para sa mga animations natin

public class Level8Part1Manager : MonoBehaviour
{
    public static Level8Part1Manager instance;

    [Header("Generator Settings")]
    public float maxPower = 100f;
    public float currentPower;
    public float powerDrainRate = 10f; 
    public float powerPerTap = 5f;    
    
    [Header("UI Elements")]
    public Image powerBarFill;
    public TMP_Text missionText;

    [Header("Blackout & Transition Effect")]
    public Image darknessOverlay; 
    public float maxDarkness = 0.95f; 
    // --- BAGONG DAGDAG ---
    public TMP_Text goodJobText; // Dito natin ilalagay ang "Good Job!"
    public float fadeDuration = 1.5f; // Bilis ng pagdilim
    // ---------------------

    [Header("UI Panels")]
    public GameObject losePanel;
    public GameObject pausePanel;

    [Header("Game Data")]
    public int totalFamiliesToServe = 3;
    private int familiesServed = 0;
    [HideInInspector] public bool isGameActive = false;

    void Awake()
    {
        instance = this;
        Time.timeScale = 1; 
    }

    void Start()
    {
        currentPower = maxPower;
        isGameActive = true;
        UpdateUI();

        // Itago muna ang Good Job text sa simula
        if (goodJobText != null) goodJobText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isGameActive) return;

        currentPower -= powerDrainRate * Time.deltaTime;
        
        if (powerBarFill != null)
        {
            powerBarFill.fillAmount = currentPower / maxPower;

            if (currentPower < 30f) powerBarFill.color = Color.red;
            else if (currentPower < 60f) powerBarFill.color = Color.yellow;
            else powerBarFill.color = Color.green;
        }

        // Dilim effect habang paubos ang kuryente
        if (darknessOverlay != null)
        {
            Color overlayColor = darknessOverlay.color;
            float darknessLevel = 1f - (currentPower / maxPower);
            overlayColor.a = Mathf.Clamp(darknessLevel, 0f, maxDarkness);
            darknessOverlay.color = overlayColor;
        }

        if (currentPower <= 0)
        {
            GameOverBlackout();
        }
    }

    public void TapGenerator()
    {
        if (!isGameActive || Time.timeScale == 0) return; 

        currentPower += powerPerTap;
        if (currentPower > maxPower) currentPower = maxPower; 
    }

    void GameOverBlackout()
    {
        isGameActive = false;
        currentPower = 0;
        if (powerBarFill != null) powerBarFill.fillAmount = 0;
        
        if (darknessOverlay != null)
        {
            Color finalColor = darknessOverlay.color;
            finalColor.a = 1f; 
            darknessOverlay.color = finalColor;
        }

        if (losePanel != null) losePanel.SetActive(true);
        if (AudioManager.instance != null) 
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.loseSound);
            AudioManager.instance.PauseBGM();
        }
    }

    public void AddScore()
    {
        if (!isGameActive) return;

        familiesServed++;
        UpdateUI();
        
        if (familiesServed >= totalFamiliesToServe)
        {
            isGameActive = false;
            // --- TAWAGIN ANG BAGONG ANIMATION ---
            StartCoroutine(WinTransitionRoutine());
        }
    }

    // --- BAGONG ANIMATION ROUTINE PABALIK SA DILIM ---
    IEnumerator WinTransitionRoutine()
    {
        // 1. Ipakita ang Good Job!
        if (goodJobText != null)
        {
            goodJobText.gameObject.SetActive(true);
            goodJobText.text = "Good Job!";
        }

        // 2. Maghintay ng 1 second para mabasa
        yield return new WaitForSeconds(1f);

        // 3. Unti-unting diliman ang screen (Fade to Black)
        if (darknessOverlay != null)
        {
            float timer = 0f;
            Color c = darknessOverlay.color;
            float startAlpha = c.a;

            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                c.a = Mathf.Lerp(startAlpha, 1f, timer / fadeDuration); // Mula current alpha papuntang 1 (Solid Black)
                darknessOverlay.color = c;
                yield return null;
            }
        }

        // 4. Saka natin ilo-load ang Phase 2!
        SceneManager.LoadScene("Level8_Part2");
    }

    void UpdateUI()
    {
        if (missionText != null) missionText.text = "Families Served: " + familiesServed + " / " + totalFamiliesToServe;
    }

    // (Pause at Menu Commands - Walang nagbago dito)
    public void PauseGame() { if (pausePanel != null) pausePanel.SetActive(true); Time.timeScale = 0; }
    public void ResumeGame() { if (pausePanel != null) pausePanel.SetActive(false); Time.timeScale = 1; }
    public void RetryLevel() { Time.timeScale = 1; SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void QuitToLevelSelect() { Time.timeScale = 1; SceneManager.LoadScene("TyphoonLevelSelect"); }
}