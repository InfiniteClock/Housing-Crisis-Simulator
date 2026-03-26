using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [Header("Budget Resource Status")]
    public float initialBudget;
    public decimal currentBudget;

    [Space(10)]
    [Header("UI Objects")]
    public TextMeshProUGUI budgetText;

    private string newText;
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
        currentBudget = (decimal)initialBudget;
        UpdateBudgetUI();
    }


    //make the text UI update the current money
    private void UpdateBudgetUI()
    {
        newText = (currentBudget / 1000).ToString();
        if (newText.Length < 3)
        {
            newText = newText + ".0";
        }
        if (currentBudget <= 9999 && newText.Length < 4)
        {
            newText =  newText + "0";
        }
        budgetText.text = newText + "M";
    }

    //reduce money
    public void SpendBudget(float money)
    {
        currentBudget -= (decimal)money;
        UpdateBudgetUI();
    }

    //increase money
    public void AddBudget(float money)
    {
        currentBudget += (decimal)money;
        UpdateBudgetUI();
    }

}
