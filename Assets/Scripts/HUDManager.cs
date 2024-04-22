using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Runner");
    }
    public void RestartGame()
    {
        SceneManager.LoadScene("Runner");
        Time.timeScale = 1.0f;
    }
}
