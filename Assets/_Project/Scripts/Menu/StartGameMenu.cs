using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGameMenu : Menu
{
    [SerializeField] private Button startGameButton;
    [SerializeField] private Transform menuPlayerListParent;
    [SerializeField] private MenuPlayerItem menuPlayerItemPrefab;

    private Dictionary<int, MenuPlayerItem> photonPlayerActorNumberToPlayerMap = new Dictionary<int, MenuPlayerItem>();

    protected override void OnEnable()
    {
        base.OnEnable();

        PhotonNetworkManager.LeftRoom += OnLeftRoom;
        PhotonNetworkManager.PlayerEnteredRoom += OnPlayerEnteredRoom;
        PhotonNetworkManager.PlayerExitedRoom += OnPlayerExitedRoom;

        startGameButton.onClick.AddListener(StartGame);

        Init();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        PhotonNetworkManager.LeftRoom -= OnLeftRoom;
        PhotonNetworkManager.PlayerEnteredRoom -= OnPlayerEnteredRoom;
        PhotonNetworkManager.PlayerExitedRoom -= OnPlayerExitedRoom;

        startGameButton.onClick.RemoveListener(StartGame);
    }

    private void Init()
    {
        startGameButton.gameObject.SetActive(PhotonNetworkManager.IsMasterClient());

        List<Player> players = PhotonNetworkManager.GetPlayerListInCurrentRoom();
        for (int i = 0; i < players.Count; i++)
        {
            AddPlayerToList(players[i]);
        }
    }

    private void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
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

    private void AddPlayerToList(Player player)
    {
        MenuPlayerItem menuPlayerItem = Instantiate(menuPlayerItemPrefab, menuPlayerListParent);
        menuPlayerItem.Init(player.NickName);
        photonPlayerActorNumberToPlayerMap.Add(player.ActorNumber, menuPlayerItem);
    }

    private void RemovePlayerFromList(int actorNumber)
    {
        if (photonPlayerActorNumberToPlayerMap.TryGetValue(actorNumber, out MenuPlayerItem menuPlayerItem))
        {
            Destroy(menuPlayerItem.gameObject);
        }
    }
}
