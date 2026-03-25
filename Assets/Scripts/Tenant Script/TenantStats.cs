using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TenantStats : MonoBehaviour
{
    [Header("Tenant Status")]
    public tenantType tenantIncomeLevel;
    public float familyNumber;
    public float rentBudget;

    [Space(10)]
    [Header("UI Objects")]
    public TextMeshProUGUI budgetText;

    private bool isHappy;

    public void Awake()
    {
        UpdateBudgetText(budgetText, rentBudget);
    }

    public void PayRent()
    {
        float rentBudgetInK = rentBudget / 1000;
        ResourceManager.Instance.AddBudget(rentBudgetInK);
    }

    public void UpdateBudgetText(TextMeshProUGUI ui, float rentBudget)
    {
        ui.text = "$" + rentBudget.ToString();
    }
}
