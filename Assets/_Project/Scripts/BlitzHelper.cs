using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BlitzHelper
{
    public static Sprite GetCardImage(CardData cardData)
    {
        return Resources.Load<Sprite>(Blitz.CARD_ROOT_FOLDER + Blitz.CARD_PREFIX + cardData.suit.ToString() + Blitz.CardRankToShortRankMap[cardData.rank]);
    }
}
