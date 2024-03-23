using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeckControllerUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Action DeckClicked;
    public void OnPointerClick(PointerEventData eventData)
    {
        DeckClicked?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = new Vector3(1f, 1f, 1f);
    }
}
