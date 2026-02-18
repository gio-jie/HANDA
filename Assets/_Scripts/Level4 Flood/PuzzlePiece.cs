using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PuzzlePiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Snap Settings")]
    public RectTransform correctSnapPoint;
    public float snapDistance = 60f;

    [Header("Optional")]
    public bool isEdgePiece = false;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private bool isPlaced = false;

    private Transform dragLayer;
    private ScrollRect scrollRect;

    public AudioClip onDropClip;
    private AudioSource audioSource;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        dragLayer = PuzzleManager.Instance.dragLayer;
        scrollRect = PuzzleManager.Instance.scrollRect;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = 1f;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isPlaced) return;

        scrollRect.enabled = false;

        AudioManager.instance.PlaySFX(AudioManager.instance.clickSound);

        transform.SetParent(PuzzleManager.Instance.dragLayer, true);
        transform.SetAsLastSibling();

        canvasGroup.blocksRaycasts = false;

        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isPlaced) return;

        rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //canvasGroup.blocksRaycasts = true;
        if (!isPlaced)
            canvasGroup.blocksRaycasts = true;
    
        scrollRect.enabled = true;

        float distance = Vector2.Distance(rectTransform.position, correctSnapPoint.position);
        if (distance < snapDistance)
        {
            SnapIntoPlace();
        }
        else
        {
            audioSource.PlayOneShot(onDropClip, 2f);

            if (RectTransformUtility.RectangleContainsScreenPoint(
                    PuzzleManager.Instance.scrollRect.viewport, 
                    rectTransform.position, 
                    eventData.pressEventCamera))
            {
                transform.SetParent(PuzzleManager.Instance.scrollRect.content, true);
                rectTransform.pivot = new Vector2(0, 1);
                rectTransform.anchorMin = new Vector2(0, 1);
                rectTransform.anchorMax = new Vector2(0, 1);
                isPlaced = false;

                PuzzleManager.Instance.RefreshScrollLayout();
            }
        }
    }

    void SnapIntoPlace()
    {
        //audioSource.PlayOneShot(correctPlaceSound);
        if (AudioManager.instance != null)
                AudioManager.instance.PlaySFX(AudioManager.instance.correctSound);

        transform.SetParent(correctSnapPoint);

        transform.SetParent(correctSnapPoint, false);
        rectTransform.anchoredPosition = Vector2.zero;

        isPlaced = true;

        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        PuzzleManager.Instance.RefreshScrollLayout();

        PuzzleManager.Instance.CheckEdgeCompletion();

        if (PuzzleManager.Instance.IsPuzzleComplete())
        {
            StartCoroutine(PlayFinalPieceAnimation());
        }
        else
        {
            EnableGlow();
        }
    }

    private System.Collections.IEnumerator PlayFinalPieceAnimation()
    {
        Outline outline = gameObject.AddComponent<Outline>();
        Color glowColor = Color.green;
        glowColor.a = 1f;
        outline.effectColor = glowColor;
        outline.effectDistance = new Vector2(6, 6);

        float duration = 1f;
        float timer = 0f;

        Vector3 originalScale = transform.localScale;
        Vector3 punchScale = originalScale * 1.25f;

        while (timer < duration)
        {
            float t = timer / duration;

            outline.effectColor = new Color(
                glowColor.r,
                glowColor.g,
                glowColor.b,
                Mathf.Lerp(1f, 0f, t)
            );

            transform.localScale = Vector3.Lerp(
                originalScale,
                punchScale,
                Mathf.Sin(t * Mathf.PI)
            );

            timer += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;
        Destroy(outline);

        yield return new WaitForSeconds(0.2f);

        PuzzleManager.Instance.ShowCompletionPanel();
    }

    void EnableGlow()
    {
        Outline outline = gameObject.AddComponent<Outline>();
        Color glowColor = Color.green;
        glowColor.a = 1f;
        outline.effectColor = glowColor;
        outline.effectDistance = new Vector2(3, 3);

        StartCoroutine(FadeOutline(outline));
    }

    private System.Collections.IEnumerator FadeOutline(Outline outline)
    {
        float duration = 0.5f;
        float timer = 0f;

        Color startColor = outline.effectColor;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        Vector3 originalScale = transform.localScale;
        Vector3 glowScale = originalScale * 1.15f;

        while (timer < duration)
        {
            outline.effectColor = Color.Lerp(startColor, endColor, timer / duration);
            transform.localScale = Vector3.Lerp(originalScale, glowScale, Mathf.Sin((timer / duration) * Mathf.PI));
            timer += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;
        Destroy(outline);
    }

    public bool IsPlaced()
    {
        return isPlaced;
    }
}