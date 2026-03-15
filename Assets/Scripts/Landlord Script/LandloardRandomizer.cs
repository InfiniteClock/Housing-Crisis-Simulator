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
    private List<GameObject> currentSpawnedLandlords;

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
        foreach (GameObject landlord in landlordList)
        {
            SimplifiedDrag drag = landlord.GetComponent<SimplifiedDrag>();
            drag.Canvas = phase2Canvas;
            drag.isToggleDrag = isToggleDrag;
        }

        landlordPool = new List<GameObject>(landlordList);
        currentSpawnedLandlords = new List<GameObject>();
    }

    public void RandomSpawnLandlord()
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

            GameObject spawnedLandloard = Instantiate(landlordPool[randomIndex], spawnPoint);

            //reset the shape position
            RectTransform rectTransfrom = spawnedLandloard.GetComponent<RectTransform>();
            rectTransfrom.anchoredPosition = Vector2.zero;

            spawnedLandloard.transform.SetParent(phase2Canvas.transform, true);

            //record current position
            spawnedLandloard.GetComponent<SimplifiedDrag>().RecordReturnPosiiton();

            landlordPool.RemoveAt(randomIndex);
            currentSpawnedLandlords.Add(spawnedLandloard);
        }
    }

    public void ResetLandlords()
    {
        foreach (GameObject landlord in currentSpawnedLandlords)
        {
            Destroy(landlord);
        }

        currentSpawnedLandlords.Clear();

    }
}
