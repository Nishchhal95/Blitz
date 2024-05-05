using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverWinLoseMessage;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button debugButton;

    [SerializeField] private GameController gameController;

    private void OnEnable()
    {
        restartButton.onClick.AddListener(RestartClick);
        debugButton.onClick.AddListener(DebugPressed);
        GameController.GameStarted += GameStarted;
    }

    private void OnDisable()
    {
        restartButton.onClick.RemoveListener(RestartClick);
        debugButton.onClick.RemoveListener(DebugPressed);
        GameController.GameStarted -= GameStarted;
    }

    public void ShowGameOver(string gameOverMessage, bool canRestart = false)
    {
        gameOverWinLoseMessage.SetText(gameOverMessage);
        restartButton.gameObject.SetActive(canRestart);
        gameOverPanel.SetActive(true);
    }

    public void HideGameOver()
    {
        gameOverPanel.SetActive(false);
    }

    private void DebugPressed()
    {
        GameController.IS_DEBUG = !GameController.IS_DEBUG;
    }

    private void RestartClick()
    {
        gameController.StartGame();
    }

    private void GameStarted()
    {
        debugButton.gameObject.SetActive(PhotonNetworkManager.IsMasterClient());
    }
}
