using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

[System.Serializable]
public class RadioScenario
{
    public string scenarioName; 
    public Sprite thoughtImage; 
    public AudioClip audioClip; 
    public float audioLength = 3f; 
    public string correctItem;  
}

public class Level10Part1Manager : MonoBehaviour
{
    public static Level10Part1Manager instance;

    [Header("Scenarios")]
    public RadioScenario[] scenarios;
    private int currentScenarioIndex = 0;

    // --- BAGONG DAGDAG: TUNOG NG RADYO BAGO MAGSALITA ---
    [Header("Radio Effect Audio")]
    public AudioClip radioStaticSound; 
    public float staticDuration = 1.2f; // Gaano katagal tutunog ang static bago ang boses
    // ----------------------------------------------------

    [Header("Health System")]
    public int hearts = 5;
    public Image[] heartIcons; 
    public Sprite emptyHeartSprite; 
    public Image fallingHeartPrefab; 
    public float fallSpeed = 200f;
    public float fadeDuration = 1f;

    [Header("Timer & UI")]
    public float answerTimeLimit = 5f;
    private float currentAnswerTime;
    private bool isWaitingForAnswer = false;
    public TMP_Text timerText;

    public AudioClip tickSound;
    private int lastTickSecond;
    
    [Header("Visuals")]
    public Image thoughtBubbleImage;
    public GameObject xMarkIcon;
    public GameObject checkMarkIcon;
    public TMP_Text feedbackText;

    [Header("Panels")]
    public GameObject losePanel;
    public GameObject phase2TransitionPanel; 
    public GameObject pausePanel; 

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (xMarkIcon) xMarkIcon.SetActive(false);
        if (checkMarkIcon) checkMarkIcon.SetActive(false);
        if (fallingHeartPrefab) fallingHeartPrefab.gameObject.SetActive(false);
        if (feedbackText) feedbackText.text = "";
        
        if (pausePanel) pausePanel.SetActive(false);

        timerText.text = "LISTEN...";
        
        StartCoroutine(PlayScenario(currentScenarioIndex));
    }

    void Update()
    {
        if (isWaitingForAnswer)
        {
            currentAnswerTime -= Time.deltaTime;
            
            int displaySecond = Mathf.CeilToInt(currentAnswerTime);
            
            timerText.text = displaySecond.ToString();
            timerText.color = (displaySecond <= 2) ? Color.red : Color.white;

            if (displaySecond < lastTickSecond && displaySecond > 0)
            {
                lastTickSecond = displaySecond;
                if (tickSound != null && AudioManager.instance != null)
                {
                    AudioManager.instance.PlaySFX(tickSound);
                }
            }

            if (currentAnswerTime <= 0)
            {
                isWaitingForAnswer = false;
                timerText.text = "0";
                WrongAnswer("TIME'S UP!"); 
            }
        }
    }

    IEnumerator PlayScenario(int index)
    {
        isWaitingForAnswer = false;
        
        // --- BINAGO: I-PLAY MUNA ANG RADIO STATIC ---
        timerText.text = "INCOMING..."; 
        timerText.color = Color.cyan;

        if (radioStaticSound != null && AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(radioStaticSound);
        }

        // Maghihintay muna matapos yung static bago ilabas ang bubble at audio
        yield return new WaitForSeconds(staticDuration);
        // --------------------------------------------

        timerText.text = "LISTEN...";
        timerText.color = Color.yellow;

        RadioScenario current = scenarios[index];

        if (current.thoughtImage != null)
        {
            thoughtBubbleImage.sprite = current.thoughtImage;
            thoughtBubbleImage.gameObject.SetActive(true);
        }

        if (AudioManager.instance != null && current.audioClip != null)
        {
            AudioManager.instance.PlaySFX(current.audioClip);
        }

        yield return new WaitForSeconds(current.audioLength);

        currentAnswerTime = answerTimeLimit;
        lastTickSecond = Mathf.CeilToInt(answerTimeLimit) + 1; 
        isWaitingForAnswer = true;
    }

    public void ReceiveItem(string itemDragged)
    {
        if (!isWaitingForAnswer) return; 

        isWaitingForAnswer = false;
        RadioScenario current = scenarios[currentScenarioIndex];

        if (itemDragged == current.correctItem)
        {
            CorrectAnswer();
        }
        else
        {
            WrongAnswer("MALI ANG GAMIT!");
        }
    }

    void CorrectAnswer()
    {
        StartCoroutine(ShowFeedback(checkMarkIcon, "TAMA!"));
        if (AudioManager.instance) AudioManager.instance.PlaySFX(AudioManager.instance.correctSound);
        
        NextScenario();
    }

    void WrongAnswer(string reason)
    {
        StartCoroutine(ShowFeedback(xMarkIcon, reason));
        if (AudioManager.instance) AudioManager.instance.PlaySFX(AudioManager.instance.wrongSound);
        if (PlayerPrefs.GetInt("VibrationOn", 1) == 1) Handheld.Vibrate();

        DeductHeart();
        NextScenario();
    }

    void NextScenario()
    {
        currentScenarioIndex++;
        if (hearts > 0)
        {
            if (currentScenarioIndex < scenarios.Length)
            {
                Invoke("StartNextWithDelay", 2f); 
            }
            else
            {
                Debug.Log("PHASE 1 CLEARED!");
                feedbackText.text = "GOOD JOB! Phase 2 Ready!";
                Invoke("GoToPhase2", 2f);
            }
        }
    }

    void StartNextWithDelay()
    {
        StartCoroutine(PlayScenario(currentScenarioIndex));
    }

    void DeductHeart()
    {
        hearts--;
        if (hearts >= 0 && hearts < heartIcons.Length)
        {
            Vector3 lostHeartPos = heartIcons[hearts].rectTransform.position;
            
            if (emptyHeartSprite != null)
            {
                heartIcons[hearts].sprite = emptyHeartSprite;
            }

            StartCoroutine(AnimateFallingHeart(lostHeartPos));
        }

        if (hearts <= 0)
        {
            isWaitingForAnswer = false;
            timerText.text = "0.0s";
            Invoke("GameOver", 1.5f);
        }
    }

    IEnumerator AnimateFallingHeart(Vector3 startPos)
    {
        fallingHeartPrefab.gameObject.SetActive(true);
        fallingHeartPrefab.rectTransform.position = startPos;

        Color c = fallingHeartPrefab.color; c.a = 1f; fallingHeartPrefab.color = c;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fallingHeartPrefab.rectTransform.position += Vector3.down * fallSpeed * Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            fallingHeartPrefab.color = c;
            yield return null;
        }

        fallingHeartPrefab.gameObject.SetActive(false);
    }

    IEnumerator ShowFeedback(GameObject icon, string msg)
    {
        feedbackText.text = msg;
        if (icon) icon.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        if (icon) icon.SetActive(false);
        feedbackText.text = "";
    }

    void GameOver()
    {
        if (losePanel) losePanel.SetActive(true);
        if (AudioManager.instance) AudioManager.instance.PlaySFX(AudioManager.instance.loseSound);
    }

    void GoToPhase2()
    {
        if (phase2TransitionPanel) phase2TransitionPanel.SetActive(true);
        SceneManager.LoadScene("Level10_Part2"); 
    }

    public void PauseGame() 
    { 
        if(pausePanel) pausePanel.SetActive(true); 
        Time.timeScale = 0; 
    }

    public void ResumeGame() 
    { 
        if(pausePanel) pausePanel.SetActive(false); 
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
}