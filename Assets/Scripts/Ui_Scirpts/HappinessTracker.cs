using UnityEngine;
using UnityEngine.UI;

public class HappinessTracker : MonoBehaviour
{
    public Slider tracker;
    public GameObject background;
    private Image backgroundImage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get background image for colour changes
        backgroundImage = background.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(tracker.value + " " + backgroundImage.color);
    }
}
