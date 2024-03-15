using TMPro;
using UnityEngine;

public class GamePlayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameTextElement;
    [SerializeField] private SpriteRenderer[] cardSlots;
    [SerializeField] private CardData[] cardDatas = new CardData[Blitz.NUM_OF_CARDS_PER_PLAYER];

    public void SetPlayerName(string playerName)
    {
        playerNameTextElement.SetText(playerName);
    }

    public void SetCardSlots(Sprite[] sprites)
    {
        if(sprites.Length != cardSlots.Length)
        {
            return;
        }

        for (int i = 0; i < sprites.Length; i++)
        {
            SetCardSlot(i, sprites[i]);
        }
    }

    public void SetCardSlot(int index, Sprite sprite)
    {
        cardSlots[index].sprite = sprite;
    }

    public void SetCardData(int index, CardData cardData)
    {
        cardDatas[index] = cardData;
    }
}
