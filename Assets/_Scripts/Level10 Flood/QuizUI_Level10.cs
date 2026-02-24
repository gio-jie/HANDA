using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QuizUI_Level10 : MonoBehaviour
{
    public static QuizUI_Level10 Instance;

    [Header("Quiz UI")]
    public GameObject quizPanel;
    public Image scenarioImage;
    public Image choiceAImage;
    public Image choiceBImage;

    [Header("Checkmarks")]
    [SerializeField] private GameObject choiceACheckmark;
    [SerializeField] private GameObject choiceBCheckmark;

    [Header("Hard Hat Message")]
    public GameObject hardHatMessagePanel;
    public TMPro.TMP_Text hardHatMessageText;

    private QuizQuestion currentQuestion;
    private HazardItem currentHazard;

    private void Awake()
    {
        Instance = this;
        hardHatMessagePanel.SetActive(false);
    }

    public void ShowQuestion(QuizQuestion question, HazardItem hazard)
    {
        if (PowerUpManager_Level10.Instance.SkipNextHazard())
        {
            StartCoroutine(ShowHardHatMessageThenCollect(hazard));
            return;
        }

        quizPanel.SetActive(true);
        currentQuestion = question;
        currentHazard = hazard;

        scenarioImage.sprite = question.scenarioImage;
        choiceAImage.sprite = question.choiceA;
        choiceBImage.sprite = question.choiceB;

        ResetCheckmarksAndScale();

        if (PowerUpManager_Level10.Instance.UseJournal())
        {
            ShowCheckmarkAndScaleCorrectChoice();
        }
    }

    private IEnumerator ShowHardHatMessageThenCollect(HazardItem hazard)
    {
        if (hardHatMessagePanel != null && hardHatMessageText != null)
        {
            Time.timeScale = 0f;

            hardHatMessageText.text = "Hazard Question Skipped!";
            hardHatMessagePanel.SetActive(true);

            yield return new WaitForSecondsRealtime(2f);

            hardHatMessagePanel.SetActive(false);

            Time.timeScale = 1f;
        }

        hazard.CollectHazard();
    }

    private IEnumerator ScalePanel(Transform panel, float targetScale, float duration)
    {
        Vector3 original = panel.localScale;
        Vector3 target = original * targetScale;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            panel.localScale = Vector3.Lerp(original, target, t / duration);
            yield return null;
        }

        panel.localScale = original;
    }

    private void ShowCheckmarkAndScaleCorrectChoice()
    {
        choiceACheckmark.SetActive(false);
        choiceBCheckmark.SetActive(false);

        if (currentQuestion.correctAnswerIndex == 0)
        {
            choiceACheckmark.SetActive(true);
            StartCoroutine(ScalePanel(choiceAImage.transform, 1.2f, 0.3f));
        }
        else
        {
            choiceBCheckmark.SetActive(true);
            StartCoroutine(ScalePanel(choiceBImage.transform, 1.2f, 0.3f));
        }
    }

    private void ResetCheckmarksAndScale()
    {
        choiceACheckmark.SetActive(false);
        choiceBCheckmark.SetActive(false);
        choiceAImage.transform.localScale = Vector3.one;
        choiceBImage.transform.localScale = Vector3.one;
    }

    public void ChooseAnswer(int chosenIndex)
    {
        bool isCorrect = chosenIndex == currentQuestion.correctAnswerIndex;

        if (!isCorrect)
        {
            StarManagerLevel10.Instance.RegisterWrongItem();

            if (AudioManager.instance != null)
                AudioManager.instance.PlaySFX(AudioManager.instance.warningSound);
                
            StartCoroutine(FlashThenCollect(chosenIndex));
        }
        else
        {
            StarManagerLevel10.Instance.RegisterCorrectItem();
            EndQuiz();
        }
    }

    private IEnumerator FlashThenCollect(int chosenIndex)
    {
        Image buttonImg = (chosenIndex == 0) ? choiceAImage : choiceBImage;
        if (buttonImg != null)
        {
            yield return StartCoroutine(FlashButtonRed(buttonImg));
        }

        EndQuiz();
    }

    private void EndQuiz()
    {
        quizPanel.SetActive(false);
        ResetCheckmarksAndScale();
        currentHazard.CollectHazard();
    }

    private IEnumerator FlashButtonRed(Image img, float singleFlashDuration = 0.25f, int flashCount = 2)
    {
        if (img == null) yield break;

        Color original = img.color;
        Color red = Color.red;

        for (int i = 0; i < flashCount; i++)
        {
            float half = singleFlashDuration / 2f;
            float t = 0f;

            while (t < half)
            {
                t += Time.deltaTime;
                img.color = Color.Lerp(original, red, t / half);
                yield return null;
            }

            t = 0f;
            while (t < half)
            {
                t += Time.deltaTime;
                img.color = Color.Lerp(red, original, t / half);
                yield return null;
            }
        }

        img.color = original;
    }
}