using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using TMPro;

public class WaterSwipeLevelManager : MonoBehaviour
{
    public static WaterSwipeLevelManager Instance;

    public GameObject cardPrefab;
    public Transform cardParent;
    public TextMeshProUGUI progressText;

    private Queue<WaterCardData> cardQueue;
    private List<WaterResult> results = new List<WaterResult>();
    private WaterCardData currentData;
    private GameObject currentCard;

    private int totalItems;
    private int answeredCount = 0;

    public WaterCardDatabase database;

    void Awake() { Instance = this; }

    void Start()
    {
        List<WaterCardData> shuffled =
            database.allCards.OrderBy(x => Random.value).ToList();

        cardQueue = new Queue<WaterCardData>(shuffled);
        totalItems = shuffled.Count;

        UpdateProgress();
        ShowNextCard();
    }

    public void ShowNextCard()
    {
        if (cardQueue.Count == 0)
        {
            EndLevel();
            return;
        }

        currentData = cardQueue.Dequeue();
        currentCard = Instantiate(cardPrefab, cardParent);
        currentCard.GetComponent<WaterCardUI>().Setup(currentData);
    }

   public void SubmitAnswer(WaterCardData data, bool swipedRight)
    {
        bool isCorrect = (swipedRight && data.type == WaterType.Dirty) ||
                        (!swipedRight && data.type == WaterType.Clean);

        results.Add(new WaterResult(data, swipedRight, isCorrect));

        if (isCorrect)
        {
            StarManagerLevel7.Instance.RegisterCorrect();
            AudioManager.instance.PlaySFX(AudioManager.instance.correctSound);
        }
        else
        {
            StarManagerLevel7.Instance.RegisterWrong();
            AudioManager.instance.PlaySFX(AudioManager.instance.warningSound);

            StartCoroutine(currentCard.GetComponent<WaterCardUI>().FlashRed());
        }

        answeredCount++;
        UpdateProgress();

        StartCoroutine(CardSwitch(swipedRight ? Vector3.right : Vector3.left));
    }

    IEnumerator CardSwitch(Vector3 direction)
    {
        yield return currentCard.GetComponent<WaterCardUI>().AnimateToBack(direction);

        Destroy(currentCard);
        ShowNextCard();
    }

    void UpdateProgress() => progressText.text = answeredCount + "/" + totalItems;

    void EndLevel()
    {
        ResultsUILevel7.Instance.ShowResults(results);
        StarManagerLevel7.Instance.ShowEndPanel();
    }

    public void OnRerollButton()
    {
        cardQueue.Enqueue(currentData);

        StartCoroutine(CardSwitch(Vector3.down));
    }
}