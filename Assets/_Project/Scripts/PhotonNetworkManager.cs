using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PhotonNetworkManager : MonoBehaviourPunCallbacks
{
    public static Action ConnectedToPhoton;
    public static Action JoinedLobby;
    public static Action RoomCreated;
    public static Action JoinedRoom;
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
            MaxPlayers = 6,
            PublishUserId = true
        };

        PhotonNetwork.CreateRoom("QWERTY", roomOptions);
    }

    public void JoinRoom(string roomID)
    {
        PhotonNetwork.JoinRoom(roomID);
    }

    public static bool IsMasterClient()
    {
        return PhotonNetwork.IsMasterClient;
    }

    public static Room GetCurrentRoom()
    {
        return PhotonNetwork.CurrentRoom;
    }

    public static Player GetLocalPlayer()
    {
        return PhotonNetwork.LocalPlayer;
    }

    public static int GetPlayerCountInCurrentRoom()
    {
        return PhotonNetwork.CurrentRoom.PlayerCount;
    }

    public static Dictionary<int,Player> GetPlayersInCurrentRoom()
    {
        return PhotonNetwork.CurrentRoom.Players;
    }

    public static List<Player> GetPlayerListInCurrentRoom()
    {
        return PhotonNetwork.CurrentRoom.Players.Values.ToList();
    }

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
        Debug.Log("Room Created" + PhotonNetwork.CurrentRoom.Name);
        RoomCreated?.Invoke();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Room Joined" + PhotonNetwork.CurrentRoom.Name);
        JoinedRoom?.Invoke();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Player Entered Room" + newPlayer.NickName);
        PlayerEnteredRoom?.Invoke(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("Player Left Room" + otherPlayer.NickName);
        PlayerExitedRoom?.Invoke(otherPlayer);
    }
}
