using UnityEngine;
using UnityEngine.EventSystems; // Importante para sa Drag functionality

// Ang script na ito ay nagpapagana ng Drag and Drop
public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool isCorrectItem; // Checkan natin ito sa Inspector kung tama o mali ang item
    public Transform bagTarget; // Dito natin ilalagay yung GoBag object
    public Level1Manager manager; // Reference sa manager
    
    private Vector3 originalPosition;
    private Transform originalParent;
    private Transform canvasTransform; // Para lumutang ang item habang hinihila
    private CanvasGroup canvasGroup;

    void Awake()
    {
        // Hanapin ang Canvas sa scene (ang lolo ng item na ito)
        canvasTransform = GetComponentInParent<Canvas>().transform;
        
        // Magdagdag ng CanvasGroup kung wala pa (kailangan ito para tumagos ang mouse click)
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 1. Save natin kung saan siya nakapwesto dati
        originalPosition = transform.position;
        originalParent = transform.parent;

        // 2. Ilipat siya sa Canvas level para lumutang siya sa ibabaw ng lahat
        transform.SetParent(canvasTransform);

        // 3. I-disable muna ang harang sa mouse para ma-detect natin kung nasa ibabaw siya ng bag
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Sumunod sa mouse/daliri
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        // Check kung gaano kalapit sa Bag (Distance check)
        float distance = Vector3.Distance(transform.position, bagTarget.position);

        // Kung malapit sa bag (Distance < 200 pixels example)
        if (distance < 200) 
        {
            if (isCorrectItem)
            {
                // TAMA: Tawagin ang manager at sirain ang item
                manager.AddScore();
                Destroy(gameObject); 
            }
            else
            {
                // MALI: Tawagin ang manager at ibalik sa pwesto
                manager.WrongItem();
                ReturnToShelf();
            }
        }
        else
        {
            // Kung binitawan sa kung saan-saan lang, bumalik sa pwesto
            ReturnToShelf();
        }
    }

    void ReturnToShelf()
    {
        transform.SetParent(originalParent); // Bumalik sa Grid
        transform.position = originalPosition; // Reset position
    }
}