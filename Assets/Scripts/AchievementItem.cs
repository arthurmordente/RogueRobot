using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class AchievementItem : MonoBehaviour, IPointerClickHandler
{
    public TMP_Text descriptionText;  // Componente Text de descrição, configurado no Inspector
    public TMP_Text titleText;        // Componente Text do título do achievement, configurado no Inspector
    public Image icon;                // Componente Image para o ícone, configurado no Inspector
    public string description;        // Descrição da conquista

    private void Start()
    {
        // Garante que os textos estão inicialmente ocultos
        descriptionText.enabled = false;
        titleText.enabled = true;  // Normalmente visível, mas a opacidade será ajustada
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Alterna a visibilidade do texto de descrição a cada toque
        descriptionText.enabled = !descriptionText.enabled;
        descriptionText.text = descriptionText.enabled ? description : "";
    }

    public void Initialize(Achievement achievement)
    {
        if (achievement == null)
        {
            Debug.LogError("Erro: o objeto achievement passado para Initialize é null.");
            return;
        }

        // Carrega o sprite do ícone da conquista e configura a descrição
        Sprite iconSprite = Resources.Load<Sprite>(achievement.iconPath);
        if (iconSprite != null)
        {
            icon.sprite = iconSprite;
        }
        else
        {
            Debug.LogError("Falha ao carregar o sprite do ícone: " + achievement.iconPath);
        }

        this.description = achievement.description;
        titleText.text = achievement.name;

        // Configura a opacidade de acordo com o estado de desbloqueio
        float alpha = achievement.unlocked ? 1.0f : 0.5f;
        icon.color = new Color(1, 1, 1, alpha);
        titleText.color = new Color(1, 1, 1, alpha);
    }
}
