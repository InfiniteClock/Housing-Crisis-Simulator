using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drop : MonoBehaviour, IDropHandler
{

    public void OnDrop(PointerEventData eventData)
    {
        RectTransform slot = GetComponent<RectTransform>();

        //Debug.Log("OnDrop");
        if (eventData.pointerDrag != null)
        {
            //call the snap function and pass parmeter
            eventData.pointerDrag.GetComponent<Drag>().SnapFunction(slot);
        }
    }
}
