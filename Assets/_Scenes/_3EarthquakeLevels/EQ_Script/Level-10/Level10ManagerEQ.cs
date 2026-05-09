using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro; // Siguraduhing nandito ito para sa TextMeshPro

public class Level10ManagerEQ : MonoBehaviour
{
    public static Level10ManagerEQ instance;

    [Header("Manual Question Panels")]
    public GameObject[] questionPanels; 
    public int currentQuestionIndex = 0;

    [Header("Timer Setup")]
    public Slider timerSlider;
    public Image sliderFillImage; 
    public Gradient timerGradient; 
    
    [Header("Reading Phase")]
    public float readingTimeLimit = 3.5f; 
    public Color readingColor = Color.cyan; 
    private float readingTimer;
    private bool isReadingPhase = false;

    private float questionTimer = 5f;
    private bool isQuestionActive = false;
    private bool isClickable = true; 

    [Header("In-Game UI & Feedback")]
    public TMP_Text scoreTextUI; // BAGONG DAGDAG: Dito i-drag ang "0/10" text mo
    public GameObject checkIcon; 
    public GameObject xIcon;

    [Header("Health System")]
    public int currentHealth = 5;
    public Image[] heartIcons; 
    public Sprite emptyHeartSprite; 
    
    [Header("Falling Heart Animation")]
    public Image fallingHeartPrefab; 
    public float heartFallSpeed = 200f;
    public float heartFadeDuration = 1f;

    [Header("Default UI Panels & Buttons")]
    public GameObject pausePanel;
    public Image soundButtonImage; 
    public Image musicButtonImage;
    public Color onColor = Color.green;
    public Color offColor = Color.gray;
    private bool isSoundOn = true;
    private bool isMusicOn = true;
    private bool isGameActive = true;

    void Awake()
    {
        instance = this;
        Time.timeScale = 1;
    }

    void Start()
    {
        UpdateToggleVisuals();
        
        if (StarManager.Instance != null) StarManager.Instance.SetStarsByHearts(currentHealth);
        if (fallingHeartPrefab != null) fallingHeartPrefab.gameObject.SetActive(false);

        foreach (GameObject panel in questionPanels)
        {
            if (panel != null) panel.SetActive(false);
        }

        UpdateScoreDisplay(); // Ipakita agad ang 0/10 sa simula
        ShowQuestion(0);
    }

    void Update()
    {
        if (isGameActive && isQuestionActive)
        {
            if (isReadingPhase)
            {
                readingTimer -= Time.deltaTime;
                
                if (readingTimer <= 0)
                {
                    isReadingPhase = false;
                    questionTimer = 5f; 
                }
            }
            else
            {
                questionTimer -= Time.deltaTime;
                float timeNormalized = questionTimer / 5f; 
                
                if (timerSlider != null) timerSlider.value = timeNormalized;
                if (sliderFillImage != null) sliderFillImage.color = timerGradient.Evaluate(timeNormalized);

                if (questionTimer <= 0)
                {
                    TimeRanOut();
                }
            }
        }
    }

    // ==========================================
    // --- SCORE TEXT UI UPDATE ---
    // ==========================================
    void UpdateScoreDisplay()
    {
        if (scoreTextUI != null && questionPanels != null)
        {
            // Sinisiguro natin na hindi lalagpas sa 10/10 yung display text
            int displayIndex = Mathf.Min(currentQuestionIndex, questionPanels.Length);
            scoreTextUI.text = displayIndex + "/" + questionPanels.Length;
        }
    }

    void ShowQuestion(int index)
    {
        if (index >= questionPanels.Length)
        {
            LevelComplete();
            return;
        }

        questionPanels[index].SetActive(true);
        
        readingTimer = readingTimeLimit;
        isReadingPhase = true;
        questionTimer = 5f; 
        
        if (timerSlider != null) timerSlider.value = 1f; 
        if (sliderFillImage != null) sliderFillImage.color = readingColor; 
        
        isQuestionActive = true;
        isClickable = true; 
    }

    public void AnswerCorrect() { if (!isClickable) return; ProcessAnswer(true); }
    public void AnswerWrong() { if (!isClickable) return; ProcessAnswer(false); }
    void TimeRanOut() { if (!isClickable) return; ProcessAnswer(false); }

    void ProcessAnswer(bool isCorrect)
    {
        isQuestionActive = false;
        isClickable = false;

        if (isCorrect)
        {
            if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.clickSound);
            StartCoroutine(ShowFeedbackAndNext(checkIcon));
        }
        else
        {
            TakeDamage();
            StartCoroutine(ShowFeedbackAndNext(xIcon));
        }
    }

    IEnumerator ShowFeedbackAndNext(GameObject icon)
    {
        if (icon != null) icon.SetActive(true);
        yield return new WaitForSeconds(1f); 
        if (icon != null) icon.SetActive(false);

        if (currentQuestionIndex < questionPanels.Length)
        {
            questionPanels[currentQuestionIndex].SetActive(false);
        }

        currentQuestionIndex++;
        UpdateScoreDisplay(); // I-update ang UI matapos masagutan yung tanong
        
        if (isGameActive) ShowQuestion(currentQuestionIndex);
    }

    void TakeDamage()
    {
        if (currentHealth <= 0) return;

        currentHealth--;
        
        if (PlayerPrefs.GetInt("VibrationOn", 1) == 1) Handheld.Vibrate();
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.warningSound); 

        if (StarManager.Instance != null) StarManager.Instance.SetStarsByHearts(currentHealth);

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

    void LevelComplete() { isGameActive = false; if (StarManager.Instance != null) StarManager.Instance.EndLevel(true); }

    public void RetryLevel() { if (AudioManager.instance != null) AudioManager.instance.ResumeBGM(); SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void QuitToLevelSelect() { if (AudioManager.instance != null) AudioManager.instance.ResumeBGM(); SceneManager.LoadScene("LevelSelect"); } 
    public void PauseGame() { if (pausePanel != null) pausePanel.SetActive(true); Time.timeScale = 0; if (AudioManager.instance != null) AudioManager.instance.PauseBGM(); }
    public void ResumeGame() { if (pausePanel != null) pausePanel.SetActive(false); Time.timeScale = 1; if (AudioManager.instance != null) AudioManager.instance.ResumeBGM(); }
    public void ToggleSound() { isSoundOn = !isSoundOn; AudioListener.volume = isSoundOn ? 1 : 0; UpdateToggleVisuals(); }
    public void ToggleMusic() { isMusicOn = !isMusicOn; UpdateToggleVisuals(); }
    void UpdateToggleVisuals() { if(soundButtonImage) soundButtonImage.color = isSoundOn ? onColor : offColor; if(musicButtonImage) musicButtonImage.color = isMusicOn ? onColor : offColor; }
}