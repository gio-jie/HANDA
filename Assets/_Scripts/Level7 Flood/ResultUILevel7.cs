using UnityEngine;
using System.Collections.Generic;

public class ResultsUILevel7 : MonoBehaviour
{
    public static ResultsUILevel7 Instance;
    public Transform gridParent;
    public GameObject resultCardPrefab;
    public GameObject endPanel;

    void Awake() { Instance = this; }

    public void ShowResults(List<WaterResult> results)
    {
        endPanel.SetActive(true);
        
        foreach (Transform t in gridParent) Destroy(t.gameObject);

        foreach (var r in results)
        {
            GameObject obj = Instantiate(resultCardPrefab, gridParent);
            obj.GetComponent<WaterResultCardUI>().Setup(r);
        }
    }
}