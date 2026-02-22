using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

[System.Serializable]
public class RadioScenario
{
    public string scenarioName; // Para madali mong ma-identify sa Inspector
    public Sprite thoughtImage; // Ang lilitaw sa thought bubble ni Jobert
    public AudioClip audioClip; // Ang boses sa radyo
    public float audioLength = 3f; // Gaano katagal bago mag-start ang 5-sec timer
    public string correctItem;  // Ang kailangang i-drag (e.g., "Cellphone")
}

public class Level10Part1Manager : MonoBehaviour
{
    public static Level10Part1Manager instance;

    [Header("Scenarios")]
    public RadioScenario[] scenarios;
    private int currentScenarioIndex = 0;

    [Header("Health System")]
    public int hearts = 5;
    public Image[] heartIcons; 
    public Sprite emptyHeartSprite; // --- BAGONG DAGDAG: Ang blank/outline na puso ---
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
    public GameObject phase2TransitionPanel; // Papunta sa susunod na phase

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

        timerText.text = "LISTEN...";
        
        // Simulan ang unang tanong
        StartCoroutine(PlayScenario(currentScenarioIndex));
    }

    void Update()
    {
        if (isWaitingForAnswer)
        {
            currentAnswerTime -= Time.deltaTime;
            
            // Gawing Whole Number (CeilToInt para yung 4.9 ay maging 5, 0.1 ay maging 1)
            int displaySecond = Mathf.CeilToInt(currentAnswerTime);
            
            // I-update ang text (E.g., "5", "4", "3")
            timerText.text = displaySecond.ToString();
            timerText.color = (displaySecond <= 2) ? Color.red : Color.white;

            // Tumunog ng "toot" kapag bumaba ang numero!
            if (displaySecond < lastTickSecond && displaySecond > 0)
            {
                lastTickSecond = displaySecond;
                if (tickSound != null && AudioManager.instance != null)
                {
                    AudioManager.instance.PlaySFX(tickSound);
                }
            }

            // Kapag naubos na ang oras
            if (currentAnswerTime <= 0)
            {
                isWaitingForAnswer = false;
                timerText.text = "0";
                WrongAnswer("TIME'S UP!"); // Automatic na itong magpi-play ng X sound at maglalagas ng puso!
            }
        }
    }

    IEnumerator PlayScenario(int index)
    {
        isWaitingForAnswer = false;
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

        // --- BINAGO NATIN ITO PARA SA TIMER SOUND ---
        currentAnswerTime = answerTimeLimit;
        lastTickSecond = Mathf.CeilToInt(answerTimeLimit) + 1; // Para tumunog agad paglabas ng "5"
        isWaitingForAnswer = true;
    }

    public void ReceiveItem(string itemDragged)
    {
        if (!isWaitingForAnswer) return; // Wag pansinin kung hindi pa tapos magsalita

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
                Invoke("StartNextWithDelay", 2f); // Maghintay 2 seconds bago ang next question
            }
            else
            {
                // PASADO SA LAHAT NG 10 QUESTIONS!
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
            // Kunin ang pwesto ng pusong mawawala bago palitan ang drawing
            Vector3 lostHeartPos = heartIcons[hearts].rectTransform.position;
            
            // --- DITO NATIN BINAGO: Papalitan ng empty heart outline! ---
            if (emptyHeartSprite != null)
            {
                heartIcons[hearts].sprite = emptyHeartSprite;
            }

            // Simulan ang hulog animation (Yung falling heart prefab na solid red ang mahuhulog)
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
        // SceneManager.LoadScene("Level10_Part2"); 
    }
}