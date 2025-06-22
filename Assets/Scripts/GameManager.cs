using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Opcional: Música persistente")]
    public AudioSource musicaFondo;

    [Header("UI de Pausa")]
    [Tooltip("Panel que contiene el menú de pausa (GameObject con Canvas)")]
    public GameObject pauseMenuUI;

	[Header("Temporizador")]
    public TimerManager timerManager;

    private bool isPaused = false;

    void Awake()
    {
        // Singleton: asegura que solo haya uno
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // persiste entre escenas
    }

    void Update()
    {
        // Si no estamos en la escena inicial (índice 0) y pulsamos Escape
        int idx = SceneManager.GetActiveScene().buildIndex;
        if (idx != 0 && Input.GetKeyDown(KeyCode.Escape) && idx != SceneManager.sceneCountInBuildSettings)
        {
            if (isPaused)
                Reanudar();
            else
                Pausar();
        }
    }

    /// <summary>
    /// Congela el juego y muestra el menú de pausa.
    /// </summary>
    private void Pausar()
    {
        Time.timeScale = 0f;
        isPaused = true;
        if (pauseMenuUI != null) pauseMenuUI.SetActive(true);
    }

    /// <summary>
    /// Vuelve al estado de juego normal desde pausa.
    /// </summary>
    public void Reanudar()
    {
        Time.timeScale = 1f;
        isPaused = false;
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
    }

    /// <summary>
    /// Carga la escena de menú principal (índice 0) y restaura el tiempo.
    /// </summary>
    public void MenuPrincipal()
    {
        // Asegurarnos de restaurar el tiempo y ocultar el menú de pausa
        Time.timeScale = 1f;
        isPaused = false;
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        // Cargar la escena principal (índice 0)
        SceneManager.LoadScene(0);
    }

    // Cargar por índice (escena siguiente)
    public void CargarSiguienteEscena()
    {
		int totalScenes = SceneManager.sceneCountInBuildSettings;
		int nextSceneIdx = (SceneManager.GetActiveScene().buildIndex + 1) % totalScenes;
        
        if (nextSceneIdx == 3) nextSceneIdx = 0;

        if (nextSceneIdx == 1)
			timerManager.StartTimer();

		SceneManager.LoadScene(nextSceneIdx);
    }

    // Cargar por nombre
    public void CargarEscena(string nombre)
    {
        SceneManager.LoadScene(nombre);
    }

    // Salir del juego
    public void SalirDelJuego()
    {
        Application.Quit();
        Debug.Log("Quit llamado");
    }

    // Reiniciar la escena actual
    public void ReiniciarEscena()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public bool EsUltimoNivel()
	{
		return SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 2;
	}
}
