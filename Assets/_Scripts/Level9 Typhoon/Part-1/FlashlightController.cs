using UnityEngine;
using UnityEngine.EventSystems;

public class FlashlightController : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    [Header("Ang Butas ng Ilaw (Yung malaking black image)")]
    public RectTransform flashlightMask;

    [Header("Gaano kataas ang ilaw mula sa hawakan?")]
    public float lightOffsetY = 400f; // Pwede mo itong i-adjust sa Inspector!

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        UpdateLightPosition();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Pwede tayong maglagay ng "click" sound effect dito kapag hinawakan na yung flashlight!
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 1. Sundan ng hawakan ang daliri ng player
        rectTransform.position = Input.mousePosition;

        // 2. I-update ang pwesto ng ilaw sa itaas nito
        UpdateLightPosition();
    }

    void UpdateLightPosition()
    {
        if (flashlightMask != null)
        {
            // Ang pwesto ng "butas" ay pwesto ng hawakan + dagdag na height (lightOffsetY)
            flashlightMask.position = rectTransform.position + new Vector3(0, lightOffsetY, 0);
        }
    }
}