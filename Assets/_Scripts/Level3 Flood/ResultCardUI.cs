using UnityEngine;
using UnityEngine.UI;

public class ResultCardUI : MonoBehaviour
{
    public Image background;
    public GameObject checkIcon;
    public GameObject crossIcon;
    public Image icon;
    public Button button;

    public void Setup(SortingResult result)
    {
        bool correct = result.playerChoseSave == result.itemData.shouldSave;

        background.color = result.playerChoseSave
            ? new Color(0.7f, 1f, 0.7f)
            : new Color(1f, 0.7f, 0.7f);

        checkIcon.SetActive(correct);
        crossIcon.SetActive(!correct);
    }
}