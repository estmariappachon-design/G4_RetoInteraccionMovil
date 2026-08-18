using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Cubos y Zona")]
    [SerializeField] private Transform[] cubes;
    [SerializeField] private Transform stackZone;

    [Header("Configuración de Apilado")]
    [Tooltip("Tamaño/Altura de tus cubos actuales")]
    [SerializeField] private float cubeSize = 2f;

    [Tooltip("Margen de tolerancia horizontal (radio en la zona)")]
    [SerializeField] private float positionTolerance = 0.8f;

    [Header("UI")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text cubesText;
    [SerializeField] private TMP_Text errorsText;

    [Header("Pantalla Final")]
    [SerializeField] private GameObject finishPanel;
    [SerializeField] private TMP_Text finalTimeText;
    [SerializeField] private TMP_Text finalErrorsText;

    private float elapsedTime;
    private int errors;
    private int correctlyPlaced;
    private bool gameStarted = false;
    private bool gameFinished = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (finishPanel != null)
            finishPanel.SetActive(false);

        StartGame();
        UpdateUI();
    }

    private void Update()
    {
        if (gameFinished) return;

        if (gameStarted)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerUI();
            CheckStack();
        }
    }

    public void StartGame()
    {
        gameStarted = true;
        gameFinished = false;
    }

    public void RegisterError()
    {
        if (gameFinished) return;
        if (!gameStarted) StartGame();

        errors++;
        UpdateUI();
    }

    private void CheckStack()
    {
        if (!gameStarted || stackZone == null || cubes == null || cubes.Length == 0)
            return;

        int count = 0;

        // Comprobar cuántos cubos están dentro de la STACK_ZONE
        foreach (Transform cube in cubes)
        {
            if (cube == null) continue;

            // Distancia horizontal (X, Z) desde la zona
            float horizontalDistance = Vector2.Distance(
                new Vector2(cube.position.x, cube.position.z),
                new Vector2(stackZone.position.x, stackZone.position.z)
            );

            // Si el cubo está posicionado dentro del radio de la zona de apilado
            if (horizontalDistance <= positionTolerance)
            {
                count++;
            }
        }

        correctlyPlaced = count;
        UpdateUI();

        // Si los 4 cubos están en la zona
        if (correctlyPlaced >= cubes.Length)
        {
            FinishGame();
        }
    }

    private void FinishGame()
    {
        gameFinished = true;
        gameStarted = false;

        UpdateUI();

        if (finishPanel != null)
            finishPanel.SetActive(true);

        if (finalTimeText != null)
            finalTimeText.text = "Tiempo: " + FormatTime(elapsedTime);

        if (finalErrorsText != null)
            finalErrorsText.text = "Errores: " + errors;
    }

    private void UpdateUI()
    {
        UpdateTimerUI();

        if (cubesText != null)
            cubesText.text = "Cubos: " + correctlyPlaced + " / " + cubes.Length;

        if (errorsText != null)
            errorsText.text = "Errores: " + errors;
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
            timerText.text = FormatTime(elapsedTime);
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 100f) % 100f);

        return string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
    }
}