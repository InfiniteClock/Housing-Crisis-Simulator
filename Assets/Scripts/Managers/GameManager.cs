using System.Collections;
using System.ComponentModel;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum zoneType {LowIncome, MidIncome, HighIncome}
public enum tenantType {LowIncome, MidIncome, HighIncome}
public enum LandlordTraits {Empty, IncreaseRent, DecreaseRent, IncreaseHappiness, DecreaseHappiness}

public class GameManager : MonoBehaviour
{
    public enum gameState {Play, NoPlay, Tutorial, Message}
    public enum phaseState {City, Zone, Neighbourhood, Home, End}


    [Header("Basic Setup")]
    [SerializeField] private CanvasGroup p1City;
    [SerializeField] private CanvasGroup p2Zone;
    [SerializeField] private CanvasGroup p3Neighbourhood;
    [SerializeField] private CanvasGroup p4Home;
    [SerializeField] private float fadeTimeUI;
    [SerializeField] private float fadeDelayUI;
    [SerializeField] private Slider happyBar;

    public CinemachineCamera cityCam;
    public CinemachineCamera endCam;
    public int gridX = 8;
    public int gridY = 6;

    [Header("Map Reference")]
    #region Static Variables
    public static GameManager Instance { get; private set; }
    #region Stats
    // Happiness stat
    public static int happiness { get; private set; }
    public static void AdjustHappiness(int hap)
    {
        happiness += hap;
        Instance.happyBar.value = happiness;
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
    #endregion
    #region Game Controls
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
            case phaseState.End:
                // player click give up button or the budegt doesn't support more placement in phase 1
                Instance.StartCoroutine(Instance.CanvasFade(Instance.p1City, -Instance.fadeTimeUI));
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
    public static Zone currentZone { get; private set; }

    public static void ZoneUpdate(Zone zone)
    {
        currentZone = zone;
    }

    // Current Neighbourhood
    public static Neighbourhood currentNeighbourhood { get; private set; }
    public static void NeighbourhoodUpdate(Neighbourhood hood)
    {
        currentNeighbourhood = hood;
        int openHomes = 0;
        switch (currentZone.type)
        {
            case zoneType.LowIncome:
                openHomes = Random.Range(3, 8);
                break;
            case zoneType.MidIncome:
                openHomes = Random.Range(2, 6);
                break;
            case zoneType.HighIncome:
                openHomes = Random.Range(1, 4);
                break;
            default:
                Debug.LogError("Zone type not implemented properly.");
                break;
        }

        House[] shuff = Instance.HouseShuffle(currentNeighbourhood.Homes);
        // Make sure openhomes cannot be larger than total number of homes
        openHomes = Mathf.Min(openHomes, shuff.Length);
        activeHouses = new House[openHomes];
        for (int i = 0; i < openHomes; i++)
        {
            House h = currentNeighbourhood.Homes[i];
            h.currentState = HouseState.Default;
            h.SetDefMat();
            activeHouses[i] = h;
        }
    }
    // Current array of playable houses to fill with tenants
    public static House[] activeHouses { get; private set; }
    public static void FillHouse(TenantStats tenant)
    {
        bool foundIndex = false;
        House[] temp = new House[activeHouses.Length-1];

        // Searches through all active houses to find the current selection, sets it to interacted state, then updates the list
        for (int i = 0; i < activeHouses.Length; i++)
        {
            if (foundIndex)
            {
                temp[i-1] = activeHouses[i];
            }
            else {
                if (activeHouses[i] == currentHome)
                {
                    activeHouses[i].SetInteracted();

                    // Code to connect tenant to house goes here

                    foundIndex = true;
                }
                else
                {
                    temp[i] = activeHouses[i];
                }
            }
        }
        // Updates activeHouses if we found currentHome within
        if (foundIndex)
        {
            activeHouses = temp;
            Instance.OnCycleHouse(true);
        }
        else Debug.Log("Could not find selected house in active houses array");

        // Removes the tenant object once it is placed
        Destroy(tenant.gameObject);

        Debug.Log("House Filled!");
    }
    // Current House/Apartment
    public static House currentHome { get; private set; }
    public static void HomeUpdate(House home)
    {
        // Ensure that we are updating to a default state house only
        if (home.currentState != HouseState.Default && home.currentState != HouseState.Highlighted)
        {
            Debug.Log("That house cannot be selected!");
            return;
        }
        // Ensure that the selected home is within the active homes list - otherwise, ignore
        foreach (House h in activeHouses)
        {
            if (home == h)
            {
                currentHome = home;
                currentHome.SetHighlight();
                currentHome.SetPrice();
                CameraManager.CameraSwitch(currentHome.houseCam);
                return;
            }
        }
        Debug.Log("House is not in active houses list.");
    }
    public static void HomeLock()
    {
        currentHome.SetInteracted();
        // Change currentHome to next unselected home in list, or end phase if last one
    }
    #endregion
    #endregion
    [SerializeField]
    private GameObject[] slotParent3D;
    [SerializeField]
    private GameObject[] slotParents2D;
    public GameObject[,] grid3D { get; private set; }
    public GameObject[,] grid2D { get; private set; }
    public GameObject[,] gridPlaced { get; private set; }

    [Space(10)]
    [Header("Stats in current gameloop")]
    public bool isPhase3End;
    [SerializeField, ReadOnly(true)] private zoneType currentZoneType;
    [SerializeField, ReadOnly(true)] private float currentZonePrice;
    [SerializeField, ReadOnly(true)] private LandlordTraits trait1;
    [SerializeField, ReadOnly(true)] private LandlordTraits trait2;

    public zoneType CurrentZoneType => currentZoneType;
    public float CurrentZonePrice => currentZonePrice;
    public LandlordTraits Trait1 => trait1;
    public LandlordTraits Trait2 => trait2;


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
        //happiness = 0;
        //funds = 1000;
        //housedFamilies = 0;
        //housedPeople = 0;
        //filledHomes = 0;
        //totalHomes = 0;
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

    #region Commands
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

        // Move to phase 2
        ZoneUpdate(newZoneScript);
        PhaseUpdate(phaseState.Zone);
        CameraManager.CameraSwitch(newZoneScript.zoneCam);
        
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


    //public functions to set and get current data
    public void SetCurrentZoneType(zoneType zoneType)
    {
        currentZoneType = zoneType;
    }
    public void SetCurrentZonePrice(float zonePrice)
    {
        currentZonePrice = zonePrice;
    }
    public void SetTraits(LandlordTraits t1, LandlordTraits t2)
    {
        trait1 = t1;
        trait2 = t2;
    }

    // Public function for returning a randomly shuffled house array
    public House[] HouseShuffle(House[] homes)
    {
        House[] shuffledHomes = homes;
        // This is a standard Fisher Yates shuffle of the array
        for (int i = homes.Length-1; i >= 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (shuffledHomes[i], shuffledHomes[randomIndex]) = (shuffledHomes[randomIndex], shuffledHomes[i]);
        }

        // Returns the array with shuffled index
        return shuffledHomes;
    }


    /// <summary>
    /// Public function for cycling through active houses in phase 3
    /// </summary>
    /// <param name="upCycle">If true, cycle up. Otherwise, cycle down</param>
    public void OnCycleHouse(bool upCycle)
    {
        int index = 0;
        if (activeHouses.Length <= 1)
        {
            Debug.Log("Only 1 house left!");
        }
        // Only cycle up or down if there is more than 1 house left
        else
        {
            for (int i = 0; i < activeHouses.Length; i++)
            {
                if (activeHouses[i] == currentHome)
                {
                    index = i;
                    break;
                }
            }
            if (upCycle)
            {
                if (index >= activeHouses.Length - 1) index = 0;
                else index += 1;
            }
            else
            {
                if (index <= 0) index = activeHouses.Length - 1;
                else index -= 1;
            } 
        }
        // If current house is still empty, unhighlight it before moving on
        if (currentHome != null)
            if (currentHome.currentState == HouseState.Highlighted)
                currentHome.SetDefMat();
        if (activeHouses.Length > 0)
        {
            // Update current home to next in active homes array
            HomeUpdate(activeHouses[index]);
        }
        Debug.Log("Active house index in list: "+index);
    }

    public void GameEnd()
    {
        PhaseUpdate(phaseState.End);
        CameraManager.CameraSwitch(endCam);
    }
    #endregion
}
