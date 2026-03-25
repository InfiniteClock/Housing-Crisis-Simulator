using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TenantRandomizer : MonoBehaviour
{
    public static TenantRandomizer Instance;

    public Canvas phase3Canvas;
    public List<GameObject> lowIncomeTenantList;
    public List<GameObject> midIncomeTenantList;
    public List<GameObject> highIncomeTenantList;

    public List<Transform> spawnPointList;

    [Header("Drag object options")]
    public bool isToggleDrag = false;

    private List<GameObject> tenantPool;
    public List<GameObject> currentSpawnedTenants;

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

    void Start()
    {
        //assign canvas and drag option for tenants in three lists
        foreach (GameObject tennant in lowIncomeTenantList)
        {
            SimplifiedDrag drag = tennant.GetComponent<SimplifiedDrag>();
            drag.Canvas = phase3Canvas;
            drag.isToggleDrag = isToggleDrag;
        }
        foreach (GameObject tennant in midIncomeTenantList)
        {
            SimplifiedDrag drag = tennant.GetComponent<SimplifiedDrag>();
            drag.Canvas = phase3Canvas;
            drag.isToggleDrag = isToggleDrag;
        }
        foreach (GameObject tennant in highIncomeTenantList)
        {
            SimplifiedDrag drag = tennant.GetComponent<SimplifiedDrag>();
            drag.Canvas = phase3Canvas;
            drag.isToggleDrag = isToggleDrag;
        }

        tenantPool = new List<GameObject>();
        currentSpawnedTenants = new List<GameObject>();
    }

    public void SpawnLowIncome()
    {
        tenantPool = new List<GameObject>(lowIncomeTenantList);

        for (int i = 0; i < spawnPointList.Count; i++)
        {
            RandomSpawnTenant(i, 0);
        }


    }

    public void SpawnMidIncome()
    {
        tenantPool = new List<GameObject>(midIncomeTenantList);
        for (int i = 0; i < spawnPointList.Count; i++)
        {
            RandomSpawnTenant(i, 1);
        }
    }

    public void SpawnHighIncome()
    {
        tenantPool = new List<GameObject>(highIncomeTenantList);

        for (int i = 0; i < spawnPointList.Count; i++)
        {
            RandomSpawnTenant(i, 2);
        }
    }


    //0 = low income, 1 = mid income, 2 = high income
    public void RandomSpawnTenant(int i, int incomeLevel)
    {
        if (tenantPool.Count == 0)
        {
            if (incomeLevel == 0)
            {
                tenantPool = new List<GameObject>(lowIncomeTenantList);
            }
            else if (incomeLevel == 1)
            {
                tenantPool = new List<GameObject>(midIncomeTenantList);
            }
            else if (incomeLevel == 2)
            {
                tenantPool = new List<GameObject>(highIncomeTenantList);
            }
        }

        int randomIndex = Random.Range(0, tenantPool.Count);
        Transform spawnPoint = spawnPointList[i];

        

        GameObject placedTenant = Instantiate(tenantPool[randomIndex], spawnPoint);

        //reset the tenant position
        RectTransform rectTransfrom = placedTenant.GetComponent<RectTransform>();
        rectTransfrom.anchoredPosition = Vector2.zero;

        placedTenant.transform.SetParent(phase3Canvas.transform, true);

        placedTenant.GetComponent<SimplifiedDrag>().spawnPointIndex = i;
        placedTenant.GetComponent<SimplifiedDrag>().RecordReturnPosiiton();

        

        tenantPool.RemoveAt(randomIndex);
        currentSpawnedTenants.Add(placedTenant);
    }

    public void ResetTenant()
    {
        foreach(GameObject tenant in currentSpawnedTenants)
        {
            Destroy(tenant);
        }

        currentSpawnedTenants.Clear();

        Debug.Log("wwwww");
    }



}
