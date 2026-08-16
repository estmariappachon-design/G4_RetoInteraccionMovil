using UnityEngine;

public class CubeManipulator : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float grabDistance = 8f;
    [SerializeField] private Transform holdAnchor;
    [SerializeField] private LayerMask cubeLayer;
    [SerializeField] private LayerMask placementSurfaceLayer; // Suelo y superficies donde colocar

    private Rigidbody heldCube;

    public void TryGrab()
    {
        if (heldCube != null) return;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, grabDistance, cubeLayer))
        {
            if (hit.rigidbody != null)
            {
                heldCube = hit.rigidbody;
                heldCube.isKinematic = true;

                // Desactiva colisiones mientras lo llevas
                if (heldCube.TryGetComponent<Collider>(out Collider col))
                {
                    col.enabled = false;
                }

                heldCube.transform.SetParent(holdAnchor);
                heldCube.transform.localPosition = Vector3.zero;
                heldCube.transform.localRotation = Quaternion.identity;
            }
        }
    }

    public void TryPlace()
    {
        if (heldCube == null) return;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        // Si apuntas al suelo o a otro cubo dentro del rango, lo posiciona exactamente en el punto de impacto
        if (Physics.Raycast(ray, out RaycastHit hit, grabDistance, placementSurfaceLayer))
        {
            // Coloca el cubo justo en la superficie detectada
            heldCube.transform.SetParent(null);
            heldCube.transform.position = hit.point + (hit.normal * 0.5f); // 0.5f evita que se cruce con la superficie
            heldCube.transform.rotation = Quaternion.identity; // Lo mantiene alineado y recto
        }
        else
        {
            // Si apuntas al aire, simplemente lo suelta frente a ti
            heldCube.transform.SetParent(null);
        }

        // Reactiva la física y colisiones
        if (heldCube.TryGetComponent<Collider>(out Collider col))
        {
            col.enabled = true;
        }

        heldCube.isKinematic = false;
        heldCube = null;
    }
}