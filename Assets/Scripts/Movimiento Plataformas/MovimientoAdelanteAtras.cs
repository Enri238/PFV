using UnityEngine;

public class MovimientoAdelanteAtras : MonoBehaviour
{
    [Header("Configuración Z")]
    [Tooltip("Cuánto se moverá desde la posición inicial en Z (hacia negativo)")]
    public float distancia = 1f;

    [Tooltip("Velocidad del ciclo completo (un ciclo = adelante y atrás)")]
    public float velocidad = 1f;

    private Vector3 posicionInicial;

    void Start()
    {
        // Guardamos la posición de partida
        posicionInicial = transform.localPosition;
    }

    void Update()
    {
        // Mathf.PingPong(t, distancia) oscila 0…distancia…0…distancia…
        float offsetZ = Mathf.PingPong(Time.time * velocidad, distancia);
        // Vector3.back = (0,0,-1)
        transform.localPosition = posicionInicial + Vector3.back * offsetZ;
    }
}
