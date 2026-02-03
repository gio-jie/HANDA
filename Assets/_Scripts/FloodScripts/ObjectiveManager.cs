using UnityEngine;
using System.Collections.Generic;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance;

    [System.Serializable]
    public class RoomObjectives
    {
        public HouseRoom room;
        public GameObject objectivePanel;
    }

    public List<RoomObjectives> rooms;

    Dictionary<string, ObjectiveIcon> icons = new Dictionary<string, ObjectiveIcon>();

    void Awake()
    {
        Instance = this;
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