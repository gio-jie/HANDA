using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // Need this for Scene loading

public class Level2Manager : MonoBehaviour
{
    [Header("Game Status")]
    public string currentSelectedTool = "None";
    public int totalTasks = 5; 
    private int tasksDone = 0;
    private bool isGameActive = true;
    
    [Header("Timer Settings")]
    public float timeLimit = 60f;
    public TMP_Text timerText;

    [Header("UI References")]
    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject pausePanel;
    public TMP_Text scoreText;
    public GameObject xIcon; // Feedback pag mali

    [Header("Tools Setup")]
    public GameObject[] toolButtons; // Listahan ng Buttons (Hammer, Tape, etc)

    [Header("Toggle Buttons (Pause Panel)")]
    public Image soundButtonImage; 
    public Image musicButtonImage;
    public Color onColor = Color.green;
    public Color offColor = Color.gray;
    private bool isSoundOn = true;
    private bool isMusicOn = true;

    void Start()
    {
        Time.timeScale = 1; // Unfreeze time
        UpdateToggleVisuals();
        if(scoreText != null) scoreText.text = "Repairs: 0/" + totalTasks;
    }

    void Update()
    {
        if (isGameActive)
        {
            if (timeLimit > 0)
            {
                timeLimit -= Time.deltaTime;
                if(timerText != null) 
                    timerText.text = Mathf.CeilToInt(timeLimit).ToString();
            }
            else
            {
                timeLimit = 0;
                if(timerText != null) timerText.text = "0";
                GameOver();
            }
        }
    }

    // --- TOOL SELECTION ---
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
        
        // Reset Visuals
        foreach (GameObject btn in toolButtons)
        {
            if (btn != null)
            {
                btn.transform.localScale = Vector3.one;
                Outline outline = btn.GetComponent<Outline>();
                if (outline != null) outline.enabled = false;
            }
        }

        // Highlight Clicked
        clickedButton.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        Outline clickedOutline = clickedButton.GetComponent<Outline>();
        if (clickedOutline != null) clickedOutline.enabled = true;
    }

    // --- GAMEPLAY LOGIC ---
    public void TaskCompleted()
    {
        tasksDone++;
        if(scoreText != null) scoreText.text = "Repairs: " + tasksDone + "/" + totalTasks;

        if (tasksDone >= totalTasks)
        {
            isGameActive = false;
            Debug.Log("LEVEL COMPLETE!");
            Invoke("ShowWinScreen", 1.0f);
        }
    }

    public void ApplyPenalty()
    {
        if (!isGameActive) return;
        
        timeLimit -= 5f; // Penalty
        if(xIcon != null)
        {
            xIcon.SetActive(true);
            Invoke("HideX", 1f);
        }
    }

    void HideX()
    {
        if(xIcon != null) xIcon.SetActive(false);
    }

    void GameOver()
    {
        isGameActive = false;
        if(losePanel != null) losePanel.SetActive(true);
    }

    void ShowWinScreen()
    {
        if(winPanel != null) winPanel.SetActive(true);
    }

    // --- PAUSE & NAVIGATION ---
    public void PauseGame()
    {
        if(pausePanel != null) pausePanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        if(pausePanel != null) pausePanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void RetryLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitToLevelSelect()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("TyphoonLevelSelect");
    }

    // --- TOGGLES ---
    public void ToggleSound()
    {
        isSoundOn = !isSoundOn;
        AudioListener.volume = isSoundOn ? 1 : 0;
        UpdateToggleVisuals();
    }

    public void ToggleMusic()
    {
        isMusicOn = !isMusicOn;
        UpdateToggleVisuals();
    }

    void UpdateToggleVisuals()
    {
        if(soundButtonImage != null) soundButtonImage.color = isSoundOn ? onColor : offColor;
        if(musicButtonImage != null) musicButtonImage.color = isMusicOn ? onColor : offColor;
    }
}