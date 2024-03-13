using System;
using UnityEngine;

public class CardController : MonoBehaviour
{
    [SerializeField] public CardData cardData;
    [SerializeField] public bool isFaceUp;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public void Init(CardData cardData)
    {
        this.cardData = cardData;
        FaceUp();
    }

    private void SetCardSprite(string cardName)
    {
        Sprite cardSprite = Resources.Load<Sprite>(Blitz.CARD_ROOT_FOLDER + cardName);
        spriteRenderer.sprite = cardSprite;
    }

    public void FaceUp()
    {
        isFaceUp = true;
        SetCardSprite(Blitz.CARD_PREFIX + cardData.cardName);
    }

    public void FaceDown()
    {
        isFaceUp = false;
        SetCardSprite(Blitz.CARD_BACK_FACE_NAME);
    }
}

[Serializable]
public class CardData
{
    public static int CARD_ID = -1;
    public int cardId;
    public Suit suit;
    public Rank rank;
    public int value;
    public string cardName;

    public CardData(Suit suit, Rank rank, int value)
    {
        CARD_ID++;
        cardId = CARD_ID;
        this.suit = suit;
        this.rank = rank;
        this.value = value;
        cardName = suit.ToString() + rank;
    }

    public enum Suit
    {
        Spades,
        Hearts,
        Diamonds,
        Clubs
    }

    public enum Rank
    {
        Ace,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King
    }
}
