using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance;

    [System.Serializable]
    public class RoomObjectives
    {
        public HouseRoom room;
        public GameObject objectivePanel;
    }

    [Header("Collection Settings")]
    public int targetItems = 15;
    private int currentItems = 0;

    public TMP_Text counterText;
    public GameObject winPanel;
    public Image radialFill;
    Vector3 originalScale;
    Coroutine fillRoutine;

    public List<RoomObjectives> rooms;

    Dictionary<string, ObjectiveIcon> icons = new Dictionary<string, ObjectiveIcon>();

    void Awake()
    {
        Instance = this;
        UpdateCounter();
        winPanel.SetActive(false);
        originalScale = counterText.transform.localScale;
    }

    public void ShowObjectives(HouseRoom room)
    {
        foreach (var r in rooms)
        {
            if (r.objectivePanel == null) continue;
            var animator = r.objectivePanel.GetComponent<PanelAnimator>();
            if (animator != null)
                animator.HidePanel();
            else
                r.objectivePanel.SetActive(false);
        }

        var target = rooms.Find(r => r.room == room);
        if (target != null && target.objectivePanel != null)
        {
            var animator = target.objectivePanel.GetComponent<PanelAnimator>();
            if (animator != null)
            {
                animator.bagIcon.gameObject.SetActive(true);
                animator.ShowPanel();
            }
            else
                target.objectivePanel.SetActive(true);
                }

        CacheIcons();
    }

    public void CollectItem(string id)
    {
        currentItems++;

        counterText.transform.localScale = originalScale * 1.15f;
        Invoke(nameof(ResetScale), 0.1f);

        UpdateCounter();

        MarkComplete(id);

        if(currentItems >= targetItems)
            //WinGame();
            fillRoutine = StartCoroutine(AnimateFill((float)currentItems / targetItems, true));
    }

    void ResetScale()
    {
        counterText.transform.localScale = originalScale;
    }

    void UpdateCounter()
    {
        counterText.text = currentItems + "/" + targetItems;
            
        fillRoutine = StartCoroutine(AnimateFill((float)currentItems / targetItems));
    }

    IEnumerator AnimateFill(float target, bool triggerWin = false)
    {
        float start = radialFill.fillAmount;
        float time = 0f;

        while(time < 0.2f)
        {
            time += Time.unscaledDeltaTime;
            radialFill.fillAmount = Mathf.Lerp(start, target, time / 0.2f);
            yield return null;
        }

        radialFill.fillAmount = target;

        if (triggerWin)
        {
            FindObjectOfType<StarManager>().EndLevel(true);
            WinGame();
        }
    }

    void WinGame()
    {
        winPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void HideObjectives()
    {
        foreach (var room in rooms)
        {
            if (room.objectivePanel == null) continue;

            var animator = room.objectivePanel.GetComponent<PanelAnimator>();
            if (animator != null)
                animator.HidePanel();
            else
                room.objectivePanel.SetActive(false);
        }
    }

    void CacheIcons()
    {
        icons.Clear();

        ObjectiveIcon[] found = FindObjectsOfType<ObjectiveIcon>(true);

        foreach(var icon in found)
        {
            if(!icons.ContainsKey(icon.itemID))
                icons.Add(icon.itemID, icon);
        }
    }

    public void MarkComplete(string id)
    {
        if(icons.TryGetValue(id, out var icon))
            icon.Complete();
    }
}