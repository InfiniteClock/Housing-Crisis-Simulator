using UnityEngine;
using UnityEngine.UI;

public class HappinessTracker : MonoBehaviour
{
    public Slider tracker;
    public GameObject background;
    public GameObject sprite;
    private Image backgroundImage;
    private Image spriteImage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get background image and sprite for state changes
        backgroundImage = background.GetComponent<Image>();
        spriteImage = sprite.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        // Debugger
        //Debug.Log(tracker.value + " " + backgroundImage.color + " " + spriteImage.sprite);

        if(tracker.value >= 30)
        {
            background.GetComponent<Image>().color = Color.green;
        }
        else if (tracker.value >-30 & tracker.value < 30)
        {
            background.GetComponent<Image>().color = Color.yellow;
        }
        else
        {
            background.GetComponent<Image>().color = Color.red;
        }
    }
}
