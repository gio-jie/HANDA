using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SwipeInteraction : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Settings")]
    public bool isBreaker = false; 
    public float swipeThreshold = 50f; 
    
    [Header("Visuals")]
    public GameObject doneObject; // Ito yung "Nakahugot" na version (naka-hide sa simula)

    private Vector2 startPos;
    private bool taskDone = false;
    private Level4Manager manager;

    void Start()
    {
        manager = FindFirstObjectByType<Level4Manager>();
        
        // Safety check: Siguraduhing nakatago yung "Done" object sa simula
        if (doneObject != null) doneObject.SetActive(false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (taskDone) return;
        startPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Required for EndDrag to work
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (taskDone) return;

        float verticalSwipe = startPos.y - eventData.position.y;

        if (verticalSwipe > swipeThreshold)
        {
            CompleteTask();
        }
    }

    void CompleteTask()
    {
        taskDone = true;

        // A. IPALIT ANG OBJECT
        // 1. Labasin yung "Nakahugot" na version
        if (doneObject != null) doneObject.SetActive(true);

        // B. Tawagin ang Manager
        if (manager != null)
        {
            if (isBreaker) manager.TrySwitchBreaker();
            else manager.UnplugDevice(gameObject); // Ipapasa natin ang sarili para maitago ng Manager
        }
        
        // C. Itago ang sarili (Yung nakasaksak)
        gameObject.SetActive(false);
    }
}