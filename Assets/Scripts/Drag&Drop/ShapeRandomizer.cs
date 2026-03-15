using NUnit.Framework;
using TMPro;
using System.Collections.Generic;
using UnityEngine;

public class ShapeRandomizer : MonoBehaviour
{
    public static ShapeRandomizer Instance;

    public Canvas phase1Canvas;
    public List<GameObject> shapeList;
    public List<Transform> spawnPointList;

    public TextMeshProUGUI zonePriceSlot1;
    public TextMeshProUGUI zonePriceSlot2;
    public TextMeshProUGUI zonePriceSlot3;

    [Header("Drag object options")]
    public bool isToggleDrag = false;
    public bool useScrollToRotate = false;

    private List<GameObject> shapePool;
    private Transform newSpawnPoint;

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

        //spawn the shape and remove it from the pool
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

        shapePool.RemoveAt(randomIndex);

    }

    private void ShowZonePrice(int index, GameObject placedShape)
    {
        if (index == 0)
        {
            int price = placedShape.GetComponent<Drag>().zoneCost;
            zonePriceSlot1.text = "$" + price.ToString() + "K";
        }

        if (index == 1)
        {
            int price = placedShape.GetComponent<Drag>().zoneCost;
            zonePriceSlot2.text = "$" + price.ToString() + "K";
        }

        if (index == 2)
        {
            int price = placedShape.GetComponent<Drag>().zoneCost;
            zonePriceSlot3.text = "$" + price.ToString() + "K";
        }
    }
}
