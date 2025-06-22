using UnityEngine;
using UnityEngine.UI;

public class CreditosMenuPrincipal : MonoBehaviour
{
    [Tooltip("Arrastra aquí tu botón de volver al menú principal")]
    public Button backButton;

    private GameManager gameManager;

    void Awake()
    {
        // Busca el GameManager en la escena (o en las que hayas marcado como DontDestroyOnLoad)
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
            Debug.LogError("CreditBackAssigner: No se encontró un GameManager en la escena.");
    }

    void Start()
    {
        if (backButton == null)
        {
            Debug.LogError("CreditBackAssigner: No has asignado el botón de volver.");
            return;
        }

        if (gameManager != null)
        {
            // Asigna el listener al OnClick del botón
            backButton.onClick.AddListener(gameManager.MenuPrincipal);
        }
    }
}
