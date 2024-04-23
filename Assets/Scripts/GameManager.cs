using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public AudioManager audioManager;
    public ScoreManager scoreManager;
    public TMP_Text speedDisplay;
    public TMP_Text multiplierDisplay;
    public GameObject loseScreen;

    public float initialSpeed = 1.0f;
    public float maxSpeed = 10.0f;
    public float speedIncrement = 0.1f;
    public float scoreMultiplier = 1.0f;

    public float currentSpeed;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start(){
        currentSpeed = initialSpeed;
    }
    void Update()
    {
        if (currentSpeed < maxSpeed)
        {
            currentSpeed += speedIncrement * Time.deltaTime;
        }

        speedDisplay.text = "Speed: " + currentSpeed.ToString("F2");
        multiplierDisplay.text = "Multiplier: " + scoreMultiplier.ToString("F2");
    }

    public void dilateTime()
    {
        currentSpeed = initialSpeed;
        speedIncrement += 0.1f;
        scoreMultiplier += 0.25f;
        audioManager.PlayAudio4();
    }

    public void AddPoints(float value)
    {
        value = value * scoreMultiplier * 10;
        scoreManager.AddPoints(value);
    }

    public void CollectObj(int value)
    {
        AddPoints(value);
        audioManager.PlayAudio1();
    }

    public void GameOver()
    {
        Time.timeScale = 0f;
        scoreManager.UpdateTopScores();
    }

    public void displayLoseScreen()
    {
        scoreManager.DisplayTopScores();
        loseScreen.SetActive(true);
    }
}
