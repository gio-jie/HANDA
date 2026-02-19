using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Level8Part2Manager : MonoBehaviour
{
    public static Level8Part2Manager instance;

    [Header("Timer Settings")]
    public float timeLimit = 120f; 
    public float penaltyTime = 5f;

    [Header("UI Feedback")]
    public TMP_Text timerTextUI;
    public GameObject checkIcon;
    public GameObject xIcon;
    
    [Header("Penalty Animation")]
    public TMP_Text penaltyTextUI;
    public float fallSpeed = 50f;
    public float fadeDuration = 1f;

    [Header("Slide Transitions")]
    public RectTransform[] patientUIs; // Ilalagay natin dito sina Patient 1, 2, at 3
    public RectTransform[] tableUIs;   // Ilalagay natin dito ang mga Lamesa nila
    public float slideDuration = 1f;   // Gaano kabilis mag-slide
    private int currentPatientIndex = 0;
    
    // Distansya sa labas ng screen (2500 pixels)
    private float offScreenRight = 2500f; 
    private float offScreenLeft = -2500f;

    private Vector3 penaltyOriginalPos;
    [HideInInspector] public bool isGameActive = true;

    void Awake() 
    { 
        instance = this; 
    }

    void Start()
    {
        if (penaltyTextUI != null)
        {
            penaltyOriginalPos = penaltyTextUI.rectTransform.localPosition;
            penaltyTextUI.gameObject.SetActive(false);
        }
        
        if(checkIcon) checkIcon.SetActive(false);
        if(xIcon) xIcon.SetActive(false);

        // --- I-SETUP ANG MGA PASYENTE SA UMPISA ---
        for (int i = 0; i < patientUIs.Length; i++)
        {
            if (patientUIs[i] == null || tableUIs[i] == null) continue;

            if (i == 0)
            {
                // Si Patient 1 at Table 1 ay nasa gitna (X = 0)
                patientUIs[i].anchoredPosition = new Vector2(0, patientUIs[i].anchoredPosition.y);
                tableUIs[i].anchoredPosition = new Vector2(0, tableUIs[i].anchoredPosition.y);
            }
            else
            {
                // Ang ibang Pasyente ay itatago muna sa labas sa kanan
                patientUIs[i].anchoredPosition = new Vector2(offScreenRight, patientUIs[i].anchoredPosition.y);
                tableUIs[i].anchoredPosition = new Vector2(offScreenRight, tableUIs[i].anchoredPosition.y);
            }
        }
    }

    void Update()
    {
        if (isGameActive && timeLimit > 0)
        {
            timeLimit -= Time.deltaTime;
            UpdateTimerDisplay(timeLimit);
            
            if (timeLimit <= 0) 
            {
                Debug.Log("GAME OVER! Time's Up!");
                isGameActive = false;
            }
        }
    }

    void UpdateTimerDisplay(float time)
    {
        if (timerTextUI != null)
        {
            float sec = Mathf.Max(0, Mathf.FloorToInt(time));
            timerTextUI.text = string.Format("<mspace=0.6em>{0:00}</mspace>", sec);
            timerTextUI.color = (time <= 10) ? Color.red : Color.white;
        }
    }

    public void CorrectStep() { StartCoroutine(ShowIcon(checkIcon)); }
    
    public void WrongItem()
    {
        StartCoroutine(ShowIcon(xIcon));
        timeLimit -= penaltyTime;
        if (penaltyTextUI != null) StartCoroutine(AnimatePenaltyText());
    }

    IEnumerator ShowIcon(GameObject icon)
    {
        if(icon != null) { icon.SetActive(true); yield return new WaitForSeconds(1f); icon.SetActive(false); }
    }

    IEnumerator AnimatePenaltyText()
    {
        penaltyTextUI.gameObject.SetActive(true);
        penaltyTextUI.text = "-" + penaltyTime;
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

    // --- BAGONG DAGDAG: ANG SLIDE ANIMATION TRIGGER ---
    public void NextPatient()
    {
        if (currentPatientIndex < patientUIs.Length - 1)
        {
            StartCoroutine(SlideRoutine(currentPatientIndex, currentPatientIndex + 1));
            currentPatientIndex++;
        }
        else
        {
            Debug.Log("LAHAT NG PASYENTE GAMOT NA! YOU WIN THE LEVEL!");
            isGameActive = false;
            // Dito natin pwedeng i-trigger ang Win Panel next time!
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
            // Pampadulas ng animation (SmoothStep)
            float t = Mathf.SmoothStep(0f, 1f, time / slideDuration); 

            // Ilabas pa-kaliwa ang current
            currentPatient.anchoredPosition = new Vector2(Mathf.Lerp(0, offScreenLeft, t), currentPatient.anchoredPosition.y);
            currentTable.anchoredPosition = new Vector2(Mathf.Lerp(0, offScreenLeft, t), currentTable.anchoredPosition.y);

            // Ipasok mula kanan ang susunod
            nextPatient.anchoredPosition = new Vector2(Mathf.Lerp(offScreenRight, 0, t), nextPatient.anchoredPosition.y);
            nextTable.anchoredPosition = new Vector2(Mathf.Lerp(offScreenRight, 0, t), nextTable.anchoredPosition.y);

            yield return null;
        }
    }
}