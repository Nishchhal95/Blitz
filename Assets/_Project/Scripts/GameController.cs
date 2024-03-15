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
    [SerializeField] private Dictionary<string, GamePlayer> gamePlayersMap = new Dictionary<string, GamePlayer>();
    [SerializeField] private Dictionary<string, Player> photonPlayerMap = new Dictionary<string, Player>();

    private void OnEnable()
    {
        PhotonNetworkManager.JoinedRoom += SpawnLocalPlayer;
        PhotonNetworkManager.PlayerEnteredRoom += SpawnGamePlayer;
    }

    private void OnDisable()
    {
        PhotonNetworkManager.JoinedRoom -= SpawnLocalPlayer;
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

    private void SpawnLocalPlayer()
    {
        GamePlayer gamePlayer = Instantiate(gamePlayerPrefab, spawnPointsPositions[0].SpawnPoint, Quaternion.identity, playerHolder);
        gamePlayer.SetPlayerName(PhotonNetworkManager.GetLocalPlayer().NickName);
        gamePlayersMap.Add(PhotonNetworkManager.GetLocalPlayer().UserId, gamePlayer);
        photonPlayerMap.Add(PhotonNetworkManager.GetLocalPlayer().UserId, PhotonNetworkManager.GetLocalPlayer());
        spawnPointsPositions[0].IsOccupied = true;

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
        SpawnPointData spawnPointData = spawnPointsPositions[GetNextAvailableSpawnPointIndex()];
        GamePlayer gamePlayer = Instantiate(gamePlayerPrefab, spawnPointData.SpawnPoint, Quaternion.identity, playerHolder);
        gamePlayer.SetPlayerName(player.NickName);
        gamePlayersMap.Add(player.UserId, gamePlayer);
        photonPlayerMap.Add(player.UserId, player);
        spawnPointData.IsOccupied = true;
    }

    private int GetNextAvailableSpawnPointIndex()
    {
        for (int i = 0; i < spawnPointsPositions.Length; i++)
        {
            if (!spawnPointsPositions[i].IsOccupied)
            {
                return i;
            }
        }
        return -1;
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
