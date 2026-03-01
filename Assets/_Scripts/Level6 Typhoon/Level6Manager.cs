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

    [Header("Data (Do not edit)")]
    public int currentScore = 0;
    [HideInInspector] public bool isGameActive = true; 

    [Header("UI Panels")]
    public GameObject pausePanel;
    
    [Header("In-Game UI")]
    public TMP_Text scoreTextUI; 
    public GameObject checkIcon; 
    public GameObject xIcon;

    [Header("Penalty Animation")]
    public TMP_Text penaltyTextUI; 
    public float fallSpeed = 50f;  
    public float fadeDuration = 1f; 
    private Vector3 penaltyOriginalPos; 

    void Awake()
    {
        instance = this; 
        Time.timeScale = 1;
    }

    void Start()
    {
        UpdateScoreDisplay();
        isGameActive = true;

        if (penaltyTextUI != null)
        {
            penaltyOriginalPos = penaltyTextUI.rectTransform.localPosition;
            penaltyTextUI.gameObject.SetActive(false); 
        }
    }

    void Update()
    {
        if (isGameActive && StarManager.Instance != null)
        {
            if (StarManager.Instance.GetRemainingSeconds() <= 0)
            {
                isGameActive = false;
            }
        }
    }

    public void AddScore(string itemName)
    {
        if (!isGameActive) return; 

        currentScore++;
        UpdateScoreDisplay();
        StartCoroutine(ShowFeedback(checkIcon));

        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.correctSound);

        // --- IPASA SA STAR MANAGER ANG PAGKA-TAMA ---
        if (StarManager.Instance != null)
        {
            StarManager.Instance.RegisterCorrectItem();
        }

        if (currentScore >= itemsNeeded)
        {
            isGameActive = false; 
            StartCoroutine(LevelCompleteDelay());
        }
    }

    IEnumerator LevelCompleteDelay()
    {
        yield return new WaitForSeconds(1.0f);
        if (StarManager.Instance != null)
        {
            StarManager.Instance.EndLevel(true); 
        }
    }

    public void WrongItem() 
    { 
        if (!isGameActive) return; 

        StartCoroutine(ShowFeedback(xIcon)); 
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.wrongSound);
        if (PlayerPrefs.GetInt("VibrationOn", 1) == 1) Handheld.Vibrate(); 
        
        // Tawagin ang animation kapag nagkamali
        if (penaltyTextUI != null)
        {
            StartCoroutine(AnimatePenaltyText());
        }

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

    IEnumerator AnimatePenaltyText()
    {
        penaltyTextUI.gameObject.SetActive(true);
        // Note: Nakadepende ngayon ang text na lalabas sa 'wrongItemPenalty' sa Inspector ng StarManager!
        float penaltyAmount = (StarManager.Instance != null) ? StarManager.Instance.wrongItemPenalty : 5f;
        penaltyTextUI.text = "-" + penaltyAmount;
        
        penaltyTextUI.rectTransform.localPosition = penaltyOriginalPos;
        
        Color textColor = penaltyTextUI.color;
        textColor.a = 1f; 
        penaltyTextUI.color = textColor;

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            penaltyTextUI.rectTransform.localPosition += Vector3.down * fallSpeed * Time.deltaTime;
            textColor.a = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            penaltyTextUI.color = textColor;
            yield return null; 
        }

        penaltyTextUI.gameObject.SetActive(false);
    }

    IEnumerator ShowFeedback(GameObject icon) 
    { 
        if(icon != null) { icon.SetActive(true); yield return new WaitForSeconds(1.0f); icon.SetActive(false); } 
    }

    void UpdateScoreDisplay() { if(scoreTextUI != null) scoreTextUI.text = currentScore + " / " + itemsNeeded; }

    public void RetryLevel() { Time.timeScale = 1; SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void PauseGame() { pausePanel.SetActive(true); Time.timeScale = 0; }
    public void ResumeGame() { pausePanel.SetActive(false); Time.timeScale = 1; }
    public void QuitToLevelSelect() { Time.timeScale = 1; SceneManager.LoadScene("TyphoonLevelSelect"); }
}