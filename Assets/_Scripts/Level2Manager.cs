using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; 
using System.Collections;

public class Level2Manager : MonoBehaviour
{
    public static Level2Manager instance;

    [Header("Game Settings")]
    public int totalTasks = 5; 
    public int tasksDone = 0;

    [Header("Game State")]
    public string currentSelectedTool = "None";
    public bool isGameActive = true;

    [Header("UI Panels")]
    public GameObject pausePanel;
    public GameObject hazardPanel; // (Optional kung meron)

    [Header("In-Game UI")]
    public TMP_Text scoreText;
    public GameObject xIcon; // Penalty Feedback

    [Header("Tools Setup")]
    public GameObject[] toolButtons; 

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
        if(scoreText != null) scoreText.text = "Repairs: 0/" + totalTasks;
        
        // Resume Music
        if (AudioManager.instance != null) AudioManager.instance.ResumeBGM();
    }

    void Update()
    {
        if (isGameActive && StarManager.Instance != null)
        {
            // Kung naubos ang oras sa StarManager, I-GAME OVER!
            if (StarManager.Instance.GetRemainingSeconds() <= 0)
            {
                isGameActive = false;
            }
        }
    }

    // --- LEVEL 2 SPECIFIC LOGIC (Repair) ---

    public void SelectTool(GameObject clickedButton)
    {
        if (!isGameActive) return;

        string toolName = "";
        if (clickedButton.name.Contains("Hammer")) toolName = "Hammer";
        else if (clickedButton.name.Contains("Tape")) toolName = "Tape";
        else if (clickedButton.name.Contains("Phone")) toolName = "Phone";
        else if (clickedButton.name.Contains("Toy")) toolName = "Toy";
        else if (clickedButton.name.Contains("Remote")) toolName = "Remote";

        currentSelectedTool = toolName;
        
        // Visual Reset
        foreach (GameObject btn in toolButtons)
        {
            if (btn != null)
            {
                btn.transform.localScale = Vector3.one;
                Outline outline = btn.GetComponent<Outline>();
                if (outline != null) outline.enabled = false;
            }
        }

        // Visual Highlight
        clickedButton.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        Outline clickedOutline = clickedButton.GetComponent<Outline>();
        if (clickedOutline != null) clickedOutline.enabled = true;
        
        // Play Sound
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.clickSound);
    }

    public void TaskCompleted()
    {
        if (!isGameActive) return;

        tasksDone++;
        if(scoreText != null) scoreText.text = "Repairs: " + tasksDone + "/" + totalTasks;

        // Play Correct Sound
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.correctSound);

        // --- IPASA SA STAR MANAGER ANG PAGKA-TAMA ---
        if (StarManager.Instance != null)
        {
            StarManager.Instance.RegisterCorrectItem();
        }

        if (tasksDone >= totalTasks)
        {
            // WIN!
            isGameActive = false;
            StartCoroutine(LevelCompleteDelay());
        }
    }

    // --- BAGONG DAGDAG: DELAY COROUTINE PARA HINDI MABIGLA ---
    IEnumerator LevelCompleteDelay()
    {
        yield return new WaitForSeconds(1.0f);
        if (StarManager.Instance != null)
        {
            StarManager.Instance.EndLevel(true); 
        }
    }

    public void ApplyPenalty()
    {
        if (!isGameActive) return;
        
        // Play Wrong Sound
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.wrongSound);

        if(xIcon != null)
        {
            xIcon.SetActive(true);
            Invoke("HideX", 1f);
        }

        if (PlayerPrefs.GetInt("VibrationOn", 1) == 1)
        {
            Handheld.Vibrate(); 
        }

        // --- TAWAGIN ANG STAR MANAGER PARA SA PENALTY ---
        if (StarManager.Instance != null)
        {
            StarManager.Instance.RegisterWrongItem();
            
            // Kapag na-game over na ang Star Manager
            if (StarManager.Instance.GetRemainingSeconds() <= 0 || StarManager.Instance.GetCurrentStars() == 0)
            {
                isGameActive = false;
            }
        }
    }

    void HideX() { if(xIcon != null) xIcon.SetActive(false); }

    // --- STANDARD BUTTONS ---
    public void RetryLevel() { if (AudioManager.instance != null) AudioManager.instance.ResumeBGM(); SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void QuitToLevelSelect() { if (AudioManager.instance != null) AudioManager.instance.ResumeBGM(); SceneManager.LoadScene("TyphoonLevelSelect"); }
    public void PauseGame() { pausePanel.SetActive(true); Time.timeScale = 0; if (AudioManager.instance != null) AudioManager.instance.PauseBGM(); }
    public void ResumeGame() { pausePanel.SetActive(false); Time.timeScale = 1; if (AudioManager.instance != null) AudioManager.instance.ResumeBGM(); }
    
    public void ToggleSound() { isSoundOn = !isSoundOn; AudioListener.volume = isSoundOn ? 1 : 0; UpdateToggleVisuals(); }
    public void ToggleMusic() { isMusicOn = !isMusicOn; UpdateToggleVisuals(); }
    void UpdateToggleVisuals() { if(soundButtonImage) soundButtonImage.color = isSoundOn ? onColor : offColor; if(musicButtonImage) musicButtonImage.color = isMusicOn ? onColor : offColor; }
}