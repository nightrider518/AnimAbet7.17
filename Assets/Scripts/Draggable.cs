using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
	
	public Transform parentToReturnTo = null;
	public Transform placeholderParent = null;
	string thisColor;
		
	GameObject placeholder = null;
	byte debugOnDrag = 0;
	
	public void OnBeginDrag(PointerEventData eventData) {
//		Debug.Log ("OnBeginDrag");
		
		placeholder = new GameObject();
		placeholder.transform.SetParent( this.transform.parent );
		LayoutElement le = placeholder.AddComponent<LayoutElement>();
		le.preferredWidth = this.GetComponent<LayoutElement>().preferredWidth;
		le.preferredHeight = this.GetComponent<LayoutElement>().preferredHeight;     
		le.flexibleWidth = 0;
		le.flexibleHeight = 0;

		placeholder.transform.SetSiblingIndex( this.transform.GetSiblingIndex() );
		
		parentToReturnTo = this.transform.parent;
		placeholderParent = parentToReturnTo;
		this.transform.SetParent( this.transform.parent.parent );

		if (this.parentToReturnTo.name == "MyDiscard")
		{
			le.preferredWidth = 0;
			le.preferredHeight = 0;
		}

		GetComponent<CanvasGroup>().blocksRaycasts = false;
		debugOnDrag = 1;
	}
	
	public void OnDrag(PointerEventData eventData) {
	//	Debug.Log ("OnDrag");
		
		this.transform.position = eventData.position;
		if (this.transform.parent.tag == "CardOnTable")
		{
			return;
		}
		else if (debugOnDrag > 0)
		{
			{
//				Debug.Log("Draggable OnDrag()  parent tag is " + this.transform.parent.tag);
			}

			debugOnDrag = 0;
		}
		if (placeholder.transform.parent != placeholderParent)
			placeholder.transform.SetParent(placeholderParent);

		int newSiblingIndex = placeholderParent.childCount;

		for(int i=0; i < placeholderParent.childCount; i++) {
			if(this.transform.position.x < placeholderParent.GetChild(i).position.x) {

				newSiblingIndex = i;

				if(placeholder.transform.GetSiblingIndex() < newSiblingIndex)
					newSiblingIndex--;

				break;
			}
		}

		placeholder.transform.SetSiblingIndex(newSiblingIndex);

	}
	
	public void OnEndDrag(PointerEventData eventData) {
	//	Debug.Log ("OnEndDrag");
		thisColor = this.name.Substring(0, 3);

		//	Debug.Log("Draggable ENDDRAG:  This=" + this + "   tag=" + this.tag);    //  "" +Parent= " +this   Sibling Index=" + this.transform.GetSiblingIndex());
		//	Debug.Log("Transform.parent = " + this.transform.parent + "     Return to  " + this.parentToReturnTo +  "   parent tag= " + this.transform.parent.tag);   
		//				Debug.Log("ENDDRAG:            thisColor = " + thisColor + "    current=" + currentColor);

		if (this.parentToReturnTo.name == "MyDiscard" )
		{
			Destroy(placeholder);
			this.transform.SetParent(parentToReturnTo);
			this.transform.SetSiblingIndex(this.transform.parent.childCount);
			return;
		}

		if ((DeckOfCards.currentColor == null) && (thisColor != "bla"))
			DeckOfCards.currentColor = thisColor;
			

		this.transform.SetParent(parentToReturnTo);
		this.transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
				GetComponent<CanvasGroup>().blocksRaycasts = true;
		//this.transform.GetComponentInParent<CanvasGroup>().blocksRaycasts = true;

		Destroy(placeholder);
	}
		
}
