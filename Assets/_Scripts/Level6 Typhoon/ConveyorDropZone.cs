using UnityEngine;
using UnityEngine.EventSystems;

public class ConveyorDropZone : MonoBehaviour, IDropHandler
{
    public string acceptedTag; 

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            ConveyorItem draggedItem = eventData.pointerDrag.GetComponent<ConveyorItem>();

            if (draggedItem != null)
            {
                // KUNG TAMA ANG KAHON NA NAPAGLAGYAN
                if (draggedItem.correctBoxTag == acceptedTag)
                {
                    Debug.Log("TAMA! Tinatawag ang Manager...");
                    
                    if (Level6Manager.instance != null) 
                    {
                        Level6Manager.instance.AddScore(draggedItem.itemName);
                    }

                    // --- BAGONG DAGDAG (ANG FIX NATIN) ---
                    // Itago ang panel bago tuluyang wasakin ang item!
                    if (ItemInfoPanel.instance != null)
                    {
                        ItemInfoPanel.instance.HidePanel();
                    }
                    // -------------------------------------
                    
                    Destroy(draggedItem.gameObject); // Wasakin ang item
                }
                // KUNG MALI ANG KAHON (Penalty)
                else
                {
                    Debug.Log("MALI! Tinatawag ang Penalty...");
                    
                    if (Level6Manager.instance != null) 
                    {
                        Level6Manager.instance.WrongItem();
                    }
                    
                    draggedItem.ResetPosition(); // Ibalik sa belt
                }
            }
        }
    }
}