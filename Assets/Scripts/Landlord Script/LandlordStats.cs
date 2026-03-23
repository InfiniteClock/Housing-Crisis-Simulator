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
    

    public float getDownpaymentValue()
    {
        float zonePrice = GameManager.Instance.CurrentZonePrice;
        float downPayment = zonePrice * (downPaymentRange / 100);
        return downPayment;
    }

}
