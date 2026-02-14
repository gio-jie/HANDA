using UnityEngine;

public class ClickBattery : MonoBehaviour
{
    public RadioComplex radioScript; // Tatawagan natin ang main script

    void OnMouseDown()
    {
        // Sabihan ang Radio na nakuha na ang battery
        radioScript.PickUpBattery();
    }
}