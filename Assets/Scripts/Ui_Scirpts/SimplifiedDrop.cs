using UnityEngine;
using UnityEngine;
using UnityEngine.EventSystems;

public class SimplifiedDrop : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        RectTransform slot = GetComponent<RectTransform>();

        //Debug.Log("OnDrop");
        if (eventData.pointerDrag != null)
        {
            Drag shapeScript = eventData.pointerDrag.GetComponent<Drag>();

            //if (shapeScript.canBeInteracte == false) return;
            if (shapeScript.isToggleDrag == true) return;

            //call the snap function and pass parmeter
            eventData.pointerDrag.GetComponent<Drag>().SnapFunction(slot);
        }
    }
}
