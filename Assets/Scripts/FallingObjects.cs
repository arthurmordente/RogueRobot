using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObjects : MonoBehaviour
{
    public float fallSpeed = 5.0f; // Velocidade de queda do objeto

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Move o objeto para baixo no eixo Y a cada frame, multiplicando a velocidade de queda pelo deltaTime para tornar o movimento suave e independente da taxa de quadros
        transform.position -= new Vector3(0, fallSpeed * Time.deltaTime, 0);
    }
}
