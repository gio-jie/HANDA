using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    private Image bgImage;
    private Image handleImage;
    private Vector2 inputVector; 

    void Start()
    {
        bgImage = GetComponent<Image>();
        // Hanapin ang Handle na nasa loob nito
        handleImage = transform.GetChild(0).GetComponent<Image>(); 
    }

    public void OnDrag(PointerEventData ped)
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(bgImage.rectTransform, ped.position, ped.pressEventCamera, out pos))
        {
            pos.x = (pos.x / bgImage.rectTransform.sizeDelta.x);
            pos.y = (pos.y / bgImage.rectTransform.sizeDelta.y);

            inputVector = new Vector2(pos.x * 2, pos.y * 2);
            inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;

            // Galawin ang Handle visual
            handleImage.rectTransform.anchoredPosition = new Vector2(inputVector.x * (bgImage.rectTransform.sizeDelta.x / 3), inputVector.y * (bgImage.rectTransform.sizeDelta.y / 3));
        }
    }

    public void OnPointerDown(PointerEventData ped)
    {
        OnDrag(ped); 
    }

    public void OnPointerUp(PointerEventData ped)
    {
        inputVector = Vector2.zero;
        handleImage.rectTransform.anchoredPosition = Vector2.zero;
    }

    public float Horizontal()
    {
        if (inputVector.x != 0) return inputVector.x;
        else return Input.GetAxis("Horizontal"); // Para gumana rin ang Keyboard arrow keys
    }

    public float Vertical()
    {
        if (inputVector.y != 0) return inputVector.y;
        else return Input.GetAxis("Vertical");
    }
}