using UnityEngine;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TMP_Text instructionText;

    private Coroutine animRoutine;
    private Coroutine warningRoutine;

    private string currentMission;

    void Awake()
    {
        Instance = this;
    }

    // 🎯 MAIN MISSION TEXT
    public void SetInstruction(string newText)
    {
        currentMission = newText;

        if (animRoutine != null)
            StopCoroutine(animRoutine);

        animRoutine = StartCoroutine(AnimateTextChange(newText));
    }

    // ❌ TEMP WARNING (OVERRIDES MISSION)
    public void ShowTemporaryInstruction(string warningText, float duration = 1.2f)
    {
        if (warningRoutine != null)
            StopCoroutine(warningRoutine);

        warningRoutine = StartCoroutine(WarningRoutine(warningText, duration));
    }

    IEnumerator WarningRoutine(string text, float duration)
    {
        instructionText.text = text;

        float t = 0;
        instructionText.alpha = 0;
        instructionText.transform.localScale = Vector3.one * 0.8f;

        // pop in
        while (t < 1f)
        {
            t += Time.deltaTime * 8f;

            instructionText.alpha = Mathf.Lerp(0, 1, t);
            instructionText.transform.localScale = Vector3.Lerp(
                Vector3.one * 0.8f,
                Vector3.one * 1.15f,
                t
            );

            yield return null;
        }

        instructionText.transform.localScale = Vector3.one;

        yield return new WaitForSeconds(duration);

        RestoreMissionText();
    }

    // 🔄 RESTORE MISSION
    public void RestoreMissionText()
    {
        if (animRoutine != null)
            StopCoroutine(animRoutine);

        animRoutine = StartCoroutine(AnimateTextChange(currentMission));
    }

    // 🎬 YOUR EXISTING ANIMATION (UNCHANGED BUT CLEANED)
    IEnumerator AnimateTextChange(string newText)
    {
        float t = 0;
        Vector3 startScale = Vector3.one;

        while (t < 1)
        {
            t += Time.deltaTime * 8f;

            instructionText.alpha = Mathf.Lerp(1, 0, t);
            instructionText.transform.localScale = Vector3.Lerp(
                startScale,
                Vector3.one * 0.8f,
                t
            );

            yield return null;
        }

        instructionText.text = newText;

        instructionText.transform.localScale = Vector3.one * 0.8f;

        t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * 8f;

            instructionText.alpha = Mathf.Lerp(0, 1, t);

            float scale = Mathf.Lerp(0.8f, 1.1f, t);
            instructionText.transform.localScale = Vector3.one * scale;

            yield return null;
        }

        instructionText.transform.localScale = Vector3.one;
    }
}