using UnityEngine;
using UnityEngine.EventSystems;

public class RationDragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string itemType; // I-type sa Inspector kung "Food" o "Water"
    
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
        
        // Ilabas sa panel para tagos sa buong screen pag dinrag
        transform.SetParent(transform.root); 
        transform.SetAsLastSibling(); // Ilagay sa pinaka-ibabaw
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData) 
    {
        transform.position = Input.mousePosition; // Sumunod sa mouse/daliri
    }

    public void OnEndDrag(PointerEventData eventData) 
    {
        canvasGroup.blocksRaycasts = true;
        
        // Ibalik sa lamesa (Inventory) kahit saan mo pa mabitawan
        transform.SetParent(startParent);
        transform.position = startPos; 
    }
}