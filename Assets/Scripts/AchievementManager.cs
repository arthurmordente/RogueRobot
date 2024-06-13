using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Achievement
{
    public string name;
    public string description;
    public string iconPath; // Caminho para o ícone no Resources
    public bool unlocked;
}

[Serializable]
public class AchievementsData
{
    public List<Achievement> achievements = new List<Achievement>();
}

public class AchievementManager : MonoBehaviour
{
    private AchievementsData achievementsData;
    private string filePath;

    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "achievements.json");
        if (string.IsNullOrEmpty(filePath))
        {
            Debug.LogError("O caminho do arquivo de conquistas não foi definido corretamente.");
            return; // Evita proceder se o caminho estiver vazio ou nulo.
        }
        LoadAchievements();
    }


    private void LoadAchievements()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            achievementsData = JsonUtility.FromJson<AchievementsData>(json);
        }
        else
        {
            achievementsData = new AchievementsData(); // Inicializa com uma lista vazia se não existir arquivo
        }
    }

    public void SaveAchievements()
    {
        if (string.IsNullOrEmpty(filePath))
        {
            Debug.LogError("Tentativa de salvar conquistas falhou: caminho do arquivo não especificado.");
            return;
        }

        string json = JsonUtility.ToJson(achievementsData, true);
        File.WriteAllText(filePath, json);
    }


    public void UnlockAchievement(string achievementName)
    {
        foreach (var achievement in achievementsData.achievements)
        {
            if (achievement.name == achievementName && !achievement.unlocked)
            {
                achievement.unlocked = true;
                SaveAchievements();
                Debug.Log($"Achievement unlocked: {achievementName}");
                break;
            }
        }
    }

    // Adiciona uma nova conquista ao sistema (pode ser chamado do editor ou durante o desenvolvimento)
    public void AddAchievement(string name, string description, string iconPath)
    {
        // Verificar se filePath foi configurado
        if (string.IsNullOrEmpty(filePath))
        {
            Debug.LogError("Não é possível adicionar conquistas: caminho do arquivo não definido.");
            return;
        }

        // Proceder com adição e salvamento
        Achievement newAchievement = new Achievement
        {
            name = name,
            description = description,
            iconPath = iconPath,
            unlocked = false
        };

        achievementsData.achievements.Add(newAchievement);
        SaveAchievements();
    }
}