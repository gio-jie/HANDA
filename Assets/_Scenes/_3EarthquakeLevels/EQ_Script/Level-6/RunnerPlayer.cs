using UnityEngine;
using UnityEngine.UI;
using System.Collections; 

public class RunnerPlayer : MonoBehaviour
{
    [Header("Lane Settings")]
    public float laneDistance = 150f; 
    public float moveSpeed = 15f;     
    
    [Header("Running Animation (Wobble)")]
    public float wobbleSpeed = 15f;  
    public float wobbleAngle = 8f;   

    private int currentLane = 1;      
    private RectTransform rectTransform;
    private Vector3 originalRotation;
    private Image playerImage; 

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalRotation = rectTransform.localEulerAngles;
        playerImage = GetComponent<Image>(); 
    }

    void Update()
    {
        if (Level6ManagerEQ.instance != null && !Level6ManagerEQ.instance.isGameActive) return;

        float targetX = (currentLane - 1) * laneDistance;
        Vector2 targetPosition = new Vector2(targetX, rectTransform.anchoredPosition.y);
        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPosition, Time.deltaTime * moveSpeed);

        float zRot = Mathf.Sin(Time.time * wobbleSpeed) * wobbleAngle;
        rectTransform.localRotation = Quaternion.Euler(originalRotation.x, originalRotation.y, originalRotation.z + zRot);
    }

    public void MoveLeft()
    {
        if (Level6ManagerEQ.instance != null && !Level6ManagerEQ.instance.isGameActive) return;
        if (currentLane > 0)
        {
            currentLane--; 
            if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.clickSound); 
        }
    }

    public void MoveRight()
    {
        if (Level6ManagerEQ.instance != null && !Level6ManagerEQ.instance.isGameActive) return;
        if (currentLane < 2)
        {
            currentLane++; 
            if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.clickSound);
        }
    }

    public int GetCurrentLane() { return currentLane; }

    public void StopRunning()
    {
        rectTransform.localRotation = Quaternion.Euler(originalRotation);
        this.enabled = false; 
    }

    // ==========================================
    // --- BAGONG DAGDAG: KUNDAP-KUNDAP EFFECT ---
    // ==========================================
    public void PlayDamageFlicker()
    {
        if (playerImage != null) StartCoroutine(FlickerRoutine());
    }

    IEnumerator FlickerRoutine()
    {
        // Mag-blink ng 5 beses (Pula tapos babalik sa normal)
        for (int i = 0; i < 5; i++)
        {
            playerImage.color = new Color(1f, 0.5f, 0.5f, 0.5f); // Medyo red na transparent
            yield return new WaitForSeconds(0.1f);
            playerImage.color = Color.white; // Balik sa normal na itsura
            yield return new WaitForSeconds(0.1f);
        }
    }
}