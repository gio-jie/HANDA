using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class ToolDropHandler : MonoBehaviour
{
    public static ToolDropHandler Instance;

    [Header("Flash Feedback")]
    public Color flashColor = Color.red;
    public float flashDuration = 0.15f;
    public int flashCount = 2;

    private Coroutine flashCoroutine;

    public int totalTools = 3;
    private int completedToolCount;

    void Awake()
    {
        Instance = this;
    }

    public void HandleDrop(ToolItem tool, PointerEventData eventData)
    {                
        GameObject hit = eventData.pointerCurrentRaycast.gameObject;

        DropTarget target = null;

        if (hit != null)
        {
            target = hit.GetComponent<DropTarget>();

            if (target == null)
                target = hit.GetComponentInParent<DropTarget>();
        }

        if (target == null)
        {
            if (IsToolbarArea(hit))
            {
                tool.ResetToToolbar();
                return;
            }

            ReturnTool(tool);

            if (completedToolCount >= totalTools)
                return;

            PlayFlash(tool.GetComponent<Image>());
            return;
        }

        if (tool.toolID == target.requiredToolID)
        {
            StartCoroutine(ApplyCorrectTool(tool, target));
        }
        else
        {
            ReturnTool(tool);

            if (completedToolCount >= totalTools)
                return;

            PlayFlash(GetTargetImage(target));
        }
    }

    bool IsToolbarArea(GameObject hit)
    {
        if (hit == null) return false;

        return hit.CompareTag("Toolbar") ||
            hit.GetComponentInParent<RectTransform>()?.CompareTag("Toolbar") == true;
    }

    void ReturnTool(ToolItem tool)
    {
        tool.ResetToToolbar();

        if (completedToolCount >= totalTools)
            return;

        StarManagerEarthquake1.Instance.RegisterWrong();
    }

    void PlayFlash(Image img)
    {
        if (img == null) return;

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashRoutine(img));
    }

    IEnumerator FlashRoutine(Image img)
    {
        Color original = img.color;

        for (int i = 0; i < flashCount; i++)
        {
            img.color = flashColor;
            yield return new WaitForSeconds(flashDuration);

            img.color = original;
            yield return new WaitForSeconds(flashDuration);
        }

        img.color = original;
    }

    Image GetTargetImage(DropTarget target)
    {
        return target.GetComponent<Image>();
    }

    IEnumerator ApplyCorrectTool(ToolItem tool, DropTarget target)
    {
        if (target.isCompleted)
        {
            tool.ResetToToolbar();
            tool.gameObject.SetActive(true);
            yield break;
        }

        target.isCompleted = true;

        completedToolCount++;

        StarManagerEarthquake1.Instance.RegisterCorrect();
        StarManagerEarthquake1.Instance.RegisterTaskComplete();

        tool.gameObject.SetActive(false);

        Vector3 baseScale = target.transform.localScale;
        Vector3 bigScale = baseScale * target.popScale;
        Vector3 smallScale = baseScale * 0.9f;

        if (target.fixedObject != null)
            target.fixedObject.SetActive(true);
        
        if (target.correctSFX != null)
        {
            AudioSource source = target.GetComponent<AudioSource>();

            if (source != null)
                source.PlayOneShot(target.correctSFX, target.sfxVolume);
        }

        float time;

        time = 0f;
        float durationUp = 0.25f;

        while (time < durationUp)
        {
            time += Time.deltaTime;
            float t = 1f - Mathf.Pow(1f - (time / durationUp), 3f);
            target.transform.localScale = Vector3.Lerp(baseScale, bigScale, t);
            yield return null;
        }

        time = 0f;
        float durationDown = 0.25f;

        while (time < durationDown)
        {
            time += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, time / durationDown);
            target.transform.localScale = Vector3.Lerp(bigScale, smallScale, t);
            yield return null;
        }

        time = 0f;
        float durationSettle = 0.25f;

        while (time < durationSettle)
        {
            time += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, time / durationSettle);
            target.transform.localScale = Vector3.Lerp(smallScale, baseScale, t);
            yield return null;
        }

        target.transform.localScale = baseScale;

        tool.ResetToToolbar();
        tool.gameObject.SetActive(true);

        if (CheckmarkManager.Instance != null)
            CheckmarkManager.Instance.MarkComplete(target);

        StarManagerEarthquake1.Instance.FinishLevelAfterAnimation();
    }
}