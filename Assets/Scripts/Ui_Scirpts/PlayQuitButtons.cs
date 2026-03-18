using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayGameScript : MonoBehaviour
{
   
    public void PlayGame()
    {
        SceneManager.LoadScene("System Setup");
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
