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
    private int resolvedHazards = 0;

    [Header("UI Feedback")]
    public TMP_Text timerTextUI;
    public TMP_Text scoreTextUI;

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

    void UpdateTimerDisplay(float time)
    {
        if (timerTextUI != null)
        {
            if (time < 0) time = 0;
            float sec = Mathf.Max(0, Mathf.FloorToInt(time));
            timerTextUI.text = string.Format("<mspace=0.6em>{0:00}</mspace>", sec);
            timerTextUI.color = (time <= 10) ? Color.red : Color.white;
        }
    }

    void UpdateScoreDisplay()
    {
        if (scoreTextUI != null)
        {
            scoreTextUI.text = "Hazards Cleared: " + resolvedHazards + " / " + totalHazards;
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
        if (timerTextUI != null) timerTextUI.text = "00";
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