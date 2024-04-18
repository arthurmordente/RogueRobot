using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameManager instance;
    public float initialSpeed = 3.0f;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        speed = initialSpeed * instance.currentSpeed;
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}
