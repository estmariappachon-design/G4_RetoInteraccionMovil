using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Referencias de Apilado")]
    [Tooltip("Arrastra aquí tu objeto STACK_ZONE que contiene el script StackZone")]
    [SerializeField] private StackZone stackZone;

    [Tooltip("Arrastra aquí los 4 cubos jugables")]
    [SerializeField] private Transform[] cubes;

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
        if (!gameStarted || stackZone == null)
            return;

        // Consultar el conteo real directamente al Trigger de STACK_ZONE
        int count = stackZone.GetCubeCount();

        if (correctlyPlaced != count)
        {
            correctlyPlaced = count;
            UpdateUI();
        }

        int totalCubes = (cubes != null && cubes.Length > 0) ? cubes.Length : 4;

        // Victoria cuando todos los cubos requeridos están en la zona
        if (correctlyPlaced >= totalCubes)
        {
            FinishGame();
        }
    }

    private void FinishGame()
    {
        gameFinished = true;
        gameStarted = false;

        UpdateUI();

        // Reproduce el efecto opcional de victoria sobre la canción en bucle
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayVictorySFX();
        }

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

        int totalCubes = (cubes != null && cubes.Length > 0) ? cubes.Length : 4;

        if (cubesText != null)
            cubesText.text = "Cubos: " + correctlyPlaced + " / " + totalCubes;

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