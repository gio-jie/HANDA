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
    public TMP_Text emergencyTimerText; // BAGONG DAGDAG: Ang timer sa gilid ng radyo!
    public GameObject checkmarkIcon; 
    public GameObject xMarkIcon;     

    [Header("Zone State")]
    public bool hasEmergency = false;
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
        if (hasEmergency && CommandCenterManager.instance != null && CommandCenterManager.instance.isGameActive)
        {
            currentEmergencyTimer -= Time.deltaTime;
            
            // Ipakita ang oras sa ibabaw/gilid ng Radyo
            if (emergencyTimerText)
            {
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
            radioAndBubbleUI.SetActive(false);
            
            // ILIPAT ANG SASAKYAN SA ZONE NA ITO!
            unit.DeployToZone(this.transform);

            StartCoroutine(ShowFeedback(checkmarkIcon));
            StartCoroutine(ClearZoneAfterDeploy(unit.cooldownTime));
        }
        else
        {
            if (CommandCenterManager.instance != null) CommandCenterManager.instance.AddPanic("Maling team ang ipinadala!");
            StartCoroutine(ShowFeedback(xMarkIcon));
        }
    }

    IEnumerator ClearZoneAfterDeploy(float waitTime)
    {
        hasEmergency = false;
        yield return new WaitForSeconds(waitTime);
        ClearZone(); 
    }

    public void TriggerEmergency(Sprite emergencySprite, string requiredRescueTeam, float timeLimit)
    {
        hasEmergency = true;
        requiredUnit = requiredRescueTeam;
        currentEmergencyTimer = timeLimit;

        thoughtBubbleImage.sprite = emergencySprite;
        radioAndBubbleUI.SetActive(true);
    }

    public void ClearZone()
    {
        hasEmergency = false;
        requiredUnit = "";
        radioAndBubbleUI.SetActive(false);
    }

    IEnumerator ShowFeedback(GameObject icon)
    {
        if (icon) { icon.SetActive(true); yield return new WaitForSeconds(1f); icon.SetActive(false); }
    }
}