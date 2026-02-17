using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using TMPro;

public class SortingLevelManager : MonoBehaviour
{
    public static SortingLevelManager Instance;

    public GameObject cardPrefab;
    public Transform cardParent;

    public float totalTime = 120f;

    private float timer;
    private Queue<SortingItemData> itemQueue;
    private List<SortingResult> results = new List<SortingResult>();

    private GameObject currentCard;
    private SortingItemData currentData;

    public TMPro.TextMeshProUGUI progressText;
    private int totalItems;
    private int answeredCount = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        timer = totalTime;

        List<SortingItemData> shuffled =
            SortingItemDatabase.Instance.allItems
            .OrderBy(x => Random.value).ToList();

        itemQueue = new Queue<SortingItemData>(shuffled);

        totalItems = SortingItemDatabase.Instance.allItems.Count;
        UpdateProgress();

        ShowNextCard();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            EndLevel();
        }
    }

    public void ShowNextCard()
    {
        if (itemQueue.Count == 0)
        {
            EndLevel();
            return;
        }

        //SortingItemData data = itemQueue.Dequeue();
        currentData = itemQueue.Dequeue();

        currentCard = Instantiate(cardPrefab, cardParent);
        currentCard.GetComponent<ItemCardUI>().Setup(currentData);
    }

    public void SubmitAnswer(SortingItemData data, bool droppedInSave)
    {
        results.Add(new SortingResult
        {
            itemData = data,
            playerChoseSave = droppedInSave
        });

        answeredCount++;
        UpdateProgress();

        StartCoroutine(CardSwitch());
    }

    void UpdateProgress()
    {
        progressText.text = answeredCount + "/" + totalItems;
        StopAllCoroutines();
        StartCoroutine(PopProgress());
    }

    IEnumerator PopProgress()
    {
        Vector3 originalScale = progressText.transform.localScale;
        Vector3 biggerScale = originalScale * 1.25f;

        float duration = 0.12f;
        float t = 0f;

        // Scale up
        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;
            progressText.transform.localScale =
                Vector3.Lerp(originalScale, biggerScale, progress);
            yield return null;
        }

        t = 0f;

        // Scale back down
        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;
            progressText.transform.localScale =
                Vector3.Lerp(biggerScale, originalScale, progress);
            yield return null;
        }

        progressText.transform.localScale = originalScale;
    }

    IEnumerator CardSwitch()
    {
        yield return currentCard.GetComponent<ItemCardUI>().AnimateToBack();
        Destroy(currentCard);
        ShowNextCard();
    }

    public void SkipCurrentCard(SortingItemData data)
    {
        itemQueue.Enqueue(data);
        StartCoroutine(CardSwitch());
    }

    void EndLevel()
    {
        ResultsUI.Instance.ShowResults(results);
        StarManagerLevel3.Instance.ShowEndPanel();
    }

    public void OnSkip()
    {
        SkipCurrentCard(currentData);
    }
}