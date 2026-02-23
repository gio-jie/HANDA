using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

// Ito ang listahan ng mga pwede nating i-spawn (Sunog, Baha, Sugat, etc.)
[System.Serializable]
public class EmergencyType
{
    public string emergencyName;
    public Sprite thoughtBubbleSprite;
    public string requiredRescueUnit; // E.g., "Firetruck"
    public float timeToSolve = 15f;   // Ilang seconds bago tumaas ang Panic Meter pag na-ignore
}

public class CommandCenterManager : MonoBehaviour
{
    public static CommandCenterManager instance;

    [Header("Game Settings")]
    public float totalGameTime = 120f; // 2 Minutes survival time!
    public float spawnInterval = 6f;   // May lilitaw na bagong emergency every 6 seconds

    [Header("Panic Meter (Parusa)")]
    public float currentPanic = 0f;
    public float maxPanic = 100f;
    public Image panicBarFill;
    public float panicPenalty = 25f; // 4 na beses ka magkamali/mabagal, Game Over!

    [Header("UI References")]
    public TMP_Text timerText;
    public TMP_Text statusText;
    public GameObject losePanel;
    public GameObject certificatePanel;

    [Header("Zones & Emergencies")]
    public CrisisZone[] allZones; // I-drag dito ang Zone A, B, C, D
    public EmergencyType[] emergencyTypes; // Ang 5 uri ng problema

    [HideInInspector] public bool isGameActive = true;
    private float spawnTimer = 0f;

    void Awake()
    {
        instance = this;
        Time.timeScale = 1;
    }

    void Start()
    {
        if (certificatePanel) certificatePanel.SetActive(false);
        if (losePanel) losePanel.SetActive(false);
        spawnTimer = 2f; // Maghihintay ng 2 seconds bago lumitaw ang unang emergency
    }

    void Update()
    {
        if (!isGameActive) return;

        // 1. MAIN SURVIVAL TIMER
        if (totalGameTime > 0)
        {
            totalGameTime -= Time.deltaTime;
            UpdateTimerUI();
            
            // PANALO! Naka-survive si Jobert sa 2 minutes!
            if (totalGameTime <= 0) WinGame(); 
        }

        // 2. RANDOM SPAWNER LOGIC
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0)
        {
            SpawnRandomEmergency();
            // Pahihirapin natin nang kaunti ang laro habang tumatagal! Bibilis ang pag-spawn.
            spawnInterval = Mathf.Max(3f, spawnInterval - 0.2f); 
            spawnTimer = spawnInterval;
        }

        // 3. PANIC METER ANIMATION
        if (panicBarFill)
        {
            panicBarFill.fillAmount = Mathf.Lerp(panicBarFill.fillAmount, currentPanic / maxPanic, Time.deltaTime * 5f);
            panicBarFill.color = currentPanic > 60 ? Color.red : (currentPanic > 30 ? Color.yellow : Color.green);
        }

        if (currentPanic >= maxPanic) LoseGame("Umabot sa 100% ang Panic Meter!");
    }

    void UpdateTimerUI()
    {
        int min = Mathf.FloorToInt(totalGameTime / 60);
        int sec = Mathf.FloorToInt(totalGameTime % 60);
        if (timerText) timerText.text = string.Format("{0:00}:{1:00}", min, sec);
        if (totalGameTime <= 10 && timerText) timerText.color = Color.red;
    }

   void SpawnRandomEmergency()
    {
        List<CrisisZone> availableZones = new List<CrisisZone>();
        foreach (CrisisZone zone in allZones)
        {
            if (!zone.hasEmergency) availableZones.Add(zone);
        }

        if (availableZones.Count == 0) return; 

        // DECISION MAKING / TRIAGE MECHANIC! (40% chance na mangyari kung may 2 or more blangkong zones)
        if (availableZones.Count >= 2 && Random.value > 0.6f) 
        {
            // Pumili ng isang random na unit na kailangan nila (e.g., Ambulance)
            EmergencyType chosenEmergency = emergencyTypes[Random.Range(0, emergencyTypes.Length)];
            
            // Kunin ang dalawang magkaibang zones
            CrisisZone zone1 = availableZones[0];
            CrisisZone zone2 = availableZones[1];

            // Triage Timers: Ang isa ay ASAP (halimbawa: 8 seconds)
            float asapTime = 8f; 
            
            // Ang isa ay may sapat na oras para hintaying matapos yung cooldown ng una (halimbawa: 8s + 5s cooldown + 3s palugit = 16s)
            float waitTime = asapTime + 8f; 

            // I-trigger sila nang sabay!
            zone1.TriggerEmergency(chosenEmergency.thoughtBubbleSprite, chosenEmergency.requiredRescueUnit, asapTime);
            zone2.TriggerEmergency(chosenEmergency.thoughtBubbleSprite, chosenEmergency.requiredRescueUnit, waitTime);
            
            Debug.Log("TRIAGE EVENT! " + chosenEmergency.emergencyName + " x2!");
        }
        else 
        {
            // NORMAL SINGLE SPAWN
            CrisisZone chosenZone = availableZones[Random.Range(0, availableZones.Count)];
            EmergencyType chosenEmergency = emergencyTypes[Random.Range(0, emergencyTypes.Length)];
            chosenZone.TriggerEmergency(chosenEmergency.thoughtBubbleSprite, chosenEmergency.requiredRescueUnit, chosenEmergency.timeToSolve);
        }
        
        if (AudioManager.instance) AudioManager.instance.PlaySFX(AudioManager.instance.correctSound); 
    }

    public void AddPanic(string reason)
    {
        if (!isGameActive) return;
        currentPanic += panicPenalty;
        if (statusText) statusText.text = "WARNING: " + reason;
        if (AudioManager.instance) AudioManager.instance.PlaySFX(AudioManager.instance.wrongSound);
        if (PlayerPrefs.GetInt("VibrationOn", 1) == 1) Handheld.Vibrate();
    }

    void WinGame()
    {
        isGameActive = false;
        totalGameTime = 0;
        if (statusText) statusText.text = "LIGTAS ANG BARANGAY!";
        
        // Linisin lahat ng nasa mapa
        foreach (CrisisZone zone in allZones) zone.ClearZone();
        
        if (certificatePanel) certificatePanel.SetActive(true);
        if (AudioManager.instance) { AudioManager.instance.PlaySFX(AudioManager.instance.winSound); AudioManager.instance.PauseBGM(); }
    }

    void LoseGame(string reason)
    {
        isGameActive = false;
        if (statusText) statusText.text = "GAME OVER: " + reason;
        if (losePanel) losePanel.SetActive(true);
        if (AudioManager.instance) { AudioManager.instance.PlaySFX(AudioManager.instance.loseSound); AudioManager.instance.PauseBGM(); }
    }
}