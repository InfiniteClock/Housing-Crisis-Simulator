using TMPro;
using UnityEngine;

public class LandlordInfoDisplay : MonoBehaviour
{
    public static LandlordInfoDisplay Instance;

    [Header("Landlord Info")]
    [SerializeField] private float downPayment;
    [SerializeField] private LandlordTraits trait1;
    [SerializeField] private LandlordTraits trait2;


    [Header("UI Objects")]
    public TextMeshPro downPaymentText;
    public TextMeshPro Trait1InfoText;
    public TextMeshPro Trait2InfoText;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // Suicide if Instance another game manager exists
            Destroy(this.gameObject);
        }
        else
        {
            // Otherwise, set this as the instance
            Instance = this;
        }
    }

    public void showLandlordInfo()
    {

    }
}
