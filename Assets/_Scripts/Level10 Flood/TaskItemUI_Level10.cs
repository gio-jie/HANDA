using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TaskItemUI_Level10 : MonoBehaviour
{
    [Header("UI")]
    public Image iconImage;

    private bool completed = false;

    public void CompleteTask()
    {
        if (completed) return;

        completed = true;

        Color c = Color.white;
        c.a = 1f;
        iconImage.color = c;

        StartCoroutine(PopIcon());
    }

    private IEnumerator PopIcon()
    {
        Vector3 originalScale = iconImage.transform.localScale;
        Vector3 popScale = originalScale * 1.5f;

        float duration = 0.2f;
        float halfDuration = duration / 2f;
        float t = 0f;

        while (t < halfDuration)
        {
            t += Time.deltaTime;
            iconImage.transform.localScale = Vector3.Lerp(originalScale, popScale, t / halfDuration);
            yield return null;
        }

        t = 0f;
        while (t < halfDuration)
        {
            t += Time.deltaTime;
            iconImage.transform.localScale = Vector3.Lerp(popScale, originalScale, t / halfDuration);
            yield return null;
        }

        iconImage.transform.localScale = originalScale;
    }
}