using UnityEngine;
using UnityEngine.UI;

public class CheckmarkManager : MonoBehaviour
{
    public static CheckmarkManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public void MarkComplete(DropTarget target)
{
    if (target.taskIcon != null)
    {
        target.taskIcon.checkIcon.SetActive(true);
    }
}
}