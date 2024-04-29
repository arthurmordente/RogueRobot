using UnityEngine;

public class PedestrianTrackScript : MonoBehaviour
{
    public Transform[] childObjects; // Array para manter referências aos objetos filhos

    void Start()
    {
        ArrangeChildren(); // Chame esta função no Start para arranjar os filhos quando o prefab for instanciado
    }

    void ArrangeChildren()
    {
        // Define espaçamentos específicos ao longo do eixo z
        float[] zPositions = { -0.25f, 0f, 0.25f };

        // Garantir que temos exatamente três filhos para posicionar
        if (childObjects.Length == zPositions.Length)
        {
            for (int i = 0; i < childObjects.Length; i++)
            {
                // Gera uma posição x aleatória entre -0.4 e 0.4
                float randomX = Random.Range(-0.4f, 0.4f);

                // Posicione cada filho com uma posição z definida e x aleatório
                childObjects[i].localPosition = new Vector3(randomX, 2, zPositions[i]);
            }
        }
        else
        {
            Debug.LogWarning("Número de objetos filhos e posições de z não correspondem.");
        }
    }
}
