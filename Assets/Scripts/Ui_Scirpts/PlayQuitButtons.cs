using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayGameScript : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;

    public void PlayGame()
    {
        //     SceneManager.LoadScene("System Setup");

        // If player presses "Construct" button then the scene changes to the game
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = (currentSceneIndex + 1) %
            SceneManager.sceneCountInBuildSettings;
        SceneManager.LoadScene(nextSceneIndex);
    }

    public void ReturnToMenu()
    {
   //     SceneManager.LoadScene("Main Menu");

        // If player presses "Return To Menu" button then the scene changes to the main menu scene and resets the game
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = (currentSceneIndex - 1) %
            SceneManager.sceneCountInBuildSettings;
        SceneManager.LoadScene(nextSceneIndex);
    }

    public void QuitGame()
    {
        // If the "QUIT" button is pressed then the application closes, and the run in-engine ends.
       #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();

    }

    public void resumeGame()
    {
        pauseMenu.SetActive(false);
    }

}