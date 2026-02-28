using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class DraggableSanitizer : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;

    private Vector2 startPos;
    private Vector3 startScale;

    public RectTransform snapTarget;
    public float snapDistance = 50f;

    public SanitizerTask sanitizerTask;

    private bool snapped = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        startPos = rectTransform.anchoredPosition;
        startScale = transform.localScale;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (snapped) return;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (snapped) return;

        rectTransform.anchoredPosition +=
            eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (snapped) return;

        float distance = Vector2.Distance(
            rectTransform.position,
            snapTarget.position
        );

        if (distance <= snapDistance)
        {
            SnapToTarget();
        }
        else
        {
            rectTransform.anchoredPosition = startPos;
        }
    }

    IEnumerator SnapToTargetCoroutine()
    {
        snapped = true;

        // Move the bottle instantly to the snap target
        rectTransform.position = snapTarget.position;

        // Play snap sound
        if (sanitizerTask.snapSFX != null)
        {
            sanitizerTask.audioSource.PlayOneShot(sanitizerTask.snapSFX, sanitizerTask.sfxVolume);

            // Wait for the snap sound to finish
            yield return new WaitForSeconds(sanitizerTask.snapSFX.length);
        }

        // Tell the task that the bottle has snapped (optional visual logic)
        //sanitizerTask.OnBottleSnapped();

        // Start filling after snap SFX is done
        sanitizerTask.StartFilling();
    }

    void SnapToTarget()
    {
        StartCoroutine(SnapToTargetCoroutine());
    }

    public void ResetBottle()
    {
        snapped = false;
        rectTransform.anchoredPosition = startPos;
        transform.localScale = startScale;
    }
}