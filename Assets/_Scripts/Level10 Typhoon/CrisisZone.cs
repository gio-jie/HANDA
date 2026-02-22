using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class CrisisZone : MonoBehaviour, IDropHandler
{
    [Header("Zone Visuals")]
    public GameObject radioAndBubbleUI; // Ang buong group ng Radyo + Thought Bubble
    public Image thoughtBubbleImage;    // Ang picture sa loob ng bubble (e.g., Sunog)
    public Image deployedUnitImage;     // Ang lilitaw na truck na naka-park sa zone
    
    [Header("Zone State (Para sa Manager)")]
    public bool hasEmergency = false;
    public string requiredUnit = "";    // E.g., "Firetruck"
    public float currentEmergencyTimer = 0f; // Bibilis ang panic meter pag naubos ito!

    void Start()
    {
        // Linisin ang zone sa simula
        ClearZone();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!hasEmergency) return; // Wag pansinin kung walang emergency

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
            Debug.Log("TAMA! Dumating na ang " + unit.unitType);
            
            // 1. Simulan ang cooldown ng nasa inventory
            unit.StartCooldown();

            // 2. Itago ang Radio UI at Ipakita ang Truck sa Zone
            radioAndBubbleUI.SetActive(false);
            deployedUnitImage.sprite = unit.unitImage.sprite; // Kopyahin ang picture ng truck
            deployedUnitImage.gameObject.SetActive(true);

            // 3. Sabihin sa Manager na na-solve na ito!
            // if (CommandCenterManager.instance != null) CommandCenterManager.instance.EmergencySolved();

            // 4. Linisin ang zone pagkatapos ng parehong cooldown time ng truck
            StartCoroutine(ClearZoneAfterDeploy(unit.cooldownTime));
        }
        else
        {
            Debug.Log("MALI ANG IPINADALA!");
            // if (CommandCenterManager.instance != null) CommandCenterManager.instance.AddPanic("Maling team ang ipinadala!");
        }
    }

    IEnumerator ClearZoneAfterDeploy(float waitTime)
    {
        hasEmergency = false;
        yield return new WaitForSeconds(waitTime);
        ClearZone(); // Tapos na ang rescue, bakante na ulit ang zone!
    }

    public void TriggerEmergency(Sprite emergencySprite, string requiredRescueTeam, float timeLimit)
    {
        hasEmergency = true;
        requiredUnit = requiredRescueTeam;
        currentEmergencyTimer = timeLimit;

        thoughtBubbleImage.sprite = emergencySprite;
        radioAndBubbleUI.SetActive(true);
        deployedUnitImage.gameObject.SetActive(false);
    }

    public void ClearZone()
    {
        hasEmergency = false;
        requiredUnit = "";
        radioAndBubbleUI.SetActive(false);
        deployedUnitImage.gameObject.SetActive(false);
    }
}