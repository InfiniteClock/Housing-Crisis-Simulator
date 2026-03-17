using TMPro;
using UnityEngine;



public class LandlordStats : MonoBehaviour
{
    [Header("Landloard Status")]
    public float downPaymentRange;

    [HideInInspector]
    public float downPayment;

    public LandlordTraits trait1;
    public LandlordTraits trait2;
    

    [Space(10)]
    [Header("UI Objects")]
    public TextMeshProUGUI downPaymentText;


    public void PayDownPayment()
    {
        float zonePrice = GameManager.Instance.CurrentZonePrice;
        float downPayment = zonePrice * (downPaymentRange / 100);

        ResourceManager.Instance.AddBudget(downPayment);
    }
}
