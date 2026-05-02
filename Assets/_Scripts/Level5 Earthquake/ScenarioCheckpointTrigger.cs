using UnityEngine;

public class ScenarioCheckpointTrigger : MonoBehaviour
{
    [Header("References")]
    public RectTransform player;

    [Header("Scenario Data")]
    public ScenarioData scenarioData;

    [Header("Settings")]
    public float triggerRadius;

    private RectTransform checkpointRect;
    private bool isInside = false;

    void Start()
    {
        checkpointRect = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (player == null || checkpointRect == null)
            return;

        if (!isInside && IsInRange(player, checkpointRect, triggerRadius))
        {
            isInside = true;
            ScenarioManager.Instance.ShowScenario(scenarioData);
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