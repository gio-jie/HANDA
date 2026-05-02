using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class Level7ManagerEQ : MonoBehaviour
{
    public static Level7ManagerEQ instance;

    [Header("Game Status")]
    public bool isGameActive = true;
    public GameObject currentObstacleNearPlayer; 

    [Header("Cinematic Intro Sequence")]
    public Camera mainCamera;
    public MonoBehaviour cameraFollowScript; 
    public Transform playerTransform;
    public Transform exitTransform;

    [Header("UI Panels")]
    public GameObject pausePanel;

    [Header("In-Game UI")]
    public GameObject checkIcon; 
    public GameObject xIcon;
    
    [Header("UI Elements (Tools)")]
    public GameObject keyUITool; 
    public GameObject extinguisherUITool; 

    // ==========================================
    // --- BAGONG DAGDAG PARA SA ITEM ANIMATION ---
    // ==========================================
    [Header("Item Pickup Animation")]
    public Image presentationImage; // Yung image sa gitna ng screen
    public Sprite keySprite;        // Image ng Susi
    public Sprite extinguisherSprite; // Image ng Extinguisher

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

        if (AudioManager.instance != null) AudioManager.instance.ResumeBGM();

        isGameActive = false; 
        if (cameraFollowScript != null) cameraFollowScript.enabled = false; 
        if (StarManager.Instance != null) StarManager.Instance.PauseTimer(); 

        StartCoroutine(IntroSequenceRoutine()); 
    }

    void Update()
    {
        if (isGameActive && StarManager.Instance != null)
        {
            if (StarManager.Instance.GetRemainingSeconds() <= 0)
            {
                isGameActive = false;
            }
        }
    }

    IEnumerator IntroSequenceRoutine()
    {
        yield return new WaitForSeconds(0.5f); 

        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.warningSound);
        if (PlayerPrefs.GetInt("VibrationOn", 1) == 1) Handheld.Vibrate();

        Vector3 originalCamPos = mainCamera.transform.position;
        float shakeDuration = 2f;
        float elapsed = 0f;
        
        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            float x = Random.Range(-0.5f, 0.5f); 
            float y = Random.Range(-0.5f, 0.5f);
            mainCamera.transform.position = originalCamPos + new Vector3(x, y, 0);
            yield return null;
        }

        float panTime = 1.5f;
        elapsed = 0f;
        Vector3 startPan = mainCamera.transform.position;
        Vector3 targetPan = new Vector3(exitTransform.position.x, exitTransform.position.y, startPan.z);

        while (elapsed < panTime)
        {
            elapsed += Time.deltaTime;
            mainCamera.transform.position = Vector3.Lerp(startPan, targetPan, Mathf.SmoothStep(0, 1, elapsed / panTime));
            yield return null;
        }

        yield return new WaitForSeconds(1.5f); 

        elapsed = 0f;
        startPan = mainCamera.transform.position;
        targetPan = new Vector3(playerTransform.position.x, playerTransform.position.y, startPan.z);

        while (elapsed < panTime)
        {
            elapsed += Time.deltaTime;
            mainCamera.transform.position = Vector3.Lerp(startPan, targetPan, Mathf.SmoothStep(0, 1, elapsed / panTime));
            yield return null;
        }

        if (cameraFollowScript != null) cameraFollowScript.enabled = true; 
        isGameActive = true; 
        if (StarManager.Instance != null) StarManager.Instance.isTimerPaused = false; 
    }

    // ==========================================
    // --- LEVEL 7 CORE MECHANICS (MAZE & TOOLS) ---
    // ==========================================
    public void SetCurrentObstacle(GameObject obstacle) { currentObstacleNearPlayer = obstacle; }
    public void ClearCurrentObstacle() { currentObstacleNearPlayer = null; }

    public void PickUpKey()
    {
        if (!isGameActive) return;
        Debug.Log("NAKUHA ANG SUSI!");
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.clickSound); // Pwede mong palitan ng "Tada/Success" sound mamaya
        StartCoroutine(ItemPickupAnimation(keySprite, keyUITool));
    }

    public void PickUpExtinguisher()
    {
        if (!isGameActive) return;
        Debug.Log("NAKUHA ANG FIRE EXTINGUISHER!");
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.clickSound);
        StartCoroutine(ItemPickupAnimation(extinguisherSprite, extinguisherUITool));
    }

    // ========================================================
    // --- BAGONG ANIMATION ROUTINE PARA SA MGA ITEMS ---
    // ========================================================
    IEnumerator ItemPickupAnimation(Sprite itemSprite, GameObject targetUITool)
    {
        // 1. I-pause ang laro at timer
        isGameActive = false;
        if (StarManager.Instance != null) StarManager.Instance.PauseTimer();

        // 2. I-setup yung presentation image sa gitna ng screen
        presentationImage.sprite = itemSprite;
        presentationImage.gameObject.SetActive(true);
        RectTransform rect = presentationImage.rectTransform;
        
        // I-set sa gitna (position 0,0) at maliit na scale
        rect.localPosition = Vector3.zero; 
        rect.localScale = Vector3.zero;

        // 3. Palakihin (Pop-up Effect) papuntang 1.5x Size
        float elapsed = 0f;
        float duration = 0.4f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Sin((elapsed / duration) * Mathf.PI * 0.5f); // Easing out
            rect.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 1.5f, t);
            yield return null;
        }

        // 4. Maghintay nang saglit para makita ng player
        yield return new WaitForSeconds(1.2f);

        // 5. Lumipad papunta sa Toolbelt (Target Position)
        elapsed = 0f;
        duration = 0.5f;
        Vector3 startPos = rect.position; // Screen space position
        Vector3 targetPos = targetUITool.GetComponent<RectTransform>().position; // Pwesto nung tool sa panel

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            rect.position = Vector3.Lerp(startPos, targetPos, t); // Lipad
            rect.localScale = Vector3.Lerp(Vector3.one * 1.5f, Vector3.one, t); // Liliit ulit
            yield return null;
        }

        // 6. Tapusin ang animation, itago ang presentation, at buhayin ang totoong UI tool
        presentationImage.gameObject.SetActive(false);
        if (targetUITool != null) targetUITool.SetActive(true);

        // 7. I-resume ang laro at timer
        isGameActive = true;
        if (StarManager.Instance != null) StarManager.Instance.isTimerPaused = false;
    }

    public void UseTool(string toolNameUsed)
    {
        if (!isGameActive) return;

        if (currentObstacleNearPlayer != null)
        {
            ObstacleData obsData = currentObstacleNearPlayer.GetComponent<ObstacleData>();
            
            if (obsData != null && obsData.correctTool == toolNameUsed)
            {
                Debug.Log("TAMA ANG TOOL!");
                if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.clickSound);
                StartCoroutine(ShowFeedback(checkIcon));
                Destroy(currentObstacleNearPlayer);
                ClearCurrentObstacle(); 
                if (StarManager.Instance != null) StarManager.Instance.RegisterCorrectItem();
            }
            else
            {
                Debug.Log("MALI ANG TOOL!");
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
        }
    }

    public void LevelComplete()
    {
        if (!isGameActive) return;
        Debug.Log("PANALO! Nakarating sa dulo!");
        isGameActive = false;
        if (StarManager.Instance != null) StarManager.Instance.EndLevel(true); 
    }

    IEnumerator ShowFeedback(GameObject icon) { if(icon != null) { icon.SetActive(true); yield return new WaitForSeconds(1.0f); icon.SetActive(false); } }

    IEnumerator AnimatePenaltyText()
    {
        if (penaltyTextUI == null) yield break;
        penaltyTextUI.gameObject.SetActive(true);
        float penaltyAmount = (StarManager.Instance != null) ? StarManager.Instance.wrongItemPenalty : 5f;
        penaltyTextUI.text = "-" + penaltyAmount;
        penaltyTextUI.rectTransform.localPosition = penaltyOriginalPos;
        Color c = penaltyTextUI.color; c.a = 1f; penaltyTextUI.color = c;
        float timer = 0f;
        while (timer < fadeDuration) { timer += Time.deltaTime; penaltyTextUI.rectTransform.localPosition += Vector3.down * fallSpeed * Time.deltaTime; c.a = Mathf.Lerp(1f, 0f, timer / fadeDuration); penaltyTextUI.color = c; yield return null; }
        penaltyTextUI.gameObject.SetActive(false);
    }

    public void RetryLevel() { if (AudioManager.instance != null) AudioManager.instance.ResumeBGM(); SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void QuitToLevelSelect() { if (AudioManager.instance != null) AudioManager.instance.ResumeBGM(); SceneManager.LoadScene("EarthquakeLevelSelect"); }
    public void PauseGame() { if (pausePanel != null) pausePanel.SetActive(true); Time.timeScale = 0; if (AudioManager.instance != null) AudioManager.instance.PauseBGM(); }
    public void ResumeGame() { if (pausePanel != null) pausePanel.SetActive(false); Time.timeScale = 1; if (AudioManager.instance != null) AudioManager.instance.ResumeBGM(); }
    public void ToggleSound() { isSoundOn = !isSoundOn; AudioListener.volume = isSoundOn ? 1 : 0; UpdateToggleVisuals(); }
    public void ToggleMusic() { isMusicOn = !isMusicOn; UpdateToggleVisuals(); }
    void UpdateToggleVisuals() { if(soundButtonImage) soundButtonImage.color = isSoundOn ? onColor : offColor; if(musicButtonImage) musicButtonImage.color = isMusicOn ? onColor : offColor; }
}