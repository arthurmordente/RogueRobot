using UnityEngine;
using UnityEngine.UI;

public class DistanceHUD : MonoBehaviour
{
    public GameManager instance;
    public Slider distanceSlider;  // Referência ao Slider que atua como barra de distância
    public RectTransform fillRect; // RectTransform do componente de preenchimento do Slider

    public Image fillImage;
    public Color[] colors;


    void Start()
    {
        for (int i = 0; i < colors.Length; i++)
        {
            Color fullAlphaColor = colors[i];
            fullAlphaColor.a = 1f; // Força o alpha para 1
            colors[i] = fullAlphaColor;
        }
    }
    void Update()
    {
        // Obtenha a distância atual que varia de 1 a 6
        float currentDistance = instance.positionTracker.currentDistance;
        if (currentDistance >= 7){
            currentDistance = 6;
        }
        // Calcula a nova largura baseada nessa distância
        float newWidth = Mathf.Lerp(60f, 300f, currentDistance  / 6);

        // Ajusta o sizeDelta do fillRect para mudar a largura
        fillRect.sizeDelta = new Vector2(newWidth, fillRect.sizeDelta.y);


        int colorDistance = (int)currentDistance;

        if(colorDistance <= colors.Length){
            fillImage.color = colors[colorDistance];
        }
    }
}
