using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CrisisZone : MonoBehaviour, IDropHandler
{
    [Header("Zone Visuals")]
    public GameObject radioAndBubbleUI; 
    public Image thoughtBubbleImage;    
    public TMP_Text emergencyTimerText; 
    public GameObject checkmarkIcon; 
    public GameObject xMarkIcon;     

    [Header("Zone State")]
    public bool hasEmergency = false;
    // --- BAGONG DAGDAG: PRENO PARA SA TIMER ---
    public bool isResolved = false; 
    // ------------------------------------------
    public string requiredUnit = "";    
    public float currentEmergencyTimer = 0f; 

    void Start()
    {
        if (checkmarkIcon) checkmarkIcon.SetActive(false);
        if (xMarkIcon) xMarkIcon.SetActive(false);

        if (hasEmergency) radioAndBubbleUI.SetActive(true);
        else ClearZone();
    }

    void Update()
    {
        // --- BINAGO: GAGALAW LANG ANG TIMER KUNG HINDI PA RESOLVED ---
        if (hasEmergency && !isResolved && CommandCenterManager.instance != null && CommandCenterManager.instance.isGameActive)
        {
            currentEmergencyTimer -= Time.deltaTime;
            
            if (emergencyTimerText)
            {
                emergencyTimerText.gameObject.SetActive(true); // Siguraduhing nakikita
                emergencyTimerText.text = Mathf.CeilToInt(currentEmergencyTimer).ToString() + "s";
                emergencyTimerText.color = (currentEmergencyTimer <= 5f) ? Color.red : Color.yellow;
            }

            if (currentEmergencyTimer <= 0)
            {
                CommandCenterManager.instance.AddPanic("Hindi na-rescue ang " + requiredUnit + "!");
                StartCoroutine(ShowFeedback(xMarkIcon)); 
                ClearZone(); 
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!hasEmergency) return; 

        if (eventData.pointerDrag != null)
        {
            RescueUnitDrag droppedUnit = eventData.pointerDrag.GetComponent<RescueUnitDrag>();
            if (droppedUnit != null && !droppedUnit.isCoolingDown)
            {
                CheckUnit(droppedUnit);
            }
        }
    }

    void CheckUnit(RescueUnitDrag unit)
    {
        if (unit.unitType == requiredUnit)
        {
            // --- BAGONG DAGDAG: ITIGIL AT ITAGO ANG TIMER! ---
            isResolved = true; 
            if (emergencyTimerText) emergencyTimerText.gameObject.SetActive(false);
            // -------------------------------------------------

            radioAndBubbleUI.SetActive(false);
            
            unit.DeployToZone(this.transform);

            if (AudioManager.instance != null) 
            {
                AudioManager.instance.PlaySFX(AudioManager.instance.correctSound);
            }

            StartCoroutine(ShowFeedback(checkmarkIcon));
            StartCoroutine(ClearZoneAfterDeploy(unit.cooldownTime));
        }
        else
        {
            if (AudioManager.instance != null) 
            {
                AudioManager.instance.PlaySFX(AudioManager.instance.wrongSound);
            }
            
            if (CommandCenterManager.instance != null) CommandCenterManager.instance.AddPanic("Maling team ang ipinadala!");
            StartCoroutine(ShowFeedback(xMarkIcon));
        }
    }

    IEnumerator ClearZoneAfterDeploy(float waitTime)
    {
        // Hindi muna natin tatanggalin ang hasEmergency hanggat di tapos mag-deploy
        yield return new WaitForSeconds(waitTime);
        ClearZone(); 
    }

    public void TriggerEmergency(Sprite emergencySprite, string requiredRescueTeam, float timeLimit)
    {
        hasEmergency = true;
        isResolved = false; // I-reset ang preno
        requiredUnit = requiredRescueTeam;
        currentEmergencyTimer = timeLimit;

        if (emergencyTimerText) emergencyTimerText.gameObject.SetActive(true);

        thoughtBubbleImage.sprite = emergencySprite;
        radioAndBubbleUI.SetActive(true);
    }

    public void ClearZone()
    {
        hasEmergency = false;
        isResolved = false;
        requiredUnit = "";
        radioAndBubbleUI.SetActive(false);
        if (emergencyTimerText) emergencyTimerText.gameObject.SetActive(false); // Itago ang timer
    }

    IEnumerator ShowFeedback(GameObject icon)
    {
        if (icon) { icon.SetActive(true); yield return new WaitForSeconds(1f); icon.SetActive(false); }
    }
}