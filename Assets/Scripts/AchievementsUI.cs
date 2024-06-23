using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class AchievementsUI : MonoBehaviour
{
    public GameObject achievementPrefab;
    public Transform contentPanel;
    public  AchievementManager achievementManager;

    private void Start()
    {
        if (achievementManager == null)
            achievementManager = FindObjectOfType<AchievementManager>();  // Encontra automaticamente o AchievementManager na cena

        if (achievementManager == null)
        {
            Debug.LogError("AchievementManager não foi encontrado!");
            return; // Previne a execução adicional se não encontrar o manager
        }

        DisplayAchievements();  // Separa a lógica de exibição em um método separado
    }

    public void DisplayAchievements()
    {
        ClearAchievementDisplay();
        List<Achievement> achievements = achievementManager.GetAchievements();
        if (achievements == null)
        {
            Debug.LogError("Não há conquistas para mostrar.");
            return;
        }

        foreach (Achievement achievement in achievements)
        {
            GameObject item = Instantiate(achievementPrefab, contentPanel);
            TMP_Text achievementText = item.GetComponentInChildren<TMP_Text>();  // Certifique-se de que há um componente Text no prefab
            AchievementItem achievementItemScript = item.GetComponent<AchievementItem>();
            achievementItemScript.Initialize(achievement);
            if (achievementText != null)
                achievementText.text = achievement.name;
            else
                Debug.LogError("Componente Text não encontrado no prefab de conquista.");
        }
    }

    private void ClearAchievementDisplay()
    {
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);  // Destruir todos os filhos do painel de conteúdo
        }
    }
}

