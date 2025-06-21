using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFinish : MonoBehaviour
{
    [Header("Etiqueta del jugador")]
    public string playerTag = "Player";

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            CargarSiguienteConWrap();
        }
    }

    private void CargarSiguienteConWrap()
    {
        int current = SceneManager.GetActiveScene().buildIndex;
        int total   = SceneManager.sceneCountInBuildSettings;
        int next    = current + 1;

        // Si pasamos de la última, volvemos a la 0
        if (next >= total)
            next = 0;

        SceneManager.LoadScene(next);
    }
}
