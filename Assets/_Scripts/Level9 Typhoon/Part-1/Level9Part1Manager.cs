using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class Level9Part1Manager : MonoBehaviour
{
    public static Level9Part1Manager instance;

    [Header("Game Settings")]
    public int totalHazards = 4; // Live Wire, Ahas, Puno, Salamin
    public int resolvedHazards = 0;

    [Header("UI Feedback")]
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
        if (isGameActive && StarManager.Instance != null)
        {
            // Kung naubos ang oras sa StarManager, I-GAME OVER!
            if (StarManager.Instance.GetRemainingSeconds() <= 0)
            {
                FinalizeGameOver();
            }
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

        // --- IPASA SA STAR MANAGER ANG PAGKA-TAMA ---
        if (StarManager.Instance != null)
        {
            StarManager.Instance.RegisterCorrectItem();
        }

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

        if (penaltyTextUI != null) StartCoroutine(AnimatePenaltyText());

        // --- TAWAGIN ANG STAR MANAGER PARA SA PENALTY ---
        if (StarManager.Instance != null)
        {
            StarManager.Instance.RegisterWrongItem();
            
            if (StarManager.Instance.GetRemainingSeconds() <= 0 || StarManager.Instance.GetCurrentStars() == 0)
            {
                FinalizeGameOver();
            }
        }
    }

    IEnumerator AnimatePenaltyText()
    {
        penaltyTextUI.gameObject.SetActive(true);
        // Basahin ang penalty time mula sa Inspector ng StarManager!
        float penaltyAmount = (StarManager.Instance != null) ? StarManager.Instance.wrongItemPenalty : 5f;
        penaltyTextUI.text = "-" + penaltyAmount;
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
            goodJobText.text = "Good job! Let's go inside!";
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
        isGameActive = false;
        
        // --- IPASA ANG LOSE PANEL SA STAR MANAGER ---
        if (StarManager.Instance != null)
        {
            StarManager.Instance.EndLevel(false);
        }
    }

    public void PauseGame() { if (pausePanel != null) pausePanel.SetActive(true); Time.timeScale = 0; }
    public void ResumeGame() { if (pausePanel != null) pausePanel.SetActive(false); Time.timeScale = 1; }
    public void RetryLevel() { Time.timeScale = 1; SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void QuitToLevelSelect() { Time.timeScale = 1; SceneManager.LoadScene("TyphoonLevelSelect"); }
}