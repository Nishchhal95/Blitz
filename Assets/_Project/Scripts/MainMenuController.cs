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

    private void OnEnable()
    {
        joinRoomButton.onClick.AddListener(JoinRoom);
        createRoomButton.onClick.AddListener(CreateRoom);
    }
    private void OnDisable()
    {
        joinRoomButton.onClick.RemoveListener(JoinRoom);
        createRoomButton.onClick.RemoveListener(CreateRoom);
    }

    public void DisableMainMenu()
    {
        mainMenuPanel.SetActive(false);
    }

    private void CreateRoom()
    {
        SetPlayerName();
        photonNetworkManager.CreateRoom();
    }

    private void JoinRoom()
    {
        SetPlayerName();
        string roomName = roomNameInputField.text.Trim();
        if(string.IsNullOrEmpty(roomName))
        {
            return;
        }
        photonNetworkManager.JoinRoom(roomName);
    }

    private void SetPlayerName()
    {
        string playerName = userNameInputField.text.Trim();
        if(string.IsNullOrEmpty(playerName))
        {
            return;
        }
        PhotonNetwork.LocalPlayer.NickName = playerName;
    }
}
