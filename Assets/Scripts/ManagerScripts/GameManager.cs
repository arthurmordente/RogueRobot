using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public PlayerMovement player;
    public AudioManager audioManager;
    public ScoreManager scoreManager;
    public AchievementManager achv;
    public PositionTracker positionTracker;
    public TMP_Text speedDisplay;
    public TMP_Text multiplierDisplay;
    public TMP_Text accDisplay;
    public TMP_Text distanceDisplay;
    public GameObject loseScreen;
    public bool isPaused;
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
        isPaused = false;
    }
    void Update()
    {

        HandleSpeed();
        UpdateMultiplier();

        speedDisplay.text = "Speed: " + currentSpeed.ToString("F2");
        multiplierDisplay.text = "Multiplier: " + scoreMultiplier.ToString("F2");
        accDisplay.text = "Acc: " + speedIncrement.ToString("F1");
        distanceDisplay.text = "Distance: " + positionTracker.sectionDistance.ToString("");
    }

    public float GetCurrentSpeed(){
        return currentSpeed;
    }

     public float GetCurrentAcceleration(){
        return speedIncrement;
    }



    public void HandleSpeed(){
        if(player.isDead){
            currentSpeed = 0;
        }
        else if (currentSpeed < maxSpeed && currentSpeed >= 1.0f)
        {
            currentSpeed += speedIncrement * Time.deltaTime;
        }
        achv.CheckAndUnlockAchievements("speed", currentSpeed);
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

    public void PauseGame(){
        Time.timeScale = 0.0f;
        isPaused = true;
    }

    public void UnpauseGame(){
        StartCoroutine(UnpauseGameCoroutine());
    }

    // Corrotina que realiza o atraso
    private IEnumerator UnpauseGameCoroutine()
    {
        Time.timeScale = 1.0f; // Retoma o tempo do jogo imediatamente
        yield return new WaitForSeconds(0.3f); // Espera por 0.3 segundos
        isPaused = false; // Muda o estado da variável após o atraso
    }


    public void GameOver()
    {
        achv.UnlockAchievement("First timer");
        player.isDead = true;
        scoreManager.UpdateTopScores();
    }

    public void displayLoseScreen()
    {
        scoreManager.DisplayTopScores();
        loseScreen.SetActive(true);
    }
}
