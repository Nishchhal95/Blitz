using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PhotonNetworkManager : MonoBehaviourPunCallbacks
{
    public static string TEST_ROOM_NAME = "QWERTY";
    public static Action ConnectedToPhoton;
    public static Action JoinedLobby;
    public static Action RoomCreated;
    public static Action JoinedRoom;
    public static Action LeftRoom;
    public static Action<Player> PlayerEnteredRoom;
    public static Action<Player> PlayerExitedRoom;

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions()
        {
            MaxPlayers = Blitz.MAX_PLAYERS,
            PublishUserId = true
        };

        PhotonNetwork.CreateRoom(TEST_ROOM_NAME, roomOptions);
    }

    public void JoinRoom(string roomID)
    {
        PhotonNetwork.JoinRoom(roomID);
    }

    public static bool IsMasterClient() => PhotonNetwork.IsMasterClient;

    public static Room GetCurrentRoom() => PhotonNetwork.CurrentRoom;

    public static Player GetLocalPlayer() => PhotonNetwork.LocalPlayer;

    public static int GetPlayerCountInCurrentRoom() => PhotonNetwork.CurrentRoom.PlayerCount;

    public static Dictionary<int, Player> GetPlayersInCurrentRoom() => PhotonNetwork.CurrentRoom.Players;

    public static List<Player> GetPlayerListInCurrentRoom() => PhotonNetwork.CurrentRoom.Players.Values.ToList();
    public static List<Player> GetPlayerListInCurrentRoomSortedByActorNumber() => PhotonNetwork.CurrentRoom.Players.Values.OrderBy(player => player.ActorNumber).ToList();

    //Callbacks
    public override void OnConnectedToMaster()
    {
        ConnectedToPhoton?.Invoke();
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        JoinedLobby?.Invoke();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room Created " + PhotonNetwork.CurrentRoom.Name);
        RoomCreated?.Invoke();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Room Joined " + PhotonNetwork.CurrentRoom.Name);
        JoinedRoom?.Invoke();
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Room Left");
        LeftRoom?.Invoke();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Player Entered Room " + newPlayer.NickName);
        PlayerEnteredRoom?.Invoke(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("Player Left Room " + otherPlayer.NickName);
        PlayerExitedRoom?.Invoke(otherPlayer);
    }
}
