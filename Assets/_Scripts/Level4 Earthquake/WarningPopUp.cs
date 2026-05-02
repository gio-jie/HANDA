using UnityEngine;
using TMPro;
using System.Collections;

public class WarningPopUp : MonoBehaviour
{
    public static WarningPopUp Instance;

    [Header("UI")]
    public RectTransform contentRoot;
    public TMP_Text warningText;

    [Header("Settings")]
    public float displayTime = 1.5f;

    private Coroutine currentRoutine;
    private bool isPlaying = false;

    void Awake()
    {
        Instance = this;
        contentRoot.gameObject.SetActive(false);
    }

    // =========================
    // SHOW WARNING (ALLOW REPEAT)
    // =========================
    public void ShowWarning(string message)
    {
        // only prevents overlapping animations, NOT re-triggering
        if (isPlaying) return;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowRoutine(message));
    }

    IEnumerator ShowRoutine(string message)
    {
        isPlaying = true;

        contentRoot.gameObject.SetActive(true);
        warningText.text = message;

        yield return StartCoroutine(PopIn());

        yield return new WaitForSeconds(displayTime);

        yield return StartCoroutine(PopOut());

        contentRoot.gameObject.SetActive(false);

        isPlaying = false;
    }

    // =========================
    // POP IN
    // =========================
    IEnumerator PopIn()
    {
        contentRoot.localScale = Vector3.zero;

        float t = 0f;
        float duration = 0.28f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float p = t / duration;

            float scale = EaseOutBack(p);
            contentRoot.localScale = Vector3.one * scale;

            yield return null;
        }

        contentRoot.localScale = Vector3.one;
    }

    // =========================
    // POP OUT
    // =========================
    IEnumerator PopOut()
    {
        float t = 0f;
        float duration = 0.18f;

        Vector3 start = contentRoot.localScale;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float p = t / duration;

            float scale = EaseInBack(p);
            contentRoot.localScale = Vector3.Lerp(start, Vector3.zero, scale);

            yield return null;
        }

        contentRoot.localScale = Vector3.zero;
    }

    // =========================
    // EASING
    // =========================
    float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c2 = c1 * 1.525f;
        return 1 + (c2 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2));
    }

    float EaseInBack(float t)
    {
        float c1 = 1.70158f;
        float c2 = c1 * 1.525f;
        return c2 * t * t * t - c1 * t * t;
    }
}