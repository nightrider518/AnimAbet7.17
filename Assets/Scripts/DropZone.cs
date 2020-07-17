using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {
	public static string thisColor;

	public void OnPointerEnter(PointerEventData eventData) {
		//Debug.Log("OnPointerEnter");
		if(eventData.pointerDrag == null)
			return;

		Draggable d = eventData.pointerDrag.GetComponent<Draggable>();        
		// Debug.Log("DropZone onPointerEnter   d =" + d);
		if(d != null) {
			d.placeholderParent = this.transform;
		} 
	}
	
	public void OnPointerExit(PointerEventData eventData) {
		if (this.transform.childCount < 1)
		{
			DeckOfCards.currentColor = null;
		}

		if (eventData.pointerDrag == null)
		{
			return;	   
		}

		//		Debug.Log("OnPointerExit");

		Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
		if(d != null && d.placeholderParent==this.transform) {
			d.placeholderParent = d.parentToReturnTo;
		} 
	}
	
	public void OnDrop(PointerEventData eventData) {
		// Debug.Log("OnDrop:");
		// Debug.Log ("DropZone():  " + eventData.pointerDrag.name + " was dropped on " + gameObject.name);

		thisColor = eventData.pointerDrag.name.Substring(0, 3);

		if ((DeckOfCards.currentColor == null) && (thisColor != "bla")) DeckOfCards.currentColor = thisColor;

		if (DeckOfCards.currentColor == null || DeckOfCards.currentColor == thisColor || thisColor == "bla") 
		{ }
		else
		{
			Debug.Log("Words must be the same color. Black letters are allowed at any time.");   //	+ "   child count=" + this.transform.childCount);
			return;
		}

		Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
		if(d != null) {
			d.parentToReturnTo = this.transform;
		}

	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		/*   Script is not used. Saved for now in case debug statements are needed.
		 *   
		 *   
		    
		Debug.Log("collision happened: count=" + collision.contactCount); // collision.collider + "   object=" + collision.gameObject + "   name= " + collision.gameObject.name);
		for (int i = 0; i < collision.contactCount; i++)
		{
			Debug.Log("     " + collision.contacts[i].collider + " + with  " + collision.contacts[i].otherCollider + "  sib=" + this.transform.GetSiblingIndex());
		}

//		isOverDropZone = true;
//		dropZone = collision.gameObject;
*/
	}
}
