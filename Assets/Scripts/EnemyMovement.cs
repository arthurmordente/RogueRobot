using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Transform playerTransform; // Referência ao jogador para seguir seu X e Y
    public float speed = 5.0f; // Velocidade com que o inimigo se aproxima no eixo Z
    public float followDistance = 10.0f; // Distância que o inimigo deve manter atrás do jogador

    private PositionTracker positionTracker; // Referência ao PositionTracker

    void Start()
    {
        positionTracker = FindObjectOfType<PositionTracker>();
    }

    void Update()
    {
        if (positionTracker != null)
        {
            // Calcula o ponto atrás do jogador que o inimigo deve seguir
            float targetZ = playerTransform.position.z - followDistance;
            float newZPosition = Mathf.MoveTowards(transform.position.z, targetZ, speed * Time.deltaTime);
            transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, newZPosition);
        }
    }
}
