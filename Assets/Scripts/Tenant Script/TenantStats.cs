using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TenantStats : MonoBehaviour
{
    [Header("Tenant Status")]
    public int familyNumber;
    public float rentBudget;

    [Space(10)]
    [Header("UI Objects")]
    public TextMeshProUGUI budgetText;

    private bool isHappy;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PayRent()
    {
        float rentBudgetInK = rentBudget / 1000;
        ResourceManager.Instance.AddBudget(rentBudgetInK);
    }
}
