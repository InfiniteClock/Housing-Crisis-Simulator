using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class LandloardRandomizer : MonoBehaviour
{
    public static LandloardRandomizer Instance;

    public Canvas phase2Canvas;
    public List<GameObject> landlordList;
    public List<Transform> spawnPointList;

    [Header("Drag object options")]
    public bool isToggleDrag = false;

    private List<GameObject> landlordPool;
    private List<GameObject> currentPlacedLandlords;

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
        foreach (GameObject shape in landlordList)
        {
            SimplifiedDrag drag = shape.GetComponent<SimplifiedDrag>();
            drag.Canvas = phase2Canvas;
            drag.isToggleDrag = isToggleDrag;
        }

        landlordPool = new List<GameObject>(landlordList);
        currentPlacedLandlords = new List<GameObject>();
    }

    public void RandomSpawnLandloard()
    {
        landlordPool = new List<GameObject>(landlordList);

        for (int i = 0; i < spawnPointList.Count; i++)
        {
            if (landlordPool.Count == 0)
            {
                landlordPool = new List<GameObject>(landlordList);
            }

            int randomIndex = Random.Range(0, landlordPool.Count);
            Transform spawnPoint = spawnPointList[i];

            GameObject placedLandloard = Instantiate(landlordPool[randomIndex], spawnPoint);

            //reset the shape position
            RectTransform rectTransfrom = placedLandloard.GetComponent<RectTransform>();
            rectTransfrom.anchoredPosition = Vector2.zero;

            placedLandloard.transform.SetParent(phase2Canvas.transform, true);

            //record current position
            placedLandloard.GetComponent<SimplifiedDrag>().RecordReturnPosiiton();

            landlordPool.RemoveAt(randomIndex);
            currentPlacedLandlords.Add(placedLandloard);
        }
    }

    public void ResetLandlords()
    {
        foreach (GameObject landlord in currentPlacedLandlords)
        {
            Destroy(landlord);
        }

        currentPlacedLandlords.Clear();

    }
}
