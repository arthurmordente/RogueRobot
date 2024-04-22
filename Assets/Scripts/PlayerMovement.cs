using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Variáveis de configuração
    public float[] lanes = new float[3] { -6.66f, 0f, 6.66f };
    public int _currentLane = 0;
    public float speed = 5.0f; // Velocidade do jogador
    public GameManager instance;

    // Variáveis de movimento
    public float jumpHeight = 3.5f;
    public float initialJumpSpeed = 4.0f;
    public float initialSlideDuration = 1.0f;
    public float jumpSpeed = 4.0f;
    public float slideDuration = 1.0f;

    // Estados do jogador
    private bool isSliding = false;
    private bool isJumping = false;
    public bool isInvulnerable = false;
    public bool isWounded = false;
    private float woundRecoveryTime = 15.0f; // Tempo para se recuperar do estado ferido
    private float invulnerabilityTime = 3.0f; // Tempo de invulnerabilidade após ser atingido

    private Renderer playerRenderer;

    void Start()
    {
        _currentLane = 1; // Configuração inicial da pista
        playerRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        UpdateMovementSpeed();
        HandleInput();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isInvulnerable) // Primeiro, verifica se o jogador não está invulnerável
        {
            if (other.CompareTag("Enemy")) // Então, verifica se houve colisão com um objeto do tipo Enemy
            {
                HandleEnemyCollision();
                Destroy(other.gameObject); // Destruir o inimigo após colisão
            }
            else if (other.CompareTag("Wall")) // Verifica se houve colisão com um objeto do tipo Wall
            {
                HandleEnemyCollision(); 
            }
        }

        if (other.CompareTag("Coin")) // A colisão com moedas ocorre independentemente do estado de invulnerabilidade
        {
            Destroy(other.gameObject); // Destrói a moeda
            instance.CollectObj(3); // Adicionar pontos ou moedas ao total
        }
    }


    private void HandleEnemyCollision()
    {
        if (!isWounded)
        {
            // Se não está ferido, torna-se ferido e inicia a recuperação
            instance.audioManager.PlayAudio5();
            isWounded = true;
            StartCoroutine(WoundRecoveryTimer(woundRecoveryTime));
            StartCoroutine(ActivateInvulnerability(invulnerabilityTime)); // Ativa a invulnerabilidade
        }
        else
        {
            // Se já está ferido e é atingido novamente, o jogo acaba
            instance.GameOver();
        }
    }

    private IEnumerator WoundRecoveryTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        isWounded = false; // Recuperação completa, sai do estado ferido
    }

    private IEnumerator ActivateInvulnerability(float duration)
    {
        isInvulnerable = true;
        StartCoroutine(BlinkEffect(duration, 0.2f));
        yield return new WaitForSeconds(duration);
        isInvulnerable = false;
    }

    private IEnumerator BlinkEffect(float duration, float blinkInterval)
    {
        float endTime = Time.time + duration;
        while (Time.time < endTime)
        {
            if(playerRenderer != null){
                playerRenderer.enabled = !playerRenderer.enabled; // Alterna a visibilidade
            }
            yield return new WaitForSeconds(blinkInterval); // Espera pelo intervalo de piscar
        }
        if(playerRenderer != null){
            playerRenderer.enabled = true;
        }
    }

    private void UpdateMovementSpeed()
    {
        // Atualiza a velocidade baseada na velocidade do jogo
        jumpSpeed = initialJumpSpeed * instance.currentSpeed;
        slideDuration = initialSlideDuration / instance.currentSpeed;
    }

    private void HandleInput()
    {
        if (Time.timeScale != 0) // Se o jogo não está pausado
        {
            if (Input.GetKeyDown(KeyCode.A)) MoverEsquerda();
            if (Input.GetKeyDown(KeyCode.D)) MoverDireita();
            if (Input.GetKeyDown(KeyCode.W)) Pular();
            if (Input.GetKeyDown(KeyCode.S)) Slide();
            if (Input.GetKeyDown(KeyCode.E)) instance.dilateTime();
        }
    }

    public void MoverDireita()
    {
        _currentLane++;
        if (_currentLane >= lanes.Length)
        {
            _currentLane = lanes.Length - 1;
        }

        float x = lanes[_currentLane];
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }

    public void MoverEsquerda()
    {
        _currentLane--;
        if (_currentLane < 0)
        {
            _currentLane = 0;
        }

        float x = lanes[_currentLane];
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }

    public void Pular()
    {
        if (!isJumping && !isSliding)
        {
            StartCoroutine(JumpRoutine());
        }
    }

    IEnumerator JumpRoutine()
    {
        isJumping = true;
        float originalY = transform.position.y;
        float targetY = originalY + jumpHeight;

        while (transform.position.y < targetY)
        {
            float newY = Mathf.MoveTowards(transform.position.y, targetY, jumpSpeed * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            yield return null;
        }

        while (transform.position.y > originalY)
        {
            float newY = Mathf.MoveTowards(transform.position.y, originalY, jumpSpeed * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            yield return null;
        }

        isJumping = false;
    }

    public void Slide()
    {
        if (!isSliding && !isJumping)
        {
            StartCoroutine(SlideRoutine());
        }
    }

    IEnumerator SlideRoutine()
    {
        isSliding = true;
        float originalY = transform.position.y;
        float targetY = originalY - 0.5f; // Diminui a altura do deslize

        transform.Rotate(90f, 0f, 0f); // Rotaciona o objeto 90 graus no eixo X

        while (transform.position.y > targetY)
        {
            float newY = Mathf.MoveTowards(transform.position.y, targetY, Time.deltaTime);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            yield return null;
        }

        yield return new WaitForSeconds(slideDuration);

        transform.Rotate(-90f, 0f, 0f); // Restaura a rotação original do objeto

        while (transform.position.y < originalY)
        {
            float newY = Mathf.MoveTowards(transform.position.y, originalY, Time.deltaTime);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            yield return null;
        }

        isSliding = false;
    }
}
