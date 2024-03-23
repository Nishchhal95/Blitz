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
    public static Dictionary<int, List<Transform>> playerCountToSpawnPointsMap = new Dictionary<int, List<Transform>>();
    public static string CARD_ROOT_FOLDER = "Cards/";
    public static string CARD_PREFIX = "card";
    public static string CARD_BACK_FACE_NAME = "cardBack_red4";

    public const int MAX_PLAYERS = 6;
    public const int NUM_OF_CARDS_PER_PLAYER = 3;

    public CardController cardPrefab;
    public Transform[] playerSpawnPoints;
    public Transform discardPileTransform;
    public Transform deckTransform;
    public DeckController deckController;
    public int testSeed;

    private List<CardData> deck = new List<CardData>();

    private void Start()
    {
        playerCountToSpawnPointsMap = new Dictionary<int, List<Transform>>()
        {
            {1, new List<Transform>()
                { 
                    playerSpawnPoints[0] 
                } 
            },
            {2, new List<Transform>()
                { 
                    playerSpawnPoints[0],
                    playerSpawnPoints[3] 
                } 
            },
            {3, new List<Transform>()
                { 
                    playerSpawnPoints[0], 
                    playerSpawnPoints[3], 
                    playerSpawnPoints[5] 
                } 
            },
            {4, new List<Transform>()
                { 
                    playerSpawnPoints[0], 
                    playerSpawnPoints[1], 
                    playerSpawnPoints[2], 
                    playerSpawnPoints[4] 
                } 
            },
            {5, new List<Transform>()
                { 
                    playerSpawnPoints[0],
                    playerSpawnPoints[1], 
                    playerSpawnPoints[2], 
                    playerSpawnPoints[3], 
                    playerSpawnPoints[4] 
                } 
            },
            {6, new List<Transform>()
                { 
                    playerSpawnPoints[0], 
                    playerSpawnPoints[1], 
                    playerSpawnPoints[2], 
                    playerSpawnPoints[3], 
                    playerSpawnPoints[4], 
                    playerSpawnPoints[5] 
                }
            }
        };
    }

    [PunRPC]
    public void SetupGame(int seed)
    {
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

    private void DealCards()
    {
        int playerCount = PhotonNetworkManager.GetPlayerCountInCurrentRoom();
        for (int i = 0; i < playerCount; i++)
        {
            for (int j = 0; j < NUM_OF_CARDS_PER_PLAYER; j++)
            {
                SpawnCard(playerCountToSpawnPointsMap[playerCount][i], j);
            }
        }
    }

    private void SetupDiscardPile()
    {
        SpawnCard(discardPileTransform, 0);
    }

    private CardController SpawnCard(Transform parent, int cardIndex)
    {
        Vector3 cardPos = parent.transform.position + new Vector3(cardIndex * .5f, 0, .1f * cardIndex);
        CardController cardController = Instantiate(cardPrefab, cardPos, Quaternion.identity, parent);
        cardController.Init(FetchCard());
        cardController.transform.name = cardController.cardData.cardName;
        return cardController;
    }

    public CardData FetchCard()
    {
        CardData cardData = deck[0];
        deck.RemoveAt(0);
        return cardData;
    }
}
