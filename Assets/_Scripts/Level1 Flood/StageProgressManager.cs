using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class StageProgressManager : MonoBehaviour
{
    public static StageProgressManager Instance;

    [Header("UI")]
    public Slider progressSlider;
    public TMP_Text progressText;

    [Header("Stage Settings")]
    public int totalLevels = 10;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateProgress();
    }

    public void UpdateProgress()
    {
        int completedLevels = 0;

        for (int i = 1; i <= totalLevels; i++)
        {
            int stars = PlayerPrefs.GetInt("Level_" + i, 0);
            if (stars > 0) completedLevels++;
        }

        float progress = (float)completedLevels / totalLevels;

        StartCoroutine(AnimateSlider(progress));

        if (progressText != null)
            progressText.text = $"Progress: {Mathf.RoundToInt(progress * 100f)}%";
    }

    private IEnumerator AnimateSlider(float targetValue, float duration = 0.5f)
    {
        float startValue = progressSlider.value;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            progressSlider.value = Mathf.Lerp(startValue, targetValue, t / duration);
            yield return null;
        }
        progressSlider.value = targetValue;
    }
}