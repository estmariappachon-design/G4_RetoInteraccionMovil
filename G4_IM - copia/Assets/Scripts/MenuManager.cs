using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MenuManagerGlowBorder : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Configuración de Escena")]
    [SerializeField] private string nombreEscenaJuego = "NivelJuego";
    [SerializeField] private float retrasoAlCargar = 0.3f;

    [Header("Ajustes de Interacción Cute")]
    [SerializeField] private float escalaHover = 1.1f;
    [SerializeField] private float escalaClick = 0.9f;
    [SerializeField] private float velocidadAnim = 12f;

    [Header("Efecto Brillo en Borde")]
    [SerializeField] private Image imagenBrilloBorde; // Necesitamos referencia a una imagen secundaria
    [SerializeField] private Color colorBrillo = new Color(1f, 0.9f, 0.5f, 1f); // Naranja/Amarillo neón para resaltar el morado
    [SerializeField] private float velocidadBrillo = 4f;
    [SerializeField] private float intensidadBrilloMin = 0.3f; // Transparencia mínima del brillo
    [SerializeField] private float intensidadBrilloMax = 0.8f; // Transparencia máxima del brillo

    private Vector3 escalaOriginal;
    private Vector3 escalaObjetivo;
    private bool estaHover = false;
    private Color colorBrilloActual;

    private void Start()
    {
        escalaOriginal = transform.localScale;
        escalaObjetivo = escalaOriginal;

        if (imagenBrilloBorde != null)
        {
            // Aseguramos que la imagen del brillo empiece con el color y transparencia correcta
            colorBrilloActual = colorBrillo;
            colorBrilloActual.a = intensidadBrilloMin;
            imagenBrilloBorde.color = colorBrilloActual;
        }
    }

    private void Update()
    {
        // 1. Animación de Escala (Hover / Click)
        transform.localScale = Vector3.Lerp(transform.localScale, escalaObjetivo, Time.deltaTime * velocidadAnim);

        // 2. Animación de Brillo continuo SOLO en el borde (efecto neón)
        if (imagenBrilloBorde != null)
        {
            float pulsoSin = (Mathf.Sin(Time.time * velocidadBrillo) + 1f) / 2f; // Resultado 0 a 1

            // Ajustamos el rango de transparencia del pulso
            float alphaPulso = Mathf.Lerp(intensidadBrilloMin, intensidadBrilloMax, pulsoSin);

            // Si está en Hover, el brillo es más fuerte y estable
            if (estaHover)
            {
                alphaPulso = Mathf.Max(alphaPulso, 0.9f);
            }

            colorBrilloActual.a = alphaPulso;
            imagenBrilloBorde.color = colorBrilloActual;
        }
    }

    public void CargarJuego()
    {
        StartCoroutine(RutinaCargarJuego());
    }

    private IEnumerator RutinaCargarJuego()
    {
        escalaObjetivo = escalaOriginal * escalaClick;
        yield return new WaitForSeconds(retrasoAlCargar);
        SceneManager.LoadScene(nombreEscenaJuego);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        estaHover = true;
        escalaObjetivo = escalaOriginal * escalaHover;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        estaHover = false;
        escalaObjetivo = escalaOriginal;
    }
}