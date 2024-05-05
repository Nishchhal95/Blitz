using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PhotonNetworkManager : MonoBehaviourPunCallbacks
{
    public static PhotonNetworkManager Instance;
    private static PhotonNetworkManager instance;
    private static string GAME_CONFIG_KEY = "GAME_CONFIG";
    private static bool changingRegion;
    private static string region = "";

    public static string TEST_ROOM_NAME = "QWERTY";
    public static Action ConnectedToPhoton;
    public static Action JoinedLobby;
    public static Action RoomCreated;
    public static Action JoinedRoom;
    public static Action LeftRoom;
    public static Action<Player> PlayerEnteredRoom;
    public static Action<Player> PlayerExitedRoom;
    public static Action RoomListUpdated;
    public static Action<string> OnRegionChanged;

    private List<RoomInfo> rooms = new List<RoomInfo>();
    private static Dictionary<string, string> regionCodeToRegionNameMap = new Dictionary<string, string>()
    {
        {"asia","Asia"},
        {"au","Australia"},
        {"cae","Canada East"},
        {"eu","Europe"},
        {"in","India"},
        {"us","US East"},
        {"usw","US West"},
    };

    private void Awake()
    {
        if(instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        PhotonNetwork.Disconnect();
    }

    public void ConnectToPhoton()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    public void HostNewGame(BlitzGameData blitzGameData)
    {
        RoomOptions roomOptions = new RoomOptions()
        {
            MaxPlayers = blitzGameData.MaxPlayers,
            PublishUserId = true
        };

        string blitzGameDaaJson = JsonConvert.SerializeObject(blitzGameData);
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable
        {
            { GAME_CONFIG_KEY, blitzGameDaaJson }
        };
        roomOptions.CustomRoomPropertiesForLobby = new string[] { GAME_CONFIG_KEY };

        PhotonNetwork.CreateRoom(TEST_ROOM_NAME, roomOptions);
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


    public static void ChangeRegion(string region)
    {
        region = region.ToLower();
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = region;
        PhotonNetwork.PhotonServerSettings.DevRegion = region;

        if (PhotonNetwork.IsConnected)
        {
            changingRegion = true;
            PhotonNetwork.Disconnect();
        }
        OnRegionChanged?.Invoke(region);
    }
    public static bool IsConnected() => PhotonNetwork.IsConnected;
    public static bool IsMasterClient() => PhotonNetwork.IsMasterClient;
    public static Room GetCurrentRoom() => PhotonNetwork.CurrentRoom;
    public static Player GetLocalPlayer() => PhotonNetwork.LocalPlayer;
    public static int GetPlayerCountInCurrentRoom() => PhotonNetwork.CurrentRoom.PlayerCount;
    public static Dictionary<int, Player> GetPlayersInCurrentRoom() => PhotonNetwork.CurrentRoom.Players;
    public static List<Player> GetPlayerListInCurrentRoom() => PhotonNetwork.CurrentRoom.Players.Values.ToList();
    public static List<Player> GetPlayerListInCurrentRoomSortedByActorNumber() => PhotonNetwork.CurrentRoom.Players.Values.OrderBy(player => player.ActorNumber).ToList();
    public static void ForceRequestPhotonRooms() => PhotonNetwork.GetCustomRoomList(TypedLobby.Default, "1 = 1");
    public static Dictionary<string, string> GetRegionsMap() => regionCodeToRegionNameMap;
    public List<RoomInfo> GetRoomList() => rooms.ToList();
    public BlitzGameData GetCurrentRoomBlitzGameConfig()
    {
        string gameConfigJson = (string)PhotonNetwork.CurrentRoom.CustomProperties[GAME_CONFIG_KEY];

        BlitzGameData gameConfig = JsonConvert.DeserializeObject<BlitzGameData>(gameConfigJson);

        return gameConfig;
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

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected from Photon with reason " + cause.ToString());
        if(changingRegion)
        {
            changingRegion = false;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        rooms = roomList.ToList();
        Debug.Log("OnRoomListUpdate from Photon with room count " + roomList.Count());
        RoomListUpdated?.Invoke();
    }
}
