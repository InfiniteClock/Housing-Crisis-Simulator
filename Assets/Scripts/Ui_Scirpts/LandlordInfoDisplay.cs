using System.Diagnostics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LandlordInfoDisplay : MonoBehaviour
{
    public static LandlordInfoDisplay Instance;

    [Header("Landlord Info")]
    [SerializeField] private float downPayment;
    [SerializeField] private LandlordTraits trait1;
    [SerializeField] private LandlordTraits trait2;


    [Header("UI Objects")]
    public TextMeshProUGUI downPaymentText;
    public TextMeshProUGUI Trait1InfoText;
    public TextMeshProUGUI Trait2InfoText;
    public Button confirmButton;


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

    public void GetLandlordInfo(float dpValue, LandlordTraits t1, LandlordTraits t2)
    {
        downPayment = dpValue;
        trait1 = t1;
        trait2 = t2;
    }

    public void showLandlordInfo()
    {
        downPaymentText.text = "$" + downPayment.ToString() + "K";
        ShowTrait(Trait1InfoText, trait1);
        ShowTrait(Trait2InfoText, trait2);
    }
    public void ShowTrait(TextMeshProUGUI ui, LandlordTraits trait)
    {
        switch (trait)
        {
            case LandlordTraits.Empty:
                ui.text = "";
                break;
            case LandlordTraits.IncreaseHappiness:
                ui.text = "Increase Happiness";
                break;
            case LandlordTraits.DecreaseHappiness:
                ui.text = "Decrease Happiness";
                break;
            case LandlordTraits.IncreaseRent:
                ui.text = "Increase Rent";
                break;
            case LandlordTraits.DecreaseRent:
                ui.text = "Decrease Rent";
                break;
        }
    }


    public void ConfirmLandlord()
    {
        if (downPayment == 0)
        {
            return;
        }
        else
        {
            PickLord();
            LandloardRandomizer.Instance.ResetLandlords();
            if (GameManager.Instance.CurrentZoneType == zoneType.LowIncome)
            {
                TenantRandomizer.Instance.SpawnLowIncome();
            }
            else if (GameManager.Instance.CurrentZoneType == zoneType.MidIncome)
            {
                TenantRandomizer.Instance.SpawnMidIncome();
            }
            else if (GameManager.Instance.CurrentZoneType == zoneType.HighIncome)
            {
                TenantRandomizer.Instance.SpawnHighIncome();
            }
        }
            
    }

    private void PickLord()
    {
        GameManager.PhaseUpdate(GameManager.phaseState.Neighbourhood);

        // Pick a random neighbourhood within the zone to focus on in phase 3
        int randHood = UnityEngine.Random.Range(0, GameManager.currentZone.hoods.Length);
        GameManager.NeighbourhoodUpdate(GameManager.currentZone.hoods[randHood]);
        CameraManager.CameraSwitch(GameManager.currentNeighbourhood.hoodCam);

        GameManager.Instance.SetTraits(trait1, trait2);
        ResourceManager.Instance.AddBudget(downPayment);
    }
}
