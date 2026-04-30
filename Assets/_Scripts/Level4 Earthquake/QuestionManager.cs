using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class QuestionManager : MonoBehaviour
{
    public static QuestionManager Instance;

    [Header("Panel")]
    public GameObject panel;

    [Header("Content")]
    public RectTransform contentRoot;

    [Header("UI")]
    public TMP_Text questionText;
    public Image choiceAImage;
    public Image choiceBImage;

    [Header("References")]
    public MazePlayerEarthquake player;
    public VirtualJoystick joystick;

    [Header("Progress")]
    public int totalQuestions = 5;
    private int answeredCount = 0;

    [Header("Results")]
    public Transform resultContainer;
    public GameObject resultItemPrefab;

    private List<PlayerAnswerData> playerAnswers = new List<PlayerAnswerData>();

    private CheckpointQuestion currentQuestion;
    private bool isBusy = false;
    private Image selectedImage;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    // =========================
    // SHOW QUESTION
    // =========================
    public void ShowQuestion(CheckpointQuestion data)
    {
        if (isBusy) return;

        isBusy = true;
        currentQuestion = data;

        StartCoroutine(QuestionFlow());
    }

    IEnumerator QuestionFlow()
    {
        if (player != null)
            player.LockInput();

        yield return StartCoroutine(SmoothStopPlayer());
        yield return new WaitForSecondsRealtime(0.15f);

        // reset joystick (fix stuck input)
        if (joystick != null)
        {
            joystick.gameObject.SetActive(false);
            joystick.gameObject.SetActive(true);
        }

        panel.SetActive(true);

        questionText.text = currentQuestion.question;
        choiceAImage.sprite = currentQuestion.choiceAImage;
        choiceBImage.sprite = currentQuestion.choiceBImage;

        ResetImageColors();

        yield return StartCoroutine(PopIn());
    }

    // =========================
    // ANSWERS
    // =========================
    public void ChooseA()
    {
        if (!isBusy) return;

        selectedImage = choiceAImage;
        StartCoroutine(CheckAnswerFlow(true));
    }

    public void ChooseB()
    {
        if (!isBusy) return;

        selectedImage = choiceBImage;
        StartCoroutine(CheckAnswerFlow(false));
    }

    // =========================
    // ANSWER FLOW
    // =========================
    IEnumerator CheckAnswerFlow(bool choseA)
    {
        bool isCorrect = (choseA == currentQuestion.isChoiceACorrect);

        // ✅ SAVE PLAYER ANSWER
        Sprite chosenSprite = choseA ? currentQuestion.choiceAImage : currentQuestion.choiceBImage;

        playerAnswers.Add(new PlayerAnswerData
        {
            chosenImage = chosenSprite,
            isCorrect = isCorrect,
            description = currentQuestion.description
        });

        // ⭐ gameplay feedback
        if (isCorrect)
        {
            if (StarManagerEarthquake4.Instance != null)
                StarManagerEarthquake4.Instance.RegisterCorrectItem();
        }
        else
        {
            if (StarManagerEarthquake4.Instance != null)
                StarManagerEarthquake4.Instance.RegisterWrongItem();

            if (selectedImage != null)
                yield return StartCoroutine(FlashWrongChoice(selectedImage));
        }

        answeredCount++;

        // close panel
        yield return StartCoroutine(PopOut());

        // ✅ IF FINISHED → SHOW RESULTS
        if (answeredCount >= totalQuestions)
        {
            GenerateResultsUI();

            if (StarManagerEarthquake4.Instance != null)
                StarManagerEarthquake4.Instance.ShowEndPanel();
        }
    }

    // =========================
    // RESULT GENERATION
    // =========================
    void GenerateResultsUI()
    {
        // clear old
        foreach (Transform child in resultContainer)
            Destroy(child.gameObject);

        foreach (var answer in playerAnswers)
        {
            GameObject item = Instantiate(resultItemPrefab, resultContainer);

            EarthquakeResult ui = item.GetComponent<EarthquakeResult>();
            if (ui != null)
                ui.Setup(answer);
        }
    }

    // =========================
    // FLASH WRONG
    // =========================
    IEnumerator FlashWrongChoice(Image img)
    {
        Color original = img.color;
        Color red = new Color(1f, 0.3f, 0.3f, 1f);

        float t = 0f;
        float duration = 0.15f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            img.color = Color.Lerp(original, red, t / duration);
            yield return null;
        }

        t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            img.color = Color.Lerp(red, original, t / duration);
            yield return null;
        }

        img.color = original;
    }

    void ResetImageColors()
    {
        choiceAImage.color = Color.white;
        choiceBImage.color = Color.white;
    }

    // =========================
    // SMOOTH STOP
    // =========================
    IEnumerator SmoothStopPlayer()
    {
        float t = 0f;
        float duration = 0.25f;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        Vector2 startVel = rb.linearVelocity;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            rb.linearVelocity = Vector2.Lerp(startVel, Vector2.zero, EaseOutQuad(t / duration));
            yield return null;
        }

        player.HardStop();
    }

    float EaseOutQuad(float t)
    {
        return t * (2f - t);
    }

    // =========================
    // POP IN
    // =========================
    IEnumerator PopIn()
    {
        contentRoot.localScale = Vector3.zero;

        float time = 0f;
        float duration = 0.25f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float scale = EaseOutBack(time / duration);
            contentRoot.localScale = Vector3.one * scale;
            yield return null;
        }

        contentRoot.localScale = Vector3.one;
    }

    // =========================
    // POP OUT
    // =========================
    IEnumerator PopOut()
    {
        float time = 0f;
        float duration = 0.2f;

        Vector3 start = contentRoot.localScale;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float scale = EaseInBack(time / duration);
            contentRoot.localScale = Vector3.Lerp(start, Vector3.zero, scale);
            yield return null;
        }

        panel.SetActive(false);

        if (player != null)
            player.UnlockInput();

        isBusy = false;
    }

    // =========================
    // EASING
    // =========================
    float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c2 = c1 * 1.525f;
        return 1 + (c2 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2));
    }

    float EaseInBack(float t)
    {
        float c1 = 1.70158f;
        float c2 = c1 * 1.525f;
        return c2 * t * t * t - c1 * t * t;
    }
}