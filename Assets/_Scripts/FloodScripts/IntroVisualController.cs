using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class IntroVisualController : MonoBehaviour
{
    [Header("References")]
    public DialogueManager dialogueManager;

    [Header("NPC")]
    public Image npcImage;
    public Sprite neutralSprite;
    public Sprite dykSprite;
    public Sprite shockSprite;
    public Sprite happySprite;
    public Sprite neutralHandSprite;
    public Sprite amazedSprite;

    [Header("Images")]
    public RectTransform cagayanMap;
    public RectTransform cagayanText;
    public RectTransform typhoonImage;
    public RectTransform waterLevelMeter;
    public Image exclamationImage;

    public CanvasGroup mapCanvas;
    public CanvasGroup mapTextCanvas;
    public CanvasGroup typhoonCanvas;
    public CanvasGroup meterCanvas;
    public CanvasGroup exclamationCanvas;

    [Header("Flood Water")]
    public Image floodWaterImage;
    public float waterRiseDuration = 0.6f;

    [Header("Dialogue Background")]
    public Image dialogueBackground;

    [Header("Dialogue Background Fade")]
    public float bgFadeDuration = 0.4f;

    [Header("Map Pan Settings")]
    public Vector2 panStart = new Vector2(-50f, 145f);
    public Vector2 panEnd   = new Vector2(37f, 145f);
    public float panDuration = 6f;

    [Header("UI Groups")]
    public GameObject introUIRoot;
    public GameObject gameplayUIRoot;
    private int lastIndex = -1;
    private Coroutine panRoutine;

    private Sprite originalBgSprite;
    private Color originalBgColor;

    private string floodIntroKey = "FloodIntroSeen";
    bool meterShown = false;
    private Vector2 npcOriginalPos;
    private Vector2 npcOriginalSize;
    public TextMeshProUGUI buttonText;

    float GetWaterLevelForIndex(int index)
    {
        switch (index)
        {
            case 5: return 0.647f;
            case 6: return 0.721f;
            case 7: return 0.839f;
            case 8: return 1.00f;
            default: return floodWaterImage.fillAmount;
        }
    }

    void Start()
    {
        PlayerPrefs.DeleteKey("FloodIntroSeen");
        PlayerPrefs.DeleteKey("JobertTriviaSeen");
        PlayerPrefs.Save();
        
        if (dialogueBackground != null)
        {
            originalBgSprite = dialogueBackground.sprite;
            originalBgColor  = dialogueBackground.color;
        }

        int floodSeen = PlayerPrefs.GetInt(floodIntroKey, 0);

        if (floodSeen == 0)
        {
            EnterIntroMode();
            HideAll();
        }
        else
        {
            ExitIntroMode();
            if (dialogueManager != null)
                PlayerPrefs.SetInt("JobertTriviaSeen", 1);
        }

        if (npcImage != null)
        {
            RectTransform rt = npcImage.rectTransform;
            npcOriginalPos = rt.anchoredPosition;
            npcOriginalSize = rt.sizeDelta;
        }
    }

    void Update()
    {
        if (dialogueManager == null) return;

        int index = dialogueManager.CurrentIndex;
        if (index == lastIndex) return;

        lastIndex = index;
        HandleVisuals(index);
        UpdateNpcEmotion(index);
    }

    void UpdateNpcEmotion(int dialogueIndex)
    {
        if (npcImage == null) return;

        RectTransform rt = npcImage.rectTransform;

        switch (dialogueIndex)
        {
            case 0:
                npcImage.sprite = neutralSprite;
                rt.anchoredPosition = npcOriginalPos;
                rt.sizeDelta = npcOriginalSize;
                break;
            case 1:
            case 2:
                npcImage.sprite = dykSprite;
                rt.anchoredPosition = npcOriginalPos;
                rt.sizeDelta = npcOriginalSize;
                break;
            case 3:
            case 8:
                npcImage.sprite = shockSprite;
                rt.anchoredPosition = npcOriginalPos;
                rt.sizeDelta = npcOriginalSize;
                break;
            case 4:
            case 5:
            case 9:
                npcImage.sprite = happySprite;
                rt.anchoredPosition = npcOriginalPos;
                rt.sizeDelta = npcOriginalSize;
                break;
            case 6:
                npcImage.sprite = neutralSprite;
                rt.anchoredPosition = npcOriginalPos;
                rt.sizeDelta = npcOriginalSize;
                break;
            case 7:
                npcImage.sprite = neutralHandSprite;
                rt.anchoredPosition = npcOriginalPos;
                rt.sizeDelta = npcOriginalSize;
                break;
            case 10:
                npcImage.sprite = amazedSprite;
                rt.anchoredPosition = new Vector2(-347.3635f, 194.2306f);
                rt.sizeDelta = new Vector2(705.65f, 666.1307f);
                if(buttonText != null) buttonText.text = "LET'S GO!";
                break;
            default:
                npcImage.sprite = neutralSprite;
                rt.anchoredPosition = npcOriginalPos;
                rt.sizeDelta = npcOriginalSize;
                break;
        }
    }

    void HandleVisuals(int index)
    {
        if (index == 1)
        {
            ShowMap();
            DarkenDialogueBackground();
        }
        else if (index == 2)
        {
            ForceDarkDialogueBackground();
            StartMapRotationSequence();
        }
        else if (index == 3)
        {
            ForceMapFullyShown();
        }
        else if (index >= 4 && index <= 8)
        {
            if (index == 4)
            {
                HideMap();
            }

            if (!meterShown)
            {
                ShowMeter();
                meterShown = true;
            }

            if (index >= 5 && index <= 8)
            {
                AnimateWaterLevel(index);
            }

            if (index == 8)
            {
                ShowExclamation();
            }
        }
        else if (index == 9)
        {
            RestoreDialogueBackground();
            HideFlood();
        }
        else if (index == 11)
        {
            ExitIntroMode();
        }
    }

    void ShowMap()
    {
        StopAllCoroutines();
        HideAll();

        cagayanText.gameObject.SetActive(true);
        cagayanMap.gameObject.SetActive(true);
        mapCanvas.alpha = 0f;
        cagayanMap.anchoredPosition = panStart;

        StartCoroutine(FadeCanvas(mapCanvas, 1f, 0.6f));
        StartCoroutine(FadeCanvas(mapTextCanvas, 1f, 0.6f));
        panRoutine = StartCoroutine(PanMap());
    }

    void ForceMapFullyShown()
    {
        StopCoroutine(nameof(PanMap));
        StopCoroutine(nameof(FadeCanvas));

        cagayanMap.gameObject.SetActive(true);
        cagayanText.gameObject.SetActive(true);

        mapCanvas.alpha = 1f;
        mapTextCanvas.alpha = 1f;
        cagayanMap.anchoredPosition = panEnd;
    }

    void HideMap()
    {
        if (panRoutine != null)
            StopCoroutine(panRoutine);

        StopCoroutine(nameof(RotateMapThenShowTyphoon));

        cagayanMap.gameObject.SetActive(false);
        cagayanText.gameObject.SetActive(false);
        typhoonImage.gameObject.SetActive(false);
        exclamationImage.gameObject.SetActive(false);

        mapCanvas.alpha = 0f;
        mapTextCanvas.alpha = 0f;
        typhoonCanvas.alpha = 0f;
        exclamationCanvas.alpha = 0f;

        cagayanMap.localRotation = Quaternion.identity;
    }

    void StartMapRotationSequence()
    {
        StopAllCoroutines();

        cagayanMap.gameObject.SetActive(true);
        cagayanText.gameObject.SetActive(true);

        StartCoroutine(RotateMapThenShowTyphoon());
    }

    IEnumerator RotateMapThenShowTyphoon()
    {
        Quaternion rot1 = Quaternion.Euler(27.765f, 0f, 0f);
        Quaternion rot2 = Quaternion.Euler(27.765f, 36.667f, 0f);

        yield return StartCoroutine(RotateMap(rot1, 0.6f));
        yield return StartCoroutine(RotateMap(rot2, 0.6f));

        ShowTyphoon();
    }

    IEnumerator RotateMap(Quaternion target, float duration)
    {
        Quaternion start = cagayanMap.localRotation;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            cagayanMap.localRotation = Quaternion.Slerp(start, target, t / duration);
            yield return null;
        }

        cagayanMap.localRotation = target;
    }

    void ShowTyphoon()
    {
        typhoonImage.gameObject.SetActive(true);
        typhoonCanvas.alpha = 0f;

        StartCoroutine(BlinkTyphoon());
    }

    IEnumerator BlinkTyphoon()
    {
        for (int i = 0; i < 3; i++)
        {
            yield return StartCoroutine(FadeCanvas(typhoonCanvas, 1f, 0.2f));
            yield return new WaitForSeconds(0.15f);
            yield return StartCoroutine(FadeCanvas(typhoonCanvas, 0f, 0.2f));
            yield return new WaitForSeconds(0.15f);
        }

        typhoonCanvas.alpha = 1f;
    }

    void ShowMeter()
    {
        StopAllCoroutines();

        waterLevelMeter.gameObject.SetActive(true);
        meterCanvas.alpha = 0f;

        StartCoroutine(FadeCanvas(meterCanvas, 1f, 0.6f));
    }

    void AnimateWaterLevel(int dialogueIndex)
    {
        float targetFill = GetWaterLevelForIndex(dialogueIndex);

        StopCoroutine(nameof(RaiseWater));
        StartCoroutine(RaiseWater(targetFill));
    }

    IEnumerator RaiseWater(float target)
    {
        float start = floodWaterImage.fillAmount;
        float t = 0f;

        while (t < waterRiseDuration)
        {
            t += Time.deltaTime;
            floodWaterImage.fillAmount = Mathf.Lerp(start, target, t / waterRiseDuration);
            yield return null;
        }

        floodWaterImage.fillAmount = target;
    }

    void ShowExclamation()
    {
        exclamationImage.gameObject.SetActive(true);
        exclamationCanvas.alpha = 0f;

        StartCoroutine(BlinkExclamation());
    }

    IEnumerator BlinkExclamation()
    {
        for (int i = 0; i < 3; i++)
        {
            yield return StartCoroutine(FadeCanvas(exclamationCanvas, 1f, 0.2f));
            yield return new WaitForSeconds(0.15f);
            yield return StartCoroutine(FadeCanvas(exclamationCanvas, 0f, 0.2f));
            yield return new WaitForSeconds(0.15f);
        }

        exclamationCanvas.alpha = 1f;
    }

    void HideFlood()
    {
        StopCoroutine(nameof(RaiseWater));
        StartCoroutine(FadeOutFlood());
    }

    IEnumerator FadeOutFlood()
    {
        float duration = 0.5f;

        float startFill = floodWaterImage.fillAmount;
        float startMeterAlpha = meterCanvas.alpha;
        float startExclaimAlpha = exclamationCanvas.alpha;

        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = t / duration;

            floodWaterImage.fillAmount = Mathf.Lerp(startFill, 0f, lerp);

            meterCanvas.alpha = Mathf.Lerp(startMeterAlpha, 0f, lerp);
            exclamationCanvas.alpha = Mathf.Lerp(startExclaimAlpha, 0f, lerp);

            yield return null;
        }

        floodWaterImage.fillAmount = 0f;
        meterCanvas.alpha = 0f;
        exclamationCanvas.alpha = 0f;

        waterLevelMeter.gameObject.SetActive(false);
        exclamationImage.gameObject.SetActive(false);

        meterShown = false;
    }

    void HideAll()
    {
        if (panRoutine != null)
            StopCoroutine(panRoutine);

        meterShown = false;
        cagayanMap.localRotation = Quaternion.identity;
        
        cagayanMap.gameObject.SetActive(false);
        cagayanText.gameObject.SetActive(false);
        waterLevelMeter.gameObject.SetActive(false);
        typhoonImage.gameObject.SetActive(false);
        floodWaterImage.fillAmount = 0f;

        mapCanvas.alpha = 0f;
        mapTextCanvas.alpha = 0f;
        meterCanvas.alpha = 0f;
        typhoonCanvas.alpha = 0f;
    }

    void DarkenDialogueBackground()
    {
        if (dialogueBackground == null) return;

        StopCoroutine(nameof(FadeToDarkBg));
        StartCoroutine(FadeToDarkBg());
    }

    void ForceDarkDialogueBackground()
    {
        StopCoroutine(nameof(FadeToDarkBg));
        StopCoroutine(nameof(FadeToOriginalBg));

        dialogueBackground.sprite = null;
        dialogueBackground.color = new Color32(0x5C, 0x5C, 0x5C, 0xFF);
    }

    void RestoreDialogueBackground()
    {
        if (dialogueBackground == null) return;

        StopCoroutine(nameof(FadeToOriginalBg));
        StartCoroutine(FadeToOriginalBg());
    }

    IEnumerator FadeToDarkBg()
    {
        yield return StartCoroutine(FadeImageAlpha(dialogueBackground, 0f));

        dialogueBackground.sprite = null;
        dialogueBackground.color = new Color32(0x5C, 0x5C, 0x5C, 0xFF);

        yield return StartCoroutine(FadeImageAlpha(dialogueBackground, 1f));
    }

    IEnumerator FadeToOriginalBg()
    {
        yield return StartCoroutine(FadeImageAlpha(dialogueBackground, 0f));

        dialogueBackground.sprite = originalBgSprite;
        dialogueBackground.color  = originalBgColor;

        yield return StartCoroutine(FadeImageAlpha(dialogueBackground, 1f));
    }

    IEnumerator FadeCanvas(CanvasGroup canvas, float target, float duration)
    {
        float start = canvas.alpha;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            canvas.alpha = Mathf.Lerp(start, target, t / duration);
            yield return null;
        }

        canvas.alpha = target;
    }

    IEnumerator FadeImageAlpha(Image image, float target)
    {
        float start = image.color.a;
        float t = 0f;

        while (t < bgFadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(start, target, t / bgFadeDuration);
            image.color = new Color(
                image.color.r,
                image.color.g,
                image.color.b,
                a
            );
            yield return null;
        }

        image.color = new Color(
            image.color.r,
            image.color.g,
            image.color.b,
            target
        );
    }

    IEnumerator PanMap()
    {
        float t = 0f;

        while (t < panDuration)
        {
            t += Time.deltaTime;
            cagayanMap.anchoredPosition =
                Vector2.Lerp(panStart, panEnd, t / panDuration);
            yield return null;
        }
    }

    void EnterIntroMode()
    {
        if (introUIRoot != null)
            introUIRoot.SetActive(true);

        if (gameplayUIRoot != null)
            gameplayUIRoot.SetActive(false);
    }

    void ExitIntroMode()
    {
        if (introUIRoot != null)
            introUIRoot.SetActive(false);

        if (gameplayUIRoot != null)
            gameplayUIRoot.SetActive(true);
    }

    public void Skip()
    {
        ExitIntroMode();
    }
}