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
    private List<GameObject> currentSpawnedTenants;

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
        
    }

    public void SpawnTenantMidIncome()
    {
        tenantPool = new List<GameObject>(midIncomeTenantList);
    }

    public void SpawnHighIncome()
    {
        tenantPool = new List<GameObject>(highIncomeTenantList);
    }

    public void RandomSpawnTenant(int i)
    {
        //what the fuck to write in here!!!!!!!!!


    }



}
