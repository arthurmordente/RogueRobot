using UnityEngine;
using UnityEngine.UI;

public class SpeedometerHUD : MonoBehaviour
{
    public Image pointer;
    public GameObject blocked;
    public float minPointerAngle = 145.0f;
    public float maxPointerAngle = -145.0f;
    public float minSpeed = 1.0f;
    public float maxSpeed = 6.0f;
    public Color[] accelerationColors; // Deve ter 5 cores correspondendo a [-0.2, -0.1, 0, 0.1, 0.2]

    private GameManager instance;

    void Start()
    {
        instance = FindObjectOfType<GameManager>(); // Obtenha a referência ao GameManager
    }

    void Update()
    {
        UpdatePointer();
    }

    private void UpdatePointer()
    {
        // Obter a velocidade atual e aceleração
        float speed = Mathf.Clamp(instance.GetCurrentSpeed(), minSpeed, maxSpeed);
        float normalizedSpeed = (speed - minSpeed) / (maxSpeed - minSpeed);
        float angle = Mathf.Lerp(minPointerAngle, maxPointerAngle, normalizedSpeed);
        pointer.transform.rotation = Quaternion.Euler(0, 0, angle);

        // Atualizar a cor do ponteiro baseada na aceleração
        float acceleration = instance.GetCurrentAcceleration();
        int colorIndex = (int)((acceleration + 0.2f) * 10); // Mapeia [-0.2, -0.1, 0, 0.1, 0.2] para [0, 1, 2, 3, 4]
        pointer.color = accelerationColors[colorIndex];
    }


    
    public void BlockHUD(){
        blocked.SetActive(!blocked.activeSelf);
    }

}
