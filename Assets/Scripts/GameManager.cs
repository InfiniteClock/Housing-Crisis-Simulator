using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum gameState { City, Zone, Neighborhood, Home }

    public static GameManager instance;
    public static gameState currentState;
    public static int happiness;
    public static int funds;
    public static int housedFamilies;
    public static int housedPeople;
    public static int filledHomes;
    public static int totalHomes;

    // Reference to Funds UI text
    // Reference to Happiness UI text
    // Reference to Happiness UI Slider


}
