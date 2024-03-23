using UnityEngine;

public static class BlitzHelper
{
    public static Sprite GetCardImage(CardData cardData)
    {
        return Resources.Load<Sprite>(Blitz.CARD_ROOT_FOLDER + Blitz.CARD_PREFIX + cardData.suit.ToString() + Blitz.CardRankToShortRankMap[cardData.rank]);
    }

    public static Sprite GetFaceDownImage()
    {
        return Resources.Load<Sprite>(Blitz.CARD_ROOT_FOLDER + Blitz.CARD_BACK_FACE_NAME);
    }
}
