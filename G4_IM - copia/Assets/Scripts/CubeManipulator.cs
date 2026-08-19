using UnityEngine;

public class CubeManipulator : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform holdAnchor;

    [Header("Distancias y Ajustes")]
    [SerializeField] private float grabDistance = 10f;

    [Tooltip("Distancia a la que se mantendrá el cubo respecto a la cámara al agarrarlo (Aumenta este valor para alejarlo)")]
    [SerializeField] private float holdDistance = 4.5f; // <--- Aumentamos de 2f a 4.5f (o lo que prefieras)

    [Tooltip("Inclinación del rayo hacia el suelo (0 = al frente, 0.3 = inclinado hacia abajo)")]
    [SerializeField] private float downwardAngle = 0.3f;

    [Header("Capas")]
    [SerializeField] private LayerMask cubeLayer;

    private Rigidbody heldCube;
    private Collider heldCollider;

    private void Update()
    {
        if (heldCube != null)
        {
            UpdateHeldCubePosition();
        }
    }

    private void UpdateHeldCubePosition()
    {
        if (playerCamera == null || heldCube == null)
            return;

        // Si hay HoldAnchor lo usa, pero si no, o si quieres usar holdDistance directamente:
        Vector3 targetPosition = (holdAnchor != null)
            ? holdAnchor.position
            : playerCamera.transform.position + playerCamera.transform.forward * holdDistance;

        heldCube.transform.position = targetPosition;
        heldCube.transform.rotation = Quaternion.identity;
    }

    public void TryGrab()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.StartGame();

        if (heldCube != null)
            return;

        if (playerCamera == null) return;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 directionDown = (ray.direction - Vector3.up * downwardAngle).normalized;

        if (Physics.Raycast(ray.origin, directionDown, out RaycastHit hit, grabDistance, cubeLayer))
        {
            if (hit.rigidbody != null)
            {
                heldCube = hit.rigidbody;
                heldCollider = heldCube.GetComponent<Collider>();

                heldCube.linearVelocity = Vector3.zero;
                heldCube.angularVelocity = Vector3.zero;
                heldCube.isKinematic = true;

                if (heldCollider != null)
                {
                    heldCollider.enabled = false;
                }

                UpdateHeldCubePosition();
            }
        }
        else
        {
            if (GameManager.Instance != null)
                GameManager.Instance.RegisterError();
        }
    }

    public void TryPlace()
    {
        if (heldCube == null)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.RegisterError();

            return;
        }

        Rigidbody cubeToRelease = heldCube;

        if (heldCollider != null)
        {
            heldCollider.enabled = true;
        }

        cubeToRelease.transform.rotation = Quaternion.identity;

        cubeToRelease.isKinematic = false;
        cubeToRelease.useGravity = true;

        cubeToRelease.linearVelocity = Vector3.zero;
        cubeToRelease.angularVelocity = Vector3.zero;

        heldCube = null;
        heldCollider = null;
    }
}