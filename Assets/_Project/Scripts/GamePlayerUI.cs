using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayerUI : MonoBehaviour
{
    [SerializeField] private string playerName;
    [SerializeField] private CardData[] cards;
    [SerializeField] private TextMeshProUGUI playerNameTextField;
    [SerializeField] private Image[] cardImages;

    public void Init(string playerName)
    {
        this.playerName = playerName;
        SetPlayerName(playerName);
    }

    public void SetCardInfo(CardData[] cards)
    {
        this.cards = cards;
        for (int i = 0; i < cards.Length; i++) 
        {
            UpdateCardImage(i, cards[i]);
        }
    }

    private void SetPlayerName(string playerName)
    {
        playerNameTextField.SetText(playerName);
    }

    private void UpdateCardImage(int index, CardData cardData)
    {
        cardImages[index].sprite = BlitzHelper.GetCardImage(cardData);
    }
}
