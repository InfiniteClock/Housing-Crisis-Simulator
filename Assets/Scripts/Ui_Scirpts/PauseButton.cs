using UnityEngine;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour
{
    public GameObject pauseScreen;
    public Button menuButton;
   // [SerializeField] GameObject pauseMenu;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        menuButton.onClick.AddListener(menuButtonClicked);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            menuButtonClicked();
        }
    }

    void menuButtonClicked()
    {
        if (pauseScreen.activeSelf != true) {
            Debug.Log("Click");
            pauseScreen.SetActive(true);
        }
    }


    public void resumeGame()
    {
        // if the "Resume" button is clicked the game resumes and the pause menu hides
        pauseScreen.SetActive(false);
    }
}
