using UnityEngine;
using UnityEngine.EventSystems;

public class RationDragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string itemType; 
    public static RationDragItem currentlyDraggedItem;

    // Ginawa nating Public para ma-access ng pasyente
    public Vector3 startPos; 
    public Transform startParent; 
    private CanvasGroup canvasGroup;
    
    // BAGONG DAGDAG: Para pwedeng i-lock sa ere o sa pasyente
    public bool isLocked = false; 

    void Start() 
    { 
        canvasGroup = GetComponent<CanvasGroup>(); 
        if(!canvasGroup) canvasGroup = gameObject.AddComponent<CanvasGroup>(); 
    }

    public void OnBeginDrag(PointerEventData eventData) 
    {
        if (isLocked) return; // Wag pagalawin kung naka-lock na

        currentlyDraggedItem = this;
        startPos = transform.position;
        startParent = transform.parent;
        
        transform.SetParent(transform.root); 
        transform.SetAsLastSibling(); 
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData) 
    {
        if (isLocked) return;
        transform.position = Input.mousePosition; 
    }

    public void OnEndDrag(PointerEventData eventData) 
    {
        if (isLocked) return; // Wag pabalikin sa lamesa kung naka-lock!
        
        currentlyDraggedItem = null;
        canvasGroup.blocksRaycasts = true;
        
        ReturnToTable();
    }

    // BAGONG DAGDAG: Command para pabalikin sa lamesa manually
    public void ReturnToTable()
    {
        transform.SetParent(startParent);
        transform.position = startPos; 
        canvasGroup.blocksRaycasts = true;
    }
}