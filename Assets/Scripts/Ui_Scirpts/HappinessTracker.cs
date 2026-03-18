using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HappinessTracker : MonoBehaviour
{
    public Slider tracker;
    public GameObject background;
    public GameObject sprite;
    private Image backgroundImage;
    private Image spriteImage;
    public List<Sprite> faceSprites;
    public List<Color> bgColour;
    public float barSize;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get background image and sprite for state changes
        backgroundImage = background.GetComponent<Image>();
        spriteImage = sprite.GetComponent<Image>();
        barSize = tracker.maxValue - tracker.minValue;
    }

    // Update is called once per frame
    void Update()
    {
        // Debugger
        //Debug.Log(tracker.value + " " + backgroundImage.color + " " + spriteImage.sprite);
        for (int i = 0; i < faceSprites.Count; i++)
        {
            if (tracker.value - tracker.minValue >= barSize / faceSprites.Count * i)
            {
                spriteImage.sprite = faceSprites[i];
                background.GetComponent<Image>().color = bgColour[i];
            }

        }
    }
}
/*background.GetComponent<Image>().color = Color.green;
*/