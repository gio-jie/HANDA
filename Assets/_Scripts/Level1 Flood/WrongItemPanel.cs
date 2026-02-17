using UnityEngine;

public class WrongItemPanel : MonoBehaviour
{
    public static WrongItemPanel Instance;

    ObjectiveItem pendingItem;

    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void Show(ObjectiveItem item)
    {
        pendingItem = item;
        gameObject.SetActive(true);

        if (GameManager.Instance != null)
            GameManager.Instance.isPaused = true;
    }

    public void OnYes()
    {
        gameObject.SetActive(false);

        if (GameManager.Instance != null)
            GameManager.Instance.isPaused = false;

        pendingItem.ConfirmWrongCollection();
    }

    public void OnNo()
    {
        gameObject.SetActive(false);

        if (GameManager.Instance != null)
            GameManager.Instance.isPaused = false;
    }
}