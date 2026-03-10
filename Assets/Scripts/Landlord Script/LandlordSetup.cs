using UnityEngine;
using UnityEngine.UI;

public class LandlordSetup : MonoBehaviour
{
    public Sprite landlordSprite;
    public string landloaedName;
    public int companyInt;
    public int Trait01;
    public int Trait02;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Image thisSprite = GetComponent<Image>();
        thisSprite.sprite = landlordSprite;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
