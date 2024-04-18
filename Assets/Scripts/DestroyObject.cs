using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    public Transform playerPos; // Referência para o jogador
    public float destroyOffset = 25.0f; // Distância para destruir o objeto após o jogador passar por ele
    public float delayBeforeDestroyCheck = 2.0f; // Atraso antes de começar a verificar a condição de destruição
    private bool shouldCheckForDestroy = false;

    void Start()
    {
        Invoke(nameof(EnableDestroyCheck), delayBeforeDestroyCheck);
    }

    void Update()
    {
        if (shouldCheckForDestroy && transform.position.z < (playerPos.position.z - destroyOffset))
        {
            // Debug.Log("A posição do jogador é: " + playerPos.position.z);
            // Debug.Log("A posição do objeto é: " + transform.position.z);
            
            Destroy(gameObject); // Destroi o objeto
        }
    }

    void EnableDestroyCheck()
    {
        shouldCheckForDestroy = true;
    }
}
