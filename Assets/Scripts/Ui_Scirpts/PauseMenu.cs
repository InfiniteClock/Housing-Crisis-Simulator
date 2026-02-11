using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseScreen;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            exitMenu();
        }
    }

    void exitMenu()
    {
        pauseScreen.SetActive(false);
    }
}
