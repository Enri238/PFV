using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuInicial : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject menuPrincipal;
    public GameObject menuControles;
    public RectTransform botonesContainer;

    public RectTransform textoControles;   // NUEVO
    public RectTransform botonVolver;      // NUEVO

    [Header("Animación")]
    public float tiempoEntreCaidas = 0.05f;
    public float duracionCaida = 0.4f;
    public float distanciaCaida = 800f;

    private Vector2[] posicionesOriginales;
    private Vector2 posOriginalTexto;
    private Vector2 posOriginalBoton;

    void Start()
    {
        GuardarPosicionesOriginales();
        posOriginalTexto = textoControles.anchoredPosition;
        posOriginalBoton = botonVolver.anchoredPosition;

        textoControles.gameObject.SetActive(false);
        botonVolver.gameObject.SetActive(false);
        menuControles.SetActive(false);
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
            StartCoroutine(Deslizar(botonVolver, posOriginalBoton, duracionCaida));
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
