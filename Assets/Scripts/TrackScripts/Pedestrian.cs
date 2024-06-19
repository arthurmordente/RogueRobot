using UnityEngine;

public class Pedestrian : MonoBehaviour
{
    public float speed = 1.0f; // Velocidade do movimento
    public float minX = -11f; // Posição mínima de x
    public float maxX = 11f; // Posição máxima de x

    private float targetX; // Posição alvo atual
    private bool movingToMax = true; // Controla a direção do movimento

    void Start()
    {
        // Define aleatoriamente a direção inicial do movimento
        movingToMax = Random.Range(0, 2) == 0; // Gera 0 ou 1 aleatoriamente
        speed = Random.Range(5, 15);
        targetX = movingToMax ? maxX : minX; // Define o alvo com base na direção
    }

    void Update()
    {
        // Move o objeto horizontalmente para a posição alvo
        Vector3 newPosition = transform.position;
        newPosition.x = Mathf.MoveTowards(newPosition.x, targetX, speed * Time.deltaTime);
        transform.position = newPosition;

        // Verifica se o objeto alcançou a posição alvo
        if (Mathf.Approximately(newPosition.x, targetX))
        {
            // Troca a direção do movimento
            if (movingToMax)
            {
                targetX = minX;
                movingToMax = false;
            }
            else
            {
                targetX = maxX;
                movingToMax = true;
            }
        }
    }
}
