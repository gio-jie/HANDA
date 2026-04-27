using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class BuildingTarget : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    [Header("Building Settings")]
    public string correctTag; 
    private bool isInspected = false; 
    private int originalSiblingIndex;

    [Header("Zoom Settings")]
    public float zoomScale = 2f; 
    public float zoomSpeed = 8f; 
    private Vector2 originalPos;
    private Vector3 originalScale;
    public bool isZoomed = false;

    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPos = rectTransform.anchoredPosition;
        originalScale = rectTransform.localScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isInspected) return; 

        if (!isZoomed)
        {
            // ==========================================
            // --- BAGONG DAGDAG: IPAIBAW ANG BUILDING ---
            // ==========================================
            originalSiblingIndex = transform.GetSiblingIndex(); // Tandaan ang orihinal na layer
            transform.SetAsLastSibling(); // I-bato sa pinaka-ibabaw ng screen
            // ==========================================

            if (Level6ManagerEQ.instance != null) 
                Level6ManagerEQ.instance.OnBuildingZoomedIn(this.transform);
            
            StartCoroutine(ZoomRoutine(Vector2.zero, Vector3.one * zoomScale));
            isZoomed = true;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (isInspected || !isZoomed) return; 

        if (eventData.pointerDrag != null)
        {
            DraggableTag droppedTag = eventData.pointerDrag.GetComponent<DraggableTag>();

            if (droppedTag != null)
            {
                if (droppedTag.tagType == correctTag)
                {
                    // --- TAMA ANG TAG ---
                    isInspected = true;
                    
                    // --- BAGONG LOGIC: GUMAWA NG CLONE! ---
                    // 1. I-duplicate yung buong tag na hinatak ng player
                    GameObject tagClone = Instantiate(eventData.pointerDrag);
                    
                    // 2. Gawing anak ng building ang clone
                    tagClone.transform.SetParent(this.transform, false);
                    
                    // 3. I-snap sa gitna ng building
                    tagClone.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                    // 4. Tanggalin ang mga script sa clone para maging normal na Image na lang siya (hindi na pwedeng hatakin)
                    Destroy(tagClone.GetComponent<DraggableTag>());
                    Destroy(tagClone.GetComponent<CanvasGroup>());
                    // ----------------------------------------

                    // Tawagin ang GameManager
                    if (Level6ManagerEQ.instance != null) Level6ManagerEQ.instance.CorrectTagPlaced();

                    // Simulan ang Zoom Out
                    StartCoroutine(ZoomOutRoutine());
                    
                    // Pansinin: HINDI NA NATIN GINAGALAW YUNG ORIGINAL TAG! 
                    // Kusang babalik 'yon sa gilid dahil sa DraggableTag script natin.
                }
                else
                {
                    // --- MALI ANG TAG ---
                    if (Level6ManagerEQ.instance != null) Level6ManagerEQ.instance.WrongTagPlaced();
                }
            }
        }
    }

    IEnumerator ZoomRoutine(Vector2 targetPos, Vector3 targetScale)
    {
        while (Vector2.Distance(rectTransform.anchoredPosition, targetPos) > 0.1f || 
               Vector3.Distance(rectTransform.localScale, targetScale) > 0.01f)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPos, Time.deltaTime * zoomSpeed);
            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, targetScale, Time.deltaTime * zoomSpeed);
            yield return null;
        }
        
        rectTransform.anchoredPosition = targetPos;
        rectTransform.localScale = targetScale;
    }

    IEnumerator ZoomOutRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        
        isZoomed = false;

        if (Level6ManagerEQ.instance != null) 
            Level6ManagerEQ.instance.OnBuildingZoomedOut();
        
        // ==========================================
        // --- BAGONG DAGDAG: IBALIK SA DATING LAYER ---
        // ==========================================
        transform.SetSiblingIndex(originalSiblingIndex); 
        // ==========================================

        StartCoroutine(ZoomRoutine(originalPos, originalScale));
    }
}