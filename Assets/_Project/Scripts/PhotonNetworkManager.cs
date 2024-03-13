using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonNetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private MainMenuController mainMenuController;
    [SerializeField] private Blitz blitz;

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions()
        {
            MaxPlayers = 6
        };

        PhotonNetwork.CreateRoom("QWERTY", roomOptions);
    }

    public void JoinRoom(string roomID)
    {
        PhotonNetwork.JoinRoom(roomID);
    }

    //Callbacks
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room Created" + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Room Joined" + PhotonNetwork.CurrentRoom.Name);
        mainMenuController.DisableMainMenu();
        blitz.SetupGame(PhotonNetwork.PlayerList.Length);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Player Entered Room" + newPlayer.NickName);
    }
}
