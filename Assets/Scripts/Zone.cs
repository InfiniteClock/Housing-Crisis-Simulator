using Unity.Cinemachine;
using UnityEngine;

public class Zone : MonoBehaviour
{
    [SerializeField]
    private enum Type { Low, Med, High, Apartment }
    public CinemachineCamera zoneCam;

    public Neighbourhood[] hoods { get; private set; }
    private void Start()
    {
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
