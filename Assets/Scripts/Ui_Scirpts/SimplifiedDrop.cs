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
            SimplifiedDrag shapeScript = eventData.pointerDrag.GetComponent<SimplifiedDrag>();

            //if (shapeScript.canBeInteracte == false) return;
            if (shapeScript.isToggleDrag == true) return;

            //call the snap function and pass parmeter
            shapeScript.SnapFunction(slot);
        }
    }
}
