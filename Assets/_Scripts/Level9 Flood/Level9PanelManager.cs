using UnityEngine;

public class Level9PanelManager : MonoBehaviour
{
    public static Level9PanelManager Instance;

    [Header("Panels")]
    public GameObject wastePanel;
    public GameObject sanitizerPanel;
    public GameObject waterPanel;
    public GameObject dataPanel;
    public GameObject cookPanel;

    private GameObject currentOpenPanel;

    void Awake()
    {
        Instance = this;
    }

    public void OpenPanel(GameObject panelToOpen)
    {
        if (currentOpenPanel != null)
        {
            CloseCurrentPanel();
        }

        panelToOpen.SetActive(true);
        currentOpenPanel = panelToOpen;
    }

    public void CloseCurrentPanel()
    {
        if (currentOpenPanel == null) return;

        TryResetTask(currentOpenPanel);

        currentOpenPanel.SetActive(false);
        currentOpenPanel = null;
    }

    void TryResetTask(GameObject panel)
    {
        WasteTask waste = panel.GetComponent<WasteTask>();
        if (waste != null && !waste.IsCompleted())
            waste.ResetTask();

        SanitizerTask sanitizer = panel.GetComponent<SanitizerTask>();
        if (sanitizer != null && !sanitizer.IsCompleted())
            sanitizer.ResetTask();

        WaterTask water = panel.GetComponent<WaterTask>();
        if (water != null && !water.IsCompleted())
            water.ResetTask();

        DataTask data = panel.GetComponent<DataTask>();
        if (data != null)
            data.ResetIfNotFinished();
    }
}