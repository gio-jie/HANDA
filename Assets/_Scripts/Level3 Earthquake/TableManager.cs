using UnityEngine;

public class TableManager : MonoBehaviour
{
    public static TableManager Instance;

    public Table[] tables;

    private bool hoverEnabled = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        RefreshTables();
    }

    public void RefreshTables()
    {
        tables = FindObjectsOfType<Table>();
    }

    public void EnableHover(bool enable)
    {
        hoverEnabled = enable;

        if (!enable)
            ResetAllTables();
    }

    public void HighlightTable(Table hovered)
    {
        if (!hoverEnabled || tables == null) return;

        foreach (var t in tables)
        {
            if (t == null) continue;

            bool isHovered = (t == hovered);

            t.SetHover(isHovered);
        }
    }

    void ResetAllTables()
    {
        if (tables == null) return;

        foreach (var t in tables)
        {
            if (t == null) continue;

            t.SetHover(false);
        }
    }
}