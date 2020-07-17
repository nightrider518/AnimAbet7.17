
using UnityEngine;
//  using System.Collections;        NOT NEEDED?
using UnityEngine.EventSystems;
using UnityEditor;
using System;
using System.Windows;

public class AdditionZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
	public static string addedColor;
	public static string playedColor;

	bool firstAdd = true;
	internal static string origWord = "";
	internal static string expandedWord;
	string addedName;
	string addedLetter;
	internal static int addedValue;
	internal static int extendedOpp;

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (eventData.pointerDrag == null)
			return;

		Draggable d = eventData.pointerDrag.GetComponent<Draggable>(); 
		//Debug.Log("AdditionZone onPointerEnter   d =" + d   + "    parentToReturnTo=" + d.parentToReturnTo   +   "    this.parent = " + this.transform.parent + "   this=" + this);
		//Debug.Log("                     parent tag =" +  this.transform.parent.tag + "   this=" + this);

		if (this == d.parentToReturnTo)
		{
			d.GetComponent<Draggable>().enabled = false;
			eventData.pointerDrag = null;
			return;
		}

		if (d != null)
		{
			d.placeholderParent = this.transform;
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (eventData.pointerDrag == null)
		{
			return;
		}

		//		Debug.Log("AdditionZone OnPointerExit");
		// 	Debug.Log("             onPointerEnxit   d =" + d + "    parentToReturnTo=" + d.parentToReturnTo + "    this.parent = " + this.transform.parent + "   this=" + this);

		Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
		if (d != null && d.placeholderParent == this.transform)
		{
			d.placeholderParent = d.parentToReturnTo;
		}
	}


	public void OnDrop(PointerEventData eventData)
	{
		// Debug.Log("OnDrop:");
		Debug.Log ("AdditionZone    " + eventData.pointerDrag.name + " was dropped on " + gameObject.name);
		if (gameObject.name == "Canvas") { return; }
		extendedOpp = Convert.ToInt32(gameObject.name.Substring(0, 1));
		if (this.transform.childCount <=1) { return; }

		addedColor = eventData.pointerDrag.name.Substring(0, 3);
		playedColor = this.transform.GetChild(0).name.Substring(0, 3);

		if (playedColor == "New")
		{
			playedColor = this.transform.GetChild(1).name.Substring(0, 3);
		}

		if (addedColor == playedColor || addedColor == "bla" || playedColor == "bla") { }
		else
		{
			Debug.Log("Words must be the same color. Black letters are allowed at any time.");
#if UNITY_EDITOR
			EditorUtility.DisplayDialog("Color Rule", "Words must be the same color. Black letters are allowed at any time.", "Ok");
#endif
//			MessageBox.Show("HandleMessage", "Words must be the same color. Black letters are allowed at any time.");
			return;
		}

		Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
		if (d != null)
		{
			d.parentToReturnTo = this.transform;
		}

		if (firstAdd)
		{
			origWord = "";
			for (int k = 0; k < this.transform.childCount; k++)
			{
				if (this.transform.GetChild(k).name.Substring(0, 1) != "N")
				{
					origWord += this.transform.GetChild(k).name.Substring(this.transform.GetChild(k).name.Length - 1, 1);
				}
			}
			firstAdd = false;
			addedValue = 0;
		}
// ??S		else origWord = expandedWord;

		addedName = eventData.pointerDrag.name;
		addedLetter = eventData.pointerDrag.name.Substring(addedName.Length - 1, 1);
		addedValue += DeckOfCards.LetVal[addedLetter];

		expandedWord = "";
		for (int k = 0; k < this.transform.childCount; k++)
		{
			if(this.transform.GetChild(k).name.Substring(0,1) == "N")
			{
				this.transform.GetChild(k).name = addedName;
			}
			expandedWord += this.transform.GetChild(k).name.Substring(this.transform.GetChild(k).name.Length - 1, 1);
		}

// ?? CAN I MAKE THIS WORK ??		DeckOfCards.EnableAddButton(); 
		// DeckOfCards.playerScore += addedValue;
		// DeckOfCards.messageString = origWord + " to " + expandedWord + " scores " + addedValue + " points, total= " + DeckOfCards.playerScore +".  Make a new word, or discard";
		// Debug.Log(origWord + "to " + expandedWord + " scores " + addedValue + " points" + ".  Total = " + DeckOfCards.playerScore);

	}
}