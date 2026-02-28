using UnityEngine;
using TMPro;

public class ItemInfoPanel_Level_6 : MonoBehaviour
{
    public static ItemInfoPanel_Level_6 instance;

    public GameObject panelHolder; // Yung mismong UI Panel object
    public TMP_Text nameText;
    public TMP_Text weightText;
    public TMP_Text expiryText;

    void Awake()
    {
        instance = this;
        panelHolder.SetActive(false); // Nakatago sa simula
    }

    public void ShowPanel(string n, string w, string e)
    {
        nameText.text = "Item: " + n;
        weightText.text = "Weight: " + w;
        
        // Pwede nating kulayan ng pula pag expired!
        expiryText.text = "Expiry: " + e;

        panelHolder.SetActive(true); // Palitawin!
    }

    public void HidePanel()
    {
        panelHolder.SetActive(false); // Itago pag binitawan!
    }
}