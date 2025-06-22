using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFinish : MonoBehaviour
{
    [Header("Etiqueta del jugador")]
    public string playerTag = "Player";

	[Header("Canvas de victoria")]
    public GameObject victoryCanvas;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI recordText;

	private GameManager _gameManager;

	void Awake()
	{
		_gameManager = FindObjectOfType<GameManager>();
	}

	void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            if (!_gameManager.EsUltimoNivel())
                _gameManager.CargarSiguienteEscena();
            else
            {
                TimerManager timerManager = _gameManager.timerManager;
				timerManager.StopTimer();
				
                if (victoryCanvas)
                {
					timeText.text = timerManager.GetTimeString();
                    recordText.text = timerManager.IsNewRecord() ? "¡Nuevo récord!" : "Récord: " + timerManager.GetRecordString();
					victoryCanvas.SetActive(true);
				}
			}
		}
    }

    public void SiguienteEscena()
    {
        _gameManager.CargarSiguienteEscena();
    }

    //private void CargarSiguienteConWrap()
    //{
    //    int current = SceneManager.GetActiveScene().buildIndex;
    //    int total   = SceneManager.sceneCountInBuildSettings;
    //    int next    = current + 1;

    //    // Si pasamos de la última, volvemos a la 0
    //    if (next >= total)
    //        next = 0;

    //    SceneManager.LoadScene(next);
    //}
}
