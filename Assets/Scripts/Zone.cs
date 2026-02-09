using Unity.Cinemachine;
using UnityEngine;

public class Zone : MonoBehaviour
{
    [SerializeField]
    private enum Type { Low, Med, High, Apartment }
    public CinemachineCamera zoneCam;
    public Transform zoneCamLocalOrientation { get; private set; }
    public Neighbourhood[] hoods { get; private set; }
    private void Awake()
    {
        // Stores the inspector set camera transform for later
        zoneCamLocalOrientation = zoneCam.transform;
        hoods = GetComponentsInChildren<Neighbourhood>(true);
    }
    public void SetDefMat()
    {
        foreach (Neighbourhood hood in hoods)
        {
            hood.SetDefMat();
        }
    }
    public void SetInteractable()
    {
        foreach (Neighbourhood hood in hoods)
        {
            hood.SetInteractable();
        }
    }
    public void SetNonInteractable()
    {
        foreach (Neighbourhood hood in hoods)
        {
            hood.SetNonInteractable();
        }
    }
    public void SetMouseEnter()
    {
        foreach (Neighbourhood hood in hoods)
        {
            hood.SetMouseEnter();
        }
    }
    public void SetMouseExit()
    {
        foreach (Neighbourhood hood in hoods)
        {
            hood.SetMouseExit();
        }
    }
}
