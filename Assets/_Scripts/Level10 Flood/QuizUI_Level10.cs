using UnityEngine;
using UnityEngine.UI;

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
        // (You can add feedback here if you want)

        quizPanel.SetActive(false);

        // Always collect hazard after answering
        currentHazard.CollectHazard();
    }
}