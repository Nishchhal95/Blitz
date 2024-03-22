using TMPro;
using UnityEngine;

public class GamePlayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameTextElement;
    [SerializeField] private SpriteRenderer[] cardSlots;
    [SerializeField] private GamePlayerData gamePlayerData = null;

    public void Init(GamePlayerData gamePlayerData)
    {
        this.gamePlayerData = gamePlayerData;
        SetPlayerName(gamePlayerData.PlayerName);
    }

    private void SetPlayerName(string playerName)
    {
        playerNameTextElement.SetText(playerName);
    }

    public void SetCardSlot(int index, Sprite sprite)
    {
        cardSlots[index].sprite = sprite;
    }

    public void SetCardData(int index, CardData cardData)
    {
        gamePlayerData.CardDatas[index] = cardData;
    }
}

public class GamePlayerData
{
    public string UserID;
    public string PlayerName;
    public CardData[] CardDatas = new CardData[Blitz.NUM_OF_CARDS_PER_PLAYER];
}
