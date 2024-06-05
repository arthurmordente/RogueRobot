using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject[] prefabs;
    public float[] lanes = new float[3]{-6.66f,0f,6.66f};
    public Transform jogadorTransform;

    
    void Start()
    {
        InvokeRepeating(nameof(InstanciarObjetos),0.1f,1.1f);
    }

    void Update()
    {
        
    }

    public void InstanciarObjetos(){
        int index = Random.Range(0, prefabs.Length);
        int randPos = Random.Range(0, lanes.Length);
        Vector3 pos = new Vector3(lanes[randPos], transform.position.y, transform.position.z);

        Quaternion rotation = Quaternion.identity;

        // Se index for 0, ajusta a rotação para 90 graus no eixo X
        if (index == 0) {
            rotation = Quaternion.Euler(90, 0, 0);
        }

        // Instancia o prefab com a posição e rotação definidas
        GameObject clone = Instantiate(prefabs[index], pos, rotation);
        DestroyObject scriptDoObjeto = clone.GetComponent<DestroyObject>();
        if(scriptDoObjeto != null){
            scriptDoObjeto.playerPos = jogadorTransform;
        }
    }
}
