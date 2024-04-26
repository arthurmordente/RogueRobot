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
    private Vector2 touchStart;
    private float touchStartTime;
    bool doubleTapRegistered;
    private float lastTapTime = 0;
    private const float doubleTapDelta = 0.3f;
    private bool isTapLong = false;


    void Start()
    {
        _currentLane = 1; // Configuração inicial da pista
        playerRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        UpdateMovementSpeed();
        HandleMobileInput();
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

    private void HandleMobileInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStart = touch.position;
                    touchStartTime = Time.time;
                    
                    // Reseta o flag de double tap se o tempo entre toques for maior que o permitido para double taps
                    if (Time.time - lastTapTime > doubleTapDelta)
                    {
                        doubleTapRegistered = false;
                        isTapLong = false;  // Reseta o estado de tap longo
                    }
                    break;
                case TouchPhase.Ended:
                    if (!doubleTapRegistered && !isTapLong) // Processa como swipe ou tap se não houve double tap ou tap longo anterior
                    {
                        float duration = Time.time - touchStartTime;
                        float distance = (touch.position - touchStart).magnitude;

                        if (duration >= 0.3f && distance < 50f) // Condições para tap longo
                        {
                            Acelerar();
                            isTapLong = true; // Marca que um tap longo foi detectado
                        }
                        else if (duration < 0.3f && distance < 50f) // Condições para double tap
                        {
                            if (Time.time - lastTapTime < doubleTapDelta)
                            {
                                Desacelerar();
                                doubleTapRegistered = true;
                                lastTapTime = 0;
                            }
                            else
                            {
                                lastTapTime = Time.time;
                            }
                        }
                        else
                        {
                            ProcessSwipeOrTap(touch.position); // Chama a função de processamento de swipe
                        }
                    }
                    break;
            }
        }
    }

    private void ProcessSwipeOrTap(Vector2 touchEnd)
    {
        if (!isTapLong)  // Só processa swipes se não estiver em um estado de tap longo
        {
            float x = touchEnd.x - touchStart.x;
            float y = touchEnd.y - touchStart.y;

            if (Mathf.Abs(x) > Mathf.Abs(y))
            {
                if (x > 0) MoverDireita();
                else MoverEsquerda();
            }
            else
            {
                if (y > 0) Pular();
                else Slide();
            }
        }
    }

    public void Acelerar()
    {
        // Implement the logic for acceleration here
        Debug.Log("Acelerando");
    }

    public void Desacelerar()
    {
        // Implement the logic for deceleration here
        Debug.Log("Desacelerando");
    }
    
    private void MoverDireita()
    {
        _currentLane++;
        if (_currentLane >= lanes.Length)
        {
            _currentLane = lanes.Length - 1;
        }

        float x = lanes[_currentLane];
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }

    private void MoverEsquerda()
    {
        _currentLane--;
        if (_currentLane < 0)
        {
            _currentLane = 0;
        }

        float x = lanes[_currentLane];
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }

    private void Pular()
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

    private void Slide()
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
