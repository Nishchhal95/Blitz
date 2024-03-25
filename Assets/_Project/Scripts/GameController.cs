using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviourPun
{
    private static bool isDebug;

    public static int MINIMUM_PLAYERS_TO_START_GAME = 0;
    public static int MAX_PLAYERS = 6;
    public static bool IS_DEBUG { get { return isDebug; } set { isDebug = value; DebugPressed?.Invoke(); } }

    public static event Action DebugPressed;
    public static Action GameStarted;

    [SerializeField] private Blitz blitz;
    [SerializeField] private GamePlayerUI gamePlayerUIPrefab;
    [SerializeField] private PlayerCountToSpawnPoints[] playerCountToPlayerSlotsSetupMap = new PlayerCountToSpawnPoints[Blitz.MAX_PLAYERS];
    [SerializeField] private Dictionary<int, GamePlayerUI> actorIDToGamePlayerUIMap = new Dictionary<int, GamePlayerUI>();

    [SerializeField] private DeckControllerUI deckControllerUI;
    [SerializeField] private CardControllerUI cardPrefab;
    [SerializeField] private CardControllerUI discardCardController;
    [SerializeField] private Transform discardPileTransform;

    [SerializeField] private int currentActorTurn = 1;
    [SerializeField] private bool fetchedNewCard = false;

    [SerializeField] private bool playerKnocked = false;
    [SerializeField] private int knockedPlayerActorNumber = -1;
    [SerializeField] private int turnEndActorNumber = -1;

    [SerializeField] private bool gameOver = false;

    private void OnEnable()
    {
        deckControllerUI.DeckClicked += OnDeckClicked;
    }

    private void OnDisable()
    {
        deckControllerUI.DeckClicked -= OnDeckClicked;

        if(discardCardController != null)
        {
            discardCardController.CardClicked -= OnDiscardPileClicked;
        }
    }

    public void StartGame()
    {
        int playersInRoom = PhotonNetworkManager.GetPlayerCountInCurrentRoom();
        if (playersInRoom < MINIMUM_PLAYERS_TO_START_GAME || playersInRoom > MAX_PLAYERS)
        {
            return;
        }

        // Lock Room
        PhotonNetworkManager.GetCurrentRoom().IsOpen = false;

        photonView.RPC("StartGameForAll", RpcTarget.All);

        // Generate and Shuffle Deck for all players
        int randomSeed = UnityEngine.Random.Range(1, 99999);
        blitz.GetComponent<PhotonView>().RPC("SetupGame", RpcTarget.All, randomSeed);

        //Spawn Players for all players
        photonView.RPC("SpawnPlayers", RpcTarget.All);
    }

    [PunRPC]
    private void StartGameForAll()
    {
        if(actorIDToGamePlayerUIMap != null)
        {
            foreach (GamePlayerUI gamePlayer in actorIDToGamePlayerUIMap.Values)
            {
                Destroy(gamePlayer.gameObject);
            }
        }
        actorIDToGamePlayerUIMap = new Dictionary<int, GamePlayerUI>();

        if(discardCardController != null)
        {
            Destroy(discardCardController.gameObject);
        }
        discardCardController = null;

        gameOver = false;
        currentActorTurn = 1;
        fetchedNewCard = false;

        playerKnocked = false;
        knockedPlayerActorNumber = -1;
        turnEndActorNumber = -1;

        GameUIController.Instance.HideGameOver();

        GameStarted?.Invoke();
    }

    [PunRPC]
    private void SpawnPlayers()
    {
        int playerCount = PhotonNetworkManager.GetPlayerCountInCurrentRoom();
        // This also gets you a sorted list at which the player joined.
        List<Player> players = PhotonNetworkManager.GetPlayerListInCurrentRoomSortedByActorNumber();
        foreach (Player player in players)
        {
            Debug.Log($"NickName:{player.NickName}, UserID:{player.UserId}, ActorNumber:{player.ActorNumber}");
        }

        PlayerCountToSpawnPoints playerCountToSpawnPoints = null;
        for (int i = 0; i < playerCountToPlayerSlotsSetupMap.Length; i++)
        {
            if (playerCountToPlayerSlotsSetupMap[i].Count == playerCount)
            {
                playerCountToSpawnPoints = playerCountToPlayerSlotsSetupMap[i];
                break;
            }
        }

        if(playerCountToSpawnPoints == null)
        {
            Debug.LogError("Cannot find SpawnPoint array for " + playerCount + " player count");
            return;
        }

        int currentActorID = PhotonNetworkManager.GetLocalPlayer().ActorNumber;
        for (int i = 0; i < playerCountToSpawnPoints.SpawnPoints.Length; i++)
        {
            Player currentPlayer = players[currentActorID - 1];
            GamePlayerUI gamePlayerUI = SpawnGamePlayer(currentPlayer, playerCountToSpawnPoints.SpawnPoints[i]);
            currentActorID++;
            if (currentActorID > playerCount)
            {
                currentActorID = 1;
            }
        }

        DistributeCards();
        SetupDeck();
        SetupDiscardPile();
        UpdateCurrentTurn();
    }

    private void DistributeCards()
    {
        List<Player> players = PhotonNetworkManager.GetPlayerListInCurrentRoomSortedByActorNumber();

        foreach (Player player in players)
        {
            List<CardData> cardData = new List<CardData>(3);
            for (int j = 0; j < 3; j++)
            {
                cardData.Add(blitz.FetchCard());
            }
            actorIDToGamePlayerUIMap[player.ActorNumber].SetCardsInfo(cardData, 
                PhotonNetworkManager.GetLocalPlayer().ActorNumber != player.ActorNumber);
        }
    }

    private void SetupDeck()
    {
        deckControllerUI.gameObject.SetActive(true);
    }

    private void SetupDiscardPile()
    {
        CardData cardData = blitz.FetchCard();
        discardCardController = Instantiate(cardPrefab, discardPileTransform);
        discardCardController.transform.localPosition = Vector3.zero;
        discardCardController.SetCardData(cardData);
        discardCardController.CardClicked += OnDiscardPileClicked;
    }

    private void UpdateCurrentTurn()
    {
        actorIDToGamePlayerUIMap[currentActorTurn].SetCurrentTurnIndicator(true);
    }

    private GamePlayerUI SpawnGamePlayer(Player player, Transform parentTransform)
    {
        GamePlayerUI gamePlayerUI = Instantiate(gamePlayerUIPrefab, Vector2.zero, Quaternion.identity, parentTransform);
        gamePlayerUI.transform.localPosition = Vector2.zero;
        gamePlayerUI.Init(player.NickName, player.ActorNumber);
        gamePlayerUI.PlayerCardClicked += OnPlayerCardClicked;
        gamePlayerUI.PlayerKnockClicked += OnPlayerKnockClicked;

        actorIDToGamePlayerUIMap.Add(player.ActorNumber, gamePlayerUI);

        return gamePlayerUI;
    }

    private void OnDeckClicked()
    {
        if(!gameOver && IsMyTurn && !fetchedNewCard)
        {
            photonView.RPC("FetchedNewCardFromDeck", RpcTarget.All);
        }
    }

    private void OnDiscardPileClicked(CardData cardData)
    {
        if (!gameOver && IsMyTurn && !fetchedNewCard)
        {
            photonView.RPC("FetchedNewCardFromDiscardPile", RpcTarget.All);
        }
    }

    private void OnPlayerCardClicked(int actorNumber, CardData cardData)
    {
        if (gameOver || !IsMyTurn || currentActorTurn != actorNumber || !fetchedNewCard)
        {
            return;
        }

        photonView.RPC("RemoveCardFromPlayer", RpcTarget.All, cardData.cardId);
    }

    private void OnPlayerKnockClicked(int actorNumber)
    {
        if (gameOver && !IsMyTurn || currentActorTurn != actorNumber)
        {
            return;
        }

        if (playerKnocked)
        {
            return;
        }

        photonView.RPC("PlayerKnockForAll", RpcTarget.All, actorNumber);
    }

    [PunRPC]
    private void PlayerKnockForAll(int actorNumber)
    {
        playerKnocked = true;
        knockedPlayerActorNumber = actorNumber;
        turnEndActorNumber = actorNumber - 1;
        if (turnEndActorNumber <= 0)
        {
            turnEndActorNumber = PhotonNetworkManager.GetPlayerCountInCurrentRoom();
        }

        InGameMessageController.Instance.ShowMessage($"Player {actorIDToGamePlayerUIMap[actorNumber].GetName()} KNOCKED! " +
            $"Game will end after {actorIDToGamePlayerUIMap[turnEndActorNumber].GetName()} turn");
    }

    [PunRPC]
    private void FetchedNewCardFromDeck()
    {
        CardData cardData = blitz.FetchCard();
        actorIDToGamePlayerUIMap[currentActorTurn].SetCardInfo(cardData, 
            PhotonNetworkManager.GetLocalPlayer().ActorNumber != currentActorTurn);
        fetchedNewCard = true;
    }

    [PunRPC]
    private void FetchedNewCardFromDiscardPile()
    {
        CardData cardData = discardCardController.GetCardData();
        discardCardController.HideCard();

        actorIDToGamePlayerUIMap[currentActorTurn].SetCardInfo(cardData, 
            PhotonNetworkManager.GetLocalPlayer().ActorNumber != currentActorTurn);
        fetchedNewCard = true;
    }

    [PunRPC]
    private void RemoveCardFromPlayer(int cardID)
    {
        CardData cardData = actorIDToGamePlayerUIMap[currentActorTurn].FindCardByID(cardID);
        actorIDToGamePlayerUIMap[currentActorTurn].TakeCard(cardID, 
            PhotonNetworkManager.GetLocalPlayer().ActorNumber != currentActorTurn);

        discardCardController.SetCardData(cardData);

        EndTurn();
    }

    private void EndTurn()
    {
        if (gameOver)
        {
            return;
        }
        CheckGameOver();
        if (gameOver)
        {
            return;
        }
        NextTurn();
    }

    private void NextTurn()
    {
        actorIDToGamePlayerUIMap[currentActorTurn].SetCurrentTurnIndicator(false);
        currentActorTurn++;
        if (currentActorTurn > PhotonNetworkManager.GetPlayerCountInCurrentRoom())
        {
            currentActorTurn = 1;
        }
        fetchedNewCard = false;
        actorIDToGamePlayerUIMap[currentActorTurn].SetCurrentTurnIndicator(true);
    }

    private void CheckGameOver()
    {
        if (IsBlitz())
        {
            HandleBlitz();
        }
        else if (IsKnock() && currentActorTurn == turnEndActorNumber)
        {
            HandleKnock();
        }
    }

    private void HandleBlitz()
    {
        Debug.Log($"Blitzing!!");
        gameOver = true;
        Debug.Log($"Player {actorIDToGamePlayerUIMap[currentActorTurn].GetName()} got BLITZ!");
        ShowAllPlayerCards();
        GameUIController.Instance.ShowGameOver($"Player {actorIDToGamePlayerUIMap[currentActorTurn].GetName()} got BLITZ!", PhotonNetworkManager.IsMasterClient());
    }

    private void HandleKnock()
    {
        Debug.Log($"Knocking!!");
        float maxScore = 0;
        GamePlayerUI maxScoreGamePlayer = null;

        float lowestScore = int.MaxValue;
        GamePlayerUI lowestScoreGamePlayer = null;
        foreach (GamePlayerUI gamePlayerUI in actorIDToGamePlayerUIMap.Values)
        {
            float playerScore = gamePlayerUI.GetScore();
            if (playerScore > maxScore)
            {
                maxScore = playerScore;
                maxScoreGamePlayer = gamePlayerUI;
            }

            if (playerScore < lowestScore)
            {
                lowestScore = playerScore;
                lowestScoreGamePlayer = gamePlayerUI;
            }
        }

        Debug.Log($"Player {maxScoreGamePlayer.GetName()} has highest score of {maxScoreGamePlayer.GetScore()}!");
        Debug.Log($"Player {lowestScoreGamePlayer.GetName()} has lowest score of {lowestScoreGamePlayer.GetScore()}!");

        string message = $"Player {maxScoreGamePlayer.GetName()} has highest score of {maxScoreGamePlayer.GetScore()}! " +
            $"\n Player {lowestScoreGamePlayer.GetName()} has lowest score of {lowestScoreGamePlayer.GetScore()}!";
        gameOver = true;
        ShowAllPlayerCards();
        GameUIController.Instance.ShowGameOver(message, PhotonNetworkManager.IsMasterClient());
    }

    private void ShowAllPlayerCards()
    {
        foreach (GamePlayerUI gamePlayerUI in actorIDToGamePlayerUIMap.Values)
        {
            gamePlayerUI.SetCardsInfo(gamePlayerUI.GetCards());
        }
    }

    private bool IsKnock()
    {
        return playerKnocked && knockedPlayerActorNumber != -1 && turnEndActorNumber != -1;
    }

    private bool IsBlitz()
    {
        return actorIDToGamePlayerUIMap[currentActorTurn].GetScore() == 31;
    }

    private bool IsMyTurn => PhotonNetworkManager.GetLocalPlayer().ActorNumber == currentActorTurn;
}

[Serializable]
public class PlayerCountToSpawnPoints
{
    public int Count;
    public Transform[] SpawnPoints;
}
