using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class ToolDropHandlerLevel5 : MonoBehaviour
{
    public static ToolDropHandlerLevel5 Instance;

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

    // =========================================================
    // DROP ENTRY
    // =========================================================
    public void HandleDrop(ToolItemLevel5 tool, PointerEventData eventData)
    {
        GameObject hit = eventData.pointerCurrentRaycast.gameObject;

        DropTargetLevel5 target = null;
        ScenarioZone zone = null;

        if (hit != null)
        {
            target = hit.GetComponent<DropTargetLevel5>() ??
                     hit.GetComponentInParent<DropTargetLevel5>();

            zone = hit.GetComponent<ScenarioZone>();
        }

        // =========================================================
        // 🎮 GAMEPLAY (SCENARIO SYSTEM)
        // =========================================================
        if (tool.toolMode == ToolItemLevel5.ToolMode.Gameplay &&
            zone != null &&
            ScenarioManager.Instance != null &&
            ScenarioManager.Instance.currentScenario != null)
        {
            //HandleGameplayDrop(tool, hit);
            ScenarioManager.Instance.ProcessToolDrop(tool);
            Debug.Log("Dropped on: " + hit?.name);
            Debug.Log("Zone detected: " + (zone != null));
            
            return;
        }

        // =========================================================
        // 🧩 TUTORIAL SYSTEM (UNCHANGED)
        // =========================================================
        if (target != null)
        {
            if (target.requiredToolIDs.Contains(tool.toolID))
            {
                if (AudioManager.instance != null)
                    AudioManager.instance.PlaySFX(AudioManager.instance.correctSound);
                    
                StartCoroutine(ApplyCorrectTool(tool, target));
            }
            else
            {
                ReturnTool(tool);

                if (completedToolCount < totalTools &&
                    tool.toolMode == ToolItemLevel5.ToolMode.Gameplay)
                {
                    PlayFlash(GetTargetImage(target));
                }
            }

            FindObjectOfType<IntroManager>()?.ScalePlayerDown();
            return;
        }

        // =========================================================
        // ❌ NO VALID TARGET
        // =========================================================
        HandleNoTarget(tool, hit);
    }

    // =========================================================
    // 🎮 GAMEPLAY HANDLER
    // =========================================================
    void HandleGameplayDrop(ToolItemLevel5 tool, GameObject hit)
    {
        Transform snapPoint =
            ScenarioManager.Instance.GetSnapPoint(tool.toolID);

        if (snapPoint != null)
        {
            ScenarioManager.Instance.ProcessToolDrop(tool);
        }
        else
        {
            ReturnTool(tool);
        }
    }

    // =========================================================
    // ❌ NO TARGET HANDLING
    // =========================================================
    void HandleNoTarget(ToolItemLevel5 tool, GameObject hit)
    {
        FindObjectOfType<IntroManager>()?.ScalePlayerDown();

        if (IsToolbarArea(hit))
        {
            tool.ResetToToolbar();
            return;
        }

        ReturnTool(tool);

        if (tool.toolMode == ToolItemLevel5.ToolMode.Gameplay &&
            completedToolCount < totalTools)
        {
            PlayFlash(tool.GetComponent<Image>());
        }
    }

    // =========================================================
    // RETURN TOOL
    // =========================================================
    void ReturnTool(ToolItemLevel5 tool)
    {
        tool.ResetToToolbar();

        if (completedToolCount >= totalTools)
            return;

        if (tool.toolMode == ToolItemLevel5.ToolMode.Gameplay)
        {
            StarManagerEarthquake5.Instance?.RegisterWrongItem();
        }
    }

    // =========================================================
    // FLASH
    // =========================================================
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

    Image GetTargetImage(DropTargetLevel5 target)
    {
        return target.GetComponent<Image>();
    }

    // =========================================================
    // TUTORIAL FLOW (UNCHANGED)
    // =========================================================
    IEnumerator ApplyCorrectTool(ToolItemLevel5 tool, DropTargetLevel5 target)
    {
        target.isCompleted = true;
        completedToolCount++;

        FindObjectOfType<IntroManager>()?.RegisterTool(tool.toolID);

        tool.gameObject.SetActive(false);

        Vector3 baseScale = target.transform.localScale;
        Vector3 bigScale = baseScale * target.popScale;
        Vector3 smallScale = baseScale * 0.9f;

        if (target.fixedObject != null)
            target.fixedObject.SetActive(true);

        float time = 0f;
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
    }

    // =========================================================
    // TOOLBAR CHECK
    // =========================================================
    bool IsToolbarArea(GameObject hit)
    {
        if (hit == null) return false;

        return hit.CompareTag("Toolbar") ||
               hit.GetComponentInParent<RectTransform>()?.CompareTag("Toolbar") == true;
    }
}