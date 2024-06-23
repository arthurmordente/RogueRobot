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
    public string conditionType; // Tipo de condição para desbloqueio (por exemplo, "score", "levelCompleted")
    public int targetValue; // Valor-alvo necessário para desbloquear a conquista
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

    void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, "achievements.json");
        LoadAchievements();
    }

    // Método para acessar a lista de conquistas
    public List<Achievement> GetAchievements()
    {
        return achievementsData.achievements;
    }

    public void CheckAndUnlockAchievements(string conditionType, int currentValue)
    {
        bool updated = false;
        foreach (Achievement achievement in achievementsData.achievements)
        {
            if (!achievement.unlocked && achievement.conditionType == conditionType && currentValue >= achievement.targetValue)
            {
                achievement.unlocked = true;
                Debug.Log($"Achievement Unlocked: {achievement.name}");
                updated = true;
            }
        }
        if (updated) SaveAchievements();
    }

    public void CheckAndUnlockAchievements(string conditionType, float currentValue)
    {
        bool updated = false;
        foreach (Achievement achievement in achievementsData.achievements)
        {
            if (!achievement.unlocked && achievement.conditionType == conditionType && currentValue >= achievement.targetValue)
            {
                achievement.unlocked = true;
                Debug.Log($"Achievement Unlocked: {achievement.name}");
                updated = true;
            }
        }
        if (updated) SaveAchievements();
    }

    public void UnlockAchievement(string achievementName)
    {
        Achievement achievement = achievementsData.achievements.Find(a => a.name == achievementName);
        if (achievement != null && !achievement.unlocked)
        {
            achievement.unlocked = true;
            SaveAchievements();
            Debug.Log($"Achievement Unlocked: {achievement.name}");
        }
    }

    private void LoadAchievements()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            achievementsData = JsonUtility.FromJson<AchievementsData>(json);
            Debug.Log("Achievements loaded.");
        }
        else
        {
            achievementsData = new AchievementsData(); // Inicia com uma lista vazia
            achievementsData.achievements = new List<Achievement>();
            Debug.Log("No achievements file found, started with an empty list.");
        }
    }

    public void SaveAchievements()
    {
        string json = JsonUtility.ToJson(achievementsData, true);
        File.WriteAllText(filePath, json);
        Debug.Log("Achievements saved.");
    }


    public void AddAchievement(string name, string description, string iconPath)
    {
        if (Resources.Load<Sprite>(iconPath) == null)
        {
            Debug.LogError("Ícone de conquista não encontrado em: " + iconPath);
            return;
        }

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

    public void ResetAchievements()
    {
        foreach (Achievement achievement in achievementsData.achievements)
        {
            achievement.unlocked = false;  // Resetar o estado para não desbloqueado
        }
        SaveAchievements();  // Salvar as mudanças no arquivo
        Debug.Log("All achievements have been reset.");
    }

}
