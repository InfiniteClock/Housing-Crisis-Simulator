using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drag : MonoBehaviour, IPointerDownHandler, IBeginDragHandler,IEndDragHandler, IDragHandler, IPointerClickHandler
{
    [SerializeField] private Canvas canvas;
    public List<GameObject> blockLists;

    [Header("Drag obejct options")]
    [SerializeField] private bool isToggleDrag = false;
    [SerializeField] private bool useQEToRotate = false;



    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 returnPoint;
    private bool isSnapped;
    private bool canBePlaced;
    private bool isSelected;
    private bool isFollowingMouse = false;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        returnPoint = rectTransform.anchoredPosition;
        isSnapped = false;
        canBePlaced = true;
        isSelected = false;
    }


    //_____Toggle__________________________________________________________________
    //public void Update()
    //{
    //    //update the obejct position under toggle mode
    //    if (isFollowingMouse)
    //    {
    //        //make the object always centered on the mouse
    //        MouseFollow(Input.mousePosition);
    //    }
    //}

    public void OnPointerClick(PointerEventData eventData)
    {
        //disable this if not toggle
        if (!isToggleDrag) return;


        isFollowingMouse = !isFollowingMouse;

        if (isFollowingMouse)
        {
            //make the object follow the mouse after click once
            canvasGroup.alpha = 0.8f;
            canvasGroup.blocksRaycasts = false;
            isSnapped = false;
            canBePlaced = true;
            //make selected object rendered at the front layer
            rectTransform.SetAsLastSibling();

        }
        else
        {
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
            if (!isSnapped || !canBePlaced)
            {
                rectTransform.anchoredPosition = returnPoint;
            }
        }

    }


    //_____Hold__________________________________________________________________
    public void Update()
    {
        if (isSelected)
        {
            ShapeRotation(rectTransform);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //disable this if toggle
        if (isToggleDrag) return;

        //Center the object to the mouse position
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, null, out Vector2 localPoint))
        {
            rectTransform.anchoredPosition = localPoint;
            //Debug.Log("Position Zero!");
        }
        isSelected = true;
        //rectTransform.anchoredPosition = eventData.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //disable this if toggle
        if (isToggleDrag) return;

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
        //disable this if toggle
        if (isToggleDrag) return;

        //Make the obejct move with the mouse offset
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        
        //Debug.Log("OnDrag");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //disable this if toggle
        if (isToggleDrag) return;

        //Restore the obejct
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        //Debug.Log("OnEndGrag");
        if (!isSnapped || !canBePlaced)
        {
            rectTransform.anchoredPosition = returnPoint;
        }
        isSelected = false;
    }

    //_____Functions__________________________________________________________________
    public void CheckBlockSnap()
    {
        //disable this if toggle
        if (isToggleDrag) return;

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

    public void SnapFunction(RectTransform slot)
    {
        //public function called by the Drop script
        rectTransform.anchoredPosition = slot.anchoredPosition;
        CheckBlockSnap();
        CheckBlockOverlap();
    }

    //public void MouseFollow(Vector2 position)
    //{
    //    //called when the toggle option is on, make obejct follow the mouse
    //    //Center the object to the mouse position
    //    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, position, null, out Vector2 localPoint))
    //    {
    //        rectTransform.anchoredPosition = localPoint;
    //        //Debug.Log("Position Zero!");
    //    }
    //}

    public void ShapeRotation(RectTransform shape)
    {
        float step = 90f;
        if (useQEToRotate)
        {
            //uses QE to controll rotation, Q is anti-clockwise and E is clockwise
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Vector3 rotation = shape.localEulerAngles;
                rotation.z += step;
                shape.localEulerAngles = rotation;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                Vector3 rotation = shape.localEulerAngles;
                rotation.z += step;
                shape.localEulerAngles = rotation;
            }
        }
        else if (!useQEToRotate)
        {
            //uses mouse scroll to controll rotation, up is anti-clockwise and down is clockwise
            float scroll = Input.mouseScrollDelta.y;
            if (scroll > 0)
            {
                Vector3 rotation = shape.localEulerAngles;
                rotation.z += step;
                shape.localEulerAngles = rotation;
            }
            if (scroll < 0)
            {
                Vector3 rotation = shape.localEulerAngles;
                rotation.z += step;
                shape.localEulerAngles = rotation;
            }
        }
    }

}
