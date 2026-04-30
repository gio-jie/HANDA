using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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

    private CheckpointQuestion currentQuestion;
    private bool isBusy = false;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    // =========================
    // SHOW QUESTION FLOW
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
        // 🔒 STEP 1: LOCK INPUT (NO IMMEDIATE HARD STOP)
        if (player != null)
            player.LockInput();

        // 🔥 STEP 2: SMOOTH STOP (feels natural)
        yield return StartCoroutine(SmoothStopPlayer());

        // 🔥 STEP 3: WAIT (reaction pause)
        yield return new WaitForSecondsRealtime(0.15f);

        // 🔥 STEP 4: RESET JOYSTICK (fix memory bug)
        if (joystick != null)
        {
            joystick.gameObject.SetActive(false);
            joystick.gameObject.SetActive(true);
        }

        // 🔥 STEP 5: SHOW PANEL
        panel.SetActive(true);

        questionText.text = currentQuestion.question;
        choiceAImage.sprite = currentQuestion.choiceAImage;
        choiceBImage.sprite = currentQuestion.choiceBImage;

        StartCoroutine(PopIn());
    }

    // =========================
    // SMOOTH STOP (IMPORTANT)
    // =========================
    IEnumerator SmoothStopPlayer()
    {
        float t = 0f;
        float duration = 0.25f;

        Vector2 startVel = player.GetComponent<Rigidbody2D>().linearVelocity;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;

            float lerp = t / duration;

            player.GetComponent<Rigidbody2D>().linearVelocity =
                Vector2.Lerp(startVel, Vector2.zero, EaseOutQuad(lerp));

            yield return null;
        }

        player.HardStop();
    }

    float EaseOutQuad(float t)
    {
        return t * (2f - t);
    }

    // =========================
    // ANSWERS
    // =========================
    public void ChooseA() => CheckAnswer(true);
    public void ChooseB() => CheckAnswer(false);

    void CheckAnswer(bool choseA)
    {
        bool isCorrect = (choseA == currentQuestion.isChoiceACorrect);

        if (isCorrect)
        {
            Debug.Log("Correct!");

            if (StarManagerEarthquake3.Instance != null)
            {
                StarManagerEarthquake3.Instance.RegisterCorrectItem();
            }
        }
        else
        {
            Debug.Log("Wrong!");

            if (StarManagerEarthquake3.Instance != null)
            {
                StarManagerEarthquake3.Instance.RegisterWrongItem();
            }
        }

        StartCoroutine(PopOut());
    }

    // =========================
    // POP IN (cartoon)
    // =========================
    IEnumerator PopIn()
    {
        contentRoot.localScale = Vector3.zero;

        float time = 0f;
        float duration = 0.25f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / duration;

            float scale = EaseOutBack(t);
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
            float t = time / duration;

            float scale = EaseInBack(t);
            contentRoot.localScale = Vector3.Lerp(start, Vector3.zero, scale);

            yield return null;
        }

        panel.SetActive(false);

        // 🔓 RESTORE CONTROL
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