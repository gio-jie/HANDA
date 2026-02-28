using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Level8DragManager : MonoBehaviour
{
    public static Level8DragManager Instance;

    [Header("Assign in Inspector")]
    public RectTransform scrollParent;
    public RectTransform scrollViewport;
    public RectTransform dragLayer;

    private Transform originalParent;

    void Awake()
    {
        Instance = this;
    }

    public void BeginDrag(Level8DragItem item)
    {
        originalParent = item.transform.parent;

        item.GetRect().SetParent(dragLayer);
        item.GetRect().SetAsLastSibling();
    }

    public void EndDrag(Level8DragItem item, PointerEventData eventData)
    {
        RectTransform rect = item.GetRect();

        bool droppedInScroll =
            RectTransformUtility.RectangleContainsScreenPoint(
                scrollViewport,
                eventData.position,
                null);

        if (droppedInScroll)
        {
            rect.SetParent(scrollParent);
            rect.localScale = Vector3.one;

            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;

            LayoutRebuilder.ForceRebuildLayoutImmediate(scrollParent);
        }
        else
        {
            rect.SetParent(dragLayer);
        }
    }
}