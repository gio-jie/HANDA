using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ScenarioManager : MonoBehaviour
{
    public static ScenarioManager Instance;

    [Header("UI")]
    public GameObject panel;
    public RectTransform contentRoot;
    public TMP_Text scenarioText;
    public Image scenarioImage;

    [Header("Sprites")]
    public Sprite fixedScenarioSprite; // ✅ SINGLE FIXED SPRITE

    [Header("Toolbar")]
    public ToolBarAnimation toolBarAnim;

    [Header("References")]
    public MazePlayerEarthquake player;
    public VirtualJoystick joystick;

    [Header("Scenario Data")]
    public ScenarioData currentScenario;

    [Header("Snap Points")]
    public Transform broomSnapPoint;
    public Transform extinguisherSnapPoint;
    public Transform wrenchSnapPoint;

    [Header("VFX")]
    public GameObject broomVFX;
    public GameObject extinguisherVFX;

    private bool isBusy;
    private bool scenarioActive;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    // =========================================================
    // SHOW SCENARIO
    // =========================================================
    public void ShowScenario(ScenarioData data)
    {
        if (isBusy || data == null) return;

        currentScenario = data;
        StartCoroutine(ScenarioFlow(data));
    }

    IEnumerator ScenarioFlow(ScenarioData data)
    {
        isBusy = true;
        scenarioActive = true;

        player?.LockInput();

        panel.SetActive(true);
        contentRoot.localScale = Vector3.zero;

        scenarioText.text = data.mission;
        scenarioImage.sprite = data.image;

        toolBarAnim?.ShowToolBar();

        yield return PopIn(contentRoot);
    }

    // =========================================================
    // ENTRY POINT
    // =========================================================
    public void ProcessToolDrop(ToolItemLevel5 tool)
    {
        StartCoroutine(ProcessToolRoutine(tool));
    }

    // =========================================================
    // MAIN LOGIC
    // =========================================================
    IEnumerator ProcessToolRoutine(ToolItemLevel5 tool)
    {
        string toolID = tool.toolID.ToLower().Trim();

        bool isCorrect = false;

        foreach (string id in currentScenario.correctToolIDs)
        {
            if (id.ToLower().Trim() == toolID)
            {
                isCorrect = true;
                break;
            }
        }

        // =====================================================
        // ❌ WRONG TOOL
        // =====================================================
        if (!isCorrect)
        {
            tool.ResetToToolbar();

            LayoutRebuilder.ForceRebuildLayoutImmediate(
                tool.transform.parent as RectTransform
            );

            StarManagerEarthquake5.Instance?.RegisterWrongItem();

            PlayFlash(tool.GetComponent<Image>());

            yield break;
        }

        // =====================================================
        // ✅ CORRECT TOOL
        // =====================================================
        Transform snapPoint = GetSnapPoint(toolID);
        StarManagerEarthquake5.Instance?.RegisterCorrectItem();

        if (snapPoint == null)
        {
            Debug.Log("snap point null");
            tool.ResetToToolbar();
            yield break;
        }

        isBusy = true;

        GameObject toolObj = tool.gameObject;

        // SNAP
        toolObj.transform.position = snapPoint.position;
        toolObj.transform.rotation = snapPoint.rotation;
        toolObj.transform.SetParent(snapPoint);

        Vector3 originalScale = toolObj.transform.localScale;
        toolObj.transform.localScale = originalScale * 1.1f;

        // PLAY ACTION
        yield return StartCoroutine(PlayScenarioAction(tool));

        toolObj.transform.localScale = originalScale;

        // =====================================================
        // WAIT BEFORE UI CHANGE
        // =====================================================
        yield return new WaitForSecondsRealtime(0.2f);

        // =====================================================
        // CHANGE TO FIXED IMAGE (HERE IS THE FIX)
        // =====================================================
        if (scenarioImage != null && fixedScenarioSprite != null)
        {
            scenarioImage.sprite = fixedScenarioSprite;
        }

        yield return new WaitForSecondsRealtime(0.2f);

        // RETURN TOOL
        tool.ResetToToolbar();

        // CLOSE PANEL
        yield return PopOut(contentRoot);

        toolBarAnim?.HideToolBar();

        scenarioActive = false;
        isBusy = false;

        player?.UnlockInput();
    }

    // =========================================================
    // SNAP POINT
    // =========================================================
    public Transform GetSnapPoint(string toolID)
    {
        if (toolID.Contains("broom"))
            return broomSnapPoint;

        if (toolID.Contains("extinguisher"))
            return extinguisherSnapPoint;

        if (toolID.Contains("wrench"))
            return wrenchSnapPoint;

        return null;
    }

    // =========================================================
    // TOOL ACTIONS
    // =========================================================
    IEnumerator PlayScenarioAction(ToolItemLevel5 tool)
    {
        string id = tool.toolID.ToLower();

        if (id.Contains("broom") && broomVFX != null)
        {
            broomVFX.SetActive(true);
            yield return new WaitForSeconds(1.5f);
            broomVFX.SetActive(false);
        }
        else if (id.Contains("extinguisher") && extinguisherVFX != null)
        {
            extinguisherVFX.SetActive(true);
            yield return new WaitForSeconds(2f);
            extinguisherVFX.SetActive(false);
        }
        else if (id.Contains("wrench"))
        {
            Quaternion baseRot = tool.transform.rotation;
            float t = 0f;

            while (t < 1.2f)
            {
                t += Time.deltaTime;

                float angle = Mathf.Sin(t * 10f) * 10f;

                tool.transform.rotation =
                    baseRot * Quaternion.Euler(0, 0, angle);

                yield return null;
            }

            tool.transform.rotation = baseRot;
        }
    }

    // =========================================================
    // FLASH
    // =========================================================
    void PlayFlash(Image img)
    {
        if (img == null) return;
        StartCoroutine(FlashRoutine(img));
    }

    IEnumerator FlashRoutine(Image img)
    {
        Color original = img.color;

        img.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        img.color = original;
    }

    // =========================================================
    // UI ANIMATION
    // =========================================================
    IEnumerator PopIn(RectTransform target)
    {
        float t = 0f;
        float dur = 0.2f;

        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            float p = t / dur;

            target.localScale = Vector3.one * Mathf.Lerp(0f, 1f, p);
            yield return null;
        }

        target.localScale = Vector3.one;
    }

    IEnumerator PopOut(RectTransform target)
    {
        float t = 0f;
        float dur = 0.2f;

        Vector3 start = target.localScale;

        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            float p = t / dur;

            target.localScale = Vector3.Lerp(start, Vector3.zero, p);
            yield return null;
        }

        target.localScale = Vector3.zero;
        panel.SetActive(false);
    }
}