using UnityEngine;

public class HazardData : MonoBehaviour
{
    [Header("Question Data")]
    public string questionText; // Ang tanong (e.g., "May ahas!")
    public string option1Text;  // Choice A (e.g., "Batuhin")
    public string option2Text;  // Choice B (e.g., "Lumayo")
    
    [Header("Answer Key")]
    public int correctOption;   // 1 kung Option 1, 2 kung Option 2 ang tama

    private Level9Manager manager; // Tatawagan natin ang Manager

    void Start()
    {
        // Hanapin ang Manager sa scene
        manager = FindObjectOfType<Level9Manager>();
    }

    void OnMouseDown()
    {
        // Kapag clinick ang hazard, buksan ang Panel gamit ang data ko
        if(manager.isPanelActive == false) // Para bawal mag-click pag may tanong pa
        {
            manager.ShowQuestion(this);
        }
    }
}