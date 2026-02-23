using UnityEngine;
using UnityEngine.UI;

public class EndPanelManager_Level10 : MonoBehaviour
{
    public static EndPanelManager_Level10 Instance;

    public Transform gridParent;
    public GameObject itemButtonPrefab;

    void Awake()
    {
        Instance = this;
    }

    public void AddCollectedItem(string name, Sprite sprite, string description)
    {
        GameObject obj = Instantiate(itemButtonPrefab, gridParent);

        obj.GetComponent<Image>().sprite = sprite;

        obj.GetComponent<Button>().onClick.AddListener(() =>
        {
            InfoPanel_Level10.Instance.ShowInfo(name, sprite, description);
        });
    }
}