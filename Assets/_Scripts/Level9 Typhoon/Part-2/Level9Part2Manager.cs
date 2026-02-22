using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Level9Part2Manager : MonoBehaviour
{
    public static Level9Part2Manager instance;

    [Header("Player Status")]
    public bool isWearingBoots = false;
    public GameObject bootsOnPlayerImage;

    [Header("Infection Meter (Leptospirosis/Dengue)")]
    public float currentInfection = 0f;
    public float maxInfection = 100f;
    public Image infectionBarFill;
    public float infectionPenalty = 25f;

    [Header("Game Settings & Timer")]
    public float timeLimit = 60f; // 1 minute para maglinis
    public int totalHazards = 3; // Putik, Tubig, Disinfect(Floor)
    [HideInInspector] public int clearedHazards = 0;

    [Header("UI Feedback")]
    public TMP_Text statusText;
    public TMP_Text timerTextUI;
    public GameObject checkIcon;
    public GameObject xIcon;

    [Header("Star System")]
    public float goldStarThreshold = 30f;
    public float silverStarThreshold = 15f;

    [Header("UI Panels")]
    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject pausePanel;

    [Header("Win Panel Elements")]
    public Image star1; public Image star2; public Image star3;
    public TMP_Text timeFinishedText;
    public Color earnedColor = Color.yellow;

    [HideInInspector] public bool isGameActive = true;

    void Awake()
    {
        instance = this;
        Time.timeScale = 1;
        if (bootsOnPlayerImage != null) bootsOnPlayerImage.SetActive(false);
    }

    void Update()
    {
        if (!isGameActive) return;

        // Timer Logic
        if (timeLimit > 0)
        {
            timeLimit -= Time.deltaTime;
            UpdateTimerDisplay(timeLimit);
            if (timeLimit <= 0) FinalizeGameOver("Naubusan ng oras!");
        }

        // Infection Bar Logic
        if (infectionBarFill != null)
        {
            infectionBarFill.fillAmount = currentInfection / maxInfection;
            if (currentInfection > 70f) infectionBarFill.color = Color.red;
            else if (currentInfection > 40f) infectionBarFill.color = Color.yellow;
            else infectionBarFill.color = Color.green;
        }

        if (currentInfection >= maxInfection) FinalizeGameOver("Na-infect ng sakit si Jobert!");
    }

    void UpdateTimerDisplay(float time)
    {
        if (timerTextUI != null)
        {
            if (time < 0) time = 0;
            timerTextUI.text = string.Format("<mspace=0.6em>{0:00}</mspace>", Mathf.FloorToInt(time));
            timerTextUI.color = (time <= 10) ? Color.red : Color.white;
        }
    }

    public void WearBoots()
    {
        isWearingBoots = true;
        if (bootsOnPlayerImage != null) bootsOnPlayerImage.SetActive(true);
        if (statusText != null) statusText.text = "Ligtas na! Pwede nang maglinis.";
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.correctSound);
    }

    // --- TATAWAGIN PAG TAMA ANG LINIS ---
    public void HazardCleaned()
    {
        clearedHazards++;
        if (checkIcon != null) { checkIcon.SetActive(true); Invoke("HideCheck", 1f); }
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.correctSound);

        if (clearedHazards >= totalHazards)
        {
            isGameActive = false;
            statusText.text = "Ligtas at malinis na ang bahay!";
            Invoke("ShowWinScreen", 1.5f);
        }
    }

    // --- TATAWAGIN PAG MALI O DELIKADO ---
    public void AddInfection(string warningMessage)
    {
        if (!isGameActive) return;

        currentInfection += infectionPenalty;
        if (statusText != null) statusText.text = warningMessage;
        
        if (xIcon != null) { xIcon.SetActive(true); Invoke("HideX", 1f); }
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.wrongSound);
        if (PlayerPrefs.GetInt("VibrationOn", 1) == 1) Handheld.Vibrate();
    }

    void HideCheck() { if(checkIcon) checkIcon.SetActive(false); }
    void HideX() { if(xIcon) xIcon.SetActive(false); }

    void FinalizeGameOver(string reason)
    {
        timeLimit = 0;
        isGameActive = false;
        if (statusText != null) statusText.text = "GAME OVER: " + reason;
        if (losePanel != null) losePanel.SetActive(true);
        if (AudioManager.instance != null) { AudioManager.instance.PlaySFX(AudioManager.instance.loseSound); AudioManager.instance.PauseBGM(); }
    }

    void ShowWinScreen()
    {
        if (winPanel != null) winPanel.SetActive(true);
        if (AudioManager.instance != null) { AudioManager.instance.PlaySFX(AudioManager.instance.winSound); AudioManager.instance.PauseBGM(); }

        float scoreTime = timeLimit;
        if (star1) star1.color = earnedColor;
        if (scoreTime >= silverStarThreshold && star2) star2.color = earnedColor;
        if (scoreTime >= goldStarThreshold && star3) star3.color = earnedColor;

        float min = Mathf.FloorToInt(scoreTime / 60); float sec = Mathf.FloorToInt(scoreTime % 60);
        if (timeFinishedText != null) timeFinishedText.text = string.Format("Time Left: {0:00}:{1:00}", min, sec);

        PlayerPrefs.SetInt("Level10_Unlocked", 1);
        PlayerPrefs.Save();
    }

    public void RetryLevel() { Time.timeScale = 1; SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void PauseGame() { if (pausePanel != null) pausePanel.SetActive(true); Time.timeScale = 0; }
    public void ResumeGame() { if (pausePanel != null) pausePanel.SetActive(false); Time.timeScale = 1; }
    public void QuitToLevelSelect() { Time.timeScale = 1; SceneManager.LoadScene("TyphoonLevelSelect"); }
}