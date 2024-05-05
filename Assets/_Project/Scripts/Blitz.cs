using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using static CardData;

public class Blitz : MonoBehaviourPun
{
    public static Dictionary<Rank, int> CardRankToValueMap = new Dictionary<Rank, int>()
    {
        { Rank.Ace, 11 },
        { Rank.Two, 2 },
        { Rank.Three, 3 },
        { Rank.Four, 4 },
        { Rank.Five, 5 },
        { Rank.Six, 6 },
        { Rank.Seven, 7 },
        { Rank.Eight, 8 },
        { Rank.Nine, 9 },
        { Rank.Ten, 10 },
        { Rank.Jack, 10 },
        { Rank.Queen, 10 },
        { Rank.King, 10 }
    };
    public static Dictionary<Rank, string> CardRankToShortRankMap = new Dictionary<Rank, string>()
    {
        { Rank.Ace, "A" },
        { Rank.Two, "2" },
        { Rank.Three, "3" },
        { Rank.Four, "4" },
        { Rank.Five, "5" },
        { Rank.Six, "6" },
        { Rank.Seven, "7" },
        { Rank.Eight, "8" },
        { Rank.Nine, "9" },
        { Rank.Ten, "10" },
        { Rank.Jack, "J" },
        { Rank.Queen, "Q" },
        { Rank.King, "K" }
    };
    public static string CARD_ROOT_FOLDER = "Cards/";
    public static string CARD_PREFIX = "card";
    public static string CARD_BACK_FACE_NAME = "cardBack_red4";

    private List<CardData> deck = new List<CardData>();

    public static int MAX_PLAYERS { get; private set; } = 6;

    public void SetupDeckWithSeedShuffle(int seed)
    {
        deck = new List<CardData>();
        GenerateDeck();
        Shuffle(deck, seed);
    }

    private void GenerateDeck()
    {
        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
        {
            foreach (Rank rank in Enum.GetValues(typeof(Rank)))
            {
                CardData cardData = new CardData(suit, rank, CardRankToValueMap[rank]);
                deck.Add(cardData);
            }
        }
    }

    private void Shuffle<T>(IList<T> list, int seed)
    {
        var rng = new System.Random(seed);
        int n = list.Count;

        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    private void PrintDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            Debug.Log(deck[i].cardName);
        }
    }

    public CardData FetchCard()
    {
        CardData cardData = deck[0];
        deck.RemoveAt(0);
        return cardData;
    }
}
