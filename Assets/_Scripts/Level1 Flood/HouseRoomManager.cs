using UnityEngine;
using System.Collections;

public enum HouseRoom
{
    Kitchen,
    Bedroom,
    Bathroom,
    LivingRoom,
    HomeOffice
}

public class HouseRoomManager : MonoBehaviour
{
    [Header("Rooms")]
    public GameObject kitchen;
    public GameObject bedroom;
    public GameObject bathroom;
    public GameObject livingRoom;
    public GameObject homeOffice;
    public GameObject houseBlueprint;

    public GameObject kitchenBG;
    public GameObject bathroomBG;
    public GameObject studyRoomBG;
    public GameObject livingRoomBG;

    [Header("Fade")]
    public CanvasGroup fadeCanvas;
    public float fadeDuration = 0.35f;

    bool isTransitioning = false;
    public GameObject houseBlueprintButton;
    public GameObject inventoryButton;

    void Start()
    {
        HideAllRooms();
        fadeCanvas.alpha = 0f;

        houseBlueprint.SetActive(true);
        houseBlueprintButton.SetActive(false);
        inventoryButton.SetActive(true);
    }

    public void GoToKitchen()
    {
        GoToRoom(HouseRoom.Kitchen);
    }

    public void GoToBedroom()
    {
        GoToRoom(HouseRoom.Bedroom);
    }

    public void GoToBathroom()
    {
        GoToRoom(HouseRoom.Bathroom);
    }

    public void GoToLivingRoom()
    {
        GoToRoom(HouseRoom.LivingRoom);
    }

    public void GoToHomeOffice()
    {
        GoToRoom(HouseRoom.HomeOffice);
    }

    void GoToRoom(HouseRoom room)
    {
        if (isTransitioning) return;
        StartCoroutine(RoomTransition(room));
    }

    IEnumerator RoomTransition(HouseRoom room)
    {
        isTransitioning = true;

        yield return StartCoroutine(Fade(1f));

        houseBlueprint.SetActive(false);
        houseBlueprintButton.SetActive(true);
        inventoryButton.SetActive(false);

        HideAllRooms();
        HideAllBackgrounds();

        switch (room)
        {
            case HouseRoom.Kitchen:
                kitchen.SetActive(true);
                kitchenBG.SetActive(true);
                break;

            case HouseRoom.Bedroom:
                bedroom.SetActive(true);
                break;

            case HouseRoom.Bathroom:
                bathroom.SetActive(true);
                bathroomBG.SetActive(true);
                break;

            case HouseRoom.LivingRoom:
                livingRoom.SetActive(true);
                livingRoomBG.SetActive(true);
                break;

            case HouseRoom.HomeOffice:
                studyRoomBG.SetActive(true);
                homeOffice.SetActive(true);
                break;
        }

        ObjectiveManager.Instance.ShowObjectives(room);

        yield return new WaitForSeconds(0.05f);

        yield return StartCoroutine(Fade(0f));

        isTransitioning = false;
    }

    public void ReturnToBlueprint()
    {
        HideAllRooms();
        HideAllBackgrounds();
        houseBlueprint.SetActive(true);
        houseBlueprintButton.SetActive(false);
        inventoryButton.SetActive(true);

        ObjectiveManager.Instance.HideObjectives();
    }

    void HideAllBackgrounds()
    {
        kitchenBG.SetActive(false);
        bathroomBG.SetActive(false);
        studyRoomBG.SetActive(false);
        livingRoomBG.SetActive(false);
    }

    void HideAllRooms()
    {
        kitchen.SetActive(false);
        bedroom.SetActive(false);
        bathroom.SetActive(false);
        livingRoom.SetActive(false);
        homeOffice.SetActive(false);
    }

    IEnumerator Fade(float target)
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
}