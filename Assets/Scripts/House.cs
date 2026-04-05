using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public enum HouseState { Interacted, NonInteractable, Highlighted, Default}
public enum HouseSize { small, medium, large}
public class House : MonoBehaviour
{
    public string houseName;
    public int basePrice;
    public int realPrice {  get; private set; }
    private int priceMod;
    public HouseSize houseSize;

    public CinemachineCamera houseCam;
    [SerializeField]
    private Material matDefault;
    [SerializeField]
    private Material matInteracted;
    [SerializeField]
    private Material matNonInteract;
    [SerializeField]
    private Material matHighlight;

    // Sets default house state to non-interactable (greyed out)
    public HouseState currentState = HouseState.NonInteractable;
    private Material currentMat;
    private Material previousMat;
    private MeshRenderer mr;

    private void Start()
    {
        mr = GetComponent<MeshRenderer>();

        // Set the current mat to non-interactable so the functions don't risk having null values
        currentMat = matNonInteract;
        SetNonInteractable();
    }

    public void RandomizeHouseStats()
    {
        int temp = 1;
        // Determines the price of a house based on zone income type and house size
        // PriceMod determines how much landlord modifiers affect the price
        switch (GameManager.Instance.CurrentZoneType)
        {
            // Low Income Houses
            case zoneType.LowIncome:
                // Determines random house size
                temp = Random.Range(0, 2);
                houseSize = (HouseSize)temp;
                priceMod = 100;
                switch (houseSize)
                {
                    case HouseSize.small:
                        basePrice = Random.Range(4, 7) * 100;
                        break;
                    case HouseSize.medium:
                        basePrice = Random.Range(7, 10) * 100;
                        break;
                    case HouseSize.large:
                        basePrice = Random.Range(10, 13) * 100;
                        break;
                }
                break;
            // Medium Income Houses
            case zoneType.MidIncome:
                // Determines random house size
                temp = Random.Range(0, 3);
                houseSize = (HouseSize)temp;
                priceMod = 200;
                switch (houseSize)
                {
                    case HouseSize.small:
                        basePrice = Random.Range(8, 11) * 100;
                        break;
                    case HouseSize.medium:
                        basePrice = Random.Range(11, 14) * 100;
                        break;
                    case HouseSize.large:
                        basePrice = Random.Range(14, 17) * 100;
                        break;
                }
                break;
            // High Income Houses
            case zoneType.HighIncome:
                // Determines random house size
                temp = Random.Range(1, 3);
                houseSize = (HouseSize)temp;
                priceMod = 300;
                switch (houseSize)
                {
                    case HouseSize.small:
                        basePrice = Random.Range(12, 15) * 100;
                        break;
                    case HouseSize.medium:
                        basePrice = Random.Range(15, 18) * 100;
                        break;
                    case HouseSize.large:
                        basePrice = Random.Range(18, 21) * 100;
                        break;
                }
                break;
            // Apartment Complexes
            case zoneType.Highrise:
                // Determines random house size
                temp = Random.Range(0, 3);
                houseSize = (HouseSize)temp;
                priceMod = 200;
                switch (houseSize)
                {
                    case HouseSize.small:
                        basePrice = Random.Range(8, 11) * 100;
                        break;
                    case HouseSize.medium:
                        basePrice = Random.Range(11, 14) * 100;
                        break;
                    case HouseSize.large:
                        basePrice = Random.Range(14, 17) * 100;
                        break;
                }
                break;
        }
    }
    public void SetPrice()
    {
        // Find and apply landlord modifications to current price
        realPrice = basePrice;
        switch (GameManager.Instance.Trait1)
        {
            case LandlordTraits.IncreaseRent:
                realPrice += priceMod;
                break;
            case LandlordTraits.DecreaseRent:
                realPrice -= priceMod;
                break;
            default:
                break;
        }
        // Repeat for second landlord modification
        switch (GameManager.Instance.Trait2)
        {
            case LandlordTraits.IncreaseRent:
                realPrice += priceMod;
                break;
            case LandlordTraits.DecreaseRent:
                realPrice -= priceMod;
                break;
            default:
                break;
        }
    }

    public void SetDefMat()
    {
        mr.material = matDefault;
        currentMat = matDefault;
        currentState = HouseState.Default;
    }
    public void SetInteracted()
    {
        mr.material = matInteracted;
        currentMat = matInteracted;
        currentState = HouseState.Interacted;
    }
    public void SetNonInteractable()
    {
        mr.material = matNonInteract;
        currentMat = matNonInteract;
        currentState = HouseState.NonInteractable;
    }
    public void SetHighlight()
    {
        mr.material = matHighlight;
        currentMat = matHighlight;
        currentState = HouseState.Highlighted;
    }
    public void SetLocked(bool filled)
    {
        if (filled)
        {
            mr.material = matInteracted;
            currentMat = matInteracted;
            currentState = HouseState.NonInteractable;
        }
        else
        {
            mr.material = matDefault;
            currentMat = matDefault;
            currentState = HouseState.NonInteractable;
        } 
    }

    private void OnMouseDown()
    {
        // Checks if mouse is blocked by UI element first
        if (EventSystem.current.IsPointerOverGameObject()) return;

        // Changes material between highlighted and default
        switch (currentState)
        {
            case HouseState.Default:
                GameManager.HomeUpdate(this);
                GameManager.PhaseUpdate(GameManager.phaseState.Home);
                break;
            case HouseState.Highlighted:
                SetDefMat();
                CameraManager.CameraSwitch(GameManager.currentNeighbourhood.hoodCam);
                GameManager.PhaseUpdate(GameManager.phaseState.Neighbourhood);
                break;
            case HouseState.Interacted:
                Debug.Log("You have already locked in that house!");
                if (GameManager.currentPhase == GameManager.phaseState.Home)
                {
                    CameraManager.CameraSwitch(GameManager.currentNeighbourhood.hoodCam);
                    GameManager.PhaseUpdate(GameManager.phaseState.Neighbourhood);
                }
                break;
            case HouseState.NonInteractable:
                Debug.Log("You can't select that house!");
                break;
            default:
                Debug.LogError("Some terrible error has occurred in this House script");
                break;
        }
    }

    private void OnMouseEnter()
    {
        // Checks if mouse is blocked by UI element first
        if (EventSystem.current.IsPointerOverGameObject()) return;

        // Checks if in phase 3
        if (GameManager.currentPhase == GameManager.phaseState.Neighbourhood || GameManager.currentPhase == GameManager.phaseState.Home)
        {
            // Sets the highlight, but not the state
            if (currentState == HouseState.Default) mr.material = matHighlight;
        }
    }
    private void OnMouseExit()
    {
        // Removes hover highlight ONLY if still in the default state
        if (currentState == HouseState.Default) mr.material = currentMat;
    }
}
