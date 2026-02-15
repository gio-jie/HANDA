using UnityEngine;
using UnityEngine.EventSystems;

public class UIDropZone : MonoBehaviour, IDropHandler
{
    public string acceptedType; // Anong item lang ang tatanggapin dito? (e.g., "Food")

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            UIDragItem draggedItem = eventData.pointerDrag.GetComponent<UIDragItem>();

            if (draggedItem != null)
            {
                // Kung TAMA ang kahon
                if (draggedItem.itemType == acceptedType)
                {
                    Debug.Log("TAMA!");
                    
                    // 1. Tatawagin ang Add Score (Papasa yung pangalan ng item para sa checkmark)
                    Level6Manager.instance.AddScore(draggedItem.gameObject.name);
                    
                    // 2. Wasakin ang item dahil nasa loob na ng kahon
                    Destroy(draggedItem.gameObject); 
                }
                else
                {
                    // Kung MALI ang kahon o Unessential Item
                    Debug.Log("MALI! Bawas Oras!");
                    
                    // 1. Tatawagin ang Penalty system para mabawasan ng 5 seconds
                    Level6Manager.instance.WrongItem();
                    
                    // 2. Ibalik sa taas yung item para i-try ulit ng player
                    draggedItem.ResetPosition(); 
                }
            }
        }
    }
}