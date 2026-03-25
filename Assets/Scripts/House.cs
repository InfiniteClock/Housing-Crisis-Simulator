using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public enum HouseState { Interacted, NonInteractable, Highlighted, Default}
public class House : MonoBehaviour
{
    public string houseName;
    public int basePrice;
    public int realPrice {  get; private set; }
    private int priceMod;
    public string houseSize;

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

        // Houses spawn when zone is selected, so this is the correct time to determine what zone type this house belongs to
        // Determines how much the price should change if landlord increases/decreases rent
        switch (GameManager.Instance.CurrentZoneType)
        {
            case zoneType.LowIncome:
                priceMod = 100;
                break;
            case zoneType.MidIncome:
                priceMod = 200;
                break;
            case zoneType.HighIncome:
                priceMod = 300;
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
