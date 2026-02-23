using UnityEngine;

[System.Serializable]
public class QuizQuestion
{
    public Sprite scenarioImage;

    public Sprite choiceA;
    public Sprite choiceB;

    public int correctAnswerIndex; // 0 = A, 1 = B
}