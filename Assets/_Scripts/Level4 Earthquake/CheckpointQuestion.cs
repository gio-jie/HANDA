using UnityEngine;

[System.Serializable]
public class CheckpointQuestion
{
    public string question;
    public string description;

    public Sprite choiceAImage;
    public Sprite choiceBImage;

    public bool isChoiceACorrect;
}