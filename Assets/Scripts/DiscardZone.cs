using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DiscardZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
	string addedLetter;
	int addedValue;

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (eventData.pointerDrag == null)
			return;

		Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
		//Debug.Log("AdditionZone onPointerEnter   d =" + d   + "    parentToReturnTo=" + d.parentToReturnTo   +   "    this.parent = " + this.transform.parent + "   this=" + this);

		if (this == d.parentToReturnTo)
		{
			d.GetComponent<Draggable>().enabled = false;
			eventData.pointerDrag = null;
			return;
		}

/*		if (d != null)
		{
			d.placeholderParent = this.transform;
		}
*/		
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (eventData.pointerDrag == null)
		{
			return;
		}

		Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
		if (d != null && d.placeholderParent == this.transform)
		{
			d.placeholderParent = d.parentToReturnTo;
		}
	}

	public void OnDrop(PointerEventData eventData)
	{
		// Debug.Log("OnDrop:");
		// Debug.Log ("DiscardZone    " + eventData.pointerDrag.name + " was dropped on " + gameObject.name");

		Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
		if (d != null)
		{
			d.parentToReturnTo = this.transform;
		//	this.transform.position = this.transform.GetChild(0).position + Vector3.forward * this.transform.childCount;
		}
	}

}