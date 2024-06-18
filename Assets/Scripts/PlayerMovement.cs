using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Variáveis de configuração
    public float[] lanes = new float[3] { -6.66f, 0f, 6.66f };
    public int _currentLane = 0;
    public float speed = 5.0f; // Velocidade do jogador
    public GameManager instance;
    public PositionTracker posTracker;
    public SpeedometerHUD velocimeter;

    // Variáveis de movimento
    public float jumpHeight = 3.5f;
    public float initialJumpSpeed = 4.0f;
    public float initialSlideDuration = 1.0f;
    public float jumpSpeed = 4.0f;
    public float slideDuration = 1.0f;

    //Váriaveis de animação
    private Animator anim;

    // Estados do jogador
    private bool isSliding = false;
    private bool isJumping = false;
    public bool isInvulnerable = false;
    public bool isWounded = false;
    public bool isBlocked = false;
    public bool isDead = false;
    private float woundRecoveryTime = 15.0f; // Tempo para se recuperar do estado ferido
    private float invulnerabilityTime = 3.0f; // Tempo de invulnerabilidade após ser atingido

    [SerializeField] private Renderer[] playerRenderer;
    private Vector2 touchStart;
    private float touchStartTime;
    bool doubleTapRegistered;
    private float lastTapTime = 0;
    private const float doubleTapDelta = 0.3f;
    private bool isTapLong = false;


    void Start()
    {
        _currentLane = 1; // Configuração inicial da pista
        isDead = false;
        playerRenderer = GetComponentsInChildren<Renderer>();
    }

    void Update()
    {
        if(!isDead){
        UpdateMovementSpeed();
        HandleMobileInput();
        }
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
            posTracker.UpdateLastHitSection();
            instance.Reduce(0.25f);
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
            foreach (Renderer rend in playerRenderer)
            {
                if(rend != null){
                    rend.enabled = !rend.enabled; // Alterna a visibilidade
                }
                
                yield return new WaitForSeconds(blinkInterval); // Espera pelo intervalo de piscar
            }
        }
        foreach (Renderer rend in playerRenderer)
        {
            if(rend != null){
                rend.enabled = true;
            }   
        }
    }

    private void UpdateMovementSpeed()
    {
        // Atualiza a velocidade baseada na velocidade do jogo
        jumpSpeed = initialJumpSpeed * instance.currentSpeed;
        slideDuration = initialSlideDuration / instance.currentSpeed;
    }
    
    /*private void HandleInput()
    {
        if (Time.timeScale != 0) // Se o jogo não está pausado
        {
            if (Input.GetKeyDown(KeyCode.A)) MoverEsquerda();
            if (Input.GetKeyDown(KeyCode.D)) MoverDireita();
            if (Input.GetKeyDown(KeyCode.W)) Pular();
            if (Input.GetKeyDown(KeyCode.S)) Slide();
            if (Input.GetKeyDown(KeyCode.E)) instance.dilateTime();
        }
    }*/

    private void HandleMobileInput()
    {   
        if (Input.touchCount == 5)
        {
            // UNTESTED FEATURE DUE TO MAIN DEVELOPER'S LACK OF NECESSARY HARDWARE
            bool allTouchesBegan = true;
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase != TouchPhase.Began)
                {
                    allTouchesBegan = false;
                    break;
                }
            }
            
            if (allTouchesBegan)
            {
                Cheat();
                return; 
            }
        }


        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) {
                return; // Ignora o toque se estiver sobre um objeto de UI
            }
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
                            if(instance.isPaused == false){
                                ProcessSwipeOrTap(touch.position); // Chama a função de processamento de swipe
                            }
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

    public void ToggleBlock()
        {
            isBlocked = !isBlocked; // Alterna o estado de bloqueio
            velocimeter.BlockHUD();
            Debug.Log("Block status changed: " + (isBlocked ? "Blocked" : "Unblocked"));
        }


    public void Acelerar()
    {
        if (isBlocked) // Verifica se o bloqueio está ativo
        {
            Debug.Log("Acceleration blocked!");
            return;
        }
        Debug.Log("Acelerando");
        instance.Accelerate();
    }

    public void Desacelerar()
    {
        if (isBlocked) // Verifica se o bloqueio está ativo
        {
            Debug.Log("Deceleration blocked!");
            return;
        }
        Debug.Log("Desacelerando");
        instance.Reduce();
    }

    private void Cheat()
    {
        Debug.Log("Cheat activated!");
        StartCoroutine(ActivateInvulnerability(300)); // Ativa a invulnerabilidade
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
            //anim.SetBool("Pulando", true);     //Animação de pular(caio)
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
            //anim.SetBool("Slide", true);        //Animação de slide(caio)
        }
    }

    IEnumerator SlideRoutine()
{
    isSliding = true;
    float originalY = transform.position.y;
    float targetY = originalY - 0.15f; // Diminui a altura do deslize

    // Rotaciona o objeto 90 graus no eixo X
    transform.Rotate(-90f, 0f, 0f);

    // Duração fixa para mover para a posição de deslizamento
    float slideDownDuration = 0.25f; // Ajuste conforme necessário
    float slideTimer = 0f;

    while (slideTimer < slideDownDuration)
    {
        slideTimer += Time.deltaTime;
        float newY = Mathf.Lerp(originalY, targetY, slideTimer / slideDownDuration);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        yield return null;
    }

    // Espera pela duração do deslize sem alterar a posição Y
    yield return new WaitForSeconds(slideDuration);

    // Duração fixa para retornar à posição original
    float slideUpDuration = 0.25f; // Ajuste conforme necessário
    slideTimer = 0f;

    while (slideTimer < slideUpDuration)
    {
        slideTimer += Time.deltaTime;
        float newY = Mathf.Lerp(targetY, originalY, slideTimer / slideUpDuration);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        yield return null;
    }

    // Restaura a rotação original do objeto
    transform.Rotate(90f, 0f, 0f);

    isSliding = false;
}
}
