using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public new AudioSource audio;
    public void PlayGame()
    {
        //audio.Play();
        SceneManager.LoadScene("Multi");
    }

    public void click()
    {
        audio.Play();
    }
    public void QuitGame()
    {
        audio.Play();
        Debug.Log("QUIT! works?");
        Application.Quit();
    }
}
