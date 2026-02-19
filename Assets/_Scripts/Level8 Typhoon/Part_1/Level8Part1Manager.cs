using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Level8Part1Manager : MonoBehaviour
{
    public static Level8Part1Manager instance;

    [Header("Generator Settings")]
    public float maxPower = 100f;
    public float currentPower;
    public float powerDrainRate = 10f; 
    public float powerPerTap = 5f;    
    
    [Header("UI Elements")]
    public Image powerBarFill;
    public TMP_Text missionText;

    // --- BAGONG DAGDAG ---
    [Header("Blackout Effect")]
    public Image darknessOverlay; // Ang itim na screen
    public float maxDarkness = 0.95f; // Hanggang gaano kadilim? (0.95 para medyo maaninag pa)
    // ---------------------

    [Header("Game Data")]
    public int totalFamiliesToServe = 3;
    private int familiesServed = 0;
    [HideInInspector] public bool isGameActive = false;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        currentPower = maxPower;
        isGameActive = true;
        UpdateUI();
    }

    void Update()
    {
        if (!isGameActive) return;

        // 1. Ubusin ang kuryente
        currentPower -= powerDrainRate * Time.deltaTime;
        
        // 2. I-update ang haba ng UI Bar
        if (powerBarFill != null)
        {
            powerBarFill.fillAmount = currentPower / maxPower;

            if (currentPower < 30f) powerBarFill.color = Color.red;
            else if (currentPower < 60f) powerBarFill.color = Color.yellow;
            else powerBarFill.color = Color.green;
        }

        // --- BAGONG DAGDAG: DILIM EFFECT ---
        if (darknessOverlay != null)
        {
            Color overlayColor = darknessOverlay.color;
            
            // Kapag paubos ang kuryente, pataas nang pataas ang transparency (alpha)
            float darknessLevel = 1f - (currentPower / maxPower);
            
            // I-limit natin para hindi 100% pitch black agad hangga't buhay pa ang laro
            overlayColor.a = Mathf.Clamp(darknessLevel, 0f, maxDarkness);
            darknessOverlay.color = overlayColor;
        }
        // ------------------------------------

        // 3. GAME OVER KUNG NAUBOS ANG KURYENTE
        if (currentPower <= 0)
        {
            GameOverBlackout();
        }
    }

    public void TapGenerator()
    {
        if (!isGameActive) return;

        currentPower += powerPerTap;
        if (currentPower > maxPower) currentPower = maxPower; 
    }

    void GameOverBlackout()
    {
        isGameActive = false;
        currentPower = 0;
        if (powerBarFill != null) powerBarFill.fillAmount = 0;
        
        // Pag Game Over, gawin na nating 100% Solid Black!
        if (darknessOverlay != null)
        {
            Color finalColor = darknessOverlay.color;
            finalColor.a = 1f; 
            darknessOverlay.color = finalColor;
        }

        Debug.Log("GAME OVER! TOTAL BLACKOUT!");
    }

    public void AddScore()
    {
        familiesServed++;
        UpdateUI();
        
        if (familiesServed >= totalFamiliesToServe)
     {
         isGameActive = false;
         Debug.Log("YOU WIN! MAY KURYENTE NA ULIT!");

         // Maghihintay ng 3 seconds para makita yung huling "Thank you" bago lumipat
         Invoke("LoadPhase2", 3f); 
     }
    }

    void LoadPhase2()
 {
     SceneManager.LoadScene("Level8_Part2");
 }

    void UpdateUI()
    {
        if (missionText != null)
        {
            missionText.text = "Families Served: " + familiesServed + " / " + totalFamiliesToServe;
        }
    }
}