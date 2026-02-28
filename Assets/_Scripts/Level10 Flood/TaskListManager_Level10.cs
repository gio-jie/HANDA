using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TaskListManager_Level10 : MonoBehaviour
{
    public static TaskListManager_Level10 Instance;

    [Header("UI")]
    public GameObject taskListPanel;
    public RectTransform panelRect; 
    public RectTransform bagIcon;   
    public List<TaskItemUI_Level10> taskItems;

    private Vector2 panelOriginalPos;
    private Vector2 bagOriginalPos;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        panelOriginalPos = panelRect.anchoredPosition;
        if (bagIcon != null)
            bagOriginalPos = bagIcon.anchoredPosition;

        taskListPanel.SetActive(false);
        if (bagIcon != null)
            bagIcon.gameObject.SetActive(false);
    }

    public void ShowTaskList()
    {
        taskListPanel.SetActive(true);
        if (bagIcon != null)
            bagIcon.gameObject.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(OpenFromTop());
    }

    public void HideTaskList()
    {
        StopAllCoroutines();
        StartCoroutine(CloseToTop());
    }

    private IEnumerator OpenFromTop()
    {
        Vector2 startPos = panelOriginalPos + new Vector2(0, Screen.height + panelRect.rect.height);
        panelRect.anchoredPosition = startPos;

        Vector2 bagStart = bagOriginalPos;
        if (bagIcon != null)
            bagStart = bagOriginalPos + new Vector2(0, Screen.height + panelRect.rect.height);

        float t = 0f;
        float duration = 0.35f;

        float overshoot = 40f;
        Vector2 overshootPos = new Vector2(panelOriginalPos.x, panelOriginalPos.y - overshoot);

        while (t < duration)
        {
            t += Time.deltaTime;
            float factor = Mathf.Sin(t / duration * Mathf.PI * 0.5f);
            panelRect.anchoredPosition = Vector2.Lerp(startPos, overshootPos, factor);

            if (bagIcon != null)
                bagIcon.anchoredPosition = Vector2.Lerp(bagStart, bagOriginalPos - new Vector2(0, overshoot), factor);

            yield return null;
        }

        float bounceHeight = 35f;     
        float bounceDuration = 0.08f; 
        t = 0f;
        Vector2 currentPos = panelRect.anchoredPosition;

        while (t < bounceDuration)
        {
            t += Time.deltaTime;
            float factor = Mathf.Pow(t / bounceDuration, 0.5f);
            panelRect.anchoredPosition = Vector2.Lerp(currentPos, panelOriginalPos + new Vector2(0, bounceHeight), factor);
            if (bagIcon != null)
                bagIcon.anchoredPosition = Vector2.Lerp(bagIcon.anchoredPosition, bagOriginalPos + new Vector2(0, bounceHeight), factor);
            yield return null;
        }

        t = 0f;
        Vector2 peakPos = panelOriginalPos + new Vector2(0, bounceHeight);
        while (t < bounceDuration)
        {
            t += Time.deltaTime;
            float factor = Mathf.Pow(t / bounceDuration, 0.5f);
            panelRect.anchoredPosition = Vector2.Lerp(peakPos, panelOriginalPos, factor);
            if (bagIcon != null)
                bagIcon.anchoredPosition = Vector2.Lerp(peakPos, bagOriginalPos, factor);
            yield return null;
        }

        panelRect.anchoredPosition = panelOriginalPos;
        if (bagIcon != null)
            bagIcon.anchoredPosition = bagOriginalPos;
    }

    private IEnumerator CloseToTop()
    {
        Vector2 targetPos = panelOriginalPos + new Vector2(0, panelRect.rect.height + 200f);
        Vector2 startPos = panelRect.anchoredPosition;

        Vector2 bagStart = bagOriginalPos;
        Vector2 bagTarget = bagOriginalPos + new Vector2(0, panelRect.rect.height + 200f);

        float t = 0f;
        float duration = 0.2f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float factor = Mathf.Sin(t / duration * Mathf.PI * 0.5f);
            panelRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, factor);

            if (bagIcon != null)
                bagIcon.anchoredPosition = Vector2.Lerp(bagStart, bagTarget, factor);

            yield return null;
        }

        panelRect.anchoredPosition = targetPos;
        if (bagIcon != null)
            bagIcon.anchoredPosition = bagOriginalPos;

        taskListPanel.SetActive(false);
        if (bagIcon != null)
            bagIcon.gameObject.SetActive(false);
    }

    private IEnumerator BouncePosition()
    {
        float t = 0f;
        float duration = 0.1f;
        float bounceHeight = 15f;

        Vector2 startPos = panelOriginalPos;
        Vector2 peakPos = panelOriginalPos + new Vector2(0, bounceHeight);

        while (t < duration)
        {
            t += Time.deltaTime;
            float factor = Mathf.Sin(t / duration * Mathf.PI * 0.5f);
            panelRect.anchoredPosition = Vector2.Lerp(startPos, peakPos, factor);
            yield return null;
        }

        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float factor = Mathf.Sin(t / duration * Mathf.PI * 0.5f);
            panelRect.anchoredPosition = Vector2.Lerp(peakPos, startPos, factor);
            yield return null;
        }

        panelRect.anchoredPosition = startPos;
    }

    public void CompleteTask(int index)
    {
        if (index < 0 || index >= taskItems.Count) return;
        taskItems[index].CompleteTask();
    }
}