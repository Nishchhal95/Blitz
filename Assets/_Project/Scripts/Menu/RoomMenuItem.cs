using Photon.Realtime;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class RoomMenuItem : MonoBehaviour, IPointerClickHandler
{
    public event Action<RoomMenuItem> OnClick;

    [SerializeField] private TextMeshProUGUI roomNameTextField;
    [SerializeField] private TextMeshProUGUI roomPlayerTextField;
    [SerializeField] private TextMeshProUGUI roomStateTextField;

    [field: SerializeField] public RoomInfo roomInfo { get; private set; }

    public void SetData(RoomInfo roomInfo)
    {
        this.roomInfo = roomInfo;
        roomNameTextField.SetText(roomInfo.Name);
        roomPlayerTextField.SetText($"{roomInfo.PlayerCount} / {roomInfo.MaxPlayers}");
        roomStateTextField.SetText(roomInfo.IsOpen ? "Waiting" : "Started");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke(this);
    }
}
