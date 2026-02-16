using UnityEngine;
using UnityEngine.EventSystems; // Importante ito para sa UI Drag and Drop

public class UIDragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string itemType; // Halimbawa: "Food", "Hygiene", "FirstAid", "Unessential"
    
    [HideInInspector] public Vector3 startPosition;
    private CanvasGroup canvasGroup;

    void Start()
    {
        // Kusa nitong lalagyan ng CanvasGroup ang item mo para hindi ka na mahirapan
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = transform.position; // I-save ang pwesto bago i-drag
        canvasGroup.blocksRaycasts = false; // Para madetect ng mouse ang kahon sa ilalim habang hawak mo ito
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition; // Sumunod sa mouse/daliri
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true; // Ibalik ang detect
        
        // Kapag binitawan sa labas ng kahon, babalik sa orihinal na pwesto
        // (Ang Box script ang magde-destroy nito kapag tama ang kahon)
        ResetPosition();
    }

    public void ResetPosition()
    {
        transform.position = startPosition;
    }
}