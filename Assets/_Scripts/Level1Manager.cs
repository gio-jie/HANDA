using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using System.Collections;
using UnityEngine.SceneManagement; 

public class Level1Manager : MonoBehaviour
{
    public static Level1Manager instance;

    [Header("Game Settings")]
    public int itemsNeeded = 8; 

    [Header("Data (Do not edit)")]
    public int currentScore = 0;
    public bool isGameActive = true; 

    // --- CHECKLIST SYSTEM ---
    [System.Serializable]
    public struct MissionItem
    {
        public string itemName;      
        public GameObject checkmarkUI; 
    }
    
    [Header("Checklist Configuration")]
    public MissionItem[] missionItems; 
    // ----------------------------------------

    [Header("Item Cleanup")]
    public GameObject itemsContainer; 

    [Header("UI Panels")]
    public GameObject pausePanel; 

    [Header("In-Game UI")]
    public TMP_Text scoreTextUI; 
    public GameObject checkIcon; 
    public GameObject xIcon;

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
        UpdateScoreDisplay();
        UpdateToggleVisuals();
        isGameActive = true;

        if (AudioManager.instance != null) 
        {
            AudioManager.instance.ResumeBGM();
        }
    }

    void Update()
    {
        if (isGameActive && StarManager.Instance != null)
        {
            if (StarManager.Instance.GetRemainingSeconds() <= 0)
            {
                isGameActive = false;
                HideAllItems(); // TAWAGIN ANG CLEANUP!
            }
        }
    }

    public void AddScore(string collectedItemName)
    {
        if (!isGameActive) return; 

        bool foundInList = false;
        foreach (var mission in missionItems)
        {
            if (mission.itemName == collectedItemName)
            {
                if(mission.checkmarkUI != null) 
                {
                    mission.checkmarkUI.SetActive(true); 
                }
                foundInList = true;
                break;
            }
        }

        if (!foundInList)
        {
            Debug.LogWarning("Item na nakuha (" + collectedItemName + ") ay wala sa Mission List! Check names.");
        }

        currentScore++;
        UpdateScoreDisplay();
        StartCoroutine(ShowFeedback(checkIcon));

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.correctSound);
        }

        if (StarManager.Instance != null)
        {
            StarManager.Instance.RegisterCorrectItem();
        }

        if (currentScore >= itemsNeeded)
        {
            isGameActive = false; 
            
            PlayerPrefs.SetInt("Level2_Unlocked", 1);
            PlayerPrefs.Save();

            HideAllItems(); // TAWAGIN ANG CLEANUP BAGO MAG-WIN!

            StartCoroutine(LevelCompleteDelay());
        }
    }

    // ==========================================
    // --- BINAGO: ULTIMATE TAGA-LINIS NG ITEMS ---
    // ==========================================
    void HideAllItems()
    {
        // 1. Itago ang buong container sa lamesa
        if (itemsContainer != null) 
        {
            itemsContainer.SetActive(false);
        }

        // 2. Hanapin LAHAT ng objects sa screen na may nakakabit na Drag script at itago sila
        // Gamit natin ito para kahit nakawala siya sa container dahil hawak mo, matatamaan pa rin siya!
        MonoBehaviour[] allScripts = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (MonoBehaviour script in allScripts)
        {
            if (script is UnityEngine.EventSystems.IDragHandler)
            {
                script.gameObject.SetActive(false);
            }
        }
    }
    // ==========================================

    IEnumerator LevelCompleteDelay()
    {
        yield return new WaitForSeconds(1.0f);
        if (StarManager.Instance != null)
        {
            StarManager.Instance.EndLevel(true); 
        }
    }

    void UpdateScoreDisplay() { if(scoreTextUI != null) scoreTextUI.text = "" + currentScore + "/" + itemsNeeded; }
    
    public void WrongItem() 
    { 
        if (!isGameActive) return; 
        StartCoroutine(ShowFeedback(xIcon)); 
        
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.wrongSound);
        if (PlayerPrefs.GetInt("VibrationOn", 1) == 1) Handheld.Vibrate(); 
        
        if (StarManager.Instance != null)
        {
            StarManager.Instance.RegisterWrongItem();
            
            if (StarManager.Instance.GetRemainingSeconds() <= 0 || StarManager.Instance.GetCurrentStars() == 0)
            {
                isGameActive = false;
                HideAllItems(); // TAWAGIN ANG CLEANUP KUNG NA-GAME OVER DAHIL SA MALI!
            }
        }
    }

    IEnumerator ShowFeedback(GameObject icon) { if(icon) { icon.SetActive(true); yield return new WaitForSeconds(1.0f); icon.SetActive(false); } }
    
    public void RetryLevel()
    {
        if (AudioManager.instance != null) AudioManager.instance.ResumeBGM();
        Time.timeScale = 1; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void PauseGame()
    {
        pausePanel.SetActive(true); 
        Time.timeScale = 0; 
        if (AudioManager.instance != null) AudioManager.instance.PauseBGM();
    }
    public void ResumeGame()
    {
        pausePanel.SetActive(false); 
        Time.timeScale = 1; 
        if (AudioManager.instance != null) AudioManager.instance.ResumeBGM();
    }
    public void QuitToLevelSelect()
    {
        if (AudioManager.instance != null) AudioManager.instance.ResumeBGM();
        Time.timeScale = 1; 
        SceneManager.LoadScene("TyphoonLevelSelect");
    }
    public void ToggleSound() { isSoundOn = !isSoundOn; AudioListener.volume = isSoundOn ? 1 : 0; UpdateToggleVisuals(); }
    public void ToggleMusic() { isMusicOn = !isMusicOn; UpdateToggleVisuals(); }
    void UpdateToggleVisuals() { 
        if(soundButtonImage) soundButtonImage.color = isSoundOn ? onColor : offColor; 
        if(musicButtonImage) musicButtonImage.color = isMusicOn ? onColor : offColor; 
    }
}