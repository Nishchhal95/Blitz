using System;

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
