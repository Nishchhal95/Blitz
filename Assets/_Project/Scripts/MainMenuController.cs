using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private PhotonNetworkManager photonNetworkManager;
    [SerializeField] private TMP_InputField userNameInputField, roomNameInputField;
    [SerializeField] private Button joinRoomButton, createRoomButton;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private Button startGamButton;

    [SerializeField] private GameController gameController;

    private void Awake()
    {
        InitMenu();
    }

    private void OnEnable()
    {
        PhotonNetworkManager.ConnectedToPhoton += OnConnectedToPhoton;
        PhotonNetworkManager.JoinedRoom += OnJoinedRoom;
        GameController.GameStarted += OnGameStarted;
        createRoomButton.onClick.AddListener(CreateRoom);
        joinRoomButton.onClick.AddListener(JoinRoom);
        startGamButton.onClick.AddListener(StartGame);
    }
    private void OnDisable()
    {
        PhotonNetworkManager.ConnectedToPhoton -= OnConnectedToPhoton;
        PhotonNetworkManager.JoinedRoom -= OnJoinedRoom;
        GameController.GameStarted -= OnGameStarted;
        createRoomButton.onClick.RemoveListener(CreateRoom);
        joinRoomButton.onClick.RemoveListener(JoinRoom);
        startGamButton.onClick.RemoveListener(StartGame);
    }

    private void InitMenu()
    {
        createRoomButton.interactable = false;
        joinRoomButton.interactable = false;
        startGamButton.gameObject.SetActive(false);
    }

    private void OnConnectedToPhoton()
    {
        createRoomButton.interactable = true;
        joinRoomButton.interactable = true;
    }

    private void OnJoinedRoom()
    {
        mainMenuPanel.SetActive(false);
        if (PhotonNetworkManager.IsMasterClient())
        {
            startGamButton.gameObject.SetActive(true);
        }
    }

    private void OnGameStarted()
    {
        startGamButton.gameObject.SetActive(false);
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
                return;
            }
            photonNetworkManager.JoinRoom(roomName);
        }
    }

    private void StartGame()
    {
        gameController.StartGame();
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
}
