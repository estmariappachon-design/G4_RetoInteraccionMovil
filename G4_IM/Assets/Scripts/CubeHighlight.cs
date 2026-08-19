using UnityEngine;

public class CubeHighlight : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private Color highlightColor = Color.yellow;

    [Tooltip("Intensidad de la emisión.")]
    [SerializeField] private float emissionIntensity = 5f;

    [Tooltip("Cuánto se aclara el color del cubo.")]
    [SerializeField] private float colorIntensity = 1.5f;

    private Renderer[] renderers;
    private MaterialPropertyBlock propertyBlock;

    private bool isHighlighted = false;

    private static readonly int ColorID =
        Shader.PropertyToID("_BaseColor");

    private static readonly int EmissionColorID =
        Shader.PropertyToID("_EmissionColor");

    private void Awake()
    {
        renderers =
            GetComponentsInChildren<Renderer>(true);

        propertyBlock =
            new MaterialPropertyBlock();
    }

    public void SetHighlight(bool active)
    {
        isHighlighted = active;

        foreach (Renderer rend in renderers)
        {
            if (rend == null)
                continue;

            rend.GetPropertyBlock(propertyBlock);

            if (active)
            {
                // Color principal del cubo.
                propertyBlock.SetColor(
                    ColorID,
                    highlightColor *
                    colorIntensity
                );

                // Emisión fuerte.
                propertyBlock.SetColor(
                    EmissionColorID,
                    highlightColor *
                    emissionIntensity
                );
            }
            else
            {
                // Quita completamente las propiedades
                // temporales del highlight.
                propertyBlock.Clear();
            }

            rend.SetPropertyBlock(
                propertyBlock
            );
        }
    }

    private void OnDisable()
    {
        if (renderers == null)
            return;

        foreach (Renderer rend in renderers)
        {
            if (rend == null)
                continue;

            rend.SetPropertyBlock(null);
        }
    }
}