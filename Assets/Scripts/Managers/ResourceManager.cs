using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [Header("Budget Resource Status")]
    public float initialBudget;
    public float currentBudget;

    [Space(10)]
    [Header("Happiness Resource Status")]
    public float currentHappiness;
    public float happinessChangeMultiplier;

    [Space(10)]
    [Header("UI Objects")]
    public TextMeshProUGUI budgetText;
    public Slider happinessSlider;

    public bool isEndPhase3; 

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
        currentBudget = initialBudget;
        UpdateBudgetUI();
        currentHappiness = happinessSlider.value;
    }
    void Update()
    {

    }

    //make the text UI update the current money
    private void UpdateBudgetUI()
    {
        budgetText.text = "$" + currentBudget.ToString() + "K";
    }

    //reduce money
    public void SpendBudget(float money)
    {
        currentBudget -= money;
        UpdateBudgetUI();
    }

    //increase money
    public void AddBudget(float money)
    {
        currentBudget += money;
        UpdateBudgetUI();
    }

    public void DecreaseHappiness(float happinessValue)
    {
        float happinessChange = happinessValue * happinessChangeMultiplier;
        currentHappiness-= happinessChange;

        happinessSlider.value = currentHappiness;
    }

    public void IncreaseHappiness(float happinessValue)
    {
        float happinessChange = happinessValue * happinessChangeMultiplier;
        currentHappiness += happinessChange;

        happinessSlider.value = currentHappiness;
    }
}
