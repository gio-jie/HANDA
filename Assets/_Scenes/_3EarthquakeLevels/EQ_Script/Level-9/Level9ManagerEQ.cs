using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class Level9ManagerEQ : MonoBehaviour
{
    public static Level9ManagerEQ instance;

    [Header("Game Status")]
    public bool isGameActive = true;
    public GameObject currentObstacleNearPlayer; 
    public int totalVehicles = 8; // Siguraduhing naka-set ito base sa dami ng obstacles mo
    public int clearedVehicles = 0;

    [Header("Camera & Player")]
    public Camera mainCamera;
    public MonoBehaviour cameraFollowScript; 
    public Transform playerTransform;

    [Header("UI Panels")]
    public GameObject pausePanel;

    [Header("In-Game UI")]
    public TMP_Text scoreTextUI; // BAGONG DAGDAG: Dito mo i-drag yung Text na magpapakita ng 0/8
    public GameObject checkIcon; 
    public GameObject xIcon;
    public GameObject extinguisherUITool; 
    public GameObject vehicleUITool;

    [Header("Item Pickup Animation")]
    public Image presentationImage;       
    public Sprite extinguisherSprite; 
    public Sprite vehicleSprite;

    [Header("Penalty Animation")]
    public TMP_Text penaltyTextUI;
    public float fallSpeed = 50f;
    public float fadeDuration = 1f;
    private Vector3 penaltyOriginalPos;

    [Header("Toggle Buttons")]
    public Image soundButtonImage; 
    public Image musicButtonImage;
    public Color onColor = Color.green;
    public Color offColor = Color.gray;
    private bool isSoundOn = true;
    private bool isMusicOn = true;

    // ==========================================
    // --- DUCK, COVER, & HOLD QTE ---
    // ==========================================
    [Header("Duck, Cover, Hold QTE")]
    public GameObject qtePanel; 
    public Slider qteSlider; 
    public float eqInterval = 10f; 
    public float qteTimeLimit = 5f; 
    
    private float eqTimer = 0f;
    private float currentQTETime = 0f;
    public bool isQTEActive = false;
    
    private float drainRate = 0.4f; 
    private float fillPerClick = 0.15f; 
    private int waveCount = 1;

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

        if (presentationImage != null) presentationImage.gameObject.SetActive(false);
        if (qtePanel != null) qtePanel.SetActive(false);

        if (AudioManager.instance != null) AudioManager.instance.ResumeBGM();

        isGameActive = false; 
        if (cameraFollowScript != null) cameraFollowScript.enabled = false; 

        UpdateScoreDisplay(); // Update agad ng UI text pagka-start!

        StartCoroutine(StartGameRoutine()); 
    }

    void Update()
    {
        if (isGameActive && !isQTEActive && StarManager.Instance != null)
        {
            if (StarManager.Instance.GetRemainingSeconds() <= 0)
            {
                isGameActive = false;
            }

            eqTimer += Time.deltaTime;
            if (eqTimer >= eqInterval)
            {
                StartCoroutine(TriggerEarthquakeQTE());
            }
        }

        if (isQTEActive && qteSlider != null)
        {
            qteSlider.value -= drainRate * Time.deltaTime; 
            currentQTETime += Time.deltaTime; 

            if (currentQTETime >= qteTimeLimit)
            {
                FailQTE();
            }
        }
    }

    // ==========================================
    // --- UI SCORE UPDATE ---
    // ==========================================
    void UpdateScoreDisplay()
    {
        if (scoreTextUI != null)
        {
            scoreTextUI.text = clearedVehicles + "/" + totalVehicles;
        }
    }

    // ==========================================
    // --- LINDOL QTE MECHANICS ---
    // ==========================================
    IEnumerator TriggerEarthquakeQTE()
    {
        isGameActive = false; 
        if (StarManager.Instance != null) StarManager.Instance.PauseTimer(); 
        
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.warningSound);
        if (PlayerPrefs.GetInt("VibrationOn", 1) == 1) Handheld.Vibrate();

        // 1. PATAYIN MUNA ANG CAMERA FOLLOW PARA HINDI PIGILAN ANG SHAKE
        if (cameraFollowScript != null) cameraFollowScript.enabled = false;

        // 2. Screen Shake Effect (1 Second)
        Vector3 originalCamPos = mainCamera.transform.position;
        float shakeTimer = 0f;
        while (shakeTimer < 1f) 
        {
            shakeTimer += Time.deltaTime;
            float x = Random.Range(-0.3f, 0.3f); 
            float y = Random.Range(-0.3f, 0.3f);
            mainCamera.transform.position = originalCamPos + new Vector3(x, y, 0);
            yield return null;
        }
        mainCamera.transform.position = originalCamPos;

        // 3. BUHAYIN ULIT ANG CAMERA FOLLOW PAGKATAPOS NG SHAKE
        if (cameraFollowScript != null) cameraFollowScript.enabled = true;

        // 4. DITO DAPAT MAG-START ANG TIMER AT QTE!
        isQTEActive = true;      
        currentQTETime = 0f;     
        qteSlider.value = 0f;    
        qtePanel.SetActive(true); 
    }

    public void SpamClickDuckCoverHold()
    {
        if (!isQTEActive) return;

        qteSlider.value += fillPerClick;
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.clickSound);

        if (qteSlider.value >= 1f)
        {
            SuccessQTE();
        }
    }

    void SuccessQTE()
    {
        isQTEActive = false;
        qtePanel.SetActive(false);
        eqTimer = 0f; 

        waveCount++;
        drainRate += 0.15f; 
        
        isGameActive = true; 
        if (StarManager.Instance != null) StarManager.Instance.isTimerPaused = false; 
    }

    void FailQTE()
    {
        isQTEActive = false;
        qtePanel.SetActive(false);
        eqTimer = 0f; 

        if (PlayerPrefs.GetInt("VibrationOn", 1) == 1) Handheld.Vibrate();
        if (penaltyTextUI != null) StartCoroutine(AnimatePenaltyText());

        if (StarManager.Instance != null)
        {
            StarManager.Instance.RegisterWrongItem(); 
            StarManager.Instance.isTimerPaused = false; 
            if (StarManager.Instance.GetRemainingSeconds() <= 0) isGameActive = false;
        }
        
        isGameActive = true; 
    }

    // ==========================================
    // --- START GAME & CORE MECHANICS ---
    // ==========================================
    IEnumerator StartGameRoutine()
    {
        while (StarManager.Instance == null) { yield return null; }
        if (cameraFollowScript != null) cameraFollowScript.enabled = true; 
        isGameActive = true; 
        StarManager.Instance.isTimerPaused = false; 
    }

    public void SetCurrentObstacle(GameObject obstacle) { currentObstacleNearPlayer = obstacle; }
    public void ClearCurrentObstacle() { currentObstacleNearPlayer = null; }

    public void PickUpVehicle()
    {
        if (!isGameActive) return;
        StartCoroutine(ItemPickupAnimation(vehicleSprite, vehicleUITool));
    }

    IEnumerator ItemPickupAnimation(Sprite itemSprite, GameObject targetUITool)
    {
        isGameActive = false;
        if (StarManager.Instance != null) StarManager.Instance.PauseTimer();
        presentationImage.sprite = itemSprite; presentationImage.gameObject.SetActive(true);
        RectTransform rect = presentationImage.rectTransform; rect.localPosition = Vector3.zero; rect.localScale = Vector3.zero;

        float elapsed = 0f; float duration = 0.4f;
        while (elapsed < duration) { elapsed += Time.deltaTime; rect.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 1.5f, Mathf.Sin((elapsed / duration) * Mathf.PI * 0.5f)); yield return null; }
        yield return new WaitForSeconds(1.2f);

        elapsed = 0f; duration = 0.5f;
        Vector3 startPos = rect.position; Vector3 targetPos = targetUITool.GetComponent<RectTransform>().position; 
        while (elapsed < duration) { elapsed += Time.deltaTime; float t = elapsed / duration; rect.position = Vector3.Lerp(startPos, targetPos, t); rect.localScale = Vector3.Lerp(Vector3.one * 1.5f, Vector3.one, t); yield return null; }

        presentationImage.gameObject.SetActive(false);
        if (targetUITool != null) targetUITool.SetActive(true);
        isGameActive = true; if (StarManager.Instance != null) StarManager.Instance.isTimerPaused = false;
    }

    public void UseTool(string toolNameUsed)
    {
        if (!isGameActive && !isQTEActive) return;

        if (currentObstacleNearPlayer != null)
        {
            ObstacleData obsData = currentObstacleNearPlayer.GetComponent<ObstacleData>();
            if (obsData != null && obsData.correctTool == toolNameUsed)
            {
                StartCoroutine(ShowFeedback(checkIcon));
                Destroy(currentObstacleNearPlayer);
                ClearCurrentObstacle(); 

                // ==========================================
                // MAGDADAGDAG TAYO SA SCORE AT IUUPDATE ANG UI
                // ==========================================
                // ==========================================
                // MAGDADAGDAG TAYO SA SCORE AT IUUPDATE ANG UI
                // ==========================================
                clearedVehicles++;
                UpdateScoreDisplay(); 

                if (StarManager.Instance != null) StarManager.Instance.RegisterCorrectItem();

                // ---> BAGONG DAGDAG: I-CHECK KUNG TAPOS NA ANG LAHAT NG SASAKYAN <---
                if (clearedVehicles >= totalVehicles)
                {
                    LevelComplete();
                }
                UpdateScoreDisplay(); 

                if (StarManager.Instance != null) StarManager.Instance.RegisterCorrectItem();
            }
            else
            {
                if (PlayerPrefs.GetInt("VibrationOn", 1) == 1) Handheld.Vibrate(); 
                StartCoroutine(ShowFeedback(xIcon));
                if (penaltyTextUI != null) StartCoroutine(AnimatePenaltyText());
                if (StarManager.Instance != null) { StarManager.Instance.RegisterWrongItem(); if (StarManager.Instance.GetRemainingSeconds() <= 0 || StarManager.Instance.GetCurrentStars() == 0) isGameActive = false; }
            }
        }
    }

    public void LevelComplete() { if (!isGameActive) return; isGameActive = false; if (StarManager.Instance != null) StarManager.Instance.EndLevel(true); }
    IEnumerator ShowFeedback(GameObject icon) { if(icon != null) { icon.SetActive(true); yield return new WaitForSeconds(1.0f); icon.SetActive(false); } }
    IEnumerator AnimatePenaltyText() { if (penaltyTextUI == null) yield break; penaltyTextUI.gameObject.SetActive(true); float penaltyAmount = (StarManager.Instance != null) ? StarManager.Instance.wrongItemPenalty : 5f; penaltyTextUI.text = "-" + penaltyAmount; penaltyTextUI.rectTransform.localPosition = penaltyOriginalPos; Color c = penaltyTextUI.color; c.a = 1f; penaltyTextUI.color = c; float timer = 0f; while (timer < fadeDuration) { timer += Time.deltaTime; penaltyTextUI.rectTransform.localPosition += Vector3.down * fallSpeed * Time.deltaTime; c.a = Mathf.Lerp(1f, 0f, timer / fadeDuration); penaltyTextUI.color = c; yield return null; } penaltyTextUI.gameObject.SetActive(false); }

    // ==========================================
    // --- DEFAULT GAME MANAGER BUTTONS ---
    // ==========================================
    public void RetryLevel() { if (AudioManager.instance != null) AudioManager.instance.ResumeBGM(); SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void QuitToLevelSelect() { if (AudioManager.instance != null) AudioManager.instance.ResumeBGM(); SceneManager.LoadScene("EarthquakeLevelSelect"); } 
    public void PauseGame() { if (pausePanel != null) pausePanel.SetActive(true); Time.timeScale = 0; if (AudioManager.instance != null) AudioManager.instance.PauseBGM(); }
    public void ResumeGame() { if (pausePanel != null) pausePanel.SetActive(false); Time.timeScale = 1; if (AudioManager.instance != null) AudioManager.instance.ResumeBGM(); }
    public void ToggleSound() { isSoundOn = !isSoundOn; AudioListener.volume = isSoundOn ? 1 : 0; UpdateToggleVisuals(); }
    public void ToggleMusic() { isMusicOn = !isMusicOn; UpdateToggleVisuals(); }
    void UpdateToggleVisuals() { if(soundButtonImage) soundButtonImage.color = isSoundOn ? onColor : offColor; if(musicButtonImage) musicButtonImage.color = isMusicOn ? onColor : offColor; }
}