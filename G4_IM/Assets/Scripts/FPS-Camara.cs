using UnityEngine;
using UnityEngine.InputSystem;

public class MobileFPSCamera : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private float sensitivity = 0.15f;

    private float verticalRotation = 0f;

    private void OnEnable()
    {
        if (lookAction != null)
        {
            lookAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (lookAction != null)
        {
            lookAction.action.Disable();
        }
    }

    private void Update()
    {
        if (lookAction == null || player == null)
            return;

        Vector2 lookInput = lookAction.action.ReadValue<Vector2>();

        float mouseX = lookInput.x * sensitivity;
        float mouseY = lookInput.y * sensitivity;

        player.Rotate(Vector3.up * mouseX);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(
            verticalRotation,
            -80f,
            80f
        );

        transform.localRotation =
            Quaternion.Euler(
                verticalRotation,
                0f,
                0f
            );
    }
}