using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardControllerUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private CardData cardData;
    [SerializeField] private Image image;
    [SerializeField] private bool isFaceDown;

    public Action<CardData> CardClicked;

    public void SetCardData(CardData cardData, bool isFaceDown = false)
    {
        image.enabled = true;
        this.isFaceDown = isFaceDown;
        this.cardData = cardData;
        SetCardImage();
    }

    public void HideCard()
    {
        cardData = null;
    }

    public CardData GetCardData()
    {
        return cardData;
    }

    public void FaceDown()
    {
        image.sprite = BlitzHelper.GetFaceDownImage();
    }

    public void FaceUp() 
    {
        image.sprite = BlitzHelper.GetCardImage(cardData);
    }

    private void SetCardImage()
    {
        if(isFaceDown)
        {
            FaceDown();
            return;
        }
        FaceUp();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        CardClicked?.Invoke(cardData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = new Vector3 (1.1f, 1.1f, 1.1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = new Vector3(1f, 1f, 1f);
    }
}
