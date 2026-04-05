using TMPro;
using UnityEngine;

public class TenantStats : MonoBehaviour
{
    [Header("Tenant Status")]
    public tenantType tenantIncomeLevel;
    public float familyNumber;
    public int rentBudget;

    [Space(10)]
    [Header("UI Objects")]
    public TextMeshProUGUI budgetText;

    public void Awake()
    {
        SelectBudget();
    }

    public void PayRent()
    {
        float rentBudgetInK = rentBudget / 1000;
        ResourceManager.Instance.AddBudget(rentBudgetInK);
    }

    public void UpdateBudgetText(TextMeshProUGUI ui, int rentBudget)
    {
        ui.text = "$" + rentBudget.ToString();
    }

    public void SelectBudget()
    {
        int randomNumb = 0;
        switch (tenantIncomeLevel)
        {
            case tenantType.LowIncome:
                randomNumb = Random.Range(4, 12);
                break;
            case tenantType.MidIncome:
                randomNumb = Random.Range(8, 16);
                break;
            case tenantType.HighIncome:
                randomNumb = Random.Range(12, 20);
                break;
        }
        rentBudget = randomNumb * 100;
        UpdateBudgetText(budgetText, rentBudget);
    }
}
