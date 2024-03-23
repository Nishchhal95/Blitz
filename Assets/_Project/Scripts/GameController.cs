using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviourPun
{
    public static int MINIMUM_PLAYERS_TO_START_GAME = 0;
    public static int MAX_PLAYERS = 6;
    public static Action GameStarted;

    [SerializeField] private Blitz blitz;
    [SerializeField] private GamePlayerUI gamePlayerUIPrefab;
    [SerializeField] private PlayerCountToSpawnPoints[] playerCountToPlayerSlotsSetupMap = new PlayerCountToSpawnPoints[Blitz.MAX_PLAYERS];
    [SerializeField] private Dictionary<int, GamePlayerUI> actorIDToGamePlayerUIMap = new Dictionary<int, GamePlayerUI>();

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
    }

    private void DistributeCards()
    {
        List<Player> players = PhotonNetworkManager.GetPlayerListInCurrentRoomSortedByActorNumber();

        foreach (Player player in players)
        {
            CardData[] cardData = new CardData[3];
            for (int j = 0; j < 3; j++)
            {
                cardData[j] = blitz.FetchCard();
            }
            actorIDToGamePlayerUIMap[player.ActorNumber].SetCardInfo(cardData);
        }
    }

    private GamePlayerUI SpawnGamePlayer(Player player, Transform parentTransform)
    {
        GamePlayerUI gamePlayerUI = Instantiate(gamePlayerUIPrefab, Vector2.zero, Quaternion.identity, parentTransform);
        gamePlayerUI.transform.localPosition = Vector2.zero;
        gamePlayerUI.Init(player.NickName);

        actorIDToGamePlayerUIMap.Add(player.ActorNumber, gamePlayerUI);

        return gamePlayerUI;
    }

    [PunRPC]
    private void GivePlayerCard(string userID, int cardCount)
    {
        //if(cardCount == 0)
        //{
        //    cardCount = Blitz.NUM_OF_CARDS_PER_PLAYER;
        //}

        //for (int i = 0; i < cardCount; i++)
        //{
        //    CardData cardData = blitz.FetchCard();
        //    GamePlayer gamePlayer = gamePlayersMap[userID];
        //    gamePlayer.SetCardData(i, cardData);
        //    gamePlayer.SetCardSlot(i, BlitzHelper.GetCardImage(cardData));
        //}
    }
}

[Serializable]
public class SpawnPointData
{
    public Vector2 SpawnPoint;
    public bool IsOccupied;
}

[Serializable]
public class PlayerCountToSpawnPoints
{
    public int Count;
    public Transform[] SpawnPoints;
}
