using UnityEngine;
using UnityEngine.EventSystems;

public class RadioDropZone : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            // Gamit natin yung ToolDragItem script galing Level 9
            ToolDragItem droppedTool = eventData.pointerDrag.GetComponent<ToolDragItem>();
            if (droppedTool != null)
            {
                if (Level10Part1Manager.instance != null)
                {
                    Level10Part1Manager.instance.ReceiveItem(droppedTool.toolType);
                }
            }
        }
    }
}