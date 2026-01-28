using Unity.Cinemachine;
using UnityEngine;

public class House : MonoBehaviour
{
    [SerializeField]
    private enum Type { Low, Med, High }
    [SerializeField]
    private CinemachineCamera houseCam;
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

    private void Start()
    {
        mr = GetComponent<MeshRenderer>();
        SetDefMat();
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
    private void OnMouseEnter()
    {
        mr.material = matHighlight;
    }
    private void OnMouseExit()
    {
        mr.material = currentMat;
    }


}
