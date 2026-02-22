using UnityEngine;
using UnityEngine.EventSystems;

public class ToolDragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string toolType; // I-type sa Inspector: "Cellphone", "CautionTape", "Boots"
    
    private Vector3 startPos;
    private Transform startParent;
    private CanvasGroup canvasGroup;

    void Start() 
    { 
        canvasGroup = GetComponent<CanvasGroup>(); 
        if(!canvasGroup) canvasGroup = gameObject.AddComponent<CanvasGroup>(); 
    }

    public void OnBeginDrag(PointerEventData eventData) 
    {
        startPos = transform.position;
        startParent = transform.parent;
        
        transform.SetParent(transform.root); // Ilabas para tagos sa buong screen
        transform.SetAsLastSibling(); // Ilagay sa pinaka-ibabaw para hindi matakpan
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData) 
    {
        transform.position = Input.mousePosition; 
    }

    public void OnEndDrag(PointerEventData eventData) 
    {
        canvasGroup.blocksRaycasts = true;
        
        // Pabalikin agad sa Toolbelt pagkagaling sa drag
        transform.SetParent(startParent);
        transform.position = startPos; 
    }
}