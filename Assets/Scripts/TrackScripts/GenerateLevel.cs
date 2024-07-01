using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateLevel : MonoBehaviour
{
    public GameObject[] section;
    public int zPos = 75;
    public bool creatingSec = false;
    public int secIndex;
    public Transform jogadorTransform;
    void Start()
    {
        zPos = 75;
    }

    // Update is called once per frame
    void Update()
    {
        if(creatingSec == false)
        {
            creatingSec = true;
            StartCoroutine(GenerateSection());
        }
    }



    IEnumerator GenerateSection()
    {
        secIndex = Random.Range(0, section.Length);
        GameObject sec = Instantiate(section[secIndex], new Vector3(0,0,zPos), Quaternion.identity);
        DestroyObject scriptDaSec = sec.GetComponent<DestroyObject>();
        scriptDaSec.playerPos = jogadorTransform;
        zPos += 50;
        yield return new WaitForSeconds(2);
        creatingSec = false;
    }
}
