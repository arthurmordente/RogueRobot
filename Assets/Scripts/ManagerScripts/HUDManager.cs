using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    public GameObject configScreen;

    public GameObject helpScreen;

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

    public void OpenHelpScreen(){
        helpScreen.SetActive(true);
    }

    public void CloseHelpScreen(){
        helpScreen.SetActive(false);
    }
}
