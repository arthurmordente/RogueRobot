using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Variáveis do jogador
    public float[] lanes = new float[3] { -6.66f, 0f, 6.66f };
    public int _currentLane = 0;
    public float speed = 5.0f; // Velocidade do jogador
    public GameManager instance;

    // Variáveis de movimento
    public float jumpHeight = 3.5f; // Altura do pulo ajustada para 1.5 unidades
    public float initialJumpSpeed = 4.0f; // Velocidade do pulo ajustada para 10 unidades por segundo
    public float initialSlideDuration = 1.0f; // Duração do deslize ajustada para 1 segundo
    public float jumpSpeed = 4.0f; // Velocidade do pulo ajustada para 10 unidades por segundo
    public float slideDuration = 1.0f; // Duração do deslize ajustada para 1 segundo

    // Estado do jogador
    private bool isSliding = false;
    private bool isJumping = false;
    private bool isInvulnerable = false; // Variável de controle de invulnerabilidade

    void Start()
    {
        _currentLane = 1;
    }

    void Update()
    {
        // Atualização da velocidade de pulo e duração do deslize
        jumpSpeed = initialJumpSpeed * instance.currentSpeed;
        slideDuration = initialSlideDuration / instance.currentSpeed;

        // Verifica se o jogador não está pulando, deslizando ou invulnerável
        if (Time.timeScale != 0)
        {
            // Verifica as teclas pressionadas
            if (Input.GetKeyDown(KeyCode.A))
            {
                MoverEsquerda();             
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                MoverDireita();
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                Pular();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                Slide();
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                instance.dilateTime();
            }
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
            float newY = Mathf.MoveTowards(transform.position.y, targetY, (jumpSpeed) * Time.deltaTime); // Ajuste da velocidade do pulo
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            yield return null;
        }

        while (transform.position.y > originalY)
        {
            float newY = Mathf.MoveTowards(transform.position.y, originalY, jumpSpeed * Time.deltaTime); // Ajuste da velocidade do pulo
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

    public IEnumerator AtivarInvulnerabilidade(float invulnerabilityTime)
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityTime);
        isInvulnerable = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            Destroy(other.gameObject); // Destroi a moeda
            instance.CollectObj(3);
        }
        else if (!isInvulnerable)
        {
            if (other.CompareTag("Enemy")) // Verifica se o jogador está invulnerável
            {
                Destroy(other.gameObject);
                instance.GameOver();
            }
            else if (other.CompareTag("Wall")) // Verifica se o jogador está invulnerável
            {
                instance.GameOver();
            }
        }
    }
}
