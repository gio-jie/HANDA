using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Button))]
public class ButtonPopEffect : MonoBehaviour
{
    private Vector3 originalScale;
    private Coroutine popCoroutine;
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        originalScale = transform.localScale;

        button.onClick.AddListener(PlayPop);
    }

    void PlayPop()
    {
        if (popCoroutine != null)
        {
            StopCoroutine(popCoroutine);
            transform.localScale = originalScale;
        }

        popCoroutine = StartCoroutine(Pop());
    }

    IEnumerator Pop()
    {
        float duration = 0.08f;
        float timer = 0f;

        Vector3 shrinkScale = originalScale * 0.9f;
        Vector3 expandScale = originalScale * 1.1f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            transform.localScale = Vector3.Lerp(originalScale, shrinkScale, timer / duration);
            yield return null;
        }

        timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            transform.localScale = Vector3.Lerp(shrinkScale, expandScale, timer / duration);
            yield return null;
        }

        timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            transform.localScale = Vector3.Lerp(expandScale, originalScale, timer / duration);
            yield return null;
        }

        transform.localScale = originalScale;
        popCoroutine = null;
    }
}