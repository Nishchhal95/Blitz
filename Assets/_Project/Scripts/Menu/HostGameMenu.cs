using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HostGameMenu : Menu
{
    [SerializeField] private TMP_InputField maxPlayersInputField;
    [SerializeField] private TMP_InputField maxLivesInputField;
    [SerializeField] private CustomToggle privateToggle;
    [SerializeField] private Button hostGameButton;

    [SerializeField] private BlitzGameData blitzGameData = new BlitzGameData(0,0,false);

    protected override void OnEnable()
    {
        base.OnEnable();
        maxPlayersInputField.contentType = TMP_InputField.ContentType.IntegerNumber;
        maxLivesInputField.contentType = TMP_InputField.ContentType.IntegerNumber;

        maxPlayersInputField.onValueChanged.AddListener(MaxPlayersValueChanged);
        maxLivesInputField.onValueChanged.AddListener(MaxLivesValueChanged);
        privateToggle.onValueChanged.AddListener(PrivateToggleValueChanged);

        hostGameButton.onClick.AddListener(HostGame);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        maxPlayersInputField.onValueChanged.RemoveListener(MaxPlayersValueChanged);
        maxLivesInputField.onValueChanged.RemoveListener(MaxLivesValueChanged);
        privateToggle.onValueChanged.RemoveListener(PrivateToggleValueChanged);

        hostGameButton.onClick.RemoveListener(HostGame);
    }

    private void MaxPlayersValueChanged(string newMaxPlayersString)
    {
        if (int.TryParse(newMaxPlayersString, out int maxPlayers))
        {
            blitzGameData.MaxPlayers = maxPlayers;
        }
    }

    private void MaxLivesValueChanged(string newMaxLivesString)
    {
        if (int.TryParse(newMaxLivesString, out int maxLives))
        {
            //Set Max Players
            blitzGameData.MaxLives = maxLives;
        }
    }

    private void PrivateToggleValueChanged(bool isPrivate)
    {
        blitzGameData.Private = isPrivate;
    }

    private void HostGame()
    {
        if(blitzGameData == null)
        {
            return;
        }

        PhotonNetworkManager.Instance.HostNewGame(blitzGameData);
    }
}

public class BlitzGameData
{
    public BlitzGameData(int maxPlayers, int maxLives, bool @private)
    {
        RoomCode = Guid.NewGuid().ToString();
        MaxPlayers = maxPlayers;
        MaxLives = maxLives;
        Private = @private;
    }
    public string RoomCode { get; private set; }
    public int MaxPlayers { get; set; }
    public int MaxLives { get; set; }
    public bool Private { get; set; }
}
