using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;


public class SimplifiedDrag : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private Canvas canvas;

    [Header("Drag object options")]
    public bool isToggleDrag = false;
    public int XOffset;
    public int YOffset;



    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 returnPoint;
    private Vector3 oringinRotation;
    private Vector3 currentRotation;
    private bool isSnapped;
    //private bool canBePlaced;
    //private bool isSelected = false;
    private bool isFollowingMouse = false;

    private CinemachineCamera nextCam;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        isSnapped = false;
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

            //if there is a DragObject under mouse
            else if (hit.CompareTag("DragObject"))
            {
                //get the current shape
                SimplifiedDrag hitdrag = hit.GetComponentInParent<SimplifiedDrag>();
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
        isSnapped = false;
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
        }
        else
        {
            if (GameManager.currentPhase == GameManager.phaseState.Neighbourhood || GameManager.currentPhase == GameManager.phaseState.Home)
            {
                PickHome();
                TenantStats tenant = GetComponent<TenantStats>();
                tenant.PayRent();
                rectTransform.localEulerAngles = oringinRotation;
                rectTransform.anchoredPosition = returnPoint;
            }
            else if (GameManager.currentPhase == GameManager.phaseState.Zone)
            {
                PickLord();
                rectTransform.localEulerAngles = oringinRotation;
                rectTransform.anchoredPosition = returnPoint;
            }
        }
        DeselectEffect();


    }

    public void ToggleReturn()
    {
        rectTransform.localEulerAngles = oringinRotation;
        rectTransform.anchoredPosition = returnPoint;
        DeselectEffect();

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = true;
    }

    public void MouseFollow(Vector2 position)
    {
        //called when the toggle option is on, make obejct follow the mouse
        //Center the object to the mouse position
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, position, null, out Vector2 localPoint))
        {
            localPoint.x += XOffset;
            localPoint.y += YOffset;
            rectTransform.anchoredPosition = localPoint;
            //Debug.Log("Position Zero!");
        }
    }


    //_____Hold__________________________________________________________________
    public void OnPointerDown(PointerEventData eventData)
    {
        //disable this if toggle
        if (isToggleDrag) return;

        //Center the object to the mouse position
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, null, out Vector2 localPoint))
        {
            localPoint.x += XOffset;
            localPoint.y += YOffset;
            rectTransform.anchoredPosition = localPoint;
            //Debug.Log("Position Zero!");
        }
        SelectEffect();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //disable this if toggle
        if (isToggleDrag) return;

        //reset the shape back to the 
        rectTransform.localEulerAngles = oringinRotation;
        rectTransform.anchoredPosition = returnPoint;
        DeselectEffect();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //disable this if toggle
        if (isToggleDrag) return;

        transform.SetParent(canvas.transform, true);

        isSnapped = false;
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

        //Debug.Log("OnEndGrag");

        if (!isSnapped)
        {
            rectTransform.localEulerAngles = oringinRotation;
            rectTransform.anchoredPosition = returnPoint;
        }
        else
        {
            if (GameManager.currentPhase == GameManager.phaseState.Neighbourhood || GameManager.currentPhase == GameManager.phaseState.Home)
            {
                PickHome();
                TenantStats tenant = GetComponent<TenantStats>();
                tenant.PayRent();
            }
            else if (GameManager.currentPhase == GameManager.phaseState.Zone)
            {
                PickLord();
            }
        }
        DeselectEffect();
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

    private void PickLord()
    {
        GameManager.PhaseUpdate(GameManager.phaseState.Neighbourhood);
        CameraManager.CameraSwitch(GameManager.Instance.testZone.hoods[0].hoodCam);
    }
    private void PickHome()
    {
        GameManager.PhaseUpdate(GameManager.phaseState.City);
        CameraManager.CameraSwitch(GameManager.Instance.cityCam);
    }
}
