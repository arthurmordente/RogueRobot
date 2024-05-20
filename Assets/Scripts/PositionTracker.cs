using UnityEngine;

public class PositionTracker : MonoBehaviour
{
    public GameManager instance;
    private float[] multipliers = new float[] {0.0f, 0.2f, 0.4f, 0.6f, 0.8f, 1.0f};
    public Transform playerTransform;
    public Transform enemyTransform;
    public float sectionLength = 50.0f;
    public float minDistance = 0;

    public float playerSection = 0f;  // Representa a seção atual do jogador como um float
    public float enemySection = 0f;   // Representa a seção atual do inimigo como um float
    public float currentDistance;
    public int lastScoredSection = 0;
    public int lastHitSection = 0;
    public int sectionDistance;       // Distância entre seções como um inteiro aproximado

    void Start()
    {
        // Inicializações adicionais se necessário
    }

    void Update()
    {
        // Atualiza as seções baseadas na posição Z atual
        playerSection = CalculateSection(playerTransform.position.z);
        enemySection = CalculateSection(enemyTransform.position.z);
        sectionDistance = CalculateSectionDistance(playerSection, enemySection);

        currentDistance = playerSection - enemySection;

        CheckDefeat();

        // Verifica se o jogador avançou para uma nova seção
        if ((int)playerSection > lastScoredSection)
        {
            lastScoredSection = (int)playerSection;
            instance.AddPoints(1);
        }
    }

    float CalculateSection(float positionZ)
    {
        return positionZ / sectionLength; // Retorna a seção como um valor float
    }

    int CalculateSectionDistance(float playerSection, float enemySection)
    {
        // Calcula a distância entre as seções do jogador e do inimigo
        return Mathf.RoundToInt(playerSection - enemySection); // Arredonda a diferença para o inteiro mais próximo
    }

    public float GetDistanceMultiplier()
    {
        // A distância maior que 5 retorna 1.0f
        if (sectionDistance > 5)
            return 1.0f;
        
        // Ajusta o índice para começar de 0 e pegar o valor correspondente no array
        int index = Mathf.Clamp(sectionDistance - 1, 0, multipliers.Length - 1);
        return multipliers[index];
    }

    public float GetPerfectMultiplier()
    {
        int distance = (int)(playerSection - lastHitSection);
  

        int maxDistance = Mathf.Clamp(distance, 0, 10); // Limita a distância máxima a 10 e a mínima a 2
        int index = (maxDistance / 2); // Calcula o índice com base na distância

        return multipliers[Mathf.Clamp(index, 0, multipliers.Length - 1)]; // Retorna o valor do array de multiplicadores
    }



    public void CheckDefeat()
    {
        if (playerSection - enemySection <= minDistance)
        {
            Debug.Log($"Lost due to distance. EnemySection: {enemySection}, PlayerSection: {playerSection}");
            instance.GameOver();
        }
    }

    public void UpdateLastHitSection(){
        lastHitSection = (int)playerSection;
    }
}
