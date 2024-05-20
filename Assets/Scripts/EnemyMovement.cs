using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
    public GameManager instance;
    public Transform playerTransform; // Referência ao jogador para seguir seu X e Y
    public float speed = 6.0f; // Velocidade com que o inimigo se aproxima no eixo Z
    public float originalSpeed; // Guarda a velocidade original para restauração
    public float followDistance = 10.0f; // Distância que o inimigo deve manter atrás do jogador

    private PositionTracker positionTracker; // Referência ao PositionTracker
    public PlayerMovement playerController; // Referência ao controlador do jogador

    public int skillNumber = 0;
    public float skillCooldown = 5.0f; // Tempo em segundos entre cada habilidade
    public float lastScoreCheck = 0; // Última pontuação verificada para aumento de velocidade

    void Start()
    {
        positionTracker = FindObjectOfType<PositionTracker>();
        playerController = FindObjectOfType<PlayerMovement>();
        originalSpeed = speed;
        StartCoroutine(ActivateSkills()); // Inicia a corrotina
    }

    void Update()
    {
        if (positionTracker != null)
        {
            // Movimentação padrão do inimigo
            float targetZ = playerTransform.position.z - followDistance;
            float newZPosition = Mathf.MoveTowards(transform.position.z, targetZ, speed * Time.deltaTime);
            transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, newZPosition);

            // Ajuste de velocidade baseado na pontuação
            float currentScore = instance.scoreManager.GetScore(); // Obter a pontuação como um float
            if (Mathf.Floor(currentScore / 500) > Mathf.Floor(lastScoreCheck / 500))
            {
                originalSpeed += 1f; // Aumenta a velocidade original a cada 500 pontos
                speed = originalSpeed; // Atualiza a velocidade atual
                lastScoreCheck = currentScore; // Atualiza a pontuação de referência para a base atual de 500 pontos
            }

            // Verificação para resetar a velocidade se a habilidade 6 estiver ativa e a condição for satisfeita
            if (skillNumber == 6 && Vector3.Distance(transform.position, playerTransform.position) <= 5)
            {
                speed = originalSpeed; // Reset the speed when close enough
            }
        }
    }
    IEnumerator ActivateSkills()
    {
        while (true)
        {
            ActivateSkill();
            yield return new WaitForSeconds(skillCooldown);
        }
    }

    void ActivateSkill()
    {
        skillNumber = Random.Range(1, 5);
        if (positionTracker.sectionDistance > 5)
        {
            skillNumber = 6;
        }

        switch (skillNumber)
        {
            case 1:
                StartCoroutine(DoubleSpeed());
                break;
            case 2:
                StartCoroutine(LockPlayerSkills());
                break;
            case 3:
                StartCoroutine(IncreasePlayerSpeed());
                break;
            case 4:
                StartCoroutine(DecreasePlayerSpeed());
                break;
            case 6:
                StartCoroutine(TemporarySpeedIncrease());
                break;
        }
    }

    IEnumerator TemporarySpeedIncrease()
    {
        float increasedSpeed = speed * 5;
        speed = increasedSpeed;
        while (positionTracker.sectionDistance > 5)
        {
            yield return null;
        }
        speed = originalSpeed;
        Debug.Log("Speed returned to normal as distance <= 5");
    }

    IEnumerator DoubleSpeed()
    {
        speed *= 2;
        Debug.Log("Inimigo usou habilidade: Aceleração!");
        yield return new WaitForSeconds(5);
        speed = originalSpeed;
    }

    IEnumerator LockPlayerSkills()
    {
        playerController.ToggleBlock();
        Debug.Log("Inimigo usou habilidade: Bloqueio!");
        yield return new WaitForSeconds(5);
        playerController.ToggleBlock();
    }

    IEnumerator IncreasePlayerSpeed()
    {
        instance.currentSpeed *= 1.25f;
        Debug.Log("Inimigo usou habilidade: Acelerar Jogador!");
        yield return new WaitForSeconds(5);
    }

    IEnumerator DecreasePlayerSpeed()
    {
        instance.currentSpeed *= 0.75f;
        Debug.Log("Inimigo usou habilidade: Desacelerar Jogador!");
        yield return new WaitForSeconds(5);
    }
}
