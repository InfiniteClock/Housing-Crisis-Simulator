using Unity.Cinemachine;
using UnityEngine;

public enum HouseState { Interacted, NonInteractable, Highlighted, Default}
public class House : MonoBehaviour
{
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
    private void OnMouseDown()
    {
        switch (currentState)
        {
            case HouseState.Default:
                GameManager.HomeUpdate(this);
                break;
            case HouseState.Highlighted:
                CameraManager.CameraSwitch(GameManager.currentNeighbourhood.hoodCam);
                SetDefMat();
                break;
            case HouseState.Interacted:
                Debug.Log("You have already locked in that house!");
                break;
            case HouseState.NonInteractable:
                Debug.Log("You can't select that house!");
                break;
            default:
                Debug.LogError("Some terrible error has occurred in this House script");
                break;
        }
    }

    //private void OnMouseEnter()
    //{
    //    if (GameManager.currentPhase == GameManager.phaseState.Neighbourhood || GameManager.currentPhase == GameManager.phaseState.Home)
    //    {
    //        mr.material = matHighlight;
    //    }
    //}
    //private void OnMouseExit()
    //{
    //    mr.material = currentMat;
    //}
}
