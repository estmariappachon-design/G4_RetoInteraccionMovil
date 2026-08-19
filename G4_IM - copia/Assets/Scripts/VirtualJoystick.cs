using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Referencias UI")]
    [SerializeField] private RectTransform containerBackground;
    [SerializeField] private RectTransform joystickHandle;

    [Header("Ajustes")]
    [SerializeField] private float handleLimit = 1f;

    private Vector2 inputVector = Vector2.zero;

    private void Start()
    {
        if (containerBackground == null)
            containerBackground = GetComponent<RectTransform>();

        if (joystickHandle == null && transform.childCount > 0)
            joystickHandle = transform.GetChild(0).GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            containerBackground,
            eventData.position,
            eventData.pressEventCamera,
            out position))
        {
            // Convertir la posición táctil a un valor normalizado entre -1 y 1
            Vector2 sizeDelta = containerBackground.sizeDelta;
            position.x = (position.x / sizeDelta.x) * 2;
            position.y = (position.y / sizeDelta.y) * 2;

            inputVector = new Vector2(position.x, position.y);
            inputVector = (inputVector.magnitude > 1f) ? inputVector.normalized : inputVector;

            // Mover visualmente la palanca pequeña
            if (joystickHandle != null)
            {
                joystickHandle.anchoredPosition = new Vector2(
                    inputVector.x * (containerBackground.sizeDelta.x / 2f) * handleLimit,
                    inputVector.y * (containerBackground.sizeDelta.y / 2f) * handleLimit
                );
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Al soltar el dedo, el joystick regresa al centro
        inputVector = Vector2.zero;
        if (joystickHandle != null)
        {
            joystickHandle.anchoredPosition = Vector2.zero;
        }
    }

    // Devuelve el vector de movimiento (X = Izquierda/Derecha, Y = Adelante/Atrás)
    public Vector2 GetInputVector()
    {
        return inputVector;
    }

    public float Horizontal => inputVector.x;
    public float Vertical => inputVector.y;
}