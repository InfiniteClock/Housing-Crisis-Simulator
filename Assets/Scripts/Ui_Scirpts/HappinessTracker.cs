using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HappinessTracker : MonoBehaviour
{
    public Slider tracker;
    public GameObject background;
    public GameObject sprite;
    private Image backgroundImage;
    private Image spriteImage;
    public List<Sprite> faceSprites;
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
        if (faceSprites.Count == 3)
        {
            if (tracker.value >= 30)
            {
                background.GetComponent<Image>().color = Color.green;
                spriteImage.sprite = faceSprites[2];
            }
            else if (tracker.value > -30 & tracker.value < 30)
            {
                background.GetComponent<Image>().color = Color.yellow;
                spriteImage.sprite = faceSprites[1];
            }
            else
            {
                background.GetComponent<Image>().color = Color.red;
                spriteImage.sprite = faceSprites[0];
            }
        } else
        {
            Debug.Log("!to many sprites!");
        }
    }
}
