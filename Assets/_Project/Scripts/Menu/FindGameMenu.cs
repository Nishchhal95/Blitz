using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FindGameMenu : Menu
{
    [SerializeField] private Button refreshButton;

    [SerializeField] private Transform roomListParent;
    [SerializeField] private RoomMenuItem roomListItemPrefab;

    private Dictionary<RoomInfo, RoomMenuItem> roomInfoToMenuItemMap = new Dictionary<RoomInfo, RoomMenuItem>();

    protected override void OnEnable()
    {
        base.OnEnable();

        refreshButton.onClick.AddListener(Refresh);

        PhotonNetworkManager.RoomListUpdated += Init;

        Init();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        refreshButton.onClick.RemoveListener(Refresh);

        PhotonNetworkManager.RoomListUpdated -= Init;
    }

    private void Init()
    {
        foreach (RoomMenuItem roomMenuItem in roomInfoToMenuItemMap.Values.ToList())
        {
            Destroy(roomMenuItem.gameObject);
        }

        roomInfoToMenuItemMap.Clear();

        List<RoomInfo> rooms = PhotonNetworkManager.Instance.GetRoomList();

        foreach (RoomInfo room in rooms)
        {
            RoomMenuItem roomMenuItem = Instantiate(roomListItemPrefab, roomListParent);
            roomMenuItem.SetData(room);
            roomMenuItem.OnClick += JoinGame;
            roomInfoToMenuItemMap.Add(room, roomMenuItem);
        }
    }

    private void Refresh()
    {
        PhotonNetworkManager.ForceRequestPhotonRooms();
    }

    private void JoinGame(RoomMenuItem roomMenuItem)
    {
        PhotonNetworkManager.Instance.JoinRoom(roomMenuItem.roomInfo.Name);
    }
}
