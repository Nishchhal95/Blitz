using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GamePlayerUI : MonoBehaviour
{
    [SerializeField] private string playerName;
    [SerializeField] private int playerScore;
    [SerializeField] private int playerActorNumber;
    [SerializeField] private List<CardData> cards = new List<CardData>(4);
    [SerializeField] private TextMeshProUGUI playerNameTextField;
    [SerializeField] private TextMeshProUGUI playerScoreTextField;
    [SerializeField] private List<CardControllerUI> cardControllers = new List<CardControllerUI>(4);
    [SerializeField] private GameObject currentTurnIndicator;

    public Action<int, CardData> PlayerCardClicked;

    public void Init(string playerName, int playerActorNumber)
    {
        this.playerName = playerName;
        this.playerActorNumber = playerActorNumber;
        SetPlayerName(playerName);

        for (int i = 0; i < cardControllers.Count; i++)
        {
            cardControllers[i].CardClicked += OnCardClicked;
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < cardControllers.Count; i++)
        {
            cardControllers[i].CardClicked -= OnCardClicked;
        }
    }

    public void SetCardsInfo(List<CardData> cards)
    {
        this.cards = cards;
        for (int i = 0; i < cards.Count; i++) 
        {
            cardControllers[i].SetCardData(cards[i]);
        }
        CalculatePlayerScore();
    }

    public void SetCardInfo(CardData cardData)
    {
        cards.Add(cardData);
        cardControllers[3].SetCardData(cardData);
        CalculatePlayerScore();
    }

    public void TakeCard(int cardID)
    {
        for(int i = 0;i < cardControllers.Count;i++)
        {
            cardControllers[i].HideCard();
        }
        cards.RemoveAll(card => card.cardId == cardID);
        SetCardsInfo(cards);
        CalculatePlayerScore();
    }

    public CardData FindCardByID(int cardID)
    {
        return cards.Find(card => card.cardId.Equals(cardID));
    }

    public void SetCurrentTurnIndicator(bool toggle)
    {
        currentTurnIndicator.SetActive(toggle);
    }

    private void CalculatePlayerScore()
    {
        var maxSum = cards.GroupBy(card => card.suit).
        Select(group => new
        {
            Suit = group.Key,
            Sum = group.Sum(card => card.value)
        }).
        OrderByDescending(group => group.Sum).
        FirstOrDefault();
        playerScore = maxSum.Sum;
        playerScoreTextField.SetText("" + playerScore);
    }

    private void SetPlayerName(string playerName)
    {
        playerNameTextField.SetText(playerName);
    }

    private void OnCardClicked(CardData cardData)
    {
        PlayerCardClicked?.Invoke(playerActorNumber, cardData);
    }
}
