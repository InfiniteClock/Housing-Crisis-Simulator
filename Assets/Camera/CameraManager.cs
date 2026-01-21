using NUnit.Framework;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public CinemachineCamera startupCam;
    public static CameraManager instance { get; private set; }
    public static CinemachineCamera currentCam { get; private set; }
    public static CinemachineCamera prevCam { get; private set; }

    // This is for debuggin and testing purposes only
    public CinemachineCamera[] exampleCams;
    public int exampleCamIndex = 0;
    private void Start()
    {
        // Find and disable every camera in the scene on start. Then enable the startup cam
        CinemachineCamera[] allCams = FindObjectsByType<CinemachineCamera>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (CinemachineCamera cam in allCams)
        {
            cam.enabled = false;
        }
        startupCam.enabled = true;
        currentCam = startupCam;
        prevCam = startupCam;
    }

    // Update is called once per frame
    void Update()
    {
        // This is for debugging and testing purposes only
        if (Input.GetMouseButtonDown(0))
        {
            if (exampleCamIndex >= exampleCams.Length-1)
            {
                exampleCamIndex = 0;
                CameraSwitch(exampleCams[0]);
            }
            else
            {
                exampleCamIndex++;
                CameraSwitch(exampleCams[exampleCamIndex]);
            }
        }
    }
    public static void CameraSwitch(CinemachineCamera newCam)
    {
        // Update the cameras
        prevCam = currentCam;
        currentCam = newCam;

        // Enable the new camera, and disable the old one
        currentCam.enabled = true;
        prevCam.enabled = false;
    }
}
