using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RoomSwitcher : MonoBehaviour
{
    public RectTransform roomsContainer;
    public float slideDuration;

    public ToolBarAnimation toolBarAnim;

    public Button nextButton;
    public Button backButton;

    private int currentRoom = 0;
    private float screenWidth;

    private bool isTransitioning = false;

    void Start()
    {
        screenWidth = Screen.width;

        backButton.interactable = false;
    }

    public void GoToNextRoom()
    {
        if (isTransitioning) return;
        if (currentRoom >= 1) return;

        currentRoom++;
        StartCoroutine(NextRoomFlow());
    }

    IEnumerator NextRoomFlow()
    {
        isTransitioning = true;

        nextButton.interactable = false;
        backButton.interactable = false;

        yield return StartCoroutine(SlideToRoom(currentRoom));

        toolBarAnim.ShowToolBar();

        yield return new WaitForSeconds(0.2f);

        UpdateButtons();

        isTransitioning = false;
    }

    public void GoToPreviousRoom()
    {
        if (isTransitioning) return;
        if (currentRoom <= 0) return;

        currentRoom--;
        StartCoroutine(PreviousRoomFlow());
    }

    IEnumerator PreviousRoomFlow()
    {
        isTransitioning = true;

        nextButton.interactable = false;
        backButton.interactable = false;

        toolBarAnim.HideToolBar();

        yield return new WaitForSeconds(0.2f);

        yield return StartCoroutine(SlideToRoom(currentRoom));

        yield return new WaitForSeconds(0.1f);

        UpdateButtons();

        isTransitioning = false;
    }

    IEnumerator SlideToRoom(int roomIndex)
    {
        Vector2 startPos = roomsContainer.anchoredPosition;
        Vector2 targetPos = new Vector2(-screenWidth * roomIndex, 0);

        float time = 0;

        while (time < slideDuration)
        {
            time += Time.deltaTime;
            float t = time / slideDuration;

            t = Mathf.SmoothStep(0, 1, t);

            roomsContainer.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        roomsContainer.anchoredPosition = targetPos;
    }

    void UpdateButtons()
    {
        if (nextButton != null)
            nextButton.interactable = currentRoom < 1;

        if (backButton != null)
        {
            backButton.interactable = currentRoom > 0;
        }
    }
}