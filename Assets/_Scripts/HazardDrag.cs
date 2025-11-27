using UnityEngine;
using UnityEngine.EventSystems;

public class HazardDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform storageTarget; // Yung Bodega
    private Level3Manager manager;
    
    private Vector3 originalPosition;
    private Transform originalParent;
    private CanvasGroup canvasGroup;

    void Start()
    {
        manager = FindFirstObjectByType<Level3Manager>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = transform.position;
        originalParent = transform.parent;
        // Move to root to float above everything
        transform.SetParent(transform.root); 
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        // Check distance to Storage Shed
        float distance = Vector3.Distance(transform.position, storageTarget.position);

        if (distance < 200) // Kung malapit na sa bodega
        {
            // SUCCESS!
            manager.HazardCollected();
            Destroy(gameObject); // Alisin ang kalat
        }
        else
        {
            // Balik sa pwesto
            transform.SetParent(originalParent);
            transform.position = originalPosition;
        }
    }
}