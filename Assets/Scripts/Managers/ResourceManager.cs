using TMPro;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [Header("Resource Status")]
    public float initialBudget;
    public float currentBudget;

    [Space(10)]
    [Header("UI Objects")]
    public TextMeshProUGUI budgetText;

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
    }
    void Update()
    {

    }

    private void UpdateBudgetUI()
    {
        budgetText.text = "$" + currentBudget.ToString() + "K";
    }

    public void SpendBudget(float money)
    {
        currentBudget -= money;
        UpdateBudgetUI();
    }

    public void AddBudget(float money)
    {
        currentBudget += money;
        UpdateBudgetUI();
    }
}
