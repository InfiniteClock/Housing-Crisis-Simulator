using Unity.Cinemachine;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public enum gameState { Play, NoPlay, Tutorial, Message}
    public enum phaseState { City, Zone, Neighbourhood, Home }
    // Reference to Funds UI text
    // Reference to Happiness UI text
    // Reference to Happiness UI Slider
    public Zone testZone;
    public CinemachineCamera cityCam;
    public int gridX = 8;
    public int gridY = 6;


    #region Static Variables
    public static GameManager Instance { get; private set; }
    
    // Happiness stat
    public static int happiness { get; private set; }
    public static void AdjustHappiness(int hap)
    {
        happiness += hap;
    }

    // Funds stat
    public static int funds { get; private set; }
    public static void AdjustFunds(int money)
    {
        funds += money;
    }

    // Housed families stat
    public static int housedFamilies { get; private set; }
    public static void AdjustHousedFamilies(int families)
    {
        housedFamilies += families;
    }

    // Housed individuals stat
    public static int housedPeople { get; private set; }
    public static void AdjustHousedPeople(int people)
    {
        housedPeople += people;
    }

    // Homes filled stat
    public static int filledHomes { get; private set; }
    public static void AdjustFilledHomes(int homes)
    {
        filledHomes += homes;
    }

    // Total homes stat
    public static int totalHomes { get; private set; }
    public static void AdjustTotalHomes(int tHomes)
    {
        totalHomes += tHomes;
    }
    // Current game state
    public static gameState currentGameState { get; private set; }
    public static void GameStateUpdate(gameState state)
    {
        currentGameState = state;
    }
    // Current phase state 
    public static phaseState currentPhase { get; private set; }
    public static void PhaseUpdate(phaseState state)
    {
        currentPhase = state;
    }

    // Current Zone
    public static GameObject currentZone { get; private set; }

    public static void ZoneUpdate(GameObject zone)
    {
        currentZone = zone;
    }

    // Current Neighbourhood
    public static GameObject currentNeighbourhood { get; private set; }
    public static void NeighbourhoodUpdate(GameObject hood)
    {
        currentNeighbourhood = hood;
    }

    // Current House/Apartment
    public static GameObject currentHome { get; private set; }
    public static void HomeUpdate(GameObject home)
    {
        currentHome = home;
    }
    #endregion
    [SerializeField]
    private GameObject[] slotParent3D;
    [SerializeField]
    private GameObject[] slotParents2D;
    public GameObject[,] grid3D { get; private set; }
    public GameObject[,] grid2D { get; private set; }
    public GameObject[,] gridPlaced { get; private set; }

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
    private void Start()
    {
        grid3D = new GameObject[gridX,gridY];
        grid2D = new GameObject[gridX,gridY];
        gridPlaced = new GameObject[gridX,gridY];

        for(int i = 0; i < gridY; i++)
        {
            // Search for 3D tiles based on having a mesh renderer
            MeshRenderer[] temp = slotParent3D[i].GetComponentsInChildren<MeshRenderer>();
            int j = 0;
            foreach (MeshRenderer t in temp)
            {
                grid3D[j, i] = t.gameObject;
                j++;
            }
        }
        for(int i = 0; i < gridY; i++)
        {
            // Search for 3D tiles based on having a Drop script
            Drop[] temp = slotParents2D[i].GetComponentsInChildren<Drop>();
            int j = 0;
            foreach (Drop d in temp)
            {
                grid2D[j, i] = d.gameObject;
                j++;
            }
        }
        PhaseUpdate(phaseState.City);
        GameStateUpdate(gameState.Play);
        happiness = 0;
        funds = 1000;
        housedFamilies = 0;
        housedPeople = 0;
        filledHomes = 0;
        totalHomes = 0;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NextPhase();
        }
    }

    public void NextPhase()
    {
        switch (currentPhase)
        {
            case phaseState.City:
                PhaseUpdate(phaseState.Zone);
                CameraManager.CameraSwitch(testZone.zoneCam);
                break;
            case phaseState.Zone:
                PhaseUpdate(phaseState.Neighbourhood);
                CameraManager.CameraSwitch(testZone.hoods[0].hoodCam);
                break;
            case phaseState.Neighbourhood:
                PhaseUpdate(phaseState.Home);
                CameraManager.CameraSwitch(testZone.hoods[0].Homes[0].houseCam);
                break;
            case phaseState.Home:
                PhaseUpdate(phaseState.City);
                CameraManager.CameraSwitch(cityCam);
                break;
        }
        Debug.Log(currentPhase);
    }

    public void AddToGrid(GameObject originTile, Drag zone2D)
    {
        GameObject newZone = Instantiate(zone2D.zone);
        Zone newZoneScript = newZone.GetComponent<Zone>();

        // ---Position of the Zone---


        // Find matching 3D grid tile to 2D originTile
        Transform spawnTile = Get3DTileTransform(originTile);

        // If none found throw error message and remove object
        if (spawnTile == null)
        {
            Destroy(newZone);
            Debug.LogError("Object failed to locate origin tile match on 3D grid.");
            return;
        }
        // Change the spawn location to be at 0 in y axis 
        spawnTile.position = new Vector3(spawnTile.position.x, 0, spawnTile.position.z);
        // Apply transform position to the object
        newZone.transform.position = spawnTile.transform.position;

        // ---Rotation of the zone---
        
        // Get the z rotation of the 2D image
        float rotDeg = zone2D.transform.eulerAngles.z;

        // Rotate the entire 3D object first
        newZone.transform.Rotate(0, -rotDeg, 0, Space.Self);

        // Rotate every neighbourhood in reverse to the overal object to maintain orientation
        for (int i = 0; i < newZoneScript.hoods.Length; i++)
        {
            // Get the transform of the entire neighbourhood tile prefab
            Transform hood = newZoneScript.hoods[i].transform.parent;
            // Rotate each neighbourhood the reverse direction as the zone to maintain orientation
            hood.Rotate(0, rotDeg, 0, Space.Self);
        }

        // Reorient the zone camera
        newZoneScript.zoneCam.transform.position = newZoneScript.zoneCamLocalOrientation.position;
        newZoneScript.zoneCam.transform.rotation = newZoneScript.zoneCamLocalOrientation.rotation;
    }
    // Returns the transform of a matching 3D tile to given 2D tile object
    private Transform Get3DTileTransform(GameObject originTile)
    {
        for (int i = 0; i < gridY; i++)
        {
            for (int j = 0; j < gridX; j++)
            {
                if (grid3D[j, i].transform.parent.name == originTile.transform.parent.name)
                {
                    if (grid3D[j, i].name == originTile.name)
                    {
                        return grid3D[j, i].transform;
                    }
                }
            }
        }
        return null;
    }
}
