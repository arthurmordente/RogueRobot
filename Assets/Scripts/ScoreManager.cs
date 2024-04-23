using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public GameManager instance;
    public AudioManager audioManager;
    public TMP_Text[] topScoreTexts; // Referências para os elementos de UI dos top scores
    public TMP_Text scoreText; // Referência para o elemento de UI do score atual
    public TMP_InputField playerNameInput; // Campo de entrada para o nome do jogador
    public GameObject scoreScreen; // Tela para exibir os top scores
    public GameObject getInputSceen; // Tela para capturar o nome do jogador
    
    private float score = 0; // Score atual

    public void AddPoints(float value)
    {
        score += value;
        scoreText.text = "Score: " + score.ToString("F0");
    }

    public void ResetScores()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save(); // Garante que a remoção seja aplicada imediatamente
        Debug.Log("PlayerPrefs foram resetados.");
    }

    public void UpdateTopScores()
    {
        float[] topScores = new float[3];
        string[] topPlayers = new string[3];
        float[] topMultipliers = new float[3]; // Nova matriz para armazenar os multiplicadores
        bool newHighScore = false;
        int newScoreIndex = -1;

        for (int i = 0; i < topScores.Length; i++)
        {
            topScores[i] = PlayerPrefs.GetFloat("TopScore" + i, 0);
            topPlayers[i] = PlayerPrefs.GetString("TopPlayer" + i, "---");
            topMultipliers[i] = PlayerPrefs.GetFloat("TopMultiplier" + i, 1); // Padrão é 1 se não houver nada salvo
        }

        for (int i = 0; i < topScores.Length; i++)
        {
            if (score > topScores[i])
            {
                newHighScore = true;
                newScoreIndex = i;
                break;
            }
        }

        if (newHighScore)
        {
            audioManager.PlayAudio3();
            for (int j = topScores.Length - 1; j > newScoreIndex; j--)
            {
                topScores[j] = topScores[j - 1];
                topPlayers[j] = topPlayers[j - 1];
                topMultipliers[j] = topMultipliers[j - 1];
            }

            topScores[newScoreIndex] = score;
            topMultipliers[newScoreIndex] = instance.scoreMultiplier; // Salva o multiplicador atual junto com o score
            getInputSceen.SetActive(true);
            playerNameInput.Select();
            playerNameInput.ActivateInputField();

            playerNameInput.onEndEdit.RemoveAllListeners();
            playerNameInput.onEndEdit.AddListener(delegate { OnEndEditPlayerName(score, newScoreIndex, topScores, topPlayers, topMultipliers); });
        }
        else
        {
            instance.displayLoseScreen();
            audioManager.PlayAudio2();
        }
    }

    public void DisplayTopScores()
    {
        for (int i = 0; i < 3; i++)
        {
            float score = PlayerPrefs.GetFloat("TopScore" + i, 0);
            string player = PlayerPrefs.GetString("TopPlayer" + i, "---");
            float multiplier = PlayerPrefs.GetFloat("TopMultiplier" + i, 1); // Recupera o multiplicador

            // Atualiza o texto para incluir o multiplicador
            topScoreTexts[i].text = player + " - " + score.ToString("F0") /*+ " - x" + multiplier.ToString("F2")*/;
        }
        scoreScreen.SetActive(true);
    }

    private void OnEndEditPlayerName(float newScore, int newScoreIndex, float[] topScores, string[] topPlayers, float[] topMultipliers)
    {
        string playerName = playerNameInput.text.ToUpper();

        if (playerName.Length == 3)
        {
            topPlayers[newScoreIndex] = playerName;
            PlayerPrefs.SetFloat("TopScore" + newScoreIndex, newScore);
            PlayerPrefs.SetString("TopPlayer" + newScoreIndex, playerName);
            PlayerPrefs.SetFloat("TopMultiplier" + newScoreIndex, topMultipliers[newScoreIndex]); // Salva o novo multiplicador

            for (int i = 0; i < topScores.Length; i++)
            {
                PlayerPrefs.SetFloat("TopScore" + i, topScores[i]);
                PlayerPrefs.SetString("TopPlayer" + i, topPlayers[i]);
                PlayerPrefs.SetFloat("TopMultiplier" + i, topMultipliers[i]); // Salva todos os multiplicadores
            }

            PlayerPrefs.Save();
            Debug.Log("Novos top Scores foram alcançados");
        }
        else
        {
            Debug.LogError("O nome do jogador deve ter exatamente 3 letras. Tente novamente.");
        }

        getInputSceen.SetActive(false);
        playerNameInput.text = ""; // Limpa o campo de entrada após o uso
        instance.displayLoseScreen();
    }
}
