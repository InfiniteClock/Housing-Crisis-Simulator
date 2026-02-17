using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class SimplifiedDrag : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private Canvas canvas;
    //public Canvas Canvas
    //{
    //    get { return canvas; }
    //    set { canvas = value; }
    //}

    //public GameObject zone;
    //public List<GameObject> blockLists;

    [Header("Drag object options")]
    public bool isToggleDrag = false;

    [HideInInspector]
    //public bool canBeInteracte = true;
    //public int spawnPointIndex;



    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 returnPoint;
    private Vector3 oringinRotation;
    private Vector3 currentRotation;
    private bool isSnapped;
    //private bool canBePlaced;
    private bool isSelected;
    private bool isFollowingMouse = false;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        isSnapped = false;
        //canBePlaced = true;
        isSelected = false;

        RecordReturnPosiiton();

    }

    public void Update()
    {
        
        //Toggle logic
        if (!isToggleDrag) return;

        ToggleLogic();

    }

    //_____Toggle__________________________________________________________________
    public void ToggleLogic()
    {
        //if (!canBeInteracte) return;

        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, null, out Vector3 worldPoint);
        Vector2 mousePosition = (Vector2)worldPoint;
        Collider2D hit = Physics2D.OverlapBox(mousePosition, new Vector2(20f, 20f), 0f);

        if (isFollowingMouse)
        {
            ToggleFollow();
        }

        if (Input.GetMouseButtonDown(0))
        {
            //reset shape position when clcik the blank space
            if (hit == null)
            {
                if (isFollowingMouse)
                {
                    ToggleReturn();
                    isFollowingMouse = false;
                }
            }

            //if there is a agent under mouse
            else if (hit.CompareTag("Agent"))
            {
                Debug.Log("hey");
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

        transform.SetParent(canvas.transform, true);

        SelectEffect();
        isSelected = true;
        isSnapped = false;
        //canBePlaced = true;
        rectTransform.SetAsLastSibling();
    }

    public void ToggleFollow()
    {
        MouseFollow(Input.mousePosition);
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
    }

    public void ToggleDeselectDrop()
    {
        if (!isToggleDrag) return;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = true;

        //Center the object to the mouse position
        if (!isSnapped)
        {
            rectTransform.localEulerAngles = oringinRotation;
            rectTransform.anchoredPosition = returnPoint;
            //canBeInteracte = true;
        }
        else
        {
            //GridLock();
            //ShapeRandomizer.Instance.RandomSpawnShape(spawnPointIndex);
        }
        DeselectEffect();
        isSelected = false;


    }

    public void ToggleReturn()
    {
        rectTransform.localEulerAngles = oringinRotation;
        rectTransform.anchoredPosition = returnPoint;
        DeselectEffect();
        isSelected = false;
        //canBeInteracte = true;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = true;
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
        //if (!canBeInteracte) return;


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
        //if (!canBeInteracte) return;

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
        //if (!canBeInteracte) return;

        transform.SetParent(canvas.transform, true);

        isSnapped = false;
        //canBePlaced = true;
        //isSelected = true;
        //make selected object rendered at the front layer
        rectTransform.SetAsLastSibling();
        //Debug.Log("OnBeginGrag");
    }

    public void OnDrag(PointerEventData eventData)
    {
        //disable this if toggle
        if (isToggleDrag) return;
        //if (!canBeInteracte) return;

        //Make the obejct move with the mouse offset
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        //Debug.Log("OnDrag");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //disable this if toggle
        if (isToggleDrag) return;

        //Debug.Log("OnEndGrag");

        if (!isSnapped)
        {
            rectTransform.localEulerAngles = oringinRotation;
            rectTransform.anchoredPosition = returnPoint;
            //canBeInteracte = true;
        }
        else
        {
            //GridLock();
            //ShapeRandomizer.Instance.RandomSpawnShape(spawnPointIndex);
        }
        DeselectEffect();
        isSelected = false;
    }

    //_____Functions__________________________________________________________________
    public void CheckBlockSnap()
    {

        //this function is used to check if all the blocks in shape is inside the map
        isSnapped = true;

        Vector2 blockPosition = transform.position;

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

        //if there isn't any collider tagged with Slot,reset position
        if (!blockIsSnapped)
        {
            isSnapped = false;
        }
    }

    //public void CheckBlockOverlap()
    //{
    //    //this function is prevent shapes overlap eachother using the same logic
    //    canBePlaced = true;

    //    foreach (GameObject block in blockLists)
    //    {
    //        Vector2 blockPosition = block.transform.position;

    //        //Created an array of all the colliders in the overlapbox
    //        Collider2D[] hits = Physics2D.OverlapBoxAll(blockPosition, new Vector2(1f, 1f), 0f);

    //        bool blockIsOverlap = false;
    //        foreach (Collider2D hit in hits)
    //        {
    //            if (hit.CompareTag("Block") && hit.gameObject != block)
    //            {
    //                //if there is a collider under the box is tagged with Block, the shape cannot be placed
    //                blockIsOverlap = true;
    //                //Debug.Log("is overlap");
    //                break;
    //            }
    //        }

    //        //if triggers when the block is overlapping, thus cannot place shape
    //        if (blockIsOverlap)
    //        {
    //            canBePlaced = false;
    //            //Debug.Log("cannot place");
    //            break;
    //        }
    //    }
    //}

    public void SnapFunction(RectTransform slot)
    {
        //public function called by the Drop script
        rectTransform.anchoredPosition = slot.anchoredPosition;
        rectTransform.localEulerAngles = currentRotation;
        //canBeInteracte = false;
        CheckBlockSnap();
        //CheckBlockOverlap();
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

    public void RecordReturnPosiiton()
    {
        returnPoint = rectTransform.anchoredPosition;
        oringinRotation = rectTransform.localEulerAngles;
    }


    // Tells game manager what grid spaces are being taken up when placing object
    //private void GridLock()
    //{
    //    Vector2 blockPosition = blockLists[0].transform.position;

    //    //Created an array of all the colliders in the overlapbox
    //    Collider2D[] hits = Physics2D.OverlapBoxAll(blockPosition, new Vector2(1f, 1f), 0f);

    //    foreach (Collider2D hit in hits)
    //    {
    //        if (hit.CompareTag("Slot"))
    //        {
    //            GameManager.Instance.AddToGrid(hit.gameObject, GetComponent<Drag>());
    //            break;
    //        }
    //    }
    //}
}
