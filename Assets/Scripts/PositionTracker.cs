using UnityEngine;

public class PositionTracker : MonoBehaviour
{
    public GameManager instance;
    public Transform playerTransform;
    public Transform enemyTransform;
    public float sectionLength = 50.0f;

    public float playerSection = 0f;  // Representa a seção atual do jogador como um float
    public float enemySection = 0f;   // Representa a seção atual do inimigo como um float

    void Start()
    {
        // Não é mais necessário iniciar as variáveis nextSectionZ
    }

    void Update()
    {
        // Atualiza as seções baseadas na posição Z atual
        playerSection = CalculateSection(playerTransform.position.z);
        enemySection = CalculateSection(enemyTransform.position.z);

        // Verificar condição de derrota
        if (enemySection - 0.05f >= playerSection)
        {
            Debug.Log($"EnemySection: {enemySection}, PlayerSection: {playerSection}");
            instance.GameOver();
        }
    }

    float CalculateSection(float positionZ)
    {
        return positionZ / sectionLength; // Retorna a seção como um valor float
    }
}
