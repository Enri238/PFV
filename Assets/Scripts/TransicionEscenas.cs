using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class TransicionEscenas : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private AnimationClip animacionFinal;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator no encontrado en el GameObject.");
        }
    }
    public void Transicion(int nextSceneIdx,TimerManager timerManager)
    {
        StartCoroutine(CargarSiguienteEscena(nextSceneIdx,timerManager));
    }
    IEnumerator CargarSiguienteEscena(int nextSceneIdx,TimerManager timerManager)
    {
        // Iniciar la animación de transición
        if (animator != null)
        {
            animator.SetTrigger("Iniciar");
            
            yield return new WaitForSeconds(animacionFinal.length);
        }
        if (nextSceneIdx == 0)
            timerManager.StopTimer();
        else if (nextSceneIdx == 1)
            timerManager.StartTimer();

        SceneManager.LoadScene(nextSceneIdx);

		

        // Aquí puedes cargar la siguiente escena, por ejemplo:
        // SceneManager.LoadScene("NombreDeLaSiguienteEscena");
    }
}
