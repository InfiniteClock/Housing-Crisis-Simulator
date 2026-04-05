using TMPro;
using System.Collections.Generic;
using UnityEngine;

public class ShapeRandomizer : MonoBehaviour
{
    public static ShapeRandomizer Instance;

    public Canvas phase1Canvas;
    public List<GameObject> shapeList;
    public List<Transform> spawnPointList;

    public int zonePrice1;
    public int zonePrice2;
    public int zonePrice3;
    public zoneType zoneType1;
    public zoneType zoneType2;
    public zoneType zoneType3;

    [Header("Drag object options")]
    public bool isToggleDrag = false;
    public bool useScrollToRotate = false;

    [Header("UI Objects")]
    public TextMeshProUGUI zonePriceSlot1;
    public TextMeshProUGUI zonePriceSlot2;
    public TextMeshProUGUI zonePriceSlot3;
    public GameObject zoneTypeSlot1;
    public GameObject zoneTypeSlot2;
    public GameObject zoneTypeSlot3;
    public List<Sprite> typeList;
    public GameObject disableIcon1;
    public GameObject disableIcon2;
    public GameObject disableIcon3;

    private List<GameObject> shapePool;
    public List<GameObject> spawnedShapes;

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
        foreach (GameObject shape in shapeList)
        {
            Drag drag = shape.GetComponent<Drag>();
            drag.Canvas = phase1Canvas;
            drag.isToggleDrag = isToggleDrag;
            drag.useScrollToRotate = useScrollToRotate;
        }
        //create an copy of the shape list
        shapePool = new List<GameObject>(shapeList);

        SpawnShapes();
    }


    public void SpawnShapes()
    {
        foreach (GameObject g in spawnedShapes)
        {
            if (g  != null)
                Destroy(g);
        }
        for (int i = 0; i < spawnPointList.Count; i++)
        {
            RandomSpawnShape(i);
        }
    }

    public void RandomSpawnShape(int i)
    {
        //refill the pool if it is empty
        if (shapePool.Count == 0)
        {
            shapePool = new List<GameObject>(shapeList);
        }

        int randomIndex = Random.Range(0, shapePool.Count);
        Transform spawnPoint = spawnPointList[i];

        //spawn the shape
        GameObject placedShape = Instantiate(shapePool[randomIndex], spawnPoint);
        

        //reset the shape position
        RectTransform rectTransfrom = placedShape.GetComponent<RectTransform>();
        rectTransfrom.anchoredPosition = Vector2.zero;
        

        //find the spawnPoint after adding the offset
        Vector3 spawnPositionOffset = shapePool[randomIndex].GetComponent<Drag>().spawnPositionOffset;
        placedShape.transform.position = placedShape.transform.position + spawnPositionOffset;

        placedShape.transform.SetParent(phase1Canvas.transform, true);

        //pass the index value and record current position
        placedShape.GetComponent<Drag>().spawnPointIndex = i;
        placedShape.GetComponent<Drag>().RecordReturnPosiiton();

        //display the zone information
        ShowZoneInfo(i, placedShape);

        //remove it from the pool
        shapePool.RemoveAt(randomIndex);

        if (placedShape.GetComponent<ZoneStats>().zonePrice > (float)ResourceManager.Instance.currentBudget)
        {
            placedShape.GetComponent<Drag>().enabled = false;
        }

        //Add new shape to spawnedShapes list
        spawnedShapes.Add(placedShape);
    }

    private void ShowZoneInfo(int index, GameObject placedShape)
    {
        if (index == 0)
        {
            zonePrice1 = placedShape.GetComponent<ZoneStats>().zonePrice;
            zonePriceSlot1.text = BudgetUnitConvert(zonePrice1);
            zoneType1 = placedShape.GetComponent<ZoneStats>().zoneIncomeType;
            zoneTypeSlot1.GetComponent<UnityEngine.UI.Image>().sprite = GetZoneTypeName(zoneType1);
            if (zonePrice1 > ResourceManager.Instance.currentBudget)
            {
                disableIcon1.SetActive(true);
            }
            else
            {
                disableIcon1.SetActive(false);
            }
        }

        if (index == 1)
        {
            zonePrice2 = placedShape.GetComponent<ZoneStats>().zonePrice;
            zonePriceSlot2.text = BudgetUnitConvert(zonePrice2);
            zoneType2 = placedShape.GetComponent<ZoneStats>().zoneIncomeType;
            zoneTypeSlot2.GetComponent<UnityEngine.UI.Image>().sprite = GetZoneTypeName(zoneType2);
            if (zonePrice2 > ResourceManager.Instance.currentBudget)
            {
                disableIcon2.SetActive(true);
            }
            else
            {
                disableIcon2.SetActive(false);
            }
        }

        if (index == 2)
        {
            zonePrice3 = placedShape.GetComponent<ZoneStats>().zonePrice;
            zonePriceSlot3.text = BudgetUnitConvert(zonePrice3);
            zoneType3 = placedShape.GetComponent<ZoneStats>().zoneIncomeType;
            zoneTypeSlot3.GetComponent<UnityEngine.UI.Image>().sprite = GetZoneTypeName(zoneType3);
            if (zonePrice3 > ResourceManager.Instance.currentBudget)
            {
                disableIcon3.SetActive(true);
            }
            else
            {
                disableIcon3.SetActive(false);
            }
        }
    }

    public void RemovePlacedShapeFromList(GameObject shape)
    {
        spawnedShapes.Remove(shape);
    }

    //formatting the zone type name
    public Sprite GetZoneTypeName(zoneType zoneTypeKind)
    {
        switch (zoneTypeKind)
        {
            case zoneType.LowIncome:
                return typeList[0];

            case zoneType.MidIncome:
                return typeList[1];

            case zoneType.Highrise:
                return typeList[2];

            case zoneType.HighIncome:
                return typeList[2];
        }
        return typeList[0];
    }

    public string BudgetUnitConvert(int zonePrice)
    {
        string newText = (zonePrice / 1000f).ToString();
        if (newText.Length < 3)
        {
            newText = newText + ".0";
        }
        if (zonePrice <= 9999 && newText.Length < 4)
        {
            newText = newText + "0";
        }
        return newText + "M";
    }

    //public void ShowDisableIcon(int index)
    //{
    //    switch(index)
    //    {
    //        case 0:
    //            disableIcon1.SetActive(true);
    //            break;
    //        case 1: 
    //            disableIcon2.SetActive(true); 
    //            break;
    //        case 2:
    //            disableIcon3.SetActive(true);
    //            break;

    //    }
    //}
}
