using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SwipeInteraction : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Settings")]
    public bool isBreaker = false; 
    public float swipeThreshold = 50f; 
    
    [Header("Visuals")]
    public GameObject doneObject; // Ang "Done" version (unplugged/off)

    private Vector2 startPos;
    private bool taskDone = false;
    private Level4Manager manager;

    void Start()
    {
        manager = FindFirstObjectByType<Level4Manager>();
        if (doneObject != null) doneObject.SetActive(false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (taskDone) return;
        startPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData) { }

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

        // 1. Ipakita muna ang "Done" object (Unplugged/Off state)
        if (doneObject != null) doneObject.SetActive(true);

        // 2. Tawagin ang Manager
        if (manager != null)
        {
            if (isBreaker) 
            {
                manager.TrySwitchBreaker(); 
                // NOTE: Kung mali ang timing, tatawagin ng Manager ang ResetToActive() agad dito.
            }
            else 
            {
                manager.UnplugDevice(gameObject); 
            }
        }
        
        // 3. ITO ANG FIX: Itago lang ang sarili KUNG "Done" pa rin ang status.
        // Kung tinawag ng Manager ang ResetToActive(), magiging FALSE na ang taskDone,
        // kaya HINDI na niya itatago ang sarili niya.
        if (taskDone)
        {
            gameObject.SetActive(false);
        }
    }

    // Function para ibalik sa dati (undo)
    public void ResetToActive()
    {
        taskDone = false; // Reset status
        
        // Visual Reset
        gameObject.SetActive(true); // Ipakita ulit ang sarili (Lever Up)
        
        if (doneObject != null) 
        {
            doneObject.SetActive(false); // Itago ang "Done" version (Lever Down)
        }
    }
}