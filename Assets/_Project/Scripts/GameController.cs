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
    [SerializeField] private Transform playerHolder;
    [SerializeField] private GamePlayer gamePlayerPrefab;
    [SerializeField] private SpawnPointData[] spawnPointsPositions;
    [SerializeField] private int currentSpawnPointIndex = -1;
    [SerializeField] private Dictionary<string, GamePlayer> gamePlayersMap = new Dictionary<string, GamePlayer>();
    [SerializeField] private Dictionary<string, Player> photonPlayerMap = new Dictionary<string, Player>();

    private void OnEnable()
    {
        PhotonNetworkManager.JoinedRoom += OnJoinedRoom;
        PhotonNetworkManager.PlayerEnteredRoom += SpawnGamePlayer;
    }

    private void OnDisable()
    {
        PhotonNetworkManager.JoinedRoom -= OnJoinedRoom;
        PhotonNetworkManager.PlayerEnteredRoom -= SpawnGamePlayer;
    }

    public void StartGame()
    {
        int playersInRoom = PhotonNetworkManager.GetPlayerCountInCurrentRoom();
        if (playersInRoom < MINIMUM_PLAYERS_TO_START_GAME || playersInRoom > MAX_PLAYERS)
        {
            return;
        }

        GameStarted?.Invoke();
        blitz.GetComponent<PhotonView>().RPC("SetupGame", RpcTarget.All, 55);

        photonView.RPC("GivePlayerCard", RpcTarget.All, PhotonNetworkManager.GetLocalPlayer().UserId, 0);

        foreach (var photonPlayerUserID in photonPlayerMap.Keys)
        {
            if (photonPlayerUserID.Equals(PhotonNetworkManager.GetLocalPlayer().UserId))
            {
                continue;
            }
            photonView.RPC("GivePlayerCard", RpcTarget.All, photonPlayerUserID, 0);
        }
    }

    private void OnJoinedRoom()
    {
        SpawnLocalPlayer();
        SpawnOtherPlayers();
    }

    private void SpawnLocalPlayer()
    {
        currentSpawnPointIndex++;
        SpawnPointData spawnPointData = spawnPointsPositions[currentSpawnPointIndex];
        GamePlayer localGamePlayer = Instantiate(gamePlayerPrefab, spawnPointData.SpawnPoint, Quaternion.identity, playerHolder);
        GamePlayerData gamePlayerData = new GamePlayerData()
        {
            UserID = PhotonNetworkManager.GetLocalPlayer().UserId,
            PlayerName = PhotonNetworkManager.GetLocalPlayer().NickName
        };
        localGamePlayer.Init(gamePlayerData);

        gamePlayersMap.Add(PhotonNetworkManager.GetLocalPlayer().UserId, localGamePlayer);
        photonPlayerMap.Add(PhotonNetworkManager.GetLocalPlayer().UserId, PhotonNetworkManager.GetLocalPlayer());

        spawnPointData.IsOccupied = true;
    }

    private void SpawnOtherPlayers()
    {
        List<Player> players = PhotonNetworkManager.GetPlayerListInCurrentRoom();
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].IsLocal)
            {
                continue;
            }
            SpawnGamePlayer(players[i]);
        }
    }

    private void SpawnGamePlayer(Player player)
    {
        currentSpawnPointIndex++;
        SpawnPointData spawnPointData = spawnPointsPositions[currentSpawnPointIndex];
        GamePlayer gamePlayer = Instantiate(gamePlayerPrefab, spawnPointData.SpawnPoint, Quaternion.identity, playerHolder);
        GamePlayerData gamePlayerData = new GamePlayerData()
        {
            UserID = player.UserId,
            PlayerName = player.NickName
        };
        gamePlayer.Init(gamePlayerData);

        gamePlayersMap.Add(player.UserId, gamePlayer);
        photonPlayerMap.Add(player.UserId, player);

        spawnPointData.IsOccupied = true;
    }

    [PunRPC]
    private void GivePlayerCard(string userID, int cardCount)
    {
        if(cardCount == 0)
        {
            cardCount = Blitz.NUM_OF_CARDS_PER_PLAYER;
        }

        for (int i = 0; i < cardCount; i++)
        {
            CardData cardData = blitz.FetchCard();
            GamePlayer gamePlayer = gamePlayersMap[userID];
            gamePlayer.SetCardData(i, cardData);
            gamePlayer.SetCardSlot(i, blitz.GetCardImage(cardData));
        }
    }

    public Dictionary<string, Player> GetPhotonPlayerMap()
    {
        return photonPlayerMap;
    }

    public Dictionary<string, GamePlayer> GetGamePlayerMap()
    {
        return gamePlayersMap;
    }

    public Player GetPhotonPlayerByID(string id)
    {
        return photonPlayerMap[id];
    }

    public GamePlayer GetGamePlayerByID(string id)
    {
        return gamePlayersMap[id];
    }
}

[Serializable]
public class SpawnPointData
{
    public Vector2 SpawnPoint;
    public bool IsOccupied;
}
