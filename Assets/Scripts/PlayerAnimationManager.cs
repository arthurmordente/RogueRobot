using UnityEngine;
using System.Collections;

    public class PlayerAnimationManager : MonoBehaviour
{
    public Animator anim;
    private bool deathAnimationPlayed = false;
    private Transform playerTransform;
    public PlayerMovement player;
    private float slideDuration = 1.0f; // Ajuste conforme necessário
    private float originalY;  // Armazenar a posição Y original de forma segura

    private bool isSliding = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
        playerTransform = transform; // Assume que o script está anexado ao GameObject do jogador
        originalY = playerTransform.position.y;  // Guarda a posição Y original ao acordar
    }

    public void PlayJumpAnimation()
    {
        anim.SetTrigger("Jump");
    }

    public void PlaySlideAnimation()
    {
        StartCoroutine(SlideMovementRoutine());
        anim.SetTrigger("Slide");
    }

    public void SetDeathAnimation(bool isDead)
    {
        anim.SetBool("Death", isDead);
        if (isDead)
        {
            deathAnimationPlayed = true;
        }
    }

    public bool IsDeathAnimationFinished()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName("Death") && stateInfo.normalizedTime >= 1.0f;
    }

    public bool DeathAnimationPlayed
    {
        get { return deathAnimationPlayed; }
        set { deathAnimationPlayed = value; }
    }

    IEnumerator SlideMovementRoutine()
    {
        isSliding = true;
        float loweredY = originalY - 2.0f; // Usa a altura original armazenada
        float elapsedTime = 0;

        // Movimento para baixo
        while (elapsedTime < slideDuration / 2)
        {
            float newY = Mathf.Lerp(originalY, loweredY, elapsedTime / (slideDuration / 2));
            playerTransform.position = new Vector3(playerTransform.position.x, newY, playerTransform.position.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        playerTransform.position = new Vector3(playerTransform.position.x, loweredY, playerTransform.position.z);
        yield return new WaitForSeconds(slideDuration / 2);

        // Movimento para cima
        elapsedTime = 0;
        while (elapsedTime < slideDuration / 2)
        {
            float newY = Mathf.Lerp(loweredY, originalY, elapsedTime / (slideDuration / 2));
            playerTransform.position = new Vector3(playerTransform.position.x, newY, playerTransform.position.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Assegura que a posição final seja exatamente a original
        playerTransform.position = new Vector3(playerTransform.position.x, originalY, playerTransform.position.z);
        isSliding = false;
    }


    public float GetSlideDuration()
    {
        return slideDuration;
    }
    void LateUpdate()
    {
        // Corrige qualquer discrepância na posição Y
        if (!Mathf.Approximately(playerTransform.position.y, originalY) && !isSliding && !player.isJumping && !player.isDead)
        {
            playerTransform.position = new Vector3(playerTransform.position.x, originalY, playerTransform.position.z);
        }
    }

}
