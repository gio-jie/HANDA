using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class RescueUnitDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Unit Settings")]
    public string unitType; 
    public float cooldownTime = 5f; 

    [Header("UI & Visuals")]
    public Image unitImage; 
    public Image cooldownOverlay; 
    public TMP_Text cooldownText; 

    private Vector3 startPos;
    private Transform startParent;
    private int originalSiblingIndex; 
    private CanvasGroup canvasGroup;
    [HideInInspector] public bool isCoolingDown = false;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (!canvasGroup) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (cooldownOverlay) cooldownOverlay.gameObject.SetActive(false);
        if (cooldownText) cooldownText.text = "";
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isCoolingDown) return; 

        startPos = transform.position; // MEMORY: Tandaan ang eksaktong coordinates!
        startParent = transform.parent;
        originalSiblingIndex = transform.GetSiblingIndex(); 
        
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isCoolingDown) return;
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isCoolingDown) 
        {
            transform.SetParent(startParent);
            transform.SetSiblingIndex(originalSiblingIndex); 
            transform.position = startPos; // Ibalik sa eksaktong coordinates
            canvasGroup.blocksRaycasts = true;
        }
    }

    public void DeployToZone(Transform zoneTransform)
    {
        if (!isCoolingDown)
        {
            // Ilipat lang ang parent sa Zone, pero WAG nang baguhin ang position.
            // Hayaan siyang mag-stay kung saan eksaktong nai-drop ng player!
            transform.SetParent(zoneTransform);
            
            StartCoroutine(CooldownRoutine());
        }
    }

    IEnumerator CooldownRoutine()
    {
        isCoolingDown = true;
        canvasGroup.blocksRaycasts = false; 

        if (cooldownOverlay) cooldownOverlay.gameObject.SetActive(true);
        if (cooldownText) cooldownText.gameObject.SetActive(true);

        float timer = cooldownTime;
        while (timer > 0)
        {
            if (cooldownText) cooldownText.text = Mathf.CeilToInt(timer).ToString() + "s";
            timer -= Time.deltaTime;
            yield return null;
        }

        // --- TAPOS NA ANG COOLDOWN, UWI NA SA TOOLBELT! ---
        isCoolingDown = false;
        canvasGroup.blocksRaycasts = true;
        
        transform.SetParent(startParent);
        transform.SetSiblingIndex(originalSiblingIndex); 
        transform.position = startPos; // ITO ANG FIX: Ibalik siya eksakto kung saan siya galing!
        
        if (cooldownOverlay) cooldownOverlay.gameObject.SetActive(false);
        if (cooldownText) cooldownText.text = "";
    }
}