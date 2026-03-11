using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public enum gameState { Play, NoPlay, Tutorial, Message}
    public enum phaseState { City, Zone, Neighbourhood, Home }
    
    [SerializeField] private CanvasGroup p1City;
    [SerializeField] private CanvasGroup p2Zone;
    [SerializeField] private CanvasGroup p3Neighbourhood;
    [SerializeField] private CanvasGroup p4Home;
    [SerializeField] private float fadeTimeUI;
    [SerializeField] private float fadeDelayUI;

    // Reference to Funds UI text
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
        switch (state)
        {
            case phaseState.City:
                // Fade in City UI. Fade out Neighbourhood and Home UIs
                Instance.StartCoroutine(Instance.CanvasFade(Instance.p1City, Instance.fadeTimeUI));
                Instance.StartCoroutine(Instance.CanvasFade(Instance.p3Neighbourhood, -Instance.fadeTimeUI));
                if (Instance.p4Home.interactable) Instance.StartCoroutine(Instance.CanvasFade(Instance.p4Home, -Instance.fadeTimeUI));
                break;
            case phaseState.Zone:
                // Fade in Zone UI. Fade out City UI.
                Instance.StartCoroutine(Instance.CanvasFade(Instance.p2Zone, Instance.fadeTimeUI));
                Instance.StartCoroutine(Instance.CanvasFade(Instance.p1City, -Instance.fadeTimeUI));
                break;
            case phaseState.Neighbourhood:
                // Fade in Neighbourhood UI. Fade out Zone UI or Home UI as needed. 
                Instance.StartCoroutine(Instance.CanvasFade(Instance.p3Neighbourhood, Instance.fadeTimeUI));
                if (Instance.p2Zone.interactable) Instance.StartCoroutine(Instance.CanvasFade(Instance.p2Zone, -Instance.fadeTimeUI));
                if (Instance.p4Home.interactable) Instance.StartCoroutine(Instance.CanvasFade(Instance.p4Home, -Instance.fadeTimeUI));
                break;
            case phaseState.Home:
                // Fade in Home UI. Should fit on top of Neighbourhood UI as complement
                Instance.StartCoroutine(Instance.CanvasFade(Instance.p4Home, Instance.fadeTimeUI));
                break;
        }
    }
    /// <summary>
    /// Fades the UI into or out of view and changes its interactability
    /// </summary>
    /// <param name="target">CanvasGroup to fade</param>
    /// <param name="fadeTime">Duration of the fade. Positive fades in, Negative fades out</param>
    /// <returns></returns>
    private IEnumerator CanvasFade(CanvasGroup target, float fadeTime)
    {
        target.interactable = false;
        target.gameObject.SetActive(true);
        float timer = 0f;
        while (timer < Mathf.Abs(fadeTime))
        {
            if (fadeTime < 0) // Negative = fade out
            {
                target.alpha = Mathf.Lerp(1, 0, timer / Mathf.Abs(fadeTime));
            }
            else if (fadeTime > 0) // Positive = fade in
            {
                target.alpha = Mathf.Lerp(0, 1, timer / Mathf.Abs(fadeTime));
            }
            timer += Time.deltaTime;
            yield return null;
        }
        if (fadeTime < 0) // Negative = fade out
        {
            target.alpha = 0;
            target.interactable = false;
            target.gameObject.SetActive(false);
        }
        else if (fadeTime > 0) // Positive = fade in
        {
            target.alpha = 1;
            target.interactable = true;
        }
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

        // Set each canvas to correct state
        Instance.StartCoroutine(CanvasFade(Instance.p1City, 0.01f));
        Instance.StartCoroutine(CanvasFade(Instance.p2Zone, -0.01f));
        Instance.StartCoroutine(CanvasFade(Instance.p3Neighbourhood, -0.01f));
        Instance.StartCoroutine(CanvasFade(Instance.p4Home, -0.01f));

        currentPhase = phaseState.City;
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
        // Trigger test of phase transition by pressing spacebar
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    NextPhase();
        //}
    }

    /*
    // This is purely for testing and forcing phase transitions with a test zone 
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
    }*/

    /// <summary>
    /// Adds a new 3D zone to the grid. Then transitions to next phase
    /// </summary>
    /// <param name="originTile">The tile that the object pivot aligns to</param>
    /// <param name="zone2D">The 2D zone placed on the UI grid that aligns to this object</param>
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
        Vector3 spawn = new Vector3(spawnTile.position.x, 0, spawnTile.position.z);
        // Apply transform position to the object
        newZone.transform.position = spawn;


        // ---Rotation of the zone---
        // Get the z rotation of the 2D image
        float rotDeg = zone2D.transform.eulerAngles.z;

        // Rotate the entire 3D object first
        newZone.transform.Rotate(0, -rotDeg, 0, Space.Self);

        // Rotate every neighbourhood in reverse to the overall object to maintain orientation
        // For non-residential zones, no neighbourhoods = buildings rotate with zone, not against it
        for (int i = 0; i < newZoneScript.hoods.Length; i++)
        {
            // Get the transform of the entire neighbourhood tile prefab
            Transform hood = newZoneScript.hoods[i].transform.parent;
            // Rotate each neighbourhood the reverse direction as the zone to maintain orientation
            hood.Rotate(0, rotDeg, 0, Space.Self);
        }

        // Reorient the zone camera
        newZoneScript.zoneCam.transform.RotateAround(newZone.transform.position, Vector3.up, rotDeg);

        // If the placed zone was not a non residential zone, move to phase 2
        if (newZoneScript.GetZoneType() != Zone.Type.NonRes)
        {
            testZone = newZoneScript;
            PhaseUpdate(phaseState.Zone);
            CameraManager.CameraSwitch(newZoneScript.zoneCam);
        }
    }

    /// <summary>
    /// Returns the transform of a matching 3D tile to a given 2D tile object
    /// </summary>
    /// <param name="originTile">The 2D tile object from the 2D zone that was placed</param>
    /// <returns>the transform of the matching 3D tile space</returns>
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
