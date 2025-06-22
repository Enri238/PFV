using UnityEngine;

public class ActivarSerpienteAlEntrar : MonoBehaviour
{
    [Tooltip("Arrastra aquí el GameObject 'serpiente' que quieres activar")]
    public GameObject serpiente;

    void Start()
    {
        // Asegúrate de que la serpiente empiece desactivada
        if (serpiente != null)
            serpiente.SetActive(false);
        else
            Debug.LogWarning("No has asignado el GameObject 'serpiente' en " + name);
    }

    void OnTriggerEnter(Collider other)
    {
        // Comprueba que quien entra tiene la etiqueta Player
        if (other.CompareTag("Player") && serpiente != null)
        {
            serpiente.SetActive(true);
        }
    }
}
