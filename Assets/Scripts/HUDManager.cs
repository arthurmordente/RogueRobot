using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    public GameObject configScreen;
    public void StartGame()
    {
        SceneManager.LoadScene("Runner");
    }
    public void RestartGame()
    {
        SceneManager.LoadScene("Runner");
        Time.timeScale = 1.0f;
    }

    public void OpenConfigScreen(){
        configScreen.SetActive(true);
    }

    public void CloseConfigScreen(){
        configScreen.SetActive(false);
    }
}
