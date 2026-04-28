using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ItemScenarioPopupUI : MonoBehaviour
{
    public static ItemScenarioPopupUI Instance;

    [Header("Panel")]
    public GameObject panel;

    [Header("UI Elements")]
    public Image itemIcon;
    public Image arrowImage;
    public Image scenarioImage;
    public TextMeshProUGUI descriptionText;

    [Header("Animation Settings")]
    public float popScale = 1.2f;
    public float popDuration = 0.2f;
    public float delayBetween = 0.1f;

    [Header("Icon Wiggle")]
    public float wiggleSpeed = 6f;
    public float wiggleAmount = 15f;

    [System.Serializable]
    public class ScenarioEntry
    {
        public string itemName;
        public Sprite scenarioSprite;
        public string itemDescription;
    }

    public List<ScenarioEntry> scenarioEntries = new List<ScenarioEntry>();

    private Coroutine wiggleCoroutine;
    private bool isShowing = false;

    private float savedTimeScale = 1f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    void Update()
    {
        if (isShowing && Input.GetMouseButtonDown(0))
        {
            Close();
        }
    }

    // =========================
    // SHOW
    // =========================
    public void Show(SortingItemData data)
    {
        if (data == null || panel == null) return;

        panel.SetActive(true);
        isShowing = true;

        // pause game
        savedTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        var entry = GetScenarioEntry(data);

        if (scenarioImage != null)
            scenarioImage.sprite = entry != null ? entry.scenarioSprite : null;

        if (itemIcon != null)
            itemIcon.sprite = data.itemSprite;

        if (descriptionText != null)
        {
            if (entry != null && !string.IsNullOrEmpty(entry.itemDescription))
                descriptionText.text = entry.itemDescription;
            else
                descriptionText.text = data.description;
        }

        ResetTransforms();

        StopAllCoroutines();
        StartCoroutine(PlaySequence());

        if (wiggleCoroutine != null)
            StopCoroutine(wiggleCoroutine);

        wiggleCoroutine = StartCoroutine(WiggleIcon());
    }

    // =========================
    // GET ENTRY
    // =========================
    ScenarioEntry GetScenarioEntry(SortingItemData data)
    {
        foreach (var entry in scenarioEntries)
        {
            if (entry.itemName == data.itemName)
                return entry;
        }
        return null;
    }

    // =========================
    // SEQUENCE
    // =========================
    IEnumerator PlaySequence()
    {
        yield return StartCoroutine(Pop(itemIcon.rectTransform));
        yield return new WaitForSecondsRealtime(delayBetween);

        yield return StartCoroutine(Pop(arrowImage.rectTransform));
        yield return new WaitForSecondsRealtime(delayBetween);

        yield return StartCoroutine(Pop(scenarioImage.rectTransform));
    }

    // =========================
    // POP ANIMATION
    // =========================
    IEnumerator Pop(RectTransform target)
    {
        target.localScale = Vector3.zero;

        float t = 0f;
        while (t < popDuration)
        {
            t += Time.unscaledDeltaTime;
            float scale = Mathf.Lerp(0f, popScale, t / popDuration);
            target.localScale = Vector3.one * scale;
            yield return null;
        }

        t = 0f;
        while (t < 0.1f)
        {
            t += Time.unscaledDeltaTime;
            target.localScale = Vector3.Lerp(Vector3.one * popScale, Vector3.one, t / 0.1f);
            yield return null;
        }

        target.localScale = Vector3.one;
    }

    // =========================
    // RESET
    // =========================
    void ResetTransforms()
    {
        if (itemIcon != null)
            itemIcon.rectTransform.localScale = Vector3.zero;

        if (arrowImage != null)
            arrowImage.rectTransform.localScale = Vector3.zero;

        if (scenarioImage != null)
            scenarioImage.rectTransform.localScale = Vector3.zero;
    }

    // =========================
    // WIGGLE
    // =========================
    IEnumerator WiggleIcon()
    {
        RectTransform rt = itemIcon.rectTransform;

        while (true)
        {
            float angle = Mathf.Sin(Time.unscaledTime * wiggleSpeed) * wiggleAmount;
            rt.localRotation = Quaternion.Euler(0f, 0f, angle);
            yield return null;
        }
    }

    // =========================
    // CLOSE
    // =========================
    public void Close()
    {
        isShowing = false;

        StopAllCoroutines();

        if (wiggleCoroutine != null)
            StopCoroutine(wiggleCoroutine);

        if (itemIcon != null)
            itemIcon.rectTransform.localRotation = Quaternion.identity;

        if (panel != null)
            panel.SetActive(false);

        Time.timeScale = savedTimeScale;
    }
}