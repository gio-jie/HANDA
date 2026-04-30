using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    [Header("References")]
    public RectTransform player;
    public CheckpointQuestion questionData;

    [Header("Settings")]
    public float triggerRadius;
    private RectTransform checkpointRect;
    private bool triggered = false;

    void Start()
    {
        checkpointRect = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (triggered || player == null || checkpointRect == null)
            return;

        if (IsInRange(player, checkpointRect, triggerRadius))
        {
            triggered = true;
            QuestionManager.Instance.ShowQuestion(questionData);
        }
    }

    // =========================
    // DISTANCE-BASED CHECK
    // =========================
    bool IsInRange(RectTransform a, RectTransform b, float radius)
    {
        Vector2 posA = a.position;
        Vector2 posB = b.position;

        float distance = Vector2.Distance(posA, posB);

        return distance <= radius;
    }

    // =========================
    // OPTIONAL DEBUG (REMOVE LATER)
    // =========================
    void OnDrawGizmosSelected()
    {
        if (checkpointRect == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(checkpointRect.position, triggerRadius);
    }
}