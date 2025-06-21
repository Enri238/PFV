using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlataformaSerpiente : MonoBehaviour
{
    public enum MoveDirection { Forward, Backward, Left, Right, Up, Down }

    [Header("Plataformas")]
    [Tooltip("Arrastra aquí tus 4 plataformas en orden de cabeza a cola")]
    public List<Transform> platforms;

    [Header("Secuencia de movimientos")]
    [Tooltip("Direcciones en las que avanzará la cabeza, paso a paso")]
    public List<MoveDirection> movements;

    [Header("Parámetros de movimiento")]
    [Tooltip("Distancia que recorre cada paso (por ejemplo, el tamaño de la plataforma)")]
    public float stepDistance = 1f;
    [Tooltip("Velocidad de avance en unidades por segundo")]
    public float moveSpeed = 2f;
    [Tooltip("Retraso entre cada paso de la cabeza (en segundos)")]
    public float stepDelay = 0.1f;

    // posiciones iniciales de cada plataforma
    private List<Vector3> initialPositions = new List<Vector3>();
    // historial de posiciones de la cabeza
    private List<Vector3> pathPositions = new List<Vector3>();

    void Start()
    {
        if (platforms == null || platforms.Count == 0) return;

        // guardamos todas las posiciones de inicio
        foreach (var p in platforms)
            initialPositions.Add(p.localPosition);

        // arrancamos la rutina
        StartCoroutine(RunSnakeLoop());
    }

    IEnumerator RunSnakeLoop()
    {
        while (true)
        {
            pathPositions.Clear();
            // para cada paso de la secuencia
            foreach (var dir in movements)
            {
                // 1) calculamos nueva posición de la cabeza
                Vector3 delta = GetDirectionVector(dir) * stepDistance;
                Vector3 headPos = platforms[0].localPosition + delta;
                pathPositions.Add(headPos);

                // 2) fijamos los targets de cada plataforma
                Vector3[] targets = new Vector3[platforms.Count];
                for (int i = 0; i < platforms.Count; i++)
                {
                    int idx = pathPositions.Count - 1 - i;
                    if (idx >= 0)
                        targets[i] = pathPositions[idx];
                    else
                        targets[i] = initialPositions[i];
                }

                // 3) animamos todas las plataformas simultáneamente
                yield return StartCoroutine(MoveStep(platforms, targets));

                // 4) pequeño retraso antes del siguiente paso
                yield return new WaitForSeconds(stepDelay);
            }

            // una vez terminado, devolvemos todo a la posición inicial
            yield return StartCoroutine(MoveStep(platforms, initialPositions.ToArray()));

            // y reiniciamos el bucle
        }
    }

    IEnumerator MoveStep(List<Transform> objs, Vector3[] targets)
    {
        float duration = stepDistance / moveSpeed;
        float elapsed = 0f;

        // capturamos posiciones de arranque
        Vector3[] starts = new Vector3[objs.Count];
        for (int i = 0; i < objs.Count; i++)
            starts[i] = objs[i].localPosition;

        // interpolamos
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            for (int i = 0; i < objs.Count; i++)
                objs[i].localPosition = Vector3.Lerp(starts[i], targets[i], t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // aseguramos posición final exacta
        for (int i = 0; i < objs.Count; i++)
            objs[i].localPosition = targets[i];
    }

    Vector3 GetDirectionVector(MoveDirection dir)
    {
        switch (dir)
        {
            case MoveDirection.Forward:  return Vector3.forward;
            case MoveDirection.Backward: return Vector3.back;
            case MoveDirection.Left:     return Vector3.left;
            case MoveDirection.Right:    return Vector3.right;
            case MoveDirection.Up:       return Vector3.up;
            case MoveDirection.Down:     return Vector3.down;
            default:                     return Vector3.zero;
        }
    }
}
