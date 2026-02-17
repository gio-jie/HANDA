using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using System.Collections;
using UnityEngine.SceneManagement; 

public class Level6Manager : MonoBehaviour
{
    public static Level6Manager instance; 

    [Header("Game Settings (Conveyor)")]
    public int itemsNeeded = 10; 
    public float timeLimit = 60f; 
    public float penaltyTime = 5f; 

    [Header("Star System")]
    public float goldStarThreshold = 30f; 
    public float silverStarThreshold = 15f; 

    [Header("Data (Do not edit)")]
    public int currentScore = 0;
    [HideInInspector] public bool isGameActive = true; 
    private float finalTimeRecorded = 0f; 

    [Header("UI Panels")]
    public GameObject winPanel;
    public GameObject losePanel; 
    public GameObject pausePanel;
    
    [Header("Win Panel Elements")]
    public Image star1; public Image star2; public Image star3;
    public TMP_Text timeFinishedText; public TMP_Text bestScoreText;    
    
    [Header("Lose Panel Elements")]
    public Image loseStar1; public Image loseStar2; public Image loseStar3;
    public TMP_Text loseTimeText; public TMP_Text loseBestScoreText;

    public Color earnedColor = Color.yellow;
    public Color missingColor = Color.gray;

    [Header("In-Game UI")]
    public TMP_Text scoreTextUI; 
    public TMP_Text timerTextUI; 
    public GameObject checkIcon; 
    public GameObject xIcon;

    // --- BAGONG DAGDAG: PENALTY ANIMATION ---
    [Header("Penalty Animation")]
    public TMP_Text penaltyTextUI; // Dito ilalagay yung bagong text
    public float fallSpeed = 50f;  // Gaano kabilis babagsak?
    public float fadeDuration = 1f; // Gaano katagal bago mawala? (1 second)
    private Vector3 penaltyOriginalPos; // Memorya kung saan siya babalik
    // ----------------------------------------

    void Awake()
    {
        instance = this; 
        Time.timeScale = 1;
    }

    void Start()
    {
        UpdateScoreDisplay();
        isGameActive = true;

        // I-save ang orihinal na pwesto ng penalty text para doon siya lagi mag-uumpisa
        if (penaltyTextUI != null)
        {
            penaltyOriginalPos = penaltyTextUI.rectTransform.localPosition;
            penaltyTextUI.gameObject.SetActive(false); // Itago sa simula
        }
    }

    void Update()
    {
        if (isGameActive)
        {
            if (timeLimit > 0)
            {
                timeLimit -= Time.deltaTime;
                UpdateTimerDisplay(timeLimit); 
            }
            else
            {
                FinalizeGameOver();
            }
        }
    }

    void UpdateTimerDisplay(float timeToShow)
    {
        if (timerTextUI != null)
        {
            if (timeToShow < 0) timeToShow = 0;
            float seconds = Mathf.FloorToInt(timeToShow);
            timerTextUI.text = string.Format("<mspace=0.6em>{0:00}</mspace>", seconds);
            timerTextUI.color = (timeToShow <= 10) ? Color.red : Color.white;
        }
    }    

    public void AddScore(string itemName)
    {
        if (!isGameActive) return; 

        currentScore++;
        UpdateScoreDisplay();
        StartCoroutine(ShowFeedback(checkIcon));

        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.correctSound);

        if (currentScore >= itemsNeeded)
        {
            isGameActive = false; 
            finalTimeRecorded = timeLimit; 
            UpdateTimerDisplay(finalTimeRecorded);
            Invoke("ShowWinScreen", 1.5f);
        }
    }

    public void WrongItem() 
    { 
        if (!isGameActive) return; 

        StartCoroutine(ShowFeedback(xIcon)); 
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.wrongSound);
        if (PlayerPrefs.GetInt("VibrationOn", 1) == 1) Handheld.Vibrate(); 
        
        timeLimit -= penaltyTime; 
        if (timeLimit <= 0) FinalizeGameOver(); 
        UpdateTimerDisplay(timeLimit);

        // --- BAGONG DAGDAG ---
        // Tawagin ang animation kapag nagkamali!
        if (penaltyTextUI != null)
        {
            StartCoroutine(AnimatePenaltyText());
        }
        // ---------------------
    }

    // --- BAGONG DAGDAG: ANG LOGIC NG ANIMATION ---
    IEnumerator AnimatePenaltyText()
    {
        // 1. I-reset ang text: Buhayin, ibalik sa orihinal na pwesto, at gawing solid red.
        penaltyTextUI.gameObject.SetActive(true);
        penaltyTextUI.text = "-" + penaltyTime;
        penaltyTextUI.rectTransform.localPosition = penaltyOriginalPos;
        
        Color textColor = penaltyTextUI.color;
        textColor.a = 1f; // 1 means solid color (hindi transparent)
        penaltyTextUI.color = textColor;

        float timer = 0f;

        // 2. Loop para sa pagbagsak at pag-fade
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            // Pabagsakin pababa (Vector3.down)
            penaltyTextUI.rectTransform.localPosition += Vector3.down * fallSpeed * Time.deltaTime;

            // Gawing transparent unti-unti (Lerp from 1 to 0)
            textColor.a = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            penaltyTextUI.color = textColor;

            yield return null; // Maghintay ng next frame
        }

        // 3. Pagkatapos ng 1 second, itago na ulit
        penaltyTextUI.gameObject.SetActive(false);
    }
    // ---------------------------------------------

    IEnumerator ShowFeedback(GameObject icon) 
    { 
        if(icon != null) { icon.SetActive(true); yield return new WaitForSeconds(1.0f); icon.SetActive(false); } 
    }

    void UpdateScoreDisplay() { if(scoreTextUI != null) scoreTextUI.text = currentScore + " / " + itemsNeeded; }

    void FinalizeGameOver()
    {
        timeLimit = 0; isGameActive = false;
        if(timerTextUI != null) timerTextUI.text = "00";
        if(losePanel != null) losePanel.SetActive(true); 
        if (AudioManager.instance != null) { AudioManager.instance.PlaySFX(AudioManager.instance.loseSound); AudioManager.instance.PauseBGM(); }
    }

    void ShowWinScreen()
    {
        winPanel.SetActive(true);
        if (AudioManager.instance != null) { AudioManager.instance.PlaySFX(AudioManager.instance.winSound); AudioManager.instance.PauseBGM(); }

        float scoreTime = finalTimeRecorded; 
        if(star1) star1.color = earnedColor;
        if (scoreTime >= silverStarThreshold && star2) star2.color = earnedColor;
        if (scoreTime >= goldStarThreshold && star3) star3.color = earnedColor;

        float min = Mathf.FloorToInt(scoreTime / 60); float sec = Mathf.FloorToInt(scoreTime % 60);
        if(timeFinishedText != null) timeFinishedText.text = string.Format("Time Left: {0:00}:{1:00}", min, sec);

        PlayerPrefs.SetInt("Level7_Unlocked", 1);
        PlayerPrefs.Save();
    }

    public void RetryLevel() { Time.timeScale = 1; SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void PauseGame() { pausePanel.SetActive(true); Time.timeScale = 0; }
    public void ResumeGame() { pausePanel.SetActive(false); Time.timeScale = 1; }
    public void QuitToLevelSelect() { Time.timeScale = 1; SceneManager.LoadScene("TyphoonLevelSelect"); }
}