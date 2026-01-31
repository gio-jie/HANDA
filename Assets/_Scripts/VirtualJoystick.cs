using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [Header("Visuals")]
    public RectTransform joystickBackground; // Yung malaking bilog
    public RectTransform joystickHandle;     // Yung maliit na bilog
    
    private Vector2 inputVector; 
    private Vector2 defaultPos; // Original position

    void Start()
    {
        // Save original position
        defaultPos = joystickBackground.anchoredPosition;
    }

    public void OnPointerDown(PointerEventData ped)
    {
        // 1. Hanapin ang World Position ng tap mo
        Vector3 worldPoint;
        
        // Gamitin ang parent ng background (JoystickArea) bilang reference frame
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
            joystickBackground.parent as RectTransform, 
            ped.position, 
            ped.pressEventCamera, 
            out worldPoint))
        {
            // 2. Direktang ilipat ang joystick sa world point na nakuha (Snap to finger)
            // Note: .position ang gamit natin, HINDI .anchoredPosition
            joystickBackground.position = worldPoint;
        }

        // 3. Reset Handle sa gitna
        joystickHandle.anchoredPosition = Vector2.zero;
        inputVector = Vector2.zero;
    }

    public void OnDrag(PointerEventData ped)
    {
        Vector2 pos;
        // Calculation kung saan hihilahin ang handle relative sa background
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBackground, ped.position, ped.pressEventCamera, out pos))
        {
            pos.x = (pos.x / joystickBackground.sizeDelta.x);
            pos.y = (pos.y / joystickBackground.sizeDelta.y);

            inputVector = new Vector2(pos.x * 2, pos.y * 2);
            inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;

            // Galawin ang Handle visual
            joystickHandle.anchoredPosition = new Vector2(
                inputVector.x * (joystickBackground.sizeDelta.x / 3), 
                inputVector.y * (joystickBackground.sizeDelta.y / 3)
            );
        }
    }

    public void OnPointerUp(PointerEventData ped)
    {
        // Pag binitawan, ibalik sa gilid (default position)
        joystickBackground.anchoredPosition = defaultPos;
        
        // Reset sa gitna
        inputVector = Vector2.zero;
        joystickHandle.anchoredPosition = Vector2.zero;
    }

    public float Horizontal()
    {
        if (inputVector.x != 0) return inputVector.x;
        else return Input.GetAxis("Horizontal");
    }

    public float Vertical()
    {
        if (inputVector.y != 0) return inputVector.y;
        else return Input.GetAxis("Vertical");
    }
}