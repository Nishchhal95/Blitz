using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private string playerName;
    [SerializeField] private Button playButton;
    [SerializeField] private Button regionSelectButton;
    [SerializeField] private TextMeshProUGUI regionNameTextField;

    [Header("MenuUI")]
    [SerializeField] private GameModeMenu modeSelectionMenu;
    [SerializeField] private RegionSelectMenu regionSelectMenu;
    [SerializeField] private StartGameMenu startGameMenu;

    private void Awake()
    {
        InitMenu();
    }

    private void OnEnable()
    {
        playButton.onClick.AddListener(PlayButtonClicked);
        playerNameInputField.onValueChanged.AddListener(OnPlayerInputFieldValueChanged);
        regionSelectButton.onClick.AddListener(RegionSelectMenuClicked);
        PhotonNetworkManager.OnRegionChanged += SetRegionName;

        PhotonNetworkManager.ConnectedToPhoton += SetLocalPlayerNameOnPhoton;
        PhotonNetworkManager.JoinedRoom += LoadWaitingArea;
    }

    private void OnDisable()
    {
        playButton.onClick.RemoveListener(PlayButtonClicked);
        playerNameInputField.onValueChanged.RemoveListener(OnPlayerInputFieldValueChanged);
        regionSelectButton.onClick.RemoveListener(RegionSelectMenuClicked);
        PhotonNetworkManager.OnRegionChanged -= SetRegionName;

        PhotonNetworkManager.ConnectedToPhoton -= SetLocalPlayerNameOnPhoton;
        PhotonNetworkManager.JoinedRoom -= LoadWaitingArea;
    }

    private void InitMenu()
    {
        playButton.interactable = false;
        SetRegionName(PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion);
    }

    private void OnPlayerInputFieldValueChanged(string newValue)
    {
        if(string.IsNullOrEmpty(newValue))
        {
            playButton.interactable = false;
            return;
        }

        newValue = newValue.Trim();

        if (string.IsNullOrEmpty(newValue))
        {
            playButton.interactable = false;
            return;
        }

        playButton.interactable = true;
    }

    private void PlayButtonClicked()
    {
        if (!CanSetPlayerNameLocally())
        {
            return;
        }

        if (!PhotonNetworkManager.IsConnected())
        {
            PhotonNetworkManager.Instance.ConnectToPhoton();
        }
        modeSelectionMenu.Open();
    }

    private void SetLocalPlayerNameOnPhoton()
    {
        PhotonNetworkManager.GetLocalPlayer().NickName = playerName;
    }

    private bool CanSetPlayerNameLocally()
    {
        playerName = playerNameInputField.text;
        if (string.IsNullOrEmpty(playerName))
        {
            return false;
        }

        playerName = playerName.Trim();

        if (string.IsNullOrEmpty(playerName))
        {
            return false;
        }

        return true;
    }

    private void RegionSelectMenuClicked()
    {
        regionSelectMenu.Open();
    }

    private void SetRegionName(string region)
    {
        regionNameTextField.SetText(region);
    }

    private void LoadWaitingArea()
    {
        startGameMenu.Open();
    }
}
