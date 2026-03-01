using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement; 

[System.Serializable]
public class EmergencyType
{
    public string emergencyName;
    public Sprite thoughtBubbleSprite;
    public string requiredRescueUnit; 
    public float timeToSolve = 15f;   
}

public class CommandCenterManager : MonoBehaviour
{
    public static CommandCenterManager instance;

    [Header("Game Settings")]
    public float totalGameTime = 120f; 
    public float spawnInterval = 6f;   

    [Header("Panic Meter (Parusa)")]
    public float currentPanic = 0f;
    public float maxPanic = 100f;
    public Image panicBarFill;
    public float panicPenalty = 25f; 

    [Header("UI References")]
    public TMP_Text timerText;
    public GameObject pausePanel; 

    [Header("Win Transition Visuals")]
    public GameObject timesUpPopup; // Dito ide-drag yung buong popup (Jobert + Text)

    [Header("Certificate Transition")]
    public string certificateSceneName = "Stage1_Certificate"; 

    [Header("Zones & Emergencies")]
    public CrisisZone[] allZones; 
    public EmergencyType[] emergencyTypes; 

    [Header("Audio SFX")]
    public AudioClip alertSpawnSound; 

    [HideInInspector] public bool isGameActive = true;
    private float spawnTimer = 0f;

    void Awake()
    {
        instance = this;
        Time.timeScale = 1;
    }

    void Start()
    {
        if (pausePanel) pausePanel.SetActive(false); 
        if (timesUpPopup) timesUpPopup.SetActive(false);
        spawnTimer = 2f; 
    }

    void Update()
    {
        if (!isGameActive) return;

        if (totalGameTime > 0)
        {
            totalGameTime -= Time.deltaTime;
            UpdateTimerUI();
            
            if (totalGameTime <= 0) WinGame(); 
        }

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0)
        {
            SpawnRandomEmergency();
            spawnInterval = Mathf.Max(3f, spawnInterval - 0.2f); 
            spawnTimer = spawnInterval;
        }

        if (panicBarFill)
        {
            panicBarFill.fillAmount = Mathf.Lerp(panicBarFill.fillAmount, currentPanic / maxPanic, Time.deltaTime * 5f);
            panicBarFill.color = currentPanic > 60 ? Color.red : (currentPanic > 30 ? Color.yellow : Color.green);
        }

        if (currentPanic >= maxPanic) LoseGame("Umabot sa 100% ang Panic Meter!");
    }

    void UpdateTimerUI()
    {
        float displayTime = Mathf.Max(0, totalGameTime); 

        int min = Mathf.FloorToInt(displayTime / 60);
        int sec = Mathf.FloorToInt(displayTime % 60);
        
        if (timerText) timerText.text = string.Format("{0:00}:{1:00}", min, sec);
        if (displayTime <= 10 && timerText) timerText.color = Color.red;
    }

    void SpawnRandomEmergency()
    {
        List<CrisisZone> availableZones = new List<CrisisZone>();
        foreach (CrisisZone zone in allZones)
        {
            if (!zone.hasEmergency) availableZones.Add(zone);
        }

        if (availableZones.Count == 0) return; 

        if (availableZones.Count >= 2 && Random.value > 0.6f) 
        {
            EmergencyType chosenEmergency = emergencyTypes[Random.Range(0, emergencyTypes.Length)];
            CrisisZone zone1 = availableZones[0];
            CrisisZone zone2 = availableZones[1];

            float asapTime = 8f; 
            float waitTime = asapTime + 8f; 

            zone1.TriggerEmergency(chosenEmergency.thoughtBubbleSprite, chosenEmergency.requiredRescueUnit, asapTime);
            zone2.TriggerEmergency(chosenEmergency.thoughtBubbleSprite, chosenEmergency.requiredRescueUnit, waitTime);
        }
        else 
        {
            CrisisZone chosenZone = availableZones[Random.Range(0, availableZones.Count)];
            EmergencyType chosenEmergency = emergencyTypes[Random.Range(0, emergencyTypes.Length)];
            chosenZone.TriggerEmergency(chosenEmergency.thoughtBubbleSprite, chosenEmergency.requiredRescueUnit, chosenEmergency.timeToSolve);
        }
        
        if (alertSpawnSound != null)
        {
            AudioSource.PlayClipAtPoint(alertSpawnSound, Camera.main.transform.position, 1f);
        }
    }

    public void AddPanic(string reason)
    {
        if (!isGameActive) return;
        currentPanic += panicPenalty;
        if (AudioManager.instance) AudioManager.instance.PlaySFX(AudioManager.instance.wrongSound);
        if (PlayerPrefs.GetInt("VibrationOn", 1) == 1) Handheld.Vibrate();
    }

    void WinGame()
    {
        if (!isGameActive) return; 
        
        isGameActive = false; 
        totalGameTime = 0;
        
        foreach (CrisisZone zone in allZones) zone.ClearZone();

        if (AudioManager.instance) AudioManager.instance.PauseBGM();

        StartCoroutine(WinTransitionRoutine());
    }

    IEnumerator WinTransitionRoutine()
    {
        // 1. Ipakita ang text at image ni Jobert
        if (timesUpPopup) timesUpPopup.SetActive(true);

        // 2. Tiyak na tatambay na ito ng 3 seconds!
        yield return new WaitForSeconds(3.0f);

        // 3. Itago muna ang Popup
        if (timesUpPopup) timesUpPopup.SetActive(false);

        // 4. IPASA SA STAR MANAGER ANG WIN PANEL
        if (StarManager.Instance != null)
        {
            StarManager.Instance.EndLevel(true);
        }
    }

    void LoseGame(string reason)
    {
        if (!isGameActive) return;
        isGameActive = false;
        
        if (StarManager.Instance != null)
        {
            StarManager.Instance.EndLevel(false);
        }
    }

    public void GoToCertificateScene()
    {
        Time.timeScale = 1;

        // --- BAGONG DAGDAG: I-UNLOCK ANG STAGE 2 (FLOOD) ---
        PlayerPrefs.SetInt("Flood_Unlocked", 1);
        PlayerPrefs.Save();
        // ---------------------------------------------------

        SceneManager.LoadScene(certificateSceneName);
    }

    public void PauseGame() { if(pausePanel) pausePanel.SetActive(true); Time.timeScale = 0; }
    public void ResumeGame() { if(pausePanel) pausePanel.SetActive(false); Time.timeScale = 1; }
    public void RetryLevel() { Time.timeScale = 1; SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void QuitToLevelSelect() { Time.timeScale = 1; SceneManager.LoadScene("TyphoonLevelSelect"); }
}