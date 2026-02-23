using UnityEngine;
using System.Collections.Generic;

public class QuizManager_Level10 : MonoBehaviour
{
    public static QuizManager_Level10 Instance;

    public List<QuizQuestion> allQuestions = new List<QuizQuestion>();
    private List<QuizQuestion> remainingQuestions = new List<QuizQuestion>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        remainingQuestions = new List<QuizQuestion>(allQuestions);
    }

    public void TryShowRandomQuestion(HazardItem hazard)
    {
        if (remainingQuestions.Count == 0)
        {
            // No more questions → just collect hazard directly
            hazard.CollectHazard();
            return;
        }

        int randomIndex = Random.Range(0, remainingQuestions.Count);
        QuizQuestion selected = remainingQuestions[randomIndex];

        // REMOVE immediately so no repeat
        remainingQuestions.RemoveAt(randomIndex);

        QuizUI_Level10.Instance.ShowQuestion(selected, hazard);
    }
}