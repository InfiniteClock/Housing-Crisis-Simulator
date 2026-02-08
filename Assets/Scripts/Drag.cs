using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;

public class Drag : MonoBehaviour, IPointerDownHandler, IBeginDragHandler,IEndDragHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private Canvas canvas;
    public List<GameObject> blockLists;

    [Header("Drag obejct options")]
    public bool isToggleDrag = false;
    [SerializeField] private bool useScrollToRotate = false;

    [HideInInspector]
    public bool canBeInteracte = true;



    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 returnPoint;
    private Vector3 oringinRotation;
    private Vector3 currentRotation;
    private bool isSnapped;
    private bool canBePlaced;
    private bool isSelected;
    private bool isFollowingMouse = false;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        returnPoint = rectTransform.anchoredPosition;
        oringinRotation = rectTransform.localEulerAngles;
        isSnapped = false;
        canBePlaced = true;
        isSelected = false;
    }

    public void Update()
    {
        //shape rotation
        if (isSelected)
        {
            ShapeRotation(rectTransform);
            currentRotation = rectTransform.localEulerAngles;
        }

        //Toggle logic

        if (!isToggleDrag) return;

        ToggleLogic();

    }

    //_____Toggle__________________________________________________________________
    public void ToggleLogic()
    {
        if (!canBeInteracte) return;

        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.transform as RectTransform, Input.mousePosition,null,out Vector3 worldPoint);
        Vector2 mousePosition = (Vector2)worldPoint;
        Collider2D hit = Physics2D.OverlapBox(mousePosition, new Vector2(20f, 20f), 0f);

        if (isFollowingMouse)
        {
            ToggleFollow();
        }

        if (Input.GetMouseButtonDown(0))
        {
            //reset shape position when clcik the blank space
            if(hit == null)
            {
                if (isFollowingMouse)
                {
                    ToggleReturn();
                    isFollowingMouse = false;
                }
            }

            //if there is a block under mouse
            else if (hit.CompareTag("Block"))
            {
                //get the current shape
                Drag hitdrag = hit.GetComponentInParent<Drag>();
                if (hitdrag != this)
                {
                    //if click on other shapes, it return the shape
                    if (isFollowingMouse)
                    {
                        ToggleReturn();
                        isFollowingMouse = false;
                    }
                    return;
                }

                if (!isFollowingMouse)
                {
                    //if click on this very shape, select this one and start drag shape
                    ToggleSelectDrag();
                    isFollowingMouse = true;
                }
                else
                {
                    //return the shape
                    ToggleReturn();
                    isFollowingMouse = false;
                }

                return;
            }

            //if there is a slot under mouse
            else if (hit.CompareTag("Slot"))
            {
                if (!isFollowingMouse) return;

                RectTransform slot = hit.GetComponentInParent<RectTransform>();
                if (slot != null)
                {
                    SnapFunction(slot);       
                    ToggleDeselectDrop();
                    isFollowingMouse = false;
                }
                return;
            }

        }

    }

    private void OnDrawGizmos()
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, null, out Vector3 worldPoint);
        Vector2 mousePosition = (Vector2)worldPoint;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(mousePosition, new Vector3(100f, 100f, 0f));
    }

    public void ToggleSelectDrag()
    {
        if (!isToggleDrag) return;

        SelectEffect();
        isSelected = true;
        isSnapped = false;
        canBePlaced = true;
        rectTransform.SetAsLastSibling();
    }

    public void ToggleFollow()
    {
        MouseFollow(Input.mousePosition);
        foreach (GameObject block in blockLists)
        {
            Collider2D col = block.GetComponent<Collider2D>();
            if (col != null)
                col.enabled = false;
        }
    }

    public void ToggleDeselectDrop ()
    {
        if (!isToggleDrag) return;

        
        //Center the object to the mouse position
        if (!isSnapped || !canBePlaced)
        {
            rectTransform.localEulerAngles = oringinRotation;
            rectTransform.anchoredPosition = returnPoint;
            canBeInteracte = true;
        }
        DeselectEffect();
        isSelected = false;

        foreach (GameObject block in blockLists)
        {
            Collider2D col = block.GetComponent<Collider2D>();
            if (col != null)
                col.enabled = true;
        }
    }

    public void ToggleReturn()
    {
        rectTransform.localEulerAngles = oringinRotation;
        rectTransform.anchoredPosition = returnPoint;
        DeselectEffect();
        isSelected = false;
        canBeInteracte = true;

        foreach (GameObject block in blockLists)
        {
            Collider2D col = block.GetComponent<Collider2D>();
            if (col != null)
                col.enabled = true;
        }
    }

    public void MouseFollow(Vector2 position)
    {
        //called when the toggle option is on, make obejct follow the mouse
        //Center the object to the mouse position
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, position, null, out Vector2 localPoint))
        {
            rectTransform.anchoredPosition = localPoint;
            //Debug.Log("Position Zero!");
        }
    }


    //_____Hold__________________________________________________________________
    public void OnPointerDown(PointerEventData eventData)  
    {
        //disable this if toggle
        if (isToggleDrag) return;
        if (!canBeInteracte) return;

        //Center the object to the mouse position
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, null, out Vector2 localPoint))
        {
            rectTransform.anchoredPosition = localPoint;
            //Debug.Log("Position Zero!");
        }
        SelectEffect();
        isSelected = true;

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //disable this if toggle
        if (isToggleDrag) return;
        if (!canBeInteracte) return;

        //reset the shape back to the 
        rectTransform.localEulerAngles = oringinRotation;
        rectTransform.anchoredPosition = returnPoint;
        DeselectEffect();
        isSelected = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //disable this if toggle
        if (isToggleDrag) return;
        if (!canBeInteracte) return;

        isSnapped = false;
        canBePlaced = true;
        //isSelected = true;
        //make selected object rendered at the front layer
        rectTransform.SetAsLastSibling();
        //Debug.Log("OnBeginGrag");
    }

    public void OnDrag(PointerEventData eventData)
    {
        //disable this if toggle
        if (isToggleDrag) return;
        if (!canBeInteracte) return;

        //Make the obejct move with the mouse offset
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        //Debug.Log("OnDrag");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //disable this if toggle
        if (isToggleDrag) return;

        //Debug.Log("OnEndGrag");

        if (!isSnapped || !canBePlaced)
        {
            rectTransform.localEulerAngles = oringinRotation;
            rectTransform.anchoredPosition = returnPoint;
            canBeInteracte = true;
        }
        DeselectEffect();
        isSelected = false;
    }

    //_____Functions__________________________________________________________________
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
                    //Debug.Log("is overlap");
                    break;
                }
            }

            //if triggers when the block is overlapping, thus cannot place shape
            if (blockIsOverlap)
            {
                canBePlaced = false;
                //Debug.Log("cannot place");
                break;
            }
        }
    }

    public void SnapFunction(RectTransform slot)
    {
        //public function called by the Drop script
        rectTransform.anchoredPosition = slot.anchoredPosition;
        rectTransform.localEulerAngles = currentRotation;
        canBeInteracte = false;
        CheckBlockSnap();
        CheckBlockOverlap();
    }

    public void ShapeRotation(RectTransform shape)
    {
        float step = 90f;
        if (!useScrollToRotate)
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
                rotation.z -= step;
                shape.localEulerAngles = rotation;
            }
        }
        else if (useScrollToRotate)
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
                rotation.z -= step;
                shape.localEulerAngles = rotation;
            }
        }
    }

    public void SelectEffect()
    {
        //Make object transparent with effects allowing mouse raycasting
        canvasGroup.alpha = 0.7f;
        canvasGroup.blocksRaycasts = false;
    }
    public void DeselectEffect()
    {
        //restore the object transparent
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
    }

}
