using UnityEngine;
using UnityEngine.EventSystems;

public class ConveyorItem : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Item Details")]
    public string itemName;
    public string weight;
    public string expirationDate;
    public string correctBoxTag; 
    
    [Header("Conveyor Settings")]
    public float moveSpeed = 100f; 
    
    [Header("Off-Screen Penalty")]
    public float despawnX = -100f; // Limit sa kaliwa. Pag lumagpas dito, penalty!

    private bool isBeingHeld = false;
    private CanvasGroup canvasGroup;
    private Vector3 startDragPosition;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    void Update()
    {
        // Aandar lang kung hindi hawak
        if (!isBeingHeld)
        {
            transform.position -= new Vector3(moveSpeed * Time.deltaTime, 0, 0);

            // --- BAGONG DAGDAG: OFF-SCREEN PENALTY ---
            // Kapag lumagpas na sa limit sa kaliwa...
            if (transform.position.x <= despawnX)
            {
                Debug.Log("Nahulog ang " + itemName + "! Penalty!");
                
                if (Level6Manager.instance != null)
                {
                    Level6Manager.instance.WrongItem(); // Bawas time at labas X mark
                }
                
                // Itago ang panel baka naiwang bukas
                if (ItemInfoPanel.instance != null)
                {
                    ItemInfoPanel.instance.HidePanel();
                }

                Destroy(gameObject); // Wasakin ang item
            }
            // -----------------------------------------
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isBeingHeld = true;
        if (ItemInfoPanel.instance != null)
            ItemInfoPanel.instance.ShowPanel(itemName, weight, expirationDate);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startDragPosition = transform.position;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition; 
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isBeingHeld = false;
        canvasGroup.blocksRaycasts = true;

        if (ItemInfoPanel.instance != null)
            ItemInfoPanel.instance.HidePanel(); 

        ResetPosition(); 
    }

    public void ResetPosition()
    {
        transform.position = startDragPosition;
    }
}