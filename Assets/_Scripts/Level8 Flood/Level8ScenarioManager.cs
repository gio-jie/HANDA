using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;

[System.Serializable]
public class Level8ScenarioData
{
    public string scenarioName;
    public Sprite scenarioImage;
    public string correctItemID;
}

[System.Serializable]
public class Level8AnswerRecord
{
    public Sprite scenarioImage;
    public Sprite playerItemSprite;
    public Sprite correctItemSprite;
    public string correctItemName;
    public bool isCorrect;
}

public class Level8ScenarioManager : MonoBehaviour
{
    public static Level8ScenarioManager Instance;

    [Header("UI")]
    public Image scenarioDisplay;
    public Level8Database database;
    public TMP_Text progressText;
    public float switchDuration = 0.5f;

    [HideInInspector]
    public Level8ScenarioData currentScenario;

    private List<Level8ScenarioData> remainingScenarios;
    private CanvasGroup scenarioCanvasGroup;
    private int totalScenarios;
    private int answeredCount = 0;
    private bool isSwitching = false;

    void Awake() => Instance = this;
    void Start()
    {
        if (database == null || database.allScenarios.Count == 0)
        {
            Debug.LogError("Level8Database not assigned or empty!");
            return;
        }

        remainingScenarios = database.allScenarios.OrderBy(x => Random.value).ToList();
        totalScenarios = remainingScenarios.Count;

        scenarioCanvasGroup = scenarioDisplay.GetComponent<CanvasGroup>();
        if (scenarioCanvasGroup == null)
            scenarioCanvasGroup = scenarioDisplay.gameObject.AddComponent<CanvasGroup>();

        ShowFirstScenario();
    }

    private void ShowFirstScenario()
    {
        if (remainingScenarios.Count == 0) return;

        currentScenario = remainingScenarios[0];
        remainingScenarios.RemoveAt(0);

        scenarioDisplay.sprite = currentScenario.scenarioImage;
        scenarioCanvasGroup.alpha = 1f;
        scenarioDisplay.enabled = true;

        answeredCount = 0;
        UpdateProgressUI();
    }

    public void OnItemDropped(Level8DragItem item)
    {
        if (isSwitching) return;

        bool correct = item.itemID == currentScenario.correctItemID;

        Level8DragItem correctItem = FindObjectsOfType<Level8DragItem>()
            .FirstOrDefault(x => x.itemID == currentScenario.correctItemID);

        Level8AnswerRecord record = new Level8AnswerRecord
        {
            scenarioImage = currentScenario.scenarioImage,
            playerItemSprite = item.GetComponent<Image>()?.sprite,
            correctItemSprite = correctItem != null ? correctItem.GetComponent<Image>().sprite : null,
            correctItemName = correctItem != null ? correctItem.itemID : currentScenario.correctItemID,
            isCorrect = (item.itemID == currentScenario.correctItemID)
        };

        StarManagerLevel8.Instance.answerRecords.Add(record);

        if (correct)
        {
            bool hasNextCard = remainingScenarios.Count > 0;
            StarManagerLevel8.Instance.RegisterCorrect(hasNextCard);
            AudioManager.instance.PlaySFX(AudioManager.instance.correctSound);
        } 
        else
        {
            StarManagerLevel8.Instance.RegisterWrong();
            AudioManager.instance.PlaySFX(AudioManager.instance.warningSound);
            Image itemImage = item.GetComponent<Image>();

            if (itemImage != null)
                StartCoroutine(FlashRed(itemImage));
        }
            
        item.transform.SetParent(scenarioDisplay.transform, false);
        item.transform.localScale = Vector3.one * 0.5f;
        item.GetRect().anchoredPosition = Vector2.zero;
        item.enabled = false;

        PowerupManager.Instance.ClearHighlights();

        if (remainingScenarios.Count > 0 && correct)
        {
            float chance = Random.value;

            if (chance < 0.3f || 
                StarManagerLevel8.Instance.GetConsecutiveCorrect() % 3 == 0)
            {
                StarManagerLevel8.Instance.PauseTimer();
                PowerupManager.Instance.QueueRandomPowerup();
                StarManagerLevel8.Instance.ResumeTimer();
            }
        }

        answeredCount++;
        UpdateProgressUI();

        StartCoroutine(DelayNextScenario(0.5f));
    }

    private void UpdateProgressUI()
    {
        if (progressText != null)
            progressText.text = $"{answeredCount}/{totalScenarios}";
    }

    private IEnumerator DelayNextScenario(float delay)
    {
        yield return new WaitForSeconds(delay);
        yield return StartCoroutine(FadeScenarioToNext());
    }

    private IEnumerator FadeScenarioToNext()
    {
        isSwitching = true;

        float t = 0f;
        while (t < switchDuration)
        {
            t += Time.deltaTime;
            scenarioCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t / switchDuration);
            yield return null;
        }
        scenarioCanvasGroup.alpha = 0f;

        foreach (Transform child in scenarioDisplay.transform)
        {
            Destroy(child.gameObject);
        }

        if (remainingScenarios.Count == 0)
        {
            t = 0f;
            while (t < switchDuration)
            {
                t += Time.deltaTime;
                scenarioCanvasGroup.alpha = Mathf.Lerp(0f, 0f, t / switchDuration);
                yield return null;
            }
            scenarioCanvasGroup.alpha = 0f;

            StarManagerLevel8.Instance.ShowEndPanel();
            isSwitching = false;
            yield break;
        }

        currentScenario = remainingScenarios[0];
        remainingScenarios.RemoveAt(0);
        scenarioDisplay.sprite = currentScenario.scenarioImage;
        PowerupManager.Instance.ApplyPendingPowerup(currentScenario.correctItemID);

        t = 0f;
        while (t < switchDuration)
        {
            t += Time.deltaTime;
            scenarioCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t / switchDuration);
            yield return null;
        }
        scenarioCanvasGroup.alpha = 1f;

        isSwitching = false;
    }

    private IEnumerator FlashRed(Image img, int flashes = 2, float duration = 0.2f)
    {
        Color originalColor = img.color;

        for (int i = 0; i < flashes; i++)
        {
            img.color = Color.red;
            yield return new WaitForSeconds(duration);
            img.color = originalColor;
            yield return new WaitForSeconds(duration);
        }
    }

    public void RerollScenario()
    {
        if (isSwitching) return;
        if (remainingScenarios.Count == 0) return;

        // Put current scenario back into the pool
        remainingScenarios.Add(currentScenario);

        // Optional: reshuffle so it doesn't appear immediately again
        remainingScenarios = remainingScenarios.OrderBy(x => Random.value).ToList();

        StartCoroutine(FadeScenarioToNext());
    }
}