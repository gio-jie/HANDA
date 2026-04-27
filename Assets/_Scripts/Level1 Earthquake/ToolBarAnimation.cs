using UnityEngine;
using System.Collections;

public class ToolBarAnimation : MonoBehaviour
{
    public RectTransform toolBar;

    public float duration;
    public float overshoot = 30f;

    private Vector2 hiddenPos;
    private Vector2 finalPos;

    void Start()
    {
        finalPos = toolBar.anchoredPosition;
        hiddenPos = finalPos + new Vector2(0, -350);

        toolBar.anchoredPosition = hiddenPos;
    }

    public void ShowToolBar()
    {
        StartCoroutine(AnimateToolBar());
    }

    IEnumerator AnimateToolBar()
    {
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            float y = Mathf.Lerp(hiddenPos.y, finalPos.y + overshoot, t);
            toolBar.anchoredPosition = new Vector2(finalPos.x, y);

            yield return null;
        }

        time = 0;

        while (time < duration * 0.5f)
        {
            time += Time.deltaTime;
            float t = time / (duration * 0.5f);

            float y = Mathf.Lerp(finalPos.y + overshoot, finalPos.y, t);
            toolBar.anchoredPosition = new Vector2(finalPos.x, y);

            yield return null;
        }

        toolBar.anchoredPosition = finalPos;
    }

    public void HideToolBar()
    {
        StartCoroutine(HideAnimation());
    }

    IEnumerator HideAnimation()
    {
        float duration = 0.2f;
        float time = 0;

        Vector2 startPos = toolBar.anchoredPosition;
        Vector2 targetPos = startPos + new Vector2(0, -350);

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            t = Mathf.SmoothStep(0, 1, t);

            toolBar.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        toolBar.anchoredPosition = targetPos;
    }
}