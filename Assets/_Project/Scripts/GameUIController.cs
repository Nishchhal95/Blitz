using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    public static GameUIController Instance { get; private set; }

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverWinLoseMessage;
    [SerializeField] private Button restartButton;

    [SerializeField] private GameController gameController;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        restartButton.onClick.AddListener(RestartClick);
    }

    private void OnDisable()
    {
        restartButton.onClick.RemoveListener(RestartClick);
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

    private void RestartClick()
    {
        gameController.StartGame();
    }
}
