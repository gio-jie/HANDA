using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class Level9Part1Manager : MonoBehaviour
{
    public static Level9Part1Manager instance;

    [Header("Game Settings")]
    public float timeLimit = 60f; // 1 minute para hanapin ang hazards sa dilim
    public float penaltyTime = 5f;
    public int totalHazards = 4; // Live Wire, Ahas, Puno, Salamin
    public int resolvedHazards = 0;

    [Header("UI Feedback")]
    public TMP_Text timerTextUI;
    public TMP_Text scoreTextUI;
    
    // --- BAGONG DAGDAG: TAGATANDA NG KULAY ---
    private Color originalTimerColor; 
    // -----------------------------------------

    [Header("Penalty Animation")]
    public TMP_Text penaltyTextUI;
    public float fallSpeed = 50f;
    public float fadeDuration = 1f;
    private Vector3 penaltyOriginalPos;

    [Header("Transition Effects")]
    public TMP_Text goodJobText; 
    public Image darknessOverlay; // Para sa fade to black transition
    public float transitionSpeed = 1.5f;

    [Header("UI Panels")]
    public GameObject losePanel;
    public GameObject pausePanel;

    [HideInInspector] public bool isGameActive = true;

    void Awake()
    {
        instance = this;
        Time.timeScale = 1;

        // --- BAGONG DAGDAG: I-SAVE ANG KULAY MULA SA INSPECTOR ---
        if (timerTextUI != null)
        {
            originalTimerColor = timerTextUI.color; 
        }
    }

    void Start()
    {
        UpdateScoreDisplay();
        
        if (penaltyTextUI != null)
        {
            penaltyOriginalPos = penaltyTextUI.rectTransform.localPosition;
            penaltyTextUI.gameObject.SetActive(false);
        }

        if (goodJobText != null) goodJobText.gameObject.SetActive(false);
        
        // Siguraduhing clear ang dilim transition sa simula
        if (darknessOverlay != null) 
        {
            darknessOverlay.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (isGameActive && timeLimit > 0)
        {
            timeLimit -= Time.deltaTime;
            UpdateTimerDisplay(timeLimit);

            if (timeLimit <= 0)
            {
                FinalizeGameOver();
            }
        }
    }

    // --- BINAGO: GINAWANG MINUTES AND SECONDS + CUSTOM COLOR ---
    void UpdateTimerDisplay(float time)
    {
        if (timerTextUI != null)
        {
            if (time < 0) time = 0;

            int minutes = Mathf.FloorToInt(time / 60); 
            int seconds = Mathf.FloorToInt(time % 60); 

            // Format: MM:SS
            timerTextUI.text = string.Format("<mspace=0.6em>{0:00}:{1:00}</mspace>", minutes, seconds);
            
            // Babalik sa custom color mo imbis na laging white!
            timerTextUI.color = (time <= 10) ? Color.red : originalTimerColor;
        }
    }

    void UpdateScoreDisplay()
    {
        if (scoreTextUI != null)
        {
            scoreTextUI.text = "" + resolvedHazards + " / " + totalHazards;
        }
    }

    // --- TATAWAGIN NG HAZARD SCRIPT PAG TAMA ---
    public void AddScore()
    {
        if (!isGameActive) return;

        resolvedHazards++;
        UpdateScoreDisplay();

        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.correctSound);

        // KUNG TAPOS NA LAHAT NG HAZARDS
        if (resolvedHazards >= totalHazards)
        {
            isGameActive = false;
            StartCoroutine(WinTransitionRoutine());
        }
    }

    // --- TATAWAGIN NG HAZARD SCRIPT PAG MALI ---
    public void WrongItem()
    {
        if (!isGameActive) return;

        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.wrongSound);
        if (PlayerPrefs.GetInt("VibrationOn", 1) == 1) Handheld.Vibrate();

        timeLimit -= penaltyTime;
        if (penaltyTextUI != null) StartCoroutine(AnimatePenaltyText());

        if (timeLimit <= 0) FinalizeGameOver();
        
        // --- BAGONG DAGDAG: I-update agad ang display pagkabawas ---
        UpdateTimerDisplay(timeLimit);
    }

    IEnumerator AnimatePenaltyText()
    {
        penaltyTextUI.gameObject.SetActive(true);
        penaltyTextUI.text = "-" + penaltyTime;
        penaltyTextUI.rectTransform.localPosition = penaltyOriginalPos;

        Color c = penaltyTextUI.color; c.a = 1f; penaltyTextUI.color = c;
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            penaltyTextUI.rectTransform.localPosition += Vector3.down * fallSpeed * Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            penaltyTextUI.color = c;
            yield return null;
        }
        penaltyTextUI.gameObject.SetActive(false);
    }

    IEnumerator WinTransitionRoutine()
    {
        // 1. Ipakita ang Good Job!
        if (goodJobText != null)
        {
            goodJobText.gameObject.SetActive(true);
            goodJobText.text = "Good Job! Pasok na tayo.";
        }

        yield return new WaitForSeconds(1.5f);

        // 2. Unti-unting diliman ang screen
        if (darknessOverlay != null)
        {
            darknessOverlay.gameObject.SetActive(true);
            float timer = 0f;
            Color c = darknessOverlay.color;
            c.a = 0f; // Start transparent
            
            while (timer < transitionSpeed)
            {
                timer += Time.deltaTime;
                c.a = Mathf.Lerp(0f, 1f, timer / transitionSpeed);
                darknessOverlay.color = c;
                yield return null;
            }
        }

        // 3. Lipat sa Phase 2 (Operation Ligtas Linis)
        SceneManager.LoadScene("Level9_Part2");
    }

    void FinalizeGameOver()
    {
        timeLimit = 0;
        isGameActive = false;
        
        // --- BINAGO: GINAWANG 00:00 ---
        if (timerTextUI != null) timerTextUI.text = "00:00";
        
        if (losePanel != null) losePanel.SetActive(true);

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.loseSound);
            AudioManager.instance.PauseBGM();
        }
    }

    public void PauseGame() { if (pausePanel != null) pausePanel.SetActive(true); Time.timeScale = 0; }
    public void ResumeGame() { if (pausePanel != null) pausePanel.SetActive(false); Time.timeScale = 1; }
    public void RetryLevel() { Time.timeScale = 1; SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void QuitToLevelSelect() { Time.timeScale = 1; SceneManager.LoadScene("TyphoonLevelSelect"); }
}