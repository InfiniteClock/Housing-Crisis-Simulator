using Unity.Cinemachine;
using UnityEngine;

public class Zone : MonoBehaviour
{
    [SerializeField]
    public enum Type { Low, Med, High, Apartment, NonRes }
    public CinemachineCamera zoneCam;
    public Transform zoneCamLocalOrientation { get; private set; }
    public Neighbourhood[] hoods { get; private set; }
    [SerializeField]
    private Type zoneType;
    public Type GetZoneType() {  return zoneType; }
    private void Awake()
    {
        // Stores the inspector set camera transform for later
        zoneCamLocalOrientation = zoneCam.transform;
        // Builds a list of every neighbourhood child in this zone
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
