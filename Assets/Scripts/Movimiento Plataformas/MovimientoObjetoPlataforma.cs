using UnityEngine;

public class MovimientoObjetoPlataforma : MonoBehaviour
{
    [Header("Configuración")]
    public float altura = 1f;         // Cuánto sube desde la posición inicial
    public float velocidad = 1f;      // Frecuencia del movimiento (cuánto tarda en subir y bajar)

    private Vector3 posicionInicial;

    void Start()
    {
        // Guardamos la posición actual como base
        posicionInicial = transform.localPosition;
    }

    void Update()
    {
        // Sin oscila entre -1 y +1. Convertimos eso a 0…1 y multiplicamos por altura:
        float t = Mathf.Sin(Time.time * velocidad);      // –1 … +1
        float escala = (t + 1f) * 0.5f;                  //  0 …  1
        float desplazamiento = escala * altura;          //  0 … altura

        transform.localPosition = posicionInicial + Vector3.up * desplazamiento;
    }
}