[System.Serializable]
public class WaterResult
{
    public WaterCardData cardData;
    public bool playerSwipedRight;
    public bool isCorrect;

    public WaterResult(WaterCardData data, bool swipedRight, bool correct)
    {
        cardData = data;
        playerSwipedRight = swipedRight;
        isCorrect = correct;
    }
}