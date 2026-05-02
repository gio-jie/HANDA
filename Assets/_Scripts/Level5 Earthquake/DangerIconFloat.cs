using UnityEngine;

public class DangerIconFloat : MonoBehaviour
{
    [Header("Float Settings")]
    public float floatHeight = 10f;
    public float floatSpeed = 2f;

    [Header("Linking")]
    public ScenarioData linkedScenario;
    public GameObject iconRoot;

    private Vector3 startPos;
    private bool isRemoved;

    private bool lastPanelState;

    void Start()
    {
        startPos = transform.localPosition;

        if (iconRoot == null)
            iconRoot = gameObject;

        lastPanelState = false;
    }

    void Update()
    {
        if (isRemoved) return;

        FloatAnimation();
        CheckCompletion();
    }

    // =========================================================
    // FLOAT
    // =========================================================
    void FloatAnimation()
    {
        float offset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.localPosition = startPos + new Vector3(0f, offset, 0f);
    }

    // =========================================================
    // RELIABLE COMPLETION CHECK
    // =========================================================
    void CheckCompletion()
    {
        ScenarioManager sm = ScenarioManager.Instance;
        if (sm == null) return;

        if (sm.panel == null) return;

        bool panelOpen = sm.panel.activeSelf;

        // detect "just closed"
        if (lastPanelState == true && panelOpen == false)
        {
            // panel just finished closing → scenario completed
            if (sm.currentScenario == linkedScenario)
            {
                RemoveIcon();
            }
        }

        lastPanelState = panelOpen;
    }

    // =========================================================
    // REMOVE ICON
    // =========================================================
    void RemoveIcon()
    {
        if (isRemoved) return;

        isRemoved = true;

        if (iconRoot != null)
            Destroy(iconRoot);
        else
            Destroy(gameObject);
    }
}