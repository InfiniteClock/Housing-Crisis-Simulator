using NUnit.Framework;
using Unity.Cinemachine;
using UnityEngine;

public class Neighbourhood : MonoBehaviour
{
    public CinemachineCamera hoodCam;
    [SerializeField]
    private Material matDefault;
    [SerializeField]
    private Material matInteract;
    [SerializeField]
    private Material matNonInteract;
    [SerializeField]
    private Material matHighlight;

    private Material currentMat;
    private MeshRenderer mr;

    public House[] Homes {  get; private set; }
    [SerializeField]
    private Zone parentZone;
    private void Start()
    {
        mr = GetComponent<MeshRenderer>();
        SetDefMat();
        Homes = GetComponentsInChildren<House>(true);
        if (parentZone == null)
            parentZone = GetComponentInParent<Zone>();
    }
    private void Update()
    {
        if (GameManager.currentPhase == GameManager.phaseState.City || GameManager.currentPhase == GameManager.phaseState.Zone)
        {
            mr.gameObject.GetComponent<Collider>().enabled = true;
        }
        else
        {
            mr.gameObject.GetComponent<Collider>().enabled = false;
        }
    }

    public void SetDefMat()
    {
        mr.material = matDefault;
        currentMat = matDefault;
    }
    public void SetInteractable()
    {
        mr.material = matInteract;
        currentMat = matInteract;
    }
    public void SetNonInteractable()
    {
        mr.material = matNonInteract;
        currentMat = matNonInteract;
    }
    //private void OnMouseEnter()
    //{
    //    if (GameManager.currentPhase == GameManager.phaseState.City)
    //    {
    //        parentZone.SetMouseEnter();
    //    }
    //    if (GameManager.currentPhase == GameManager.phaseState.Zone)
    //    {
    //        SetMouseEnter();
    //    }
    //}
    public void SetMouseEnter()
    {
        mr.material = matHighlight;
    }
    //private void OnMouseExit()
    //{
    //    parentZone.SetMouseExit();
    //    SetMouseExit();
    //}
    public void SetMouseExit()
    {
        mr.material = currentMat;
    }
}
