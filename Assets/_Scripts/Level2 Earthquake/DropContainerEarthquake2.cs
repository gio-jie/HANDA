using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;

public class DropContainerEarthquake2 : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool isSaveContainer;
    public Transform containerTransform;

    [Header("Drop SFX")]
    public AudioSource audioSource;
    public AudioClip dropClip;

    [Header("Container Visuals")]
    public Image containerSprite;     // Image that changes sprite (bag/bin)
    public Sprite closedSprite;
    public Sprite openSprite;

    [Header("Wrong Flash Effect")]
    public Image containerImage;
    public Color wrongFlashColor = new Color(1f, 0.4f, 0.4f);
    public float flashDuration = 0.4f;

    private Color originalColor;

    private Coroutine flashCoroutine;
    private Coroutine scaleCoroutine;
    private Coroutine popCoroutine;

    private Vector3 baseScale;

    void Awake()
    {
        baseScale = transform.localScale;

        if (containerImage != null)
            originalColor = containerImage.color;
    }

    void Start()
    {
        SetClosed(); // default state
    }

    // =========================
    // DROP
    // =========================
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        CardDragHandler drag = eventData.pointerDrag.GetComponent<CardDragHandler>();
        ItemCardUI card = eventData.pointerDrag.GetComponentInParent<ItemCardUI>();

        if (card == null || drag == null) return;

        drag.MarkAsDropped(containerTransform);

        SortingLevelManagerEarthquake2.Instance
            .SubmitAnswer(card.data, isSaveContainer);

        bool correct = isSaveContainer == card.data.shouldSave;

        // Always play drop sound
        if (audioSource != null && dropClip != null)
            audioSource.PlayOneShot(dropClip);

        if (correct)
        {
            StarManagerEarthquake2.Instance.RegisterCorrectItem();

            if (AudioManager.instance != null)
                AudioManager.instance.PlaySFX(AudioManager.instance.correctSound);
        }
        else
        {
            StarManagerEarthquake2.Instance.RegisterWrongItem();

            if (AudioManager.instance != null)
                AudioManager.instance.PlaySFX(AudioManager.instance.warningSound);

            if (flashCoroutine != null)
                StopCoroutine(flashCoroutine);

            flashCoroutine = StartCoroutine(FlashWrong());
        }

        if (card.data.shouldSave)
        {
            ItemScenarioPopupUI.Instance.Show(card.data);
        }

        if (popCoroutine != null)
            StopCoroutine(popCoroutine);

        popCoroutine = StartCoroutine(PopEffect());

        SetClosed(); // 👈 close after drop
    }

    // =========================
    // FLASH EFFECT
    // =========================
    private IEnumerator FlashWrong()
    {
        if (containerImage == null) yield break;

        int flashes = 3;
        float half = flashDuration * 0.5f;

        for (int i = 0; i < flashes; i++)
        {
            float t = 0f;

            while (t < half)
            {
                t += Time.deltaTime;
                containerImage.color = Color.Lerp(originalColor, wrongFlashColor, t / half);
                yield return null;
            }

            t = 0f;

            while (t < half)
            {
                t += Time.deltaTime;
                containerImage.color = Color.Lerp(wrongFlashColor, originalColor, t / half);
                yield return null;
            }
        }

        containerImage.color = originalColor;
    }

    // =========================
    // HOVER (SCALE + OPEN)
    // =========================
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null &&
            eventData.pointerDrag.GetComponent<CardDragHandler>() != null)
        {
            StartScale(baseScale * 1.1f);
            SetOpen(); // 👈 open on hover
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartScale(baseScale);
        SetClosed(); // 👈 close on exit
    }

    void StartScale(Vector3 target)
    {
        if (scaleCoroutine != null)
            StopCoroutine(scaleCoroutine);

        scaleCoroutine = StartCoroutine(ScaleTo(target, 0.1f));
    }

    IEnumerator ScaleTo(Vector3 target, float duration)
    {
        Vector3 start = transform.localScale;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(start, target, t / duration);
            yield return null;
        }

        transform.localScale = target;
    }

    // =========================
    // POP EFFECT
    // =========================
    IEnumerator PopEffect()
    {
        Vector3 original = transform.localScale;
        Vector3 bigger = original * 1.15f;

        float t = 0f;

        while (t < 0.08f)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(original, bigger, t / 0.08f);
            yield return null;
        }

        t = 0f;

        while (t < 0.08f)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(bigger, baseScale, t / 0.08f);
            yield return null;
        }

        transform.localScale = baseScale;
    }

    // =========================
    // SPRITE CONTROL
    // =========================
    void SetOpen()
    {
        if (containerSprite != null && openSprite != null)
            containerSprite.sprite = openSprite;
    }

    void SetClosed()
    {
        if (containerSprite != null && closedSprite != null)
            containerSprite.sprite = closedSprite;
    }
}