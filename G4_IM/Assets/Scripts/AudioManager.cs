using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Fuentes de Audio")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Música Global")]
    [Tooltip("Canción principal que sonará durante todo el juego (Menú, Juego y Resultados)")]
    [SerializeField] private AudioClip backgroundMusic;

    [Header("Efectos (Opcional)")]
    [Tooltip("Sonido de victoria para la pantalla final (opcional, suena sobre la música)")]
    [SerializeField] private AudioClip victorySFX;

    private void Awake()
    {
        // Patron Singleton con DontDestroyOnLoad para que la música no se corte al cambiar de escena
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        PlayBackgroundMusic();
    }

    public void PlayBackgroundMusic()
    {
        if (backgroundMusic == null || musicSource == null) return;

        // Si ya está sonando la canción, no la reinicies
        if (musicSource.clip == backgroundMusic && musicSource.isPlaying) return;

        musicSource.clip = backgroundMusic;
        musicSource.loop = true; // Bucle infinito
        musicSource.Play();
    }

    public void PlayVictorySFX()
    {
        // Reproduce el sonido de victoria encima de la música de fondo
        if (victorySFX != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(victorySFX);
        }
    }
}