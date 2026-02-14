using UnityEngine;
using UnityEngine.EventSystems; // Need ito para sa Hold logic

public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public RadioComplex radioScript;
    public float direction = 1f; // 1 for Left, -1 for Right
    private bool isPressed = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }

    void Update()
    {
        if (isPressed)
        {
            radioScript.RotateTuner(direction);
        }
    }
}