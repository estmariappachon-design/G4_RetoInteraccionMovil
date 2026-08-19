using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ZonePulsate : MonoBehaviour
{
    [Header("Configuración de Parpadeo")]
    [Tooltip("Velocidad de la pulsación.")]
    [SerializeField] private float pulseSpeed = 3f;

    [Tooltip("Grosor de la línea del borde.")]
    [SerializeField] private float borderWidth = 0.04f;

    [Tooltip("Color del brillo pulsante del borde.")]
    [ColorUsage(true, true)]
    [SerializeField] private Color emissionColor = Color.cyan;

    private LineRenderer lineRenderer;
    private Material lineMaterial;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        // Configuración visual de las líneas
        lineRenderer.useWorldSpace = false;
        lineRenderer.startWidth = borderWidth;
        lineRenderer.endWidth = borderWidth;
        lineRenderer.positionCount = 16; // Puntos necesarios para cerrar un cubo en 3D

        // Material básico transparente
        lineMaterial = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.material = lineMaterial;

        // Dibujar la estructura del cubo
        DrawCubeOutline();
    }

    private void Update()
    {
        // Calcular la intensidad suave de transparencia/brillo usando la onda senoidal
        float lerpFactor = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;

        Color currentColor = emissionColor;
        currentColor.a = Mathf.Lerp(0.15f, 1f, lerpFactor); // Transparencia que parpadea

        lineRenderer.startColor = currentColor;
        lineRenderer.endColor = currentColor;
    }

    private void DrawCubeOutline()
    {
        // Puntos locales para formar el marco 3D del cubo sin tapar el centro
        Vector3[] points = new Vector3[]
        {
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3( 0.5f, -0.5f, -0.5f),
            new Vector3( 0.5f,  0.5f, -0.5f),
            new Vector3(-0.5f,  0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f,  0.5f),
            new Vector3( 0.5f, -0.5f,  0.5f),
            new Vector3( 0.5f, -0.5f, -0.5f),
            new Vector3( 0.5f,  0.5f, -0.5f),
            new Vector3( 0.5f,  0.5f,  0.5f),
            new Vector3(-0.5f,  0.5f,  0.5f),
            new Vector3(-0.5f,  0.5f, -0.5f),
            new Vector3(-0.5f,  0.5f,  0.5f),
            new Vector3(-0.5f, -0.5f,  0.5f),
            new Vector3( 0.5f, -0.5f,  0.5f),
            new Vector3( 0.5f,  0.5f,  0.5f)
        };

        lineRenderer.SetPositions(points);
    }
}