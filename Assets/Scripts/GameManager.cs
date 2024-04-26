using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public AudioManager audioManager;
    public ScoreManager scoreManager;
    public PositionTracker positionTracker;
    public TMP_Text speedDisplay;
    public TMP_Text multiplierDisplay;
    public TMP_Text accDisplay;
    public GameObject loseScreen;

    public float initialSpeed = 1.0f;
    public float maxSpeed = 6.0f;
    public float speedIncrement = 0.1f;
    public float speedMult = 0.2f;
    public float scoreMultiplier = 1.0f;
    public float baseScoreMultiplier = 1.0f;
    public float perfectionScoreMultiplier = 0.0f;
    public float distanceScoreMultiplier = 0.0f;

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

        HandleSpeed();
        UpdateMultiplier();

        speedDisplay.text = "Speed: " + currentSpeed.ToString("F2");
        multiplierDisplay.text = "Multiplier: " + scoreMultiplier.ToString("F2");
        accDisplay.text = "Acc: " + speedIncrement.ToString("F1");
    }

    public void HandleSpeed(){
        if (currentSpeed < maxSpeed && currentSpeed >= 1.0f)
        {
            currentSpeed += speedIncrement * Time.deltaTime;
        }
    }

    public void UpdateMultiplier(){
        distanceScoreMultiplier = positionTracker.GetDistanceMultiplier();
        perfectionScoreMultiplier = positionTracker.GetPerfectMultiplier();
        scoreMultiplier = baseScoreMultiplier + distanceScoreMultiplier + perfectionScoreMultiplier;

    }

    public void Accelerate()
    {
        if (currentSpeed < maxSpeed)
        {
            currentSpeed = Mathf.Min(currentSpeed * (1 + speedMult), maxSpeed);
        }
        if (speedIncrement < 0.2f)
        {
            speedIncrement += 0.1f;
        }
        audioManager.PlayAudio4();
    }

    public void Reduce()
    {
        if (currentSpeed > initialSpeed)
        {
            currentSpeed = Mathf.Max(currentSpeed * (1 - speedMult), initialSpeed);
        }
        if (speedIncrement > -0.2f)
        {
            speedIncrement -= 0.1f;
        }
        audioManager.PlayAudio4();
    }

    public void Reduce(float ReductionRate)
    {
        if (currentSpeed > initialSpeed)
        {
            currentSpeed = Mathf.Max(currentSpeed * (1 - ReductionRate), initialSpeed);
        }
        Debug.Log("Speed reduced by " + ReductionRate);
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
