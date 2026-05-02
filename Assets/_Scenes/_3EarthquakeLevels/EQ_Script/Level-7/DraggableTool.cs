using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class DraggableTool : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Ano ang pangalan ng tool na ito?")]
    public string toolName; // Halimbawa: "Broom" o "Key"

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
        // I-save ang orihinal na pwesto sa side panel
        startPosition = rectTransform.anchoredPosition;

        // Ipaibabaw at gawing transparent habang hinahatak
        transform.SetAsLastSibling();
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Sumunod sa mouse/daliri
        rectTransform.anchoredPosition += eventData.delta / GetComponentInParent<Canvas>().scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Ibalik sa pagiging solid
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // ========================================================
        // TAWAGIN ANG MANAGER PARA I-CHECK KUNG MAY NALINIS NA KALAT!
        // ========================================================
        // Titingnan niya kung nasa Level 7 o Level 8 ka, tapos doon niya ipapasa yung tool!
        if (Level7ManagerEQ.instance != null) 
        {
            Level7ManagerEQ.instance.UseTool(toolName);
        }
        else if (Level8ManagerEQ.instance != null) 
        {
            Level8ManagerEQ.instance.UseTool(toolName);
        }

        // Ibalik palagi ang tool sa gilid ng panel para magamit ulit sa susunod na kalat
        rectTransform.anchoredPosition = startPosition;
    }
}