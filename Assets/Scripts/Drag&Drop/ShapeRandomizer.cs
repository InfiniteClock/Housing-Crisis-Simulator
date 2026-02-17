using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ShapeRandomizer : MonoBehaviour
{
    public static ShapeRandomizer Instance;

    public Canvas phase1Canvas;
    public List<GameObject> shapeList;
    public List<Transform> spawnPointList;

    [Header("Drag object options")]
    public bool isToggleDrag = false;
    public bool useScrollToRotate = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;
        foreach (GameObject shape in shapeList)
        {
            Drag drag = shape.GetComponent<Drag>();
            drag.Canvas = phase1Canvas;
            drag.isToggleDrag = isToggleDrag;
            drag.useScrollToRotate = useScrollToRotate;
        }

        SpawnSceneStart();
    }


    public void SpawnSceneStart()
    {
        for(int i = 0; i < spawnPointList.Count; i++)
        {
            RandomSpawnShape(i);
        }
    }

    public void RandomSpawnShape(int i)
    {
        int randomIndex = Random.Range(0, shapeList.Count);
        Transform spawnPoint = spawnPointList[i];
        GameObject placedShape = Instantiate(shapeList[randomIndex], spawnPoint);

        //reset the shape position
        RectTransform rectTransfrom = placedShape.GetComponent<RectTransform>();
        rectTransfrom.anchoredPosition = Vector2.zero;
        placedShape.transform.SetParent(phase1Canvas.transform, true);

        //pass the index value and record current position
        placedShape.GetComponent<Drag>().spawnPointIndex = i;
        placedShape.GetComponent<Drag>().RecordReturnPosiiton();

        
    }
}
