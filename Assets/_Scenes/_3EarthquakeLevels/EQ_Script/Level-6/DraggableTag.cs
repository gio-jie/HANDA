using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableTag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Tag Settings")]
    public string tagType; // Isusulat natin dito kung "Green", "Yellow", o "Red"

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 startPosition;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // I-save ang orihinal na pwesto para kung mali ang drop, babalik siya dito
        startPosition = rectTransform.anchoredPosition;
        
        // Gawing medyo transparent habang dina-drag at patayin ang block raycasts
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Sumunod sa mouse / daliri
        rectTransform.anchoredPosition += eventData.delta / GetComponentInParent<Canvas>().scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Ibalik sa solid color kapag binitiwan
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Kung hindi siya na-drop sa tama, ibalik sa orihinal na pwesto
        rectTransform.anchoredPosition = startPosition;
    }
}