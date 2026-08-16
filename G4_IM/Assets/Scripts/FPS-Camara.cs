//using UnityEngine;
//using UnityEngine.InputSystem;

//public class MobileFPSCamera : MonoBehaviour
//{
//    [SerializeField] private Transform player;
//    [SerializeField] private InputActionReference lookAction;
//    [SerializeField] private float sensitivity = 0.15f;

//    private float verticalRotation = 0f;

//    private void OnEnable()
//    {
//        if (lookAction != null)
//        {
//            lookAction.action.Enable();
//        }
//    }

//    private void OnDisable()
//    {
//        if (lookAction != null)
//        {
//            lookAction.action.Disable();
//        }
//    }

//    private void Update()
//    {
//        if (lookAction == null || player == null)
//            return;

//        Vector2 lookInput = lookAction.action.ReadValue<Vector2>();

//        float mouseX = lookInput.x * sensitivity;
//        float mouseY = lookInput.y * sensitivity;

//        player.Rotate(Vector3.up * mouseX);

//        verticalRotation -= mouseY;
//        verticalRotation = Mathf.Clamp(
//            verticalRotation,
//            -80f,
//            80f
//        );

//        transform.localRotation =
//            Quaternion.Euler(
//                verticalRotation,
//                0f,
//                0f
//            );
//    }
//}

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class MobileFPSCamera : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private float sensitivity = 0.2f;

    private float verticalRotation = 0f;

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable(); // Habilita el rastreo nativo táctil de Unity Input System
        if (lookAction != null) lookAction.action.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
        if (lookAction != null) lookAction.action.Disable();
    }

    private void Update()
    {
        if (player == null) return;

        Vector2 lookInput = Vector2.zero;

        // Si hay toques en pantalla (móvil)
        if (Touch.activeTouches.Count > 0)
        {
            foreach (var touch in Touch.activeTouches)
            {
                // Solo lee arrastres que inicien en la mitad DERECHA de la pantalla
                if (touch.screenPosition.x > Screen.width * 0.5f)
                {
                    lookInput = touch.delta;
                    break;
                }
            }
        }
        // Fallback para PC (Lector clásico de Input Action)
        else if (lookAction != null)
        {
            lookInput = lookAction.action.ReadValue<Vector2>();
        }

        float mouseX = lookInput.x * sensitivity;
        float mouseY = lookInput.y * sensitivity;

        player.Rotate(Vector3.up * mouseX);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -80f, 80f);

        transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }
}