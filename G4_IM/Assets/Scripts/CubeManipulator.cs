using UnityEngine;

public class CubeManipulator : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform holdAnchor;

    [Header("Distancias")]
    [SerializeField] private float grabDistance = 10f;

    [Tooltip("Distancia a la que se mantiene el cubo frente al jugador.")]
    [SerializeField] private float holdDistance = 4f;

    [Header("Raycast")]
    [SerializeField] private LayerMask cubeLayer;

    [Header("Apilado")]
    [Tooltip("Si está activado, el cubo se centra automáticamente sobre el cubo inferior.")]
    [SerializeField] private bool autoStack = true;

    [Tooltip("Distancia máxima para encontrar un cubo debajo.")]
    [SerializeField] private float stackDetectionDistance = 10f;

    private Rigidbody heldCube;
    private Collider[] heldColliders;

    private CubeHighlight currentHighlightedCube;

    private void Update()
    {
        UpdateHighlight();

        if (heldCube != null)
        {
            UpdateHeldCubePosition();
        }
    }

    // =========================================================
    // DETECTAR CUBO QUE ESTAMOS MIRANDO
    // =========================================================

    private Rigidbody FindCubeLookingAt()
    {
        if (playerCamera == null)
            return null;

        // Ray que sale EXACTAMENTE desde el centro de la cámara.
        Ray ray = playerCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        // Revisamos todos los colliders que atraviesa
        // el rayo hasta la distancia máxima.
        RaycastHit[] hits = Physics.RaycastAll(
            ray,
            grabDistance,
            cubeLayer,
            QueryTriggerInteraction.Ignore
        );

        // Ordenamos los impactos por distancia.
        System.Array.Sort(
            hits,
            (a, b) => a.distance.CompareTo(b.distance)
        );

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider == null)
                continue;

            Rigidbody rb =
                hit.collider.GetComponentInParent<Rigidbody>();

            if (rb == null)
                continue;

            CubeHighlight highlight =
                rb.GetComponentInParent<CubeHighlight>();

            // Solo consideramos objetos que realmente
            // sean cubos interactuables.
            if (highlight != null)
            {
                return rb;
            }
        }

        return null;
    }

    // =========================================================
    // HIGHLIGHT
    // =========================================================

    private void UpdateHighlight()
    {
        // Si ya tenemos un cubo agarrado,
        // quitamos cualquier highlight.
        if (heldCube != null)
        {
            RemoveHighlight();
            return;
        }

        if (playerCamera == null)
        {
            RemoveHighlight();
            return;
        }

        // Utilizamos EXACTAMENTE la misma detección
        // que utilizaremos para Grab.
        Rigidbody targetCube =
            FindCubeLookingAt();

        if (targetCube == null)
        {
            RemoveHighlight();
            return;
        }

        CubeHighlight highlight =
            targetCube.GetComponentInParent<CubeHighlight>();

        if (highlight == null)
        {
            RemoveHighlight();
            return;
        }

        if (currentHighlightedCube != highlight)
        {
            RemoveHighlight();

            currentHighlightedCube = highlight;
            currentHighlightedCube.SetHighlight(true);
        }
    }

    private void RemoveHighlight()
    {
        if (currentHighlightedCube != null)
        {
            currentHighlightedCube.SetHighlight(false);
            currentHighlightedCube = null;
        }
    }

    // =========================================================
    // MOVER CUBO MIENTRAS ESTÁ AGARRADO
    // =========================================================

    private void UpdateHeldCubePosition()
    {
        if (playerCamera == null || heldCube == null)
            return;

        Vector3 targetPosition;

        if (holdAnchor != null)
        {
            targetPosition = holdAnchor.position;
        }
        else
        {
            targetPosition =
                playerCamera.transform.position +
                playerCamera.transform.forward *
                holdDistance;
        }

        heldCube.transform.position =
            targetPosition;

        // Mantener el cubo derecho.
        heldCube.transform.rotation =
            Quaternion.identity;
    }

    // =========================================================
    // AGARRAR
    // =========================================================

    public void TryGrab()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.StartGame();

        // Ya tenemos un cubo agarrado.
        if (heldCube != null)
            return;

        if (playerCamera == null)
            return;

        // Utilizamos exactamente el mismo método
        // que utiliza el Highlight.
        Rigidbody rb =
            FindCubeLookingAt();

        if (rb == null)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.RegisterError();

            return;
        }

        // Guardar Rigidbody.
        heldCube = rb;

        // Buscar todos los colliders del cubo.
        heldColliders =
            heldCube.GetComponentsInChildren<Collider>(true);

        // Detener completamente el movimiento.
        heldCube.linearVelocity = Vector3.zero;
        heldCube.angularVelocity = Vector3.zero;

        // Desactivar física mientras se sostiene.
        heldCube.isKinematic = true;

        // Desactivar colliders para evitar
        // choques mientras movemos el cubo.
        foreach (Collider col in heldColliders)
        {
            if (col != null)
                col.enabled = false;
        }

        // Quitar highlight.
        RemoveHighlight();

        // Colocarlo inmediatamente frente al jugador.
        UpdateHeldCubePosition();
    }

    // =========================================================
    // COLOCAR
    // =========================================================

    public void TryPlace()
    {
        if (heldCube == null)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.RegisterError();

            return;
        }

        Rigidbody cubeToRelease = heldCube;

        // 1. REACTIVAR COLLIDERS ANTES DE CALCULAR TAMAÑOS / BOUNDS
        if (heldColliders != null)
        {
            foreach (Collider col in heldColliders)
            {
                if (col != null)
                    col.enabled = true;
            }
        }

        // Sincronizar las transformaciones para que los Bounds lean el valor correcto.
        Physics.SyncTransforms();

        // 2. BUSCAR CUBO DE SOPORTE DEBAJO
        Rigidbody supportCube = FindCubeBelow(cubeToRelease);

        // 3. SI EXISTE UN CUBO BASE, REALIZAR EL APILADO AUTOMÁTICO
        if (autoStack && supportCube != null)
        {
            AutoStackCube(cubeToRelease, supportCube);
        }

        // Asegurar que el cubo quede con rotación limpia.
        cubeToRelease.transform.rotation = Quaternion.identity;

        // 4. REACTIVAR LA FÍSICA Y CANCELAR IMPULSOS
        cubeToRelease.isKinematic = false;
        cubeToRelease.useGravity = true;

        // Limpiar completamente las velocidades para evitar rebotes o fuerzas acumuladas.
        cubeToRelease.linearVelocity = Vector3.zero;
        cubeToRelease.angularVelocity = Vector3.zero;

        // Limpiar referencias.
        heldCube = null;
        heldColliders = null;
    }

    // =========================================================
    // BUSCAR CUBO DEBAJO
    // =========================================================

    private Rigidbody FindCubeBelow(Rigidbody cube)
    {
        if (cube == null)
            return null;

        Bounds cubeBounds = GetCombinedBounds(cube);

        Vector3 rayOrigin =
            cubeBounds.center +
            Vector3.up * 0.1f;

        RaycastHit[] hits =
            Physics.RaycastAll(
                rayOrigin,
                Vector3.down,
                stackDetectionDistance,
                cubeLayer,
                QueryTriggerInteraction.Ignore
            );

        Rigidbody closestCube = null;
        float closestDistance = Mathf.Infinity;

        foreach (RaycastHit hit in hits)
        {
            Rigidbody detectedRb =
                hit.collider.GetComponentInParent<Rigidbody>();

            if (detectedRb == null)
                continue;

            // No detectar el mismo cubo.
            if (detectedRb == cube)
                continue;

            float distance =
                Vector3.Distance(
                    rayOrigin,
                    hit.point
                );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestCube = detectedRb;
            }
        }

        return closestCube;
    }

    // =========================================================
    // APILAR AUTOMÁTICAMENTE
    // =========================================================

    private void AutoStackCube(
        Rigidbody cubeToPlace,
        Rigidbody supportCube)
    {
        if (cubeToPlace == null || supportCube == null)
            return;

        Bounds movingBounds = GetCombinedBounds(cubeToPlace);
        Bounds supportBounds = GetCombinedBounds(supportCube);

        // Diferencia entre el pivote del objeto y el centro real de sus colliders.
        Vector3 pivotToBoundsCenter =
            cubeToPlace.transform.position - movingBounds.center;

        // Colocación al contacto exacto (superficie del cubo inferior + mitad del cubo superior).
        // Se utiliza una minúscula tolerancia (0.0005f) para garantizar el contacto sin traslape.
        float desiredCenterY =
            supportBounds.max.y +
            movingBounds.extents.y +
            0.0005f;

        // Centrar perfectamente X y Z con respecto al cubo inferior.
        Vector3 desiredBoundsCenter =
            new Vector3(
                supportBounds.center.x,
                desiredCenterY,
                supportBounds.center.z
            );

        // Convertir del centro del collider a la posición del pivote del GameObject.
        Vector3 desiredPosition =
            desiredBoundsCenter + pivotToBoundsCenter;

        // Aplicar la nueva posición y rotación limpia de inmediato.
        cubeToPlace.transform.position = desiredPosition;
        cubeToPlace.transform.rotation = Quaternion.identity;

        // Resetear velocidades residuales.
        cubeToPlace.linearVelocity = Vector3.zero;
        cubeToPlace.angularVelocity = Vector3.zero;

        // Opcional: Hacer el cubo base kinematic evita que el peso acumulado desstabilice la torre.
        supportCube.isKinematic = true;
    }

    // =========================================================
    // OBTENER BOUNDS DE TODO EL CUBO
    // =========================================================

    private Bounds GetCombinedBounds(Rigidbody rb)
    {
        Collider[] colliders =
            rb.GetComponentsInChildren<Collider>(true);

        Bounds bounds =
            new Bounds(
                rb.transform.position,
                Vector3.zero
            );

        bool hasBounds = false;

        foreach (Collider col in colliders)
        {
            if (col == null)
                continue;

            if (!hasBounds)
            {
                bounds = col.bounds;
                hasBounds = true;
            }
            else
            {
                bounds.Encapsulate(col.bounds);
            }
        }

        return bounds;
    }

    // =========================================================
    // DEBUG VISUAL
    // =========================================================

    private void OnDrawGizmosSelected()
    {
        if (playerCamera == null)
            return;

        Gizmos.color = Color.yellow;

        Vector3 origin =
            playerCamera.transform.position;

        Vector3 direction =
            playerCamera.transform.forward;

        // Ray EXACTO que utilizamos para seleccionar.
        Gizmos.DrawRay(
            origin,
            direction * grabDistance
        );
    }
}