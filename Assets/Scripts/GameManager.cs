using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Opcional: Música persistente")]
    public AudioSource musicaFondo;

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

    // Cargar por índice (escena siguiente)
    public void CargarSiguienteEscena()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
}
