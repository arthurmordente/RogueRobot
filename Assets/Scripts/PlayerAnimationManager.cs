using UnityEngine;
using System.Collections;

public class PlayerAnimationManager : MonoBehaviour
{
    public Animator anim;
    private bool deathAnimationPlayed = false;
    private Transform playerTransform;
    private float slideDuration = 1.0f; // Ajuste conforme necessário

    void Awake()
    {
        anim = GetComponent<Animator>();
        playerTransform = transform; // Assume que o script está anexado ao GameObject do jogador
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
        float originalY = playerTransform.position.y;
        float loweredY = originalY - 2.0f; // Ajuste a altura do deslize conforme necessário
        float elapsedTime = 0;

        // Movimento para baixo
        while (elapsedTime < slideDuration / 2)
        {
            float newY = Mathf.Lerp(originalY, loweredY, elapsedTime / (slideDuration / 2));
            playerTransform.position = new Vector3(playerTransform.position.x, newY, playerTransform.position.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Fixa a posição abaixada ao final do movimento para baixo
        playerTransform.position = new Vector3(playerTransform.position.x, loweredY, playerTransform.position.z);

        // Mantém a posição abaixada por um curto período
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

        // Fixa a posição original ao final do movimento para cima
        playerTransform.position = new Vector3(playerTransform.position.x, originalY, playerTransform.position.z);
    }


    public float GetSlideDuration()
    {
        return slideDuration;
    }
}
