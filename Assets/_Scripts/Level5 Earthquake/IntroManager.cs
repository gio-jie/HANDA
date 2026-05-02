using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class IntroManager : MonoBehaviour
{
    [Header("Tracking")]
    public List<string> requiredToolIDs = new List<string>();
    private List<string> collectedTools = new List<string>();

    [Header("Player")]
    public RectTransform playerRect;
    public Image playerImage;
    public Sprite playerReadySprite;

    private Vector3 baseScale = Vector3.one;

    private Coroutine scaleRoutine;
    private Coroutine popRoutine;

    public float hoverScaleMultiplier = 1.1f;

    private bool isDone;

    [Header("Mission UI")]
    public TextMeshProUGUI missionText;

    [Header("Intro Panel")]
    public CanvasGroup introPanel;
    public float fadeDuration = 0.5f;

    void Start()
    {
        baseScale = playerRect.localScale;
    }

    // =========================
    // SAFE COROUTINES
    // =========================

    void StopScale()
    {
        if (scaleRoutine != null)
        {
            StopCoroutine(scaleRoutine);
            scaleRoutine = null;
        }
    }

    void StopPop()
    {
        if (popRoutine != null)
        {
            StopCoroutine(popRoutine);
            popRoutine = null;
        }
    }

    void StopAll()
    {
        StopScale();
        StopPop();
    }

    void SetScaleImmediate(Vector3 scale)
    {
        playerRect.localScale = scale;
    }

    // =========================
    // HOVER SCALE
    // =========================

    public void ScalePlayerUp()
    {
        //if (isDone) return;

        StopAll();
        scaleRoutine = StartCoroutine(ScaleRoutine(Vector3.one * hoverScaleMultiplier));
    }

    public void ScalePlayerDown()
    {
        //if (isDone) return;

        //StopAll();
        scaleRoutine = StartCoroutine(ScaleRoutine(Vector3.one));
    }

    IEnumerator ScaleRoutine(Vector3 target)
    {
        Vector3 start = baseScale;

        float time = 0f;
        float duration = 0.1f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;

            float t = Mathf.SmoothStep(0f, 1f, time / duration);
            playerRect.localScale = Vector3.Lerp(start, target, t);

            yield return null;
        }

        playerRect.localScale = target;
    }

    // =========================
    // CARTOON POP
    // =========================

    public void PlayPlayerPop()
    {
        StopAll();
        popRoutine = StartCoroutine(PlayerPopRoutine());
    }

    IEnumerator PlayerPopRoutine()
    {
        Vector3 start = baseScale;
        SetScaleImmediate(start);

        Vector3 overshoot = baseScale * 1.45f;
        Vector3 squash = baseScale * 0.8f;

        float duration = 0.06f;
        float time = 0f;

        // UP
        while (time < duration)
        {
            time += Time.unscaledDeltaTime;

            float t = Mathf.Pow(time / duration, 0.35f);
            playerRect.localScale = Vector3.Lerp(start, overshoot, t);

            yield return null;
        }

        SetScaleImmediate(overshoot);

        // DOWN
        time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;

            float t = Mathf.Pow(time / duration, 1.6f);
            playerRect.localScale = Vector3.Lerp(overshoot, squash, t);

            yield return null;
        }

        SetScaleImmediate(squash);

        // SETTLE
        time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;

            float t = Mathf.SmoothStep(0f, 1f, time / duration);
            playerRect.localScale = Vector3.Lerp(squash, baseScale, t);

            yield return null;
        }

        SetScaleImmediate(baseScale);
    }

    // =========================
    // FORCE RESET
    // =========================

    public void ForceResetScale()
    {
        StopAll();
        playerRect.localScale = baseScale;
    }

    // =========================
    // TOOL TRACKING
    // =========================

    public void RegisterTool(string toolID)
    {
        if (isDone) return;
        if (!requiredToolIDs.Contains(toolID)) return;

        if (!collectedTools.Contains(toolID))
            collectedTools.Add(toolID);

        if (collectedTools.Count >= requiredToolIDs.Count)
            StartCoroutine(CompleteIntro());
    }

    // =========================
    // COMPLETE INTRO
    // =========================

    IEnumerator CompleteIntro()
    {
        isDone = true;

        playerImage.sprite = playerReadySprite;

        playerRect.anchoredPosition = new Vector2(-21f, 16.1f);
        playerRect.sizeDelta = new Vector2(363.67f, 534.06f);

        yield return StartCoroutine(PopText("Nice! We are now ready. Let's go!"));
        yield return StartCoroutine(FadeOutPanel());

        introPanel.gameObject.SetActive(false);
    }

    // =========================
    // TEXT POP
    // =========================

    IEnumerator PopText(string text)
    {
        missionText.text = text;

        Vector3 start = missionText.transform.localScale;
        Vector3 big = start * 1.25f;

        float time = 0f;
        float duration = 0.08f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;

            float t = Mathf.Pow(time / duration, 0.5f);
            missionText.transform.localScale = Vector3.Lerp(start, big, t);

            yield return null;
        }

        time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;

            float t = Mathf.Pow(time / duration, 1.6f);
            missionText.transform.localScale = Vector3.Lerp(big, start, t);

            yield return null;
        }

        missionText.transform.localScale = start;
    }

    // =========================
    // FADE OUT
    // =========================

    IEnumerator FadeOutPanel()
    {
        float time = 0f;
        float startAlpha = introPanel.alpha;

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            introPanel.alpha = Mathf.Lerp(startAlpha, 0f, time / fadeDuration);
            yield return null;
        }

        introPanel.alpha = 0f;
        introPanel.interactable = false;
        introPanel.blocksRaycasts = false;
    }
}