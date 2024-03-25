using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private PhotonNetworkManager photonNetworkManager;
    [SerializeField] private TMP_InputField userNameInputField, roomNameInputField;
    [SerializeField] private Button joinRoomButton, createRoomButton;
    [SerializeField] private GameObject multiplayerConnectionHolder;
    [SerializeField] private GameObject playerListHolder;
    [SerializeField] private Button startGamButton;
    [SerializeField] private Button debugButton;

    [SerializeField] private Transform menuPlayerListParent;
    [SerializeField] private MenuPlayerItem menuPlayerItemPrefab;

    [SerializeField] private GameController gameController;

    [SerializeField] private Canvas mainMenuCanvas;
    [SerializeField] private Canvas gameCanvas;

    private Dictionary<int, MenuPlayerItem> photonPlayerActorNumberToPlayerMap = new Dictionary<int, MenuPlayerItem>();

    private void Awake()
    {
        InitMenu();
    }

    private void OnEnable()
    {
        PhotonNetworkManager.ConnectedToPhoton += OnConnectedToPhoton;
        PhotonNetworkManager.JoinedRoom += OnJoinedRoom;
        PhotonNetworkManager.LeftRoom += OnLeftRoom;
        PhotonNetworkManager.PlayerEnteredRoom += OnPlayerEnteredRoom;
        PhotonNetworkManager.PlayerExitedRoom += OnPlayerExitedRoom;
        GameController.GameStarted += OnGameStarted;
        createRoomButton.onClick.AddListener(CreateRoom);
        joinRoomButton.onClick.AddListener(JoinRoom);
        startGamButton.onClick.AddListener(StartGame);
        debugButton.onClick.AddListener(DebugPressed);
    }
    private void OnDisable()
    {
        PhotonNetworkManager.ConnectedToPhoton -= OnConnectedToPhoton;
        PhotonNetworkManager.JoinedRoom -= OnJoinedRoom;
        PhotonNetworkManager.LeftRoom -= OnLeftRoom;
        PhotonNetworkManager.PlayerEnteredRoom -= OnPlayerEnteredRoom;
        PhotonNetworkManager.PlayerExitedRoom -= OnPlayerExitedRoom;
        GameController.GameStarted -= OnGameStarted;
        createRoomButton.onClick.RemoveListener(CreateRoom);
        joinRoomButton.onClick.RemoveListener(JoinRoom);
        startGamButton.onClick.RemoveListener(StartGame);
        debugButton.onClick.RemoveListener(DebugPressed);
    }

    private void InitMenu()
    {
        createRoomButton.interactable = false;
        joinRoomButton.interactable = false;
        startGamButton.gameObject.SetActive(false);
        mainMenuCanvas.enabled = true;
        gameCanvas.enabled = false;
    }

    private void OnConnectedToPhoton()
    {
        createRoomButton.interactable = true;
        joinRoomButton.interactable = true;
    }

    private void OnJoinedRoom()
    {
        multiplayerConnectionHolder.SetActive(false);
        playerListHolder.SetActive(true);
        if (PhotonNetworkManager.IsMasterClient())
        {
            startGamButton.gameObject.SetActive(true);
        }

        List<Player> players = PhotonNetworkManager.GetPlayerListInCurrentRoom();
        for (int i = 0; i < players.Count; i++)
        {
            AddPlayerToList(players[i]);
        }
    }

    private void OnLeftRoom()
    {
        foreach (int actorNumber in photonPlayerActorNumberToPlayerMap.Keys)
        {
            RemovePlayerFromList(actorNumber);
        }
    }

    private void OnPlayerEnteredRoom(Player player)
    {
        AddPlayerToList(player);
    }

    private void OnPlayerExitedRoom(Player player)
    {
        RemovePlayerFromList(player.ActorNumber);
    }

    private void OnGameStarted()
    {
        startGamButton.gameObject.SetActive(false);
        playerListHolder.SetActive(false);
        mainMenuCanvas.enabled = false;
        gameCanvas.enabled = true;
    }

    private void CreateRoom()
    {
        if (TryCanSetPlayerName())
        {
            photonNetworkManager.CreateRoom();
        }
    }

    private void JoinRoom()
    {
        if (TryCanSetPlayerName())
        {
            string roomName = roomNameInputField.text.Trim();
            if (string.IsNullOrEmpty(roomName))
            {
                roomName = PhotonNetworkManager.TEST_ROOM_NAME;
            }
            photonNetworkManager.JoinRoom(roomName);
        }
    }

    private void StartGame()
    {
        gameController.StartGame();
        if (PhotonNetworkManager.IsMasterClient())
        {
            debugButton.gameObject.SetActive(true);
        }
    }

    private void DebugPressed()
    {
        GameController.IS_DEBUG = !GameController.IS_DEBUG;
    }

    private bool TryCanSetPlayerName()
    {
        string playerName = userNameInputField.text.Trim();
        if(string.IsNullOrEmpty(playerName))
        {
            return false;
        }
        PhotonNetwork.LocalPlayer.NickName = playerName;
        return true;
    }

    private void AddPlayerToList(Player player)
    {
        MenuPlayerItem menuPlayerItem = Instantiate(menuPlayerItemPrefab, menuPlayerListParent);
        menuPlayerItem.Init(player.NickName);
        photonPlayerActorNumberToPlayerMap.Add(player.ActorNumber, menuPlayerItem);
    }

    private void RemovePlayerFromList(int actorNumber)
    {
        if(photonPlayerActorNumberToPlayerMap.TryGetValue(actorNumber, out MenuPlayerItem menuPlayerItem))
        {
            Destroy(menuPlayerItem.gameObject);
        }
    }
}
