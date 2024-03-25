using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayerUI : MonoBehaviour
{
    [SerializeField] private string playerName;
    [SerializeField] private float playerScore;
    [SerializeField] private int playerActorNumber;
    [SerializeField] private bool isLocalPlayer;
    [SerializeField] private List<CardData> cards = new List<CardData>(4);
    [SerializeField] private TextMeshProUGUI playerNameTextField;
    [SerializeField] private TextMeshProUGUI playerScoreTextField;
    [SerializeField] private List<CardControllerUI> cardControllers = new List<CardControllerUI>(4);
    [SerializeField] private GameObject currentTurnIndicator;
    [SerializeField] private Button knockButton;

    public Action<int, CardData> PlayerCardClicked;
    public Action<int> PlayerKnockClicked;


    private void OnEnable()
    {
        knockButton.onClick.AddListener(OnKnockClicked);
        GameController.DebugPressed += DebugPressed;
    }

    private void OnDisable()
    {
        knockButton.onClick.RemoveListener(OnKnockClicked);
        GameController.DebugPressed -= DebugPressed;
    }

    public void Init(string playerName, int playerActorNumber)
    {
        this.playerName = playerName;
        this.playerActorNumber = playerActorNumber;
        SetPlayerName(playerName);
        isLocalPlayer = playerActorNumber == PhotonNetworkManager.GetLocalPlayer().ActorNumber;
        playerScoreTextField.gameObject.SetActive(isLocalPlayer);

        knockButton.gameObject.SetActive(isLocalPlayer);

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

    public void SetCardsInfo(List<CardData> cards, bool isFaceDown = false)
    {
        this.cards = cards;
        for (int i = 0; i < cards.Count; i++) 
        {
            cardControllers[i].SetCardData(cards[i], isFaceDown);
        }
        CalculatePlayerScore();
    }

    public void SetCardInfo(CardData cardData, bool isFaceDown = false)
    {
        cards.Add(cardData);
        cardControllers[3].SetCardData(cardData, isFaceDown);
        cardControllers[3].gameObject.SetActive(true);
        CalculatePlayerScore();
    }

    public void TakeCard(int cardID, bool isFaceDown = false)
    {
        //Remove card
        cards.RemoveAll(card => card.cardId == cardID);
        SetCardsInfo(cards, isFaceDown);

        //Hide Last Card
        cardControllers[3].HideCard();
        cardControllers[3].gameObject.SetActive(false);

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

    public float GetScore()
    {
        CalculatePlayerScore();
        return playerScore;
    }

    public string GetName()
    {
        return playerName;
    }

    public List<CardData> GetCards()
    {
        return cards;
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

        bool hasSimilarRank3Cards = cards.GroupBy(card => card.rank).Any(group => group.Count() >= 3);
        if(hasSimilarRank3Cards)
        {
            playerScore = 30.5f;
        }
        playerScore = Mathf.Max(playerScore, maxSum.Sum);
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

    private void OnKnockClicked()
    {
        PlayerKnockClicked?.Invoke(playerActorNumber);
    }

    private void DebugPressed()
    {
        if (isLocalPlayer)
        {
            return;
        }
        playerScoreTextField.gameObject.SetActive(GameController.IS_DEBUG);
        foreach(CardControllerUI cardController in cardControllers)
        {
            if(cardController != null && cardController.GetCardData() != null)
            {
                if (GameController.IS_DEBUG)
                {
                    cardController.FaceUp();
                }
                else
                {
                    cardController.FaceDown();
                }
            }
        }
    }
}
