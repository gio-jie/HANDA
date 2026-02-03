using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ObjectiveItem : MonoBehaviour
{
    public string itemID;
    bool collected = false;

    void OnMouseDown()
    {
        if (GameManager.Instance != null && GameManager.Instance.isPaused)
            return;

        Collect();
    }

    public void Collect()
    {
        if (collected) return;

        collected = true;

        ObjectiveManager.Instance.MarkComplete(itemID);

        Destroy(gameObject);
    }
}