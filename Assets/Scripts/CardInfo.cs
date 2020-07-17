using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public DeckOfCards.CardInDeck card;
    public GameObject selectedObject;

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
 //         Debug.Log(this.gameObject + " has been entered.");
        selectedObject = this.gameObject;
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        selectedObject = null;
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        if (selectedObject != null)
        {
    //Debug.Log("CardInfo Click. Pointer position x=" + pointerEventData.position.x + "  y=" + pointerEventData.position.y);
    //Debug.Log("CardInfo Click.      this = " + this + "  tag=" + this.tag + " parent=" + this.transform.parent);
            DeckOfCards.deckOfCards.ChooseCard(card);
        }
    }
    public void OnPointerUp(PointerEventData pointerEventData)
    {
//        Debug.Log("Terminated");
    }

}
