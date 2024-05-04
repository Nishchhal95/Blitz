using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrivateGameMenu : Menu
{
    [SerializeField] private TMP_InputField roomCodeInputField;
    [SerializeField] private Button joinGameButton;

    protected override void OnEnable()
    {
        base.OnEnable();

        joinGameButton.onClick.AddListener(JoinGame);
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        joinGameButton.onClick.RemoveListener(JoinGame);
    }

    private void JoinGame()
    {
        string roomCode = roomCodeInputField.text;
        if (string.IsNullOrEmpty(roomCode))
        {
            return;
        }
        roomCode = roomCode.Trim();
        if (string.IsNullOrEmpty(roomCode))
        {
            return;
        }

        PhotonNetworkManager.Instance.JoinRoom(roomCode);
    }
}
