using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class Level6ManagerEQ : MonoBehaviour
{
    public static Level6ManagerEQ instance;

    [Header("Phase 1 Settings")]
    public int buildingsRequired = 5;
    public int buildingsTagged = 0; 
    public bool isGameActive = true;

    [Header("UI Panels")]
    public GameObject pausePanel;
    public GameObject aftershockWarningPanel; 
    public GameObject phase1Panel;
    public GameObject phase2Panel;
    
    [Header("Phase 2 Win Sequence")]
    public GameObject winningPosePanel; 

    [Header("In-Game UI")]
    public GameObject checkIcon; 
    public GameObject xIcon;
    public TMP_Text scoreTextUI;

    [Header("Penalty Animation")]
    public TMP_Text penaltyTextUI;
    public float fallSpeed = 50f;
    public float fadeDuration = 1f;
    private Vector3 penaltyOriginalPos;

    [Header("Phase 2 Settings (Runner)")]
    public Slider distanceSlider;
    public float phase2Duration = 30f; 
    private float phase2Timer = 0f;
    public bool isPhase2Active = false;

    // ===============================================
    // --- UPDATED: PHASE 2 HEALTH SYSTEM (ANIMATED) ---
    // ===============================================
    [Header("Phase 2 Health System")]
    public int currentHealth = 5;
    public Image[] heartIcons; // GINAGAWANG IMAGE PARA MAPALITAN ANG SPRITE
    public Sprite emptyHeartSprite; 
    public Image fallingHeartPrefab; 
    public float heartFallSpeed = 200f;
    public float heartFadeDuration = 1f;

    [Header("Damage Effects")]
    public AudioClip jobertHurtVO; // Dito mo ilalagay yung voice ni Jobert!

    [Header("Phase 2 Warning System")]
    public GameObject[] laneWarningIcons; 

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
        UpdateToggleVisuals();
        
        if (penaltyTextUI != null)
        {
            penaltyOriginalPos = penaltyTextUI.rectTransform.localPosition;
            penaltyTextUI.gameObject.SetActive(false);
        }

        // Itago ang falling heart sa simula
        if (fallingHeartPrefab != null) fallingHeartPrefab.gameObject.SetActive(false);

        UpdateScoreDisplay();
        
        if (AudioManager.instance != null) AudioManager.instance.ResumeBGM();
    }

    void Update()
    {
        // PHASE 1 LOGIC (Tagging Timer lang ang natira)
        if (isGameActive && !isPhase2Active && StarManager.Instance != null)
        {
            if (StarManager.Instance.GetRemainingSeconds() <= 0)
            {
                isGameActive = false;
            }
        }
    }

    // --- PHASE 1 METHODS ---
    void UpdateScoreDisplay() { if (scoreTextUI != null) scoreTextUI.text = buildingsTagged + "/" + buildingsRequired; }

    public void CorrectTagPlaced()
    {
        if (!isGameActive) return;
        buildingsTagged++;
        UpdateScoreDisplay();
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.clickSound);
        StartCoroutine(ShowFeedback(checkIcon));
        if (StarManager.Instance != null) StarManager.Instance.RegisterCorrectItem();

        if (buildingsTagged >= buildingsRequired) TriggerAftershock();
    }

    public void WrongTagPlaced()
    {
        if (!isGameActive) return;
        if (PlayerPrefs.GetInt("VibrationOn", 1) == 1) Handheld.Vibrate(); 
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.warningSound);
        StartCoroutine(ShowFeedback(xIcon));
        if (penaltyTextUI != null) StartCoroutine(AnimatePenaltyText());

        if (StarManager.Instance != null)
        {
            StarManager.Instance.RegisterWrongItem();
            if (StarManager.Instance.GetRemainingSeconds() <= 0 || StarManager.Instance.GetCurrentStars() == 0)
            {
                isGameActive = false;
            }
        }
    }

    public void OnBuildingZoomedIn(Transform building) { }
    public void OnBuildingZoomedOut() { }

    IEnumerator ShowFeedback(GameObject icon) { if(icon != null) { icon.SetActive(true); yield return new WaitForSeconds(1.0f); icon.SetActive(false); } }

    IEnumerator AnimatePenaltyText()
    {
        penaltyTextUI.gameObject.SetActive(true);
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

    // --- PHASE 2 TRANSITION ---
    void TriggerAftershock()
    {
        isGameActive = false; 
        StartCoroutine(Phase2TransitionRoutine());
    }

    IEnumerator Phase2TransitionRoutine()
    {
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.warningSound);

        float shakeDuration = 2f;
        float shakeIntensity = 15f; 
        Vector3 originalPos = phase1Panel.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            float x = Random.Range(-1f, 1f) * shakeIntensity;
            float y = Random.Range(-1f, 1f) * shakeIntensity;
            phase1Panel.transform.localPosition = originalPos + new Vector3(x, y, 0);
            yield return null;
        }
        
        phase1Panel.transform.localPosition = originalPos;

        if (PlayerPrefs.GetInt("VibrationOn", 1) == 1) Handheld.Vibrate();
        if (aftershockWarningPanel != null) aftershockWarningPanel.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        if (aftershockWarningPanel != null) aftershockWarningPanel.SetActive(false);
        if (phase1Panel != null) phase1Panel.SetActive(false);
        if (phase2Panel != null) phase2Panel.SetActive(true);

        isGameActive = true; 
        isPhase2Active = true; 
        phase2Timer = 0f;
        
        if (distanceSlider != null) 
        {
            distanceSlider.value = 0f;
            distanceSlider.gameObject.SetActive(true);
        }
    }

    // ====================================================
    // --- UPDATED PHASE 2 GAMEPLAY METHODS (ANIMATED) ---
    // ====================================================
    public void TakeDamage()
    {
        if (currentHealth <= 0) return;

        currentHealth--;
        
        if (PlayerPrefs.GetInt("VibrationOn", 1) == 1) Handheld.Vibrate();
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.warningSound); 

        // --- BAGONG DAGDAG: EFFECTS AT VOICE OVER ---
        if (AudioManager.instance != null && jobertHurtVO != null)
        {
            AudioManager.instance.PlaySFX(jobertHurtVO); // Tutunog ang "Aray!"
        }

        RunnerPlayer player = FindFirstObjectByType<RunnerPlayer>();
        if (player != null) player.PlayDamageFlicker(); // Tatawagin ang Kundap-Kundap

        MovingRoad road = FindFirstObjectByType<MovingRoad>();
        if (road != null) road.SlowDownRoad(); // Tatawagin ang Slowdown
        // ---------------------------------------------

        if (currentHealth >= 0 && currentHealth < heartIcons.Length)
        {
            Vector3 lostHeartPos = heartIcons[currentHealth].rectTransform.position;
            
            if (emptyHeartSprite != null) heartIcons[currentHealth].sprite = emptyHeartSprite;
            if (fallingHeartPrefab != null) StartCoroutine(AnimateFallingHeart(lostHeartPos));
        }

        if (currentHealth <= 0)
        {
            isGameActive = false;
            if (StarManager.Instance != null) StarManager.Instance.EndLevel(false);
        }
    }

    IEnumerator AnimateFallingHeart(Vector3 startPos)
    {
        fallingHeartPrefab.gameObject.SetActive(true);
        fallingHeartPrefab.rectTransform.position = startPos;

        Color c = fallingHeartPrefab.color; c.a = 1f; fallingHeartPrefab.color = c;
        float timer = 0f;

        while (timer < heartFadeDuration)
        {
            timer += Time.deltaTime;
            fallingHeartPrefab.rectTransform.position += Vector3.down * heartFallSpeed * Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, timer / heartFadeDuration);
            fallingHeartPrefab.color = c;
            yield return null;
        }

        fallingHeartPrefab.gameObject.SetActive(false);
    }

    public void ShowWarning(int lane)
    {
        if (lane >= 0 && lane < laneWarningIcons.Length)
        {
            if (laneWarningIcons[lane] != null)
                StartCoroutine(BlinkWarningRoutine(laneWarningIcons[lane]));
        }
    }

    IEnumerator BlinkWarningRoutine(GameObject icon)
    {
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.warningSound);
        
        for(int i = 0; i < 3; i++) 
        {
            icon.SetActive(true);
            yield return new WaitForSeconds(0.15f);
            icon.SetActive(false);
            yield return new WaitForSeconds(0.15f);
        }
    }

    public void TriggerWinSequence()
    {
        isGameActive = false;
        isPhase2Active = false;

        RunnerPlayer player = FindFirstObjectByType<RunnerPlayer>();
        if (player != null) player.StopRunning();

        Debug.Log("NAKARATING SA SAFE ZONE! PLAYING WIN SEQUENCE...");
        StartCoroutine(WinSequenceRoutine());
    }

    IEnumerator WinSequenceRoutine()
    {
        yield return new WaitForSeconds(2f);
        if (winningPosePanel != null) winningPosePanel.SetActive(true);
        yield return new WaitForSeconds(3f);
        if (winningPosePanel != null) winningPosePanel.SetActive(false);
        if (StarManager.Instance != null) StarManager.Instance.EndLevel(true);
    }

    // --- BUTTONS ---
    public void RetryLevel() { if (AudioManager.instance != null) AudioManager.instance.ResumeBGM(); SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void QuitToLevelSelect() { if (AudioManager.instance != null) AudioManager.instance.ResumeBGM(); SceneManager.LoadScene("EarthquakeLevelSelect"); }
    public void PauseGame() { pausePanel.SetActive(true); Time.timeScale = 0; if (AudioManager.instance != null) AudioManager.instance.PauseBGM(); }
    public void ResumeGame() { pausePanel.SetActive(false); Time.timeScale = 1; if (AudioManager.instance != null) AudioManager.instance.ResumeBGM(); }
    public void ToggleSound() { isSoundOn = !isSoundOn; AudioListener.volume = isSoundOn ? 1 : 0; UpdateToggleVisuals(); }
    public void ToggleMusic() { isMusicOn = !isMusicOn; UpdateToggleVisuals(); }
    void UpdateToggleVisuals() { if(soundButtonImage) soundButtonImage.color = isSoundOn ? onColor : offColor; if(musicButtonImage) musicButtonImage.color = isMusicOn ? onColor : offColor; }
}