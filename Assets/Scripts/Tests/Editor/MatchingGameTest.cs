using System.Collections.Generic;
using NUnit.Framework;

public class MatchingGameTest
{
    [Test]
    public void game_start_and_all_card_covered()
    {
        CardManager cardManager = new CardManager();
        cardManager.StarGame(3);
        Assert.AreEqual(6, cardManager.GetTotalCoveredCardCount);
        Dictionary<int, int> dict_cardPattern = new Dictionary<int, int>();
        foreach (Card card in cardManager.GetAllCards)
        {
            if (dict_cardPattern.ContainsKey(card.GetPattern))
                dict_cardPattern[card.GetPattern] += 1;
            else
                dict_cardPattern[card.GetPattern] = 1;
        }

        foreach (int patternCount in dict_cardPattern.Values)
        {
            Assert.AreEqual(2, patternCount);
        }
    }
}