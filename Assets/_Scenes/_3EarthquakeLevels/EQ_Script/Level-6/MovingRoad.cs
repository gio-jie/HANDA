using UnityEngine;
using System.Collections; 

public class MovingRoad : MonoBehaviour
{
    [Header("Speed Settings")]
    public float baseSpeed = 800f;       // Ang panimulang bilis
    public float maxSpeed = 1200f;       // Ang pinakasagad na bilis sa dulo
    public float accelerationRate = 15f; // Ilang bilis ang idadagdag kada segundo
    
    [Header("Finish Line Settings")]
    public float finishLineY = -24000f; 
    
    private RectTransform rect;
    private bool hasFinished = false;
    private bool isSlowedDown = false;
    private float currentActualSpeed;    // Ito yung totoong ina-apply na galaw

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        currentActualSpeed = baseSpeed;
    }

    void Update()
    {
        if (Level6ManagerEQ.instance != null && !Level6ManagerEQ.instance.isGameActive) return;
        if (hasFinished) return;

        // ==========================================
        // --- BAGONG DAGDAG: ACCELERATION LOGIC ---
        // ==========================================
        // Dahan-dahang pabilisin ang base speed hanggang maabot ang max speed
        if (baseSpeed < maxSpeed)
        {
            baseSpeed += accelerationRate * Time.deltaTime;
        }

        // Kung HINDI siya naka-slow down (dahil nasaktan), gamitin ang mabilis na base speed
        if (!isSlowedDown)
        {
            currentActualSpeed = baseSpeed;
        }

        // I-slide pababa ang kalsada gamit ang totoong bilis
        rect.anchoredPosition += Vector2.down * currentActualSpeed * Time.deltaTime;

        // --- UPDATE SLIDER ---
        if (Level6ManagerEQ.instance != null && Level6ManagerEQ.instance.distanceSlider != null)
        {
            float progress = rect.anchoredPosition.y / finishLineY;
            Level6ManagerEQ.instance.distanceSlider.value = Mathf.Clamp01(progress);
        }

        // --- FINISH LINE LOGIC ---
        if (rect.anchoredPosition.y <= finishLineY)
        {
            hasFinished = true;
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, finishLineY);
            if (Level6ManagerEQ.instance.distanceSlider != null) Level6ManagerEQ.instance.distanceSlider.value = 1f; 
            if (Level6ManagerEQ.instance != null) Level6ManagerEQ.instance.TriggerWinSequence();
        }
    }

    // --- SLOWDOWN LOGIC (KAPAG TINAMAAN) ---
    public void SlowDownRoad()
    {
        StartCoroutine(SlowDownRoutine());
    }

    IEnumerator SlowDownRoutine()
    {
        isSlowedDown = true;
        
        // Hahatiin natin ang current speed para bumagal
        currentActualSpeed = baseSpeed / 2.5f; 
        
        yield return new WaitForSeconds(1f); // Tatagal ng 1 second ang bagal
        
        // Pagkatapos ng 1 second, babalik na siya sa patuloy na bumibilis na kalsada
        isSlowedDown = false; 
    }
}