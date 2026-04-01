using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
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
    [SerializeField] private GameObject resourcePrefab;
    [SerializeField] private Canvas interphaseUI;
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

        switch (GameManager.Instance.CurrentZoneType)
        {
            case zoneType.LowIncome:
                for (int i = 0; i < 2; i++) 
                {
                    GameObject resource = Instantiate(resourcePrefab);
                    resource.GetComponent<ResourceParticle>().spawnRadius = 100;
                    resource.GetComponent<ResourceParticle>().Spawn(
                        ResourceType.Cost,
                        Mouse.current.position.ReadValue(), 
                        budgetText.transform.position,
                        Instance.interphaseUI); 
                }
                break;
            case zoneType.MidIncome:
                for (int i = 0; i < 4; i++)
                {
                    GameObject resource = Instantiate(resourcePrefab);
                    resource.GetComponent<ResourceParticle>().spawnRadius = 100;
                    resource.GetComponent<ResourceParticle>().Spawn(
                        ResourceType.Cost,
                        Mouse.current.position.ReadValue(),
                        budgetText.transform.position,
                        Instance.interphaseUI);
                }
                break;
            case zoneType.Highrise:
                for (int i = 0; i < 4; i++)
                {
                    GameObject resource = Instantiate(resourcePrefab);
                    resource.GetComponent<ResourceParticle>().spawnRadius = 100;
                    resource.GetComponent<ResourceParticle>().Spawn(
                        ResourceType.Cost,
                        Mouse.current.position.ReadValue(),
                        budgetText.transform.position,
                        Instance.interphaseUI);
                }
                break;
            case zoneType.HighIncome:
                for (int i = 0; i < 6; i++)
                {
                    GameObject resource = Instantiate(resourcePrefab);
                    resource.GetComponent<ResourceParticle>().spawnRadius = 100;
                    resource.GetComponent<ResourceParticle>().Spawn(
                        ResourceType.Cost,
                        Mouse.current.position.ReadValue(),
                        budgetText.transform.position,
                        Instance.interphaseUI);
                }
                break;
            default:
                break;
        }
        UpdateBudgetUI();
    }

    //increase money
    public void AddBudget(float money)
    {
        currentBudget += (decimal)money;
        if (GameManager.currentPhase == GameManager.phaseState.Neighbourhood)
        {
            switch (GameManager.Instance.CurrentZoneType)
            {
                case zoneType.LowIncome:
                    for (int i = 0; i < 1; i++)
                    {
                        GameObject resource = Instantiate(resourcePrefab);
                        resource.GetComponent<ResourceParticle>().Spawn(
                            ResourceType.Profit,
                            budgetText.transform.position,
                            Mouse.current.position.ReadValue(),
                            Instance.interphaseUI);
                    }
                    break;
                case zoneType.MidIncome:
                    for (int i = 0; i < 2; i++)
                    {
                        GameObject resource = Instantiate(resourcePrefab);
                        resource.GetComponent<ResourceParticle>().Spawn(
                            ResourceType.Profit,
                            budgetText.transform.position,
                            Mouse.current.position.ReadValue(),
                            Instance.interphaseUI);
                    }
                    break;
                case zoneType.Highrise:
                    for (int i = 0; i < 2; i++)
                    {
                        GameObject resource = Instantiate(resourcePrefab);
                        resource.GetComponent<ResourceParticle>().Spawn(
                            ResourceType.Profit,
                            budgetText.transform.position,
                            Mouse.current.position.ReadValue(),
                            Instance.interphaseUI);
                    }
                    break;
                case zoneType.HighIncome:
                    for (int i = 0; i < 3; i++)
                    {
                        GameObject resource = Instantiate(resourcePrefab);
                        resource.GetComponent<ResourceParticle>().Spawn(
                            ResourceType.Profit,
                            budgetText.transform.position,
                            Mouse.current.position.ReadValue(),
                            Instance.interphaseUI);
                    }
                    break;
                default:
                    break;
            }
        }
        else if (GameManager.currentPhase == GameManager.phaseState.Home) 
        { 
            if (GameManager.performance > 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    GameObject resource = Instantiate(resourcePrefab);
                    resource.GetComponent<ResourceParticle>().Spawn(
                        ResourceType.Profit,
                        budgetText.transform.position,
                        Mouse.current.position.ReadValue(),
                        Instance.interphaseUI);
                }
            }
            else if (GameManager.performance == 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    GameObject resource = Instantiate(resourcePrefab);
                    resource.GetComponent<ResourceParticle>().Spawn(
                        ResourceType.Profit,
                        budgetText.transform.position,
                        Mouse.current.position.ReadValue(),
                        Instance.interphaseUI);
                }
            }
            else if (GameManager.performance > -5)
            {
                for (int i = 0; i < 2; i++)
                {
                    GameObject resource = Instantiate(resourcePrefab);
                    resource.GetComponent<ResourceParticle>().Spawn(
                        ResourceType.Profit,
                        budgetText.transform.position,
                        Mouse.current.position.ReadValue(),
                        Instance.interphaseUI);
                }
            }
            else
            {
                GameObject resource = Instantiate(resourcePrefab);
                resource.GetComponent<ResourceParticle>().Spawn(
                    ResourceType.Profit,
                    budgetText.transform.position,
                    Mouse.current.position.ReadValue(),
                    Instance.interphaseUI);
            }
        }

            UpdateBudgetUI();
    }

}
