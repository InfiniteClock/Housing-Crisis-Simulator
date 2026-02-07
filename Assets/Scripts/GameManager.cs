using Unity.Cinemachine;
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
    #region Static Variables
    public static GameManager instance;
    
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

    private void Start()
    {
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
}
