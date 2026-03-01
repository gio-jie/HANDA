using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; 
using System.Collections; 

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
    public TMP_Text score;

    [Header("Blackout & Transition Effect")]
    public Image darknessOverlay; 
    public float maxDarkness = 0.95f; 
    public TMP_Text goodJobText; 
    public float fadeDuration = 1.5f; 

    [Header("Penalty Visuals (- Electricity)")]
    public Image penaltyIconImage; 
    public float penaltyShowDuration = 0.5f; 
    public float penaltyFadeDuration = 1.0f; 
    private Coroutine currentPenaltyRoutine; 

    [Header("UI Panels")]
    public GameObject pausePanel;

    [Header("Game Data")]
    public int totalFamiliesToServe = 3;
    public int familiesServed = 0;
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

        if (goodJobText != null) goodJobText.gameObject.SetActive(false);
        
        if (penaltyIconImage != null) penaltyIconImage.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isGameActive) return;

        // --- BAGONG DAGDAG: StarManager Timeout/Fail Check ---
        if (StarManager.Instance != null && StarManager.Instance.GetRemainingSeconds() <= 0)
        {
            GameOverBlackout();
            return;
        }

        currentPower -= powerDrainRate * Time.deltaTime;
        
        if (powerBarFill != null)
        {
            powerBarFill.fillAmount = currentPower / maxPower;

            if (currentPower < 30f) powerBarFill.color = Color.red;
            else if (currentPower < 60f) powerBarFill.color = Color.yellow;
            else powerBarFill.color = Color.green;
        }

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

    public void TriggerPenaltyFeedback()
    {
        if (!isGameActive) return;

        if (penaltyIconImage != null)
        {
            if (currentPenaltyRoutine != null) StopCoroutine(currentPenaltyRoutine);
            currentPenaltyRoutine = StartCoroutine(AnimatePenaltyRoutine());
        }

        // --- TAWAGIN ANG STAR MANAGER PARA SA PENALTY LOGIC ---
        if (StarManager.Instance != null)
        {
            StarManager.Instance.RegisterWrongItem();
            
            if (StarManager.Instance.GetRemainingSeconds() <= 0 || StarManager.Instance.GetCurrentStars() == 0)
            {
                GameOverBlackout();
            }
        }
    }

    IEnumerator AnimatePenaltyRoutine()
    {
        penaltyIconImage.gameObject.SetActive(true); 

        Color c = penaltyIconImage.color;
        c.a = 1f;
        penaltyIconImage.color = c;

        yield return new WaitForSeconds(penaltyShowDuration);

        float timer = 0f;
        while (timer < penaltyFadeDuration)
        {
            timer += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, timer / penaltyFadeDuration); 
            penaltyIconImage.color = c;
            yield return null;
        }

        penaltyIconImage.gameObject.SetActive(false); 
    }

    void GameOverBlackout()
    {
        if (!isGameActive) return;

        isGameActive = false;
        currentPower = 0;
        if (powerBarFill != null) powerBarFill.fillAmount = 0;
        
        if (darknessOverlay != null)
        {
            Color finalColor = darknessOverlay.color;
            finalColor.a = 1f; 
            darknessOverlay.color = finalColor;
        }

        // --- IPASA ANG LOSE PANEL SA STAR MANAGER ---
        if (StarManager.Instance != null)
        {
            StarManager.Instance.EndLevel(false);
        }
    }

    public void AddScore()
    {
        if (!isGameActive) return;

        familiesServed++;
        UpdateUI();

        if (StarManager.Instance != null) StarManager.Instance.RegisterCorrectItem();

        if (familiesServed >= totalFamiliesToServe)
        {
            isGameActive = false;
            StartCoroutine(WinTransitionRoutine());
        }
    }

    IEnumerator WinTransitionRoutine()
    {
        if (goodJobText != null)
        {
            goodJobText.gameObject.SetActive(true);
            goodJobText.text = "Good Job!";
        }

        yield return new WaitForSeconds(1f);

        if (darknessOverlay != null)
        {
            float timer = 0f;
            Color c = darknessOverlay.color;
            float startAlpha = c.a;

            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                c.a = Mathf.Lerp(startAlpha, 1f, timer / fadeDuration);
                darknessOverlay.color = c;
                yield return null;
            }
        }

        SceneManager.LoadScene("Level8_Part2");
    }

    void UpdateUI()
    {
        if (score != null) score.text = "" + familiesServed + " / " + totalFamiliesToServe;
    }

    public void PauseGame() { if (pausePanel != null) pausePanel.SetActive(true); Time.timeScale = 0; }
    public void ResumeGame() { if (pausePanel != null) pausePanel.SetActive(false); Time.timeScale = 1; }
    public void RetryLevel() { Time.timeScale = 1; SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void QuitToLevelSelect() { Time.timeScale = 1; SceneManager.LoadScene("TyphoonLevelSelect"); }
}