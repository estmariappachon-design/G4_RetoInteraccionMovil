using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionInput : MonoBehaviour
{
    [SerializeField] private InputActionReference move;
    [SerializeField] private InputActionReference look;
    [SerializeField] private InputActionReference grab;
    [SerializeField] private InputActionReference place;

    public Vector2 MoveValue { get; private set; }
    public Vector2 LookValue { get; private set; }

    private void OnEnable()
    {
        move.action.Enable();
        look.action.Enable();
        grab.action.Enable();
        place.action.Enable();

        grab.action.performed += OnGrab;
        place.action.performed += OnPlace;
    }

    private void OnDisable()
    {
        grab.action.performed -= OnGrab;
        place.action.performed -= OnPlace;
    }

    private void Update()
    {
        MoveValue = move.action.ReadValue<Vector2>();
        LookValue = look.action.ReadValue<Vector2>();
    }

    private void OnGrab(InputAction.CallbackContext ctx) => SendMessage("TryGrab", SendMessageOptions.DontRequireReceiver);
    private void OnPlace(InputAction.CallbackContext ctx) => SendMessage("TryPlace", SendMessageOptions.DontRequireReceiver);
}