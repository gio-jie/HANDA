using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class CardDragHandler : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rect;
    private CanvasGroup canvasGroup;
    private Vector3 originalPos;

    private bool wasDropped = false;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        wasDropped = false;
        originalPos = rect.position;
        canvasGroup.blocksRaycasts = false;
        transform.localScale = Vector3.one * 1.1f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rect.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        transform.localScale = Vector3.one;

        if (!wasDropped)
        {
            rect.position = originalPos;
        }
    }

    public void MarkAsDropped(Transform container)
    {
        wasDropped = true;
        StartCoroutine(ShrinkIntoContainer(container));
    }

    IEnumerator ShrinkIntoContainer(Transform container)
    {
        rect.SetParent(container);

        Vector3 startScale = rect.localScale;
        Vector3 endScale = Vector3.zero;

        float t = 0f;
        float duration = 0.2f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;

            rect.localScale = Vector3.Lerp(startScale, endScale, progress);
            yield return null;
        }

        Destroy(gameObject); // remove the image
    }
}