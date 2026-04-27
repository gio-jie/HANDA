using UnityEngine;

public class SafeZoneTrigger : MonoBehaviour
{
    private bool isHit = false;
    private RunnerPlayer player;

    void Start()
    {
        player = FindFirstObjectByType<RunnerPlayer>();
    }

    void Update()
    {
        if (Level6ManagerEQ.instance != null && (!Level6ManagerEQ.instance.isGameActive || !Level6ManagerEQ.instance.isPhase2Active)) return;
        if (player == null) return;

        // BUG-FREE METHOD: I-convert ang pwesto nila base sa mismong Screen Pixels para hindi malito ang Unity sa Canvas Scale!
        Vector2 safeZoneScreenPos = RectTransformUtility.WorldToScreenPoint(null, GetComponent<RectTransform>().position);
        Vector2 playerScreenPos = RectTransformUtility.WorldToScreenPoint(null, player.GetComponent<RectTransform>().position);

        // Kapag pumantay o bumaba na ang Safe Zone sa level ni Jobert (may allowance na 100 pixels para sa ulo niya)
        if (!isHit && safeZoneScreenPos.y <= playerScreenPos.y + 100f)
        {
            isHit = true;
            Debug.Log("BOOM! TUMAPAK NA SA SAFE ZONE!");
            Level6ManagerEQ.instance.TriggerWinSequence(); 
        }
    }
}