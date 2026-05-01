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

    [Header("UI Panels")]
    public GameObject pausePanel;

    [Header("In-Game UI")]
    public GameObject checkIcon; 
    public GameObject xIcon;
    
    [Header("UI Elements (Tools)")]
    public GameObject keyUITool; 
    public GameObject extinguisherUITool; 

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
        
        // I-setup ang penalty text sa simula para hindi nakaharang
        if (penaltyTextUI != null)
        {
            penaltyOriginalPos = penaltyTextUI.rectTransform.localPosition;
            penaltyTextUI.gameObject.SetActive(false);
        }

        if (AudioManager.instance != null) AudioManager.instance.ResumeBGM();
    }

    void Update()
    {
        // I-check kung naubusan na ng oras sa StarManager
        if (isGameActive && StarManager.Instance != null)
        {
            if (StarManager.Instance.GetRemainingSeconds() <= 0)
            {
                isGameActive = false;
            }
        }
    }

    // ==========================================
    // --- LEVEL 7 CORE MECHANICS (MAZE & TOOLS) ---
    // ==========================================
    public void SetCurrentObstacle(GameObject obstacle)
    {
        currentObstacleNearPlayer = obstacle;
    }

    public void ClearCurrentObstacle()
    {
        currentObstacleNearPlayer = null;
    }

    public void PickUpKey()
    {
        if (!isGameActive) return;
        Debug.Log("NAKUHA ANG SUSI!");
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.clickSound);
        if (keyUITool != null) keyUITool.SetActive(true); 
    }

    public void PickUpExtinguisher()
    {
        if (!isGameActive) return;
        Debug.Log("NAKUHA ANG FIRE EXTINGUISHER!");
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.clickSound);
        if (extinguisherUITool != null) extinguisherUITool.SetActive(true); 
    }

    public void UseTool(string toolNameUsed)
    {
        if (!isGameActive) return;

        if (currentObstacleNearPlayer != null)
        {
            ObstacleData obsData = currentObstacleNearPlayer.GetComponent<ObstacleData>();
            
            if (obsData != null && obsData.correctTool == toolNameUsed)
            {
                // --- CORRECT TOOL LOGIC ---
                Debug.Log("TAMA ANG TOOL!");
                if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.clickSound);
                StartCoroutine(ShowFeedback(checkIcon));

                Destroy(currentObstacleNearPlayer);
                ClearCurrentObstacle(); 
                
                if (StarManager.Instance != null) StarManager.Instance.RegisterCorrectItem();
            }
            else
            {
                // --- WRONG TOOL LOGIC ---
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
        
        if (StarManager.Instance != null) 
        {
            StarManager.Instance.EndLevel(true); 
        }
    }

    // ==========================================
    // --- STANDARD UI & ANIMATION UTILITIES ---
    // ==========================================
    IEnumerator ShowFeedback(GameObject icon) 
    { 
        if(icon != null) 
        { 
            icon.SetActive(true); 
            yield return new WaitForSeconds(1.0f); 
            icon.SetActive(false); 
        } 
    }

    IEnumerator AnimatePenaltyText()
    {
        if (penaltyTextUI == null) yield break;

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

    // ==========================================
    // --- STANDARD BUTTON FUNCTIONS ---
    // ==========================================
    public void RetryLevel() { if (AudioManager.instance != null) AudioManager.instance.ResumeBGM(); SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void QuitToLevelSelect() { if (AudioManager.instance != null) AudioManager.instance.ResumeBGM(); SceneManager.LoadScene("EarthquakeLevelSelect"); }
    public void PauseGame() { if (pausePanel != null) pausePanel.SetActive(true); Time.timeScale = 0; if (AudioManager.instance != null) AudioManager.instance.PauseBGM(); }
    public void ResumeGame() { if (pausePanel != null) pausePanel.SetActive(false); Time.timeScale = 1; if (AudioManager.instance != null) AudioManager.instance.ResumeBGM(); }
    
    public void ToggleSound() { isSoundOn = !isSoundOn; AudioListener.volume = isSoundOn ? 1 : 0; UpdateToggleVisuals(); }
    public void ToggleMusic() { isMusicOn = !isMusicOn; UpdateToggleVisuals(); }
    void UpdateToggleVisuals() { if(soundButtonImage) soundButtonImage.color = isSoundOn ? onColor : offColor; if(musicButtonImage) musicButtonImage.color = isMusicOn ? onColor : offColor; }
}