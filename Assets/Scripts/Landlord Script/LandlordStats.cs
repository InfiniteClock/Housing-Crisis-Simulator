using TMPro;
using UnityEngine;

public enum LandlordTrits
{
    Empty, 
    increaseRent, 
    DecreaseRent, 
    IncreaseHappiness, 
    DecreaseHappiness
}

public class LandlordStats : MonoBehaviour
{
    [Header("Landloard Status")]
    public float downPaymentRange;

    [HideInInspector]
    public float downPayment;

    public LandlordTrits trit1;
    public LandlordTrits trit2;
    

    [Space(10)]
    [Header("UI Objects")]
    public TextMeshProUGUI downPaymentText;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
