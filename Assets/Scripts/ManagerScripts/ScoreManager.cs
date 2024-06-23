using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

[Serializable]
public class ScoreEntry
{
    public float score;
    public string playerName;
    public float multiplier;
}

[Serializable]
public class ScoreData
{
    public List<ScoreEntry> topScores = new List<ScoreEntry>();
}

public class ScoreManager : MonoBehaviour
{
    public GameManager instance;
    public AudioManager audioManager;
    public AchievementManager achv;
    public TMP_Text[] topScoreTexts; // Referências para os elementos de UI dos top scores
    public TMP_Text scoreText;
    public TMP_InputField playerNameInput; // Campo de entrada para o nome do jogador
    public GameObject scoreScreen; // Tela para exibir os top scores
    public GameObject getInputScreen; // Tela para capturar o nome do jogador
    
    private float score = 0; // Score atual
    private ScoreData scoreData;
    private string filePath;

    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "scoreData.json");
        LoadScores();
    }

    private void LoadScores()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            scoreData = JsonUtility.FromJson<ScoreData>(json);
        }
        else
        {
            scoreData = new ScoreData(); // Inicia um novo conjunto de dados de pontuação se o arquivo não existir
        }
    }

    private void SaveScores()
    {
        string json = JsonUtility.ToJson(scoreData, true);
        File.WriteAllText(filePath, json);
    }

    public void AddPoints(float value)
    {
        score += value;
        scoreText.text = "Score: " + score.ToString("F0");
        achv.CheckAndUnlockAchievements("score", score);

    }

    public float GetScore(){
        return score;
    }

    public void UpdateTopScores()
    {
        ScoreEntry newEntry = new ScoreEntry { score = score, playerName = "", multiplier = instance.scoreMultiplier };

        // Determina se o novo score é um high score
        bool newHighScore = false;
        int newScoreIndex = -1;

        // Percorre a lista de scores existente para encontrar a posição correta do novo score
        for (int i = 0; i < scoreData.topScores.Count; i++)
        {
            if (score > scoreData.topScores[i].score)
            {
                newHighScore = true;
                newScoreIndex = i;
                break;
            }
        }

        // Se for um novo high score, inserimos na posição correta
        if (newHighScore)
        {
            audioManager.PlayAudio3();
            // Se há espaço na lista ou se é necessário substituir o menor score
            if (scoreData.topScores.Count < 3) // Supondo que você deseja manter os 3 melhores scores
            {
                scoreData.topScores.Insert(newScoreIndex, newEntry);
            }
            else
            {
                // Desloca os scores para baixo para fazer espaço para o novo score
                for (int j = scoreData.topScores.Count - 1; j > newScoreIndex; j--)
                {
                    scoreData.topScores[j] = scoreData.topScores[j - 1];
                }
                scoreData.topScores[newScoreIndex] = newEntry;
            }

            // Ativa a tela para capturar o nome do jogador
            getInputScreen.SetActive(true);
            playerNameInput.Select();
            playerNameInput.ActivateInputField();

            playerNameInput.onEndEdit.RemoveAllListeners();
            playerNameInput.onEndEdit.AddListener(delegate { OnEndEditPlayerName(score, newScoreIndex); });
        }
        else if (scoreData.topScores.Count < 3) // Caso não seja maior, mas ainda há espaço na lista
        {
            audioManager.PlayAudio3();
            scoreData.topScores.Add(newEntry);
            getInputScreen.SetActive(true);
            playerNameInput.Select();
            playerNameInput.ActivateInputField();

            playerNameInput.onEndEdit.RemoveAllListeners();
            playerNameInput.onEndEdit.AddListener(delegate { OnEndEditPlayerName(score, scoreData.topScores.Count - 1); });
        }
        else
        {
            // Não é um high score e a lista está cheia
            instance.displayLoseScreen();
            audioManager.PlayAudio2();
        }
    }

    public void DisplayTopScores()
    {
        for (int i = 0; i < topScoreTexts.Length; i++)
        {
            if (i < scoreData.topScores.Count)
            {
                topScoreTexts[i].text = scoreData.topScores[i].playerName + " - " + scoreData.topScores[i].score.ToString("F0");
            }
        }
        scoreScreen.SetActive(true);
    }

    private void OnEndEditPlayerName(float newScore, int newScoreIndex)
    {
        string playerName = playerNameInput.text.ToUpper();

        if (playerName.Length == 3)
        {
            scoreData.topScores[newScoreIndex].playerName = playerName;
            SaveScores();
            Debug.Log("Novos top Scores foram salvos.");
        }
        else
        {
            Debug.LogError("O nome do jogador deve ter exatamente 3 letras. Tente novamente.");
        }

        getInputScreen.SetActive(false);
        playerNameInput.text = ""; // Limpa o campo de entrada após o uso
        instance.displayLoseScreen();
    }

    public void ResetScores()
    {
        // Limpa a lista de top scores
        scoreData.topScores.Clear();
        
        // Salva a lista vazia no arquivo JSON para persistir a remoção
        SaveScores();
        
        Debug.Log("Todos os scores foram resetados.");

        // Atualiza a interface do usuário para refletir a lista vazia
        foreach (TMP_Text scoreText in topScoreTexts)
        {
            scoreText.text = "---";
        }
    }

}
