using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drag : MonoBehaviour, IPointerDownHandler, IBeginDragHandler,IEndDragHandler, IDragHandler
{
    [SerializeField] private Canvas canvas;
    public List<GameObject> blockLists;

    [Header("Drag obejct options")]
       

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 returnPoint;
    private bool isSnapped;
    private bool canBePlaced;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        returnPoint = rectTransform.anchoredPosition;
        isSnapped = false;
        canBePlaced = true;
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        //Debug.Log("OnPointerDown");

        //Center the object to the mouse position
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, null, out Vector2 localPoint))
        {
            rectTransform.anchoredPosition = localPoint;
            //Debug.Log("Position Zero!");
        }

        //rectTransform.anchoredPosition = eventData.position;
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        //Make object transparent with effects allowing mouse raycasting
        canvasGroup.alpha = 0.8f;
        canvasGroup.blocksRaycasts = false;
        isSnapped = false;
        canBePlaced = true;
        //make selected object rendered at the front layer
        rectTransform.SetAsLastSibling();
        //Debug.Log("OnBeginGrag");
    }


    public void OnDrag(PointerEventData eventData)
    {
        //Make the obejct move with the mouse offset
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        //Debug.Log("OnDrag");
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        //Restore the obejct
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        //Debug.Log("OnEndGrag");


        if (!isSnapped || !canBePlaced)
        {
            rectTransform.anchoredPosition = returnPoint;
        }
    }


    public void CheckBlockSnap()
    {
        //this function is used to check if all the blocks in shape is inside the map
        isSnapped = true;

        foreach (GameObject block in blockLists)
        {
            Vector2 blockPosition = block.transform.position;

            //Created an array of all the colliders in the overlapbox
            Collider2D[] hits = Physics2D.OverlapBoxAll(blockPosition, new Vector2(1f, 1f), 0f);

            bool blockIsSnapped = false;
            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("Slot"))
                {
                    //if there is a collider under the box is tagged with Slot, the shape is in range
                    blockIsSnapped = true;
                    break;
                }
            }

            //if there isn't any collider tagged with Slot, break the block foreach and reset position
            if (!blockIsSnapped)
            {
                isSnapped = false;
                break;
            }
        }
    }

    public void CheckBlockOverlap()
    {
        //this function is prevent shapes overlap eachother using the same logic
        canBePlaced = true;

        foreach (GameObject block in blockLists)
        {
            Vector2 blockPosition = block.transform.position;

            //Created an array of all the colliders in the overlapbox
            Collider2D[] hits = Physics2D.OverlapBoxAll(blockPosition, new Vector2(1f, 1f), 0f);

            bool blockIsOverlap = false;
            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("Block") && hit.gameObject != block)
                {
                    //if there is a collider under the box is tagged with Block, the shape cannot be placed
                    blockIsOverlap = true;
                    Debug.Log("is overlap");
                    break;
                }
            }

            //if triggers when the block is overlapping, thus cannot place shape
            if (blockIsOverlap)
            {
                canBePlaced = false;
                Debug.Log("cannot place");
                break;
            }
        }
    }


    //public function called by the Drop script
    public void SnapFunction(RectTransform slot)
    {
        rectTransform.anchoredPosition = slot.anchoredPosition;
        CheckBlockSnap();
        CheckBlockOverlap();
    }

    
}
