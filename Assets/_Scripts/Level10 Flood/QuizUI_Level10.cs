using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QuizUI_Level10 : MonoBehaviour
{
    public static QuizUI_Level10 Instance;

    public GameObject quizPanel;
    public Image scenarioImage;
    public Image choiceAImage;
    public Image choiceBImage;

    private QuizQuestion currentQuestion;
    private HazardItem currentHazard;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowQuestion(QuizQuestion question, HazardItem hazard)
    {
        quizPanel.SetActive(true);

        currentQuestion = question;
        currentHazard = hazard;

        scenarioImage.sprite = question.scenarioImage;
        choiceAImage.sprite = question.choiceA;
        choiceBImage.sprite = question.choiceB;
    }

    public void ChooseAnswer(int chosenIndex)
    {
        bool isCorrect = chosenIndex == currentQuestion.correctAnswerIndex;

        if (!isCorrect)
        {
            // Start flashing wrong button, then continue with collection
            StartCoroutine(FlashThenCollect(chosenIndex));
        }
        else
        {
            // Correct answer → collect immediately
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

        // After flash, finalize
        EndQuiz();
    }

    private void EndQuiz()
    {
        quizPanel.SetActive(false);
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

            // Fade to red
            while (t < half)
            {
                t += Time.deltaTime;
                img.color = Color.Lerp(original, red, t / half);
                yield return null;
            }

            t = 0f;
            // Fade back to original
            while (t < half)
            {
                t += Time.deltaTime;
                img.color = Color.Lerp(red, original, t / half);
                yield return null;
            }
        }

        // Ensure final color is correct
        img.color = original;
    }
}