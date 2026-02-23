using UnityEngine;
using System.Collections;

public class RoomSwitcher_Level10 : MonoBehaviour
{
    [Header("Main Panels")]
    public GameObject blueprintPanel;

    [Header("All Rooms")]
    public GameObject[] rooms;

    [Header("Fade Settings")]
    public CanvasGroup fadeCanvas;
    public float fadeDuration = 0.25f;

    private void Awake()
    {
        if (fadeCanvas != null)
            fadeCanvas.alpha = 0f;
    }

    public void OpenRoom(GameObject roomToOpen)
    {
        if (fadeCanvas != null)
            StartCoroutine(FadeRoomChange(() =>
            {
                blueprintPanel.SetActive(false);

                foreach (GameObject room in rooms)
                    room.SetActive(false);

                roomToOpen.SetActive(true);

                TaskListManager_Level10.Instance.ShowTaskList();
            }));
        else
        {
            blueprintPanel.SetActive(false);
            foreach (GameObject room in rooms) room.SetActive(false);
            roomToOpen.SetActive(true);
            TaskListManager_Level10.Instance.ShowTaskList();
        }
    }

    private IEnumerator FadeRoomChange(System.Action action)
    {
        yield return StartCoroutine(Fade(1f));

        action?.Invoke();

        yield return StartCoroutine(Fade(0f));
    }

    private IEnumerator Fade(float target)
    {
        float start = fadeCanvas.alpha;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(start, target, t / fadeDuration);
            yield return null;
        }

        fadeCanvas.alpha = target;
    }

    public void BackToBlueprint()
    {
        foreach (GameObject room in rooms)
        {
            room.SetActive(false);
        }

        blueprintPanel.SetActive(true);

        TaskListManager_Level10.Instance.HideTaskList();
    }
}