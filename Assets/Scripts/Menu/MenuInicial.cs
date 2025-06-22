using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuInicial : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject menuPrincipal;
    public GameObject menuControles;
    public GameObject menuOpciones;          // NUEVO
    public RectTransform botonesContainer;

    public RectTransform textoControles;
    public RectTransform botonVolver;

    public RectTransform textoOpciones;      // NUEVO
    public RectTransform botonOpcionesVolver;// NUEVO
    public RectTransform sliderOpciones;     // NUEVO

    [Header("Animación")]
    public float tiempoEntreCaidas = 0.05f;
    public float duracionCaida = 0.4f;
    public float distanciaCaida = 800f;

    private Vector2[] posicionesOriginales;
    private Vector2 posOriginalTexto;
    private Vector2 posOriginalBoton;
    private Vector2 posOriginalTextoOpciones;    // NUEVO
    private Vector2 posOriginalBotonOpciones;    // NUEVO
    private Vector2 posOriginalSliderOpciones;   // NUEVO

    private GameManager _gameManager;

    void Start()
    {
        GuardarPosicionesOriginales();

        posOriginalTexto = textoControles.anchoredPosition;
        posOriginalBoton = botonVolver.anchoredPosition;

        posOriginalTextoOpciones = textoOpciones.anchoredPosition;     // NUEVO
        posOriginalBotonOpciones = botonOpcionesVolver.anchoredPosition; // NUEVO
        posOriginalSliderOpciones = sliderOpciones.anchoredPosition;     // NUEVO

        // Ocultar menús secundarios al inicio
        textoControles.gameObject.SetActive(false);
        botonVolver.gameObject.SetActive(false);
        menuControles.SetActive(false);

        textoOpciones.gameObject.SetActive(false);       // NUEVO
        botonOpcionesVolver.gameObject.SetActive(false); // NUEVO
        sliderOpciones.gameObject.SetActive(false);      // NUEVO
        menuOpciones.SetActive(false);                   // NUEVO

        _gameManager = FindObjectOfType<GameManager>();
    }

    void GuardarPosicionesOriginales()
    {
        var hijos = botonesContainer.GetComponentsInChildren<RectTransform>();
        posicionesOriginales = new Vector2[hijos.Length];

        for (int i = 0; i < hijos.Length; i++)
            posicionesOriginales[i] = hijos[i].anchoredPosition;
    }

    public void Jugar()
    {
        Debug.Log("Cargando siguiente escena..." + SceneManager.GetActiveScene().buildIndex);
        _gameManager.CargarSiguienteEscena();
    }

    public void Salir()
    {
        Application.Quit();
        Debug.Log("Quit llamado (en el editor no se cierra).");
    }

    public void MostrarControles()
    {
        StartCoroutine(AnimarCaida(botonesContainer, () =>
        {
            botonesContainer.gameObject.SetActive(false);
            menuControles.SetActive(true);

            textoControles.anchoredPosition = posOriginalTexto + Vector2.down * distanciaCaida;
            botonVolver.anchoredPosition    = posOriginalBoton + Vector2.down * distanciaCaida;

            textoControles.gameObject.SetActive(true);
            botonVolver.gameObject.SetActive(true);

            StartCoroutine(Deslizar(textoControles, posOriginalTexto, duracionCaida));
            StartCoroutine(Deslizar(botonVolver,    posOriginalBoton,    duracionCaida));
        }));
    }

    public void VolverAlMenu()
    {
        StartCoroutine(Deslizar(textoControles, textoControles.anchoredPosition + Vector2.down * distanciaCaida, duracionCaida));
        StartCoroutine(Deslizar(botonVolver,    botonVolver.anchoredPosition    + Vector2.down * distanciaCaida, duracionCaida, () =>
        {
            textoControles.gameObject.SetActive(false);
            botonVolver.gameObject.SetActive(false);
            menuControles.SetActive(false);
            botonesContainer.gameObject.SetActive(true);
            StartCoroutine(AnimarSubida(botonesContainer));
        }));
    }

    // NUEVO: Mostrar Opciones
    public void MostrarOpciones()
    {
        StartCoroutine(AnimarCaida(botonesContainer, () =>
        {
            Debug.Log("Mostrando opciones...");
            botonesContainer.gameObject.SetActive(false);
            menuOpciones.SetActive(true);

            textoOpciones.anchoredPosition      = posOriginalTextoOpciones + Vector2.down * distanciaCaida;
            botonOpcionesVolver.anchoredPosition = posOriginalBotonOpciones + Vector2.down * distanciaCaida;
            sliderOpciones.anchoredPosition      = posOriginalSliderOpciones + Vector2.down * distanciaCaida;

            textoOpciones.gameObject.SetActive(true);
            botonOpcionesVolver.gameObject.SetActive(true);
            sliderOpciones.gameObject.SetActive(true);

            StartCoroutine(Deslizar(textoOpciones,      posOriginalTextoOpciones,    duracionCaida));
            StartCoroutine(Deslizar(botonOpcionesVolver, posOriginalBotonOpciones,    duracionCaida));
            StartCoroutine(Deslizar(sliderOpciones,      posOriginalSliderOpciones,   duracionCaida));
        }));
    }

    // NUEVO: Volver desde Opciones
    public void VolverDeOpciones()
    {
        Debug.Log("Volviendo al menú principal desde opciones...");
        StartCoroutine(Deslizar(textoOpciones, textoOpciones.anchoredPosition + Vector2.down * distanciaCaida, duracionCaida));
        StartCoroutine(Deslizar(botonOpcionesVolver, botonOpcionesVolver.anchoredPosition + Vector2.down * distanciaCaida, duracionCaida));
        StartCoroutine(Deslizar(sliderOpciones, sliderOpciones.anchoredPosition + Vector2.down * distanciaCaida, duracionCaida, () =>
        {
            textoOpciones.gameObject.SetActive(false);
            botonOpcionesVolver.gameObject.SetActive(false);
            sliderOpciones.gameObject.SetActive(false);
            menuOpciones.SetActive(false);
            botonesContainer.gameObject.SetActive(true);
            StartCoroutine(AnimarSubida(botonesContainer));
        }));
    }

    public void MostrarCreditos()
    {
        int totalScenes     = SceneManager.sceneCountInBuildSettings;
        int creditSceneIdx  = totalScenes - 1;
        Debug.Log("Cargando créditos (escena " + creditSceneIdx + ")");
        SceneManager.LoadScene(creditSceneIdx);
    }

    IEnumerator AnimarCaida(RectTransform contenedor, System.Action alFinalizar)
    {
        var hijos = contenedor.GetComponentsInChildren<RectTransform>();

        for (int i = 0; i < hijos.Length; i++)
        {
            StartCoroutine(Caer(hijos[i], i));
            yield return new WaitForSeconds(tiempoEntreCaidas);
        }

        yield return new WaitForSeconds(duracionCaida + tiempoEntreCaidas * hijos.Length);
        alFinalizar?.Invoke();
    }

    IEnumerator Caer(RectTransform rt, int index)
    {
        if (rt == botonesContainer) yield break;

        Vector2 inicio = rt.anchoredPosition;
        Vector2 destino = inicio + Vector2.down * distanciaCaida;
        float t = 0;

        while (t < duracionCaida)
        {
            rt.anchoredPosition = Vector2.Lerp(inicio, destino, t / duracionCaida);
            t += Time.deltaTime;
            yield return null;
        }

        rt.anchoredPosition = destino;
    }

    IEnumerator AnimarSubida(RectTransform contenedor)
    {
        var hijos = contenedor.GetComponentsInChildren<RectTransform>();

        for (int i = 0; i < hijos.Length; i++)
        {
            StartCoroutine(Subir(hijos[i], i));
            yield return new WaitForSeconds(tiempoEntreCaidas);
        }
    }

    IEnumerator Subir(RectTransform rt, int index)
    {
        if (rt == botonesContainer) yield break;

        Vector2 destino = posicionesOriginales[index];
        Vector2 inicio = destino + Vector2.down * distanciaCaida;
        rt.anchoredPosition = inicio;

        float t = 0;
        while (t < duracionCaida)
        {
            rt.anchoredPosition = Vector2.Lerp(inicio, destino, t / duracionCaida);
            t += Time.deltaTime;
            yield return null;
        }

        rt.anchoredPosition = destino;
    }

    IEnumerator Deslizar(RectTransform rt, Vector2 destino, float duracion, System.Action alFinalizar = null)
    {
        Vector2 inicio = rt.anchoredPosition;
        float t = 0;

        while (t < duracion)
        {
            rt.anchoredPosition = Vector2.Lerp(inicio, destino, t / duracion);
            t += Time.deltaTime;
            yield return null;
        }

        rt.anchoredPosition = destino;
        alFinalizar?.Invoke();
    }
}
