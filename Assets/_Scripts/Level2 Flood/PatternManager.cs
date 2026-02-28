using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class PatternManager : MonoBehaviour
{
    public static PatternManager Instance;

    [Header("Switches")]
    public List<SwitchButton> allSwitches;
    public int correctSwitchCount;

    [Header("Pattern UI")]
    public Transform patternIconHolder;
    public GameObject patternIconPrefab;
    public Slider patternTimer;
    public TMP_Text patternLabel;

    [Header("Main Breaker")]
    public GameObject mainBreaker;
    public Slider breakerSlider;

    [Header("Settings")]
    public int totalPatterns = 5;
    public float patternTime = 10f;
    public float timePenalty = 2f;

    private List<SwitchButton> correctSwitches = new List<SwitchButton>();
    private List<SwitchButton> currentPattern = new List<SwitchButton>();
    private int currentPatternIndex = 0;
    private int currentInputIndex = 0;
    private float timer;
    private bool patternActive;

    private List<PatternIconUI> currentIcons = new List<PatternIconUI>();
    private Vector3 labelOriginalScale;


    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        mainBreaker.SetActive(false);
        breakerSlider.value = 1;

        foreach(var sw in allSwitches)
        {
            if(sw.isCorrectSwitch)
                correctSwitches.Add(sw);
        }

        labelOriginalScale = patternLabel.transform.localScale;
        StartPattern();
    }

    void Update()
    {
        if(patternActive)
        {
            timer -= Time.deltaTime;
            patternTimer.value = timer;

            if(timer <= 0)
            {
                WaterSystem.Instance.RaiseWater();
                FailPattern();
            }
        }
    }

    IEnumerator LabelPop()
    {
        float duration = 0.35f;
        float time = 0f;

        Vector3 startScale = labelOriginalScale;
        Vector3 overshootScale = labelOriginalScale * 1.25f;
        Vector3 undershootScale = labelOriginalScale * 0.9f;

        while (time < duration * 0.4f)
        {
            float t = time / (duration * 0.4f);
            patternLabel.transform.localScale = Vector3.Lerp(startScale, overshootScale, EaseOutBack(t));
            time += Time.deltaTime;
            yield return null;
        }

        time = 0f;

        while (time < duration * 0.3f)
        {
            float t = time / (duration * 0.3f);
            patternLabel.transform.localScale = Vector3.Lerp(overshootScale, undershootScale, t);
            time += Time.deltaTime;
            yield return null;
        }

        time = 0f;

        while (time < duration * 0.3f)
        {
            float t = time / (duration * 0.3f);
            patternLabel.transform.localScale = Vector3.Lerp(undershootScale, startScale, t);
            time += Time.deltaTime;
            yield return null;
        }

        patternLabel.transform.localScale = startScale;
    }

    float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;

        return 1 + c3 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2);
    }

    void StartPattern()
    {
        patternActive = true;
        timer = patternTime;
        patternTimer.maxValue = patternTime;

        currentPattern.Clear();
        currentInputIndex = 0;

        patternLabel.text = "Pattern " + (currentPatternIndex + 1) + " / " + totalPatterns;

        List<SwitchButton> temp = new List<SwitchButton>(correctSwitches);

        for(int i = 0; i < correctSwitchCount; i++)
        {
            int rand = Random.Range(0, temp.Count);
            currentPattern.Add(temp[rand]);
            temp.RemoveAt(rand);
        }

        StartCoroutine(ShowPatternIcons());
    }

    IEnumerator ShowPatternIcons()
    {
        List<Coroutine> fadeCoroutines = new List<Coroutine>();

        foreach (var icon in currentIcons)
        {
            fadeCoroutines.Add(StartCoroutine(icon.AnimateOut()));
        }

        yield return new WaitForSeconds(0.2f);

        foreach (var icon in currentIcons)
        {
            Destroy(icon.gameObject);
        }

        currentIcons.Clear();

        foreach (var sw in currentPattern)
        {
            GameObject iconObj = Instantiate(patternIconPrefab, patternIconHolder);
            PatternIconUI iconUI = iconObj.GetComponent<PatternIconUI>();

            iconUI.SetSprite(sw.GetComponent<Image>().sprite);
            iconUI.SetInvisibleInstant();

            currentIcons.Add(iconUI);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(
            patternIconHolder.GetComponent<RectTransform>());

        yield return null;

        foreach (var icon in currentIcons)
        {
            yield return icon.AnimateIn();
            icon.StartFlashing();
        }
    }

    public void OnSwitchClicked(SwitchButton clicked)
    {
        if(!patternActive) return;

        if(!clicked.isCorrectSwitch)
        {
            WrongClick(clicked);
            return;
        }

        if(clicked != currentPattern[currentInputIndex])
        {
            WrongClick(clicked);
            return;
        }

        if(clicked.isCorrectSwitch)
        {
            currentInputIndex++;
            StarManagerLevel2.Instance.RegisterCorrectItem();

            currentIcons[currentInputIndex - 1].MarkComplete();

            if(currentInputIndex >= currentPattern.Count)
            {
                AudioManager.instance.PlaySFX(AudioManager.instance.correctSound);
                PatternCompleted();
            }
        }        
    }

    void WrongClick(SwitchButton clicked)
    {
        AudioManager.instance.PlaySFX(AudioManager.instance.warningSound);
        clicked.PulseRed();
        StarManagerLevel2.Instance.RegisterWrongItem();

        currentInputIndex = 0;
        ResetIcons();
    }

    void PatternCompleted()
    {
        patternActive = false;
        currentPatternIndex++;
        StartCoroutine(LabelPop());

        if(currentPatternIndex >= totalPatterns)
        {
            mainBreaker.SetActive(true);
        }
        else
        {
            StartPattern();
        }
    }

    void FailPattern()
    {
        patternActive = false;
        currentInputIndex = 0;
        ResetIcons();
        StartPattern();
    }

    void ResetIcons()
    {
        foreach(var icon in currentIcons)
        {
            icon.ResetMark();
        }
    }
}