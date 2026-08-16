using UnityEngine;
using UnityEngine.InputSystem;

public class CubeGrabber : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform holdPoint;

    [Header("Input System")]
    [SerializeField] private InputActionReference grabAction;
    [SerializeField] private InputActionReference placeAction;

    [Header("Configuraci�n")]
    [SerializeField] private float grabDistance = 4f;
    [SerializeField] private float sphereRadius = 0.35f;
    [SerializeField] private LayerMask interactableLayer;

    private Rigidbody grabbedObject;

    private void OnEnable()
    {
        if (grabAction != null)
        {
            grabAction.action.Enable();
            grabAction.action.performed += OnGrab;
        }

        if (placeAction != null)
        {
            placeAction.action.Enable();
            placeAction.action.performed += OnPlace;
        }
    }

    private void OnDisable()
    {
        if (grabAction != null)
        {
            grabAction.action.performed -= OnGrab;
            grabAction.action.Disable();
        }

        if (placeAction != null)
        {
            placeAction.action.performed -= OnPlace;
            placeAction.action.Disable();
        }
    }

    private void OnGrab(InputAction.CallbackContext context)
    {
        if (grabbedObject != null)
            return;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.SphereCast(ray, sphereRadius, out RaycastHit hit, grabDistance, interactableLayer))
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();

            if (rb != null)
            {
                grabbedObject = rb;

                // Detener cualquier velocidad o rotaci�n residual previa
                grabbedObject.linearVelocity = Vector3.zero;
                grabbedObject.angularVelocity = Vector3.zero;

                // Desactivar respuesta a fuerzas f�sicas
                grabbedObject.isKinematic = true;

                // Asignar el HoldPoint como padre para un seguimiento suave en pantalla
                grabbedObject.transform.SetParent(holdPoint);
                grabbedObject.transform.localPosition = Vector3.zero;
                grabbedObject.transform.localRotation = Quaternion.identity;
            }
        }
    }

    private void OnPlace(InputAction.CallbackContext context)
    {
        if (grabbedObject == null)
            return;

        // Desvincular del HoldPoint
        grabbedObject.transform.SetParent(null);

        // Reactivar la f�sica
        grabbedObject.isKinematic = false;

        // Limpiar velocidades
        grabbedObject.linearVelocity = Vector3.zero;
        grabbedObject.angularVelocity = Vector3.zero;

        grabbedObject = null;
    }

    private void OnDrawGizmosSelected()
    {
        if (playerCamera == null)
            return;

        Gizmos.color = Color.yellow;
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        Gizmos.DrawRay(ray.origin, ray.direction * grabDistance);
        Gizmos.DrawWireSphere(ray.origin + ray.direction * grabDistance, sphereRadius);
    }
}