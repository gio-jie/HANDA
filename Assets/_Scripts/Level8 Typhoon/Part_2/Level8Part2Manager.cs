using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement; 

public class Level8Part2Manager : MonoBehaviour
{
    public static Level8Part2Manager instance;

    [Header("UI Feedback")]
    public GameObject checkIcon;
    public GameObject xIcon;
    
    [Header("Penalty Animation")]
    public TMP_Text penaltyTextUI;
    public float fallSpeed = 50f;
    public float fadeDuration = 1f;
    private Vector3 penaltyOriginalPos;

    [Header("Slide Transitions")]
    public RectTransform[] patientUIs; 
    public RectTransform[] tableUIs;   
    public float slideDuration = 1f;   
    private int currentPatientIndex = 0;

    [Header("Scene Transition (Fade In)")]
    public Image fadeOverlay; // Ang itim na screen na liliwanag
    
    private float offScreenRight = 2500f; 
    private float offScreenLeft = -2500f;

    [Header("UI Panels")]
    public GameObject pausePanel;

    [HideInInspector] public bool isGameActive = true;

    void Awake() 
    { 
        instance = this; 
        Time.timeScale = 1; 
    }

    void Start()
    {
        isGameActive = true;

        if (penaltyTextUI != null)
        {
            penaltyOriginalPos = penaltyTextUI.rectTransform.localPosition;
            penaltyTextUI.gameObject.SetActive(false);
        }
        
        if(checkIcon) checkIcon.SetActive(false);
        if(xIcon) xIcon.SetActive(false);

        // I-setup ang mga pasyente
        for (int i = 0; i < patientUIs.Length; i++)
        {
            if (patientUIs[i] == null || tableUIs[i] == null) continue;

            if (i == 0)
            {
                patientUIs[i].anchoredPosition = new Vector2(0, patientUIs[i].anchoredPosition.y);
                tableUIs[i].anchoredPosition = new Vector2(0, tableUIs[i].anchoredPosition.y);
            }
            else
            {
                patientUIs[i].anchoredPosition = new Vector2(offScreenRight, patientUIs[i].anchoredPosition.y);
                tableUIs[i].anchoredPosition = new Vector2(offScreenRight, tableUIs[i].anchoredPosition.y);
            }
        }

        // SIMULAN ANG FADE IN
        if (fadeOverlay != null)
        {
            fadeOverlay.gameObject.SetActive(true);
            Color c = fadeOverlay.color;
            c.a = 1f; // Solid Black sa simula
            fadeOverlay.color = c;
            StartCoroutine(FadeInRoutine());
        }
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

    public void CorrectStep() 
    { 
        if (!isGameActive) return;

        StartCoroutine(ShowIcon(checkIcon)); 
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.correctSound);

        // --- IPASA SA STAR MANAGER ANG PAGKA-TAMA ---
        if (StarManager.Instance != null)
        {
            StarManager.Instance.RegisterCorrectItem();
        }
    }
    
    public void WrongItem()
    {
        if (!isGameActive) return;

        StartCoroutine(ShowIcon(xIcon));
        
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.wrongSound);
        if (PlayerPrefs.GetInt("VibrationOn", 1) == 1) Handheld.Vibrate(); 

        if (penaltyTextUI != null) StartCoroutine(AnimatePenaltyText());

        // --- TAWAGIN ANG STAR MANAGER PARA SA PENALTY ---
        if (StarManager.Instance != null)
        {
            StarManager.Instance.RegisterWrongItem();
            
            if (StarManager.Instance.GetRemainingSeconds() <= 0 || StarManager.Instance.GetCurrentStars() == 0)
            {
                isGameActive = false;
            }
        }
    }

    IEnumerator ShowIcon(GameObject icon)
    {
        if(icon != null) { icon.SetActive(true); yield return new WaitForSeconds(1f); icon.SetActive(false); }
    }

    IEnumerator AnimatePenaltyText()
    {
        penaltyTextUI.gameObject.SetActive(true);
        // Basahin ang penalty time mula sa Inspector ng StarManager!
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

    public void NextPatient()
    {
        if (!isGameActive) return;

        if (currentPatientIndex < patientUIs.Length - 1)
        {
            StartCoroutine(SlideRoutine(currentPatientIndex, currentPatientIndex + 1));
            currentPatientIndex++;
        }
        else
        {
            Debug.Log("LAHAT NG PASYENTE GAMOT NA! YOU WIN THE LEVEL!");
            isGameActive = false;
            StartCoroutine(LevelCompleteDelay());
        }
    }

    IEnumerator LevelCompleteDelay()
    {
        yield return new WaitForSeconds(1.5f); // 1.5 seconds delay tulad ng sa lumang script mo
        if (StarManager.Instance != null)
        {
            StarManager.Instance.EndLevel(true); 
        }
    }

    IEnumerator SlideRoutine(int currentIndex, int nextIndex)
    {
        float time = 0;
        RectTransform currentPatient = patientUIs[currentIndex];
        RectTransform currentTable = tableUIs[currentIndex];
        RectTransform nextPatient = patientUIs[nextIndex];
        RectTransform nextTable = tableUIs[nextIndex];

        while (time < slideDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, time / slideDuration); 

            currentPatient.anchoredPosition = new Vector2(Mathf.Lerp(0, offScreenLeft, t), currentPatient.anchoredPosition.y);
            currentTable.anchoredPosition = new Vector2(Mathf.Lerp(0, offScreenLeft, t), currentTable.anchoredPosition.y);

            nextPatient.anchoredPosition = new Vector2(Mathf.Lerp(offScreenRight, 0, t), nextPatient.anchoredPosition.y);
            nextTable.anchoredPosition = new Vector2(Mathf.Lerp(offScreenRight, 0, t), nextTable.anchoredPosition.y);

            yield return null;
        }
    }

    // --- BUTTONS ---
    // PAALALA: Ang RetryLevel na ito ay maglo-load ng "Active Scene" (Part 2 lang). 
    // Kung gusto mo siyang bumalik sa Part 1 kapag namatay, palitan mo yung line sa baba ng: SceneManager.LoadScene("Level8_Part1");
    public void RetryLevel() { Time.timeScale = 1; SceneManager.LoadScene(SceneManager.GetActiveScene().name); } 
    public void PauseGame() { if(pausePanel != null) pausePanel.SetActive(true); Time.timeScale = 0; }
    public void ResumeGame() { if(pausePanel != null) pausePanel.SetActive(false); Time.timeScale = 1; }
    public void QuitToLevelSelect() { Time.timeScale = 1; SceneManager.LoadScene("TyphoonLevelSelect"); }

    IEnumerator FadeInRoutine()
    {
        float timer = 0f;
        Color c = fadeOverlay.color;
        float fadeTime = 1.5f; 

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, timer / fadeTime); 
            fadeOverlay.color = c;
            yield return null;
        }

        fadeOverlay.gameObject.SetActive(false); 
    }
}