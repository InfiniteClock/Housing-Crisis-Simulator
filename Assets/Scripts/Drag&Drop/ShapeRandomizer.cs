using NUnit.Framework;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShapeRandomizer : MonoBehaviour
{
    public static ShapeRandomizer Instance;

    public Canvas phase1Canvas;
    public List<GameObject> shapeList;
    public List<Transform> spawnPointList;

    public int zonePrice1;
    public int zonePrice2;
    public int zonePrice3;

    [Header("Drag object options")]
    public bool isToggleDrag = false;
    public bool useScrollToRotate = false;

    [Header("UI Objects")]
    public TextMeshProUGUI zonePriceSlot1;
    public TextMeshProUGUI zonePriceSlot2;
    public TextMeshProUGUI zonePriceSlot3;
    public TextMeshProUGUI zoneTypeSlot1;
    public TextMeshProUGUI zoneTypeSlot2;
    public TextMeshProUGUI zoneTypeSlot3;
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

        SpawnSceneStart();
    }


    public void SpawnSceneStart()
    {
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

        //display the zone price
        ShowZonePrice(i, placedShape);

        //remove it from the pool
        shapePool.RemoveAt(randomIndex);

        if (placedShape.GetComponent<ZoneStats>().zonePrice > (float)ResourceManager.Instance.currentBudget)
        {
            int index = placedShape.GetComponent<Drag>().spawnPointIndex;
            ShowDisableIcon(index);
            placedShape.GetComponent<Drag>().enabled = false;
        }

        //Add new shape to spawnedShapes list
        spawnedShapes.Add(placedShape);
    }

    private void ShowZonePrice(int index, GameObject placedShape)
    {
        if (index == 0)
        {
            zonePrice1 = placedShape.GetComponent<ZoneStats>().zonePrice;
            zonePriceSlot1.text = "$" + zonePrice1.ToString() + "K";
        }

        if (index == 1)
        {
            zonePrice2 = placedShape.GetComponent<ZoneStats>().zonePrice;
            zonePriceSlot2.text = "$" + zonePrice2.ToString() + "K";
        }

        if (index == 2)
        {
            zonePrice3 = placedShape.GetComponent<ZoneStats>().zonePrice;
            zonePriceSlot3.text = "$" + zonePrice3.ToString() + "K";
        }
    }

    public void RemovePlacedShapeFromList(GameObject shape)
    {
        spawnedShapes.Remove(shape);
    }

    public void ShowDisableIcon(int index)
    {
        switch(index)
        {
            case 0:
                disableIcon1.SetActive(true);
                break;
            case 1: 
                disableIcon2.SetActive(true); 
                break;
            case 2:
                disableIcon3.SetActive(true);
                break;

        }
    }
}
