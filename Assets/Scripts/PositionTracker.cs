using UnityEngine;

public class PositionTracker : MonoBehaviour
{
    public GameManager instance;
    public Transform playerTransform;
    public Transform enemyTransform;
    public float sectionLength = 50.0f;

    public float playerSection = 0f;  // Agora usando float
    public float enemySection = 0f;  // Agora usando float
    private float nextPlayerSectionZ;
    private float nextEnemySectionZ;

    void Start()
    {
        // Inicializa a posição Z da próxima seção para ambos, jogador e inimigo
        nextPlayerSectionZ = playerTransform.position.z + sectionLength;
        nextEnemySectionZ = enemyTransform.position.z + sectionLength - enemyTransform.position.z;
    }

    void Update()
    {
        UpdatePlayerSection();
        UpdateEnemySection();
        
        // Verificar condição de derrota
        if (enemySection >= playerSection)
        {
            Debug.Log($"enemySection: {enemySection}, playerSection: {playerSection}");
            GameManager.instance.GameOver();
        }
    }

    void UpdatePlayerSection()
    {
        if (playerTransform.position.z >= nextPlayerSectionZ)
        {
            playerSection += 1f; // Incrementa uma seção completa
            nextPlayerSectionZ += sectionLength;
            instance.AddPoints(1);
        }
        else
        {
            // Atualiza a seção do jogador como uma fração
            playerSection = (int)playerSection + (playerTransform.position.z - (nextPlayerSectionZ - sectionLength)) / sectionLength;
        }
    }

    void UpdateEnemySection()
    {
        if (enemyTransform.position.z >= nextEnemySectionZ)
        {
            enemySection += 1f; // Incrementa uma seção completa
            nextEnemySectionZ += sectionLength;
        }
        else
        {
            // Atualiza a seção do inimigo como uma fração
            enemySection = (int)enemySection + (enemyTransform.position.z - (nextEnemySectionZ - sectionLength)) / sectionLength;
        }
    }
}
